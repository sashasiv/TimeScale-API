using Microsoft.EntityFrameworkCore;
using infotecs.Models;

namespace infotecs.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        // DbSet для каждой таблицы
        public DbSet<Files> Files { get; set; }
        public DbSet<Values> Values { get; set; }
        public DbSet<TResults> TResults { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Настройка таблицы Files
            modelBuilder.Entity<Files>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.FileName)
                    .IsRequired()
                    .HasMaxLength(255);
                entity.Property(e => e.RecordsCount)
                    .IsRequired();
            });

            // Настройка таблицы Values
            modelBuilder.Entity<Values>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.FileId)
                    .IsRequired();
                entity.Property(e => e.Date)
                    .IsRequired();
                entity.Property(e => e.ExecutionTime)
                    .IsRequired();
                entity.Property(e => e.Value)
                    .IsRequired();

                // Связь с Files (один файл → много записей)
                entity.HasOne(v => v.File)
                    .WithMany(f => f.Values)
                    .HasForeignKey(v => v.FileId)
                    .OnDelete(DeleteBehavior.Cascade);
            });

            // Настройка таблицы Results
            modelBuilder.Entity<TResults>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.FileId)
                    .IsRequired();
                entity.Property(e => e.DeltaDate_seconds)
                    .IsRequired();
                entity.Property(e => e.StartDate)
                    .IsRequired();
                entity.Property(e => e.AverageExecutionTime)
                    .IsRequired();
                entity.Property(e => e.AverageValue)
                    .IsRequired();
                entity.Property(e => e.MedianValue)
                    .IsRequired();
                entity.Property(e => e.MaxValue)
                    .IsRequired();
                entity.Property(e => e.MinValue)
                    .IsRequired();

                // Связь с Files (один файл → один результат)
                entity.HasOne(r => r.File)
                    .WithMany() // У Files нет навигационного свойства на Results
                    .HasForeignKey(r => r.FileId)
                    .OnDelete(DeleteBehavior.Cascade);
            });
        }
    }
}
