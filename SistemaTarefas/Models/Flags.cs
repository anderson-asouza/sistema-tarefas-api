using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SistemaTarefas.Models
{
    [Table("Flags")]
    public class Flags
    {
        [Key]
        [Column("FLA_ID")]
        public int FlaId { get; set; }

        [Required]
        [Column("FLA_Rotulo")]
        public string FlaRotulo { get; set; } = string.Empty;

        [Column("FLA_Cor")]
        public string FlaCor { get; set; } = "#FFFFFF";

        public virtual ICollection<Tarefas> Tarefas { get; set; } = new List<Tarefas>();
    }
}
