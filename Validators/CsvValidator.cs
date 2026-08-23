using infotecs.Models;
using infotecs.Models.DTOs;
using System.Globalization;

namespace infotecs.Validators
{
    public class CsvValidator
    {
        // Константы валидатора
        ///////////////////////
        private static readonly DateTimeOffset MinDate = new DateTimeOffset(2000, 1, 1, 0, 0, 0, TimeSpan.Zero); // Минимальная дата
        private static readonly DateTimeOffset MaxDate = DateTimeOffset.UtcNow; // Текущая дата (UTC)

        // Форматы дат для парсинга
        private static readonly string[] AllowedDateFormats = new[]
        { "yyyy-MM-ddTHH-mm-ss.ffffZ" };

        private const int MAX_ROWS = 10000; // Максимальное число строк

        public class RowValidationResult
        {
            public bool IsValid { get; set; }
            public List<string> Errors { get; set; } = new();
            public CsvRecordDto? Record { get; set; }
        }

        public class FileValidationResult
        {
            public bool IsValid { get; set; }
            public List<string> Errors { get; set; } = new();
            public List<CsvRecordDto> ValidRecords { get; set; } = new();
        }

        // Проверка заголовка
        // ///////////////////
        public (bool IsValid, string ErrorMessage) ValidateHeaders(string[] headers)
        {
            var expectedHeaders = new[] { "Date", "ExecutionTime", "Value" };

            if (headers.Length != expectedHeaders.Length)
            {
                return (false, $"Неверное количество колонок. Ожидается: {expectedHeaders.Length}, Получено: {headers.Length}");
            }

            for (int i = 0; i < expectedHeaders.Length; i++)
            {
                if (!string.Equals(headers[i].Trim(), expectedHeaders[i], StringComparison.OrdinalIgnoreCase))
                {
                    return (false, $"Неверный заголовок колонки {i + 1}. Ожидается: {expectedHeaders[i]}, Получено: {headers[i]}");
                }
            }

            return (true, string.Empty);
        }

        // Проверка даты
        ////////////////
        public (bool IsValid, string ErrorMessage, DateTimeOffset? Date) ValidateDate(string dateString)
        {
            // Дата пустая
            if (string.IsNullOrWhiteSpace(dateString))
                return (false, "Дата не может быть пустой", null);

            // Неверный формат даты
            if (!DateTimeOffset.TryParseExact(
                dateString,
                AllowedDateFormats,
                CultureInfo.InvariantCulture,
                DateTimeStyles.AssumeUniversal,
                out DateTimeOffset parsedDate))
                return (false, $"Неверный формат даты: {dateString}. Ожидается: ГГГГ-ММ-ДДTчч-мм-сс.ммммZ", null);

            // Дата не раньше 01.01.2000
            if (parsedDate < MinDate)
                    return (false, $"Дата не может быть раньше 01.01.2000. Получено: {parsedDate:yyyy-MM-dd}", null);

            // Дата не позже текущей
            if (parsedDate > MaxDate)
                return (false, $"Дата не может быть позже текущей. Получено: {parsedDate:yyyy-MM-dd HH:mm:ss}. Текущая: {MaxDate:yyyy-MM-dd HH:mm:ss}", null);

            return (true, string.Empty, parsedDate);
        }
        // Проверка времени выполнения
        ///////////////////////////////
        public (bool IsValid, string ErrorMessage, double? ExecutionTime) ValidateExecutionTime(string ExecutionTimeStr)
        {
            // Не пустой
            if (string.IsNullOrWhiteSpace(ExecutionTimeStr))
                return (false, "ExecutionTime не может быть пустым", null);

            // Парсинг
            if (!double.TryParse(ExecutionTimeStr.Trim(), NumberStyles.Float, CultureInfo.InvariantCulture, out double executionTime))
                return (false, $"Некорректное значение ExecutionTime: {ExecutionTimeStr}", null);

            // ПРОВЕРКА: NaN (Not a Number)
            if (double.IsNaN(executionTime))
                return (false, $"ExecutionTime не может быть NaN (Not a Number). Получено: {ExecutionTimeStr}", null);

            // ПРОВЕРКА: Infinity
            if (double.IsInfinity(executionTime))
                return (false, $"ExecutionTime не может быть бесконечностью (Infinity). Получено: {ExecutionTimeStr}", null);

            // Не отрицательное
            if (executionTime < 0)
                return (false, $"ExecutionTime не может быть отрицательным. Получено: {executionTime}", null);

            return (true, string.Empty, executionTime);
        }

        // Проверка значения
        /////////////////////
        public (bool IsValid, string ErrorMessage, double? Value) ValidateValue(string valueStr)
        {
            // Не пустой
            if (string.IsNullOrWhiteSpace(valueStr))
                return (false, "Value не может быть пустым", null);

            // Парсинг
            if (!double.TryParse(valueStr.Trim(), NumberStyles.Float, CultureInfo.InvariantCulture, out double value))
                return (false, $"Некорректное значение Value: {valueStr}", null);

            // ПРОВЕРКА: NaN (Not a Number)
            if (double.IsNaN(value))
                return (false, $"Value не может быть NaN (Not a Number). Получено: {valueStr}", null);

            // ПРОВЕРКА: Infinity
            if (double.IsInfinity(value))
                return (false, $"Value не может быть бесконечностью (Infinity). Получено: {valueStr}", null);

            // Не отрицательное
            if (value < 0)
                return (false, $"Value не может быть отрицательным. Получено: {value}", null);

            return (true, string.Empty, value);
        }

        // Проверка строки
        ////////////////////////////
        public RowValidationResult ValidateRow(string[] fields, int lineNumber)
        {
            var result = new RowValidationResult();
            var errors = new List<string>();

            // В строке должно быть 3 поля
            if (fields.Length != 3)
            {
                errors.Add($"Строка {lineNumber}: Неверное количество полей. Ожидается 3, получено {fields.Length}");
                result.IsValid = false;
                result.Errors = errors;
                return result; // Далье можно не проверять
            }

            // Проверка даты
            var (isDateValid, dateError, parsedDate) = ValidateDate(fields[0]);
            if (!isDateValid)
            {
                errors.Add($"Строка {lineNumber}: {dateError}");
            }

            var (isTimeValid, timeError, executionTime) = ValidateExecutionTime(fields[1]);
            if (!isTimeValid)
            {
                errors.Add($"Строка {lineNumber}: {timeError}");
            }

            var (isValueValid, valueError, value) = ValidateValue(fields[2]);
            if (!isValueValid)
            {
                errors.Add($"Строка {lineNumber}: {valueError}");
            }

            if (errors.Count > 0)
            {
                result.IsValid = false;
                result.Errors = errors;
                return result;
            }

            result.IsValid = true;
            result.Record = new CsvRecordDto
            {
                Date = parsedDate!.Value,
                ExecutionTime = executionTime!.Value,
                Value = value!.Value
            };

            return result;
        }

        // Проверка файла
        /////////////////
        public async Task<FileValidationResult> ValidateFileAsync(Stream fileStream)
        {
            var result = new FileValidationResult();
            var allErrors = new List<string>();

            using var reader = new StreamReader(fileStream);

            // Файл не пуст
            var firstLine = await reader.ReadLineAsync();
            if (string.IsNullOrEmpty(firstLine))
            {
                allErrors.Add("Файл пуст");
                result.IsValid = false;
                result.Errors = allErrors;
                return result;
            }

            // Проверка заголовка
            var headers = firstLine.Split(';');
            var headerResult = ValidateHeaders(headers);
            if (!headerResult.IsValid)
            {
                allErrors.Add(headerResult.ErrorMessage);
                result.IsValid = false;
                result.Errors = allErrors;
                return result;
            }

            int rowCount = 0;
            bool hasErrors = false;

            while (!reader.EndOfStream)
            {
                var line = await reader.ReadLineAsync();
                if (string.IsNullOrWhiteSpace(line)) continue;

                rowCount++;

                // Проверка лимита строк
                if (rowCount > MAX_ROWS)
                {
                    allErrors.Add($"Превышен лимит строк. Максимум {MAX_ROWS} строк. Получено: {rowCount}");
                    result.IsValid = false;
                    result.Errors = allErrors;
                    return result;
                }

                // Чтение и валидация каждой строки
                var fields = line.Split(';');
                var rowResult = ValidateRow(fields, rowCount + 1);

                if (!rowResult.IsValid)
                {
                    hasErrors = true;
                    allErrors.AddRange(rowResult.Errors);
                }
                else
                {
                    result.ValidRecords.Add(rowResult.Record!);
                }
            }
            // Проверка наличия данных
            if (rowCount < 1)
            {
                allErrors.Add("Файл не содержит данных");
                result.IsValid = false;
                result.Errors = allErrors;
                return result;
            }
            // Если ошибки есть 
            if (hasErrors)
            {
                result.IsValid = false;
                result.Errors = allErrors;
                result.ValidRecords.Clear();
                return result;
            }

            // Если ошибок нет
            result.IsValid = true;
            result.Errors = allErrors;

            return result;
        }
    }
}
