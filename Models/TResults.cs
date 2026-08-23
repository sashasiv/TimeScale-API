using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace infotecs.Models
{
    [Table("Results")]
    public class TResults
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        // Связь с Files
        [ForeignKey(nameof(File))]
        public int FileId { get; set; }
        public virtual Files File { get; set; } = null!;

        [Required]
        public double DeltaDate_seconds { get; set; }

        [Required]
        public DateTimeOffset StartDate { get; set; }

        [Required]
        public double AverageExecutionTime { get; set; }

        [Required]
        public double AverageValue { get; set; }

        [Required]
        public double MedianValue { get; set; }

        [Required]
        public double MaxValue { get; set; }

        [Required]
        public double MinValue { get; set; }
    }
}
