using SistemaTarefas.Enums;
using SistemaTarefas.Models;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

public partial class Tramites
{
    [Key]
    [Column("TRA_ID")]
    public int TraId { get; set; }

    [Column("TRA_Status")]
    public StatusTramite TraStatus { get; set; } = StatusTramite.AFazer;

    [Required]
    [Column("TRA_Ordem")]
    public int TraOrdem { get; set; }

    [Column("TRA_DataInicio")]
    public DateTime TraDataInicio { get; set; }

    [Column("TRA_DataPrevisaoTermino")]
    public DateTime TraDataPrevisaoTermino { get; set; }

    [Column("TRA_DataExecucao")]
    public DateTime? TraDataExecucao { get; set; } = null;

    [Column("TRA_DataRevisao")]
    public DateTime? TraDataRevisao { get; set; } = null;

    [Column("TRA_USU_ID_Tramitador")]
    public int? TraUsuIdTramitador { get; set; }

    [Column("TRA_USU_ID_Revisor")]
    public int? TraUsuIdRevisor { get; set; }

    [Column("TRA_NotaTramitador")]
    public string? TraNotaTramitador { get; set; }

    [Column("TRA_NotaRevisor")]
    public string? TraNotaRevisor { get; set; }

    [Column("TRA_TramiteRepetido")]
    public bool TraRepetido { get; set; } = false;

    [Required]
    [Column("TRA_TAR_ID")]
    public int TraTarId { get; set; }

    [Required]
    [Column("TRA_TTT_ID")]
    public int TraMtraId { get; set; }

    [ForeignKey(nameof(TraTarId))]
    public virtual Tarefas TraTarIdNavigation { get; set; } = null!;

    [ForeignKey(nameof(TraMtraId))]
    public virtual ModelosTramite TraTttIdNavigation { get; set; } = null!;

    [ForeignKey(nameof(TraUsuIdTramitador))]
    public virtual Usuarios? TraUsuIdTramitadorNavigation { get; set; }

    [ForeignKey(nameof(TraUsuIdRevisor))]
    public virtual Usuarios? TraUsuIdRevisorNavigation { get; set; }
}
