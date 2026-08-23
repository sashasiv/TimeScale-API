using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace infotecs.Models
{
    [Table("Values")]
    public class Values
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Required]
        [ForeignKey(nameof(File))]
        public int FileId { get; set; }

        // Связь с Files
        public virtual Files File { get; set; } = null!;

        [Required]
        public DateTimeOffset Date { get; set; }

        [Required]
        public double ExecutionTime { get; set; }

        [Required]
        public double Value { get; set; }
    }
}
