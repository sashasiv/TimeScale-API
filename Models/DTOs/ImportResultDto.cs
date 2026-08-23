namespace infotecs.Models.DTOs
{
    public class ImportResultDto
    {
        public string FileName { get; set; } = string.Empty;
        public bool IsSuccess { get; set; }
        public List<string> Errors { get; set; } = new();
    }
}
