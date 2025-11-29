using SistemaTarefas.Enums;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SistemaTarefas.Models;
public partial class Tarefas
{
    [Key]
    [Column("TAR_ID")]
    public int TarId { get; set; }

    [Required]
    [Column("TAR_Nome")]
    public string TarNomeTarefa { get; set; } = string.Empty;

    [Column("TAR_Descricao")]
    public string? TarDescricao { get; set; }

    [Required]
    [Column("TAR_DataComeco")]
    public DateTime TarDataComeco { get; set; }

    [Column("TAR_DataFinalPrevista")]
    public DateTime? TarDataFinalPrevista { get; set; }

    [Column("TAR_DataFinal")]
    public DateTime? TarDataFinal { get; set; }

    [Required]
    [Column("TAR_Status")]
    public StatusTarefa TarStatus { get; set; } = StatusTarefa.Aberta;

    [Column("TAR_FLA_ID")]
    public int? TarFlaId { get; set; }

    [Required]
    [Column("TAR_TTAR_ID")]
    public int TarMtarId { get; set; }

    [Required]
    [Column("TAR_USU_ID_Responsavel")]
    public int TarUsuIdResponsavelTarefa { get; set; }


    public virtual ModelosTarefa? TarMtar { get; set; }

    [ForeignKey(nameof(TarUsuIdResponsavelTarefa))]
    public virtual Usuarios TarUsuIdResponsavelTarefaNavigation { get; set; } = null!;

    [ForeignKey(nameof(TarFlaId))]
    public virtual Flags? TarFlaIdNavigation { get; set; }

    public virtual ICollection<Tramites> Tramites { get; set; } = new List<Tramites>();
}