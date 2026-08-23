namespace infotecs.Models.DTOs
{
    public class CsvRecordDto
    {
        public DateTimeOffset Date { get; set; }
        public double ExecutionTime { get; set; }
        public double Value { get; set; }
    }
}
