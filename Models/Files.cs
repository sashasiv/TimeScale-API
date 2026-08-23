using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace infotecs.Models
{
    // Таблица с информацией о файле
    [Table("Files")]
    public class Files
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Required]
        [MaxLength(255)]
        public string FileName { get; set; } = string.Empty;

        [Required]
        public int RecordsCount { get; set; }

        //Место для дополнительной инфрмации о файле

        //

        // Связь с Values
        public virtual ICollection<Values> Values { get; set; } = new List<Values>();
    }
}
