using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SistemaTarefas.Models;

public partial class ModelosTarefa
{
    [Key]
    [Column("MTAR_ID")]
    public int MtarId { get; set; }

    [Required]
    [Column("MTAR_Nome")]
    public string MtarNome { get; set; } = string.Empty;

    [Column("MTAR_Descricao")]
    public string MtarDescricao { get; set; } = string.Empty;

    public virtual ICollection<Tarefas> Tarefas { get; set; } = new List<Tarefas>();    
    public virtual ICollection<ModelosTramite> ModelosTramite { get; set; } = new List<ModelosTramite>();
}
