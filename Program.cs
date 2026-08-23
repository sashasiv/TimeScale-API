using Microsoft.EntityFrameworkCore;
using infotecs.Data;
using infotecs.Services;

var builder = WebApplication.CreateBuilder(args);

// 1. Добавляем контроллеры
builder.Services.AddControllers();

// 2. Добавляем Swagger
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// 3. Подключаем PostgreSQL
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseNpgsql(
        builder.Configuration.GetConnectionString("DefaultConnection")
    )
);
builder.Services.AddScoped<ICsvImportService, CsvImportService>();
builder.Services.AddScoped<IResultsOutputService, ResultsOutputService>();
builder.Services.AddScoped<IValuesService, ValuesService>();

var app = builder.Build();

// 4. Настройка Swagger в режиме разработки
if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
    app.UseSwagger();
    app.UseSwaggerUI();
}

// 5. Перенаправление на HTTPS
app.UseHttpsRedirection();

// 6. Авторизация (пока не используется)
app.UseAuthorization();

// 7. Маршрутизация контроллеров
app.MapControllers();

// Проверка БД при старте
using (var scope = app.Services.CreateScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
    try
    {
        await dbContext.Database.EnsureCreatedAsync();
        Console.WriteLine(" База данных подключена успешно!");
    }
    catch (Exception ex)
    {
        Console.WriteLine($" Ошибка подключения к БД: {ex.Message}");
    }
}

app.Run();