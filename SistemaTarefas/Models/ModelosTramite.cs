using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SistemaTarefas.Models;

public partial class ModelosTramite
{
    [Key]
    [Column("MTRA_ID")]
    public int MtraId { get; set; }
    [Required]
    [Column("MTRA_NomeTramite")]
    public string MtraNomeTramite { get; set; } = string.Empty;
    [Column("MTRA_DescricaoTramite")]
    public string MtraDescricaoTramite { get; set; } = string.Empty;
    [Required]
    [Column("MTRA_MTAR_ID")]
    public int MtraMtarId { get; set; }

    [Column("MTRA_USU_ID_Indicacao")]
    public int? MtraUsuIdIndicacao { get; set; }

    [Column("MTRA_USU_ID_Revisor")]
    public int? MtraUsuIdRevisor { get; set; }

    [Column("MTRA_DuracaoPrevistaDias")]
    public int MtraDuracaoPrevistaDias { get; set; }
    [Required]
    [Column("MTRA_Ordem")]
    public int MtraOrdem { get; set; }

    public virtual Tramites? TraMtar { get; set; } = null!;
    //public virtual ModelosTramite? MtraMtarNavigation { get; set; }
    public virtual ModelosTarefa? MtraMtarNavigation { get; set; }
    
    //public virtual ICollection<ModelosTramite> TramitesTiposTarefa { get; set; } = new List<ModelosTramite>();
    public virtual ICollection<Tramites> Tramites { get; set; } = new List<Tramites>();
    public virtual Usuarios? MtraUsuIdIndicacaoNavigation { get; set; }
    public virtual Usuarios? MtraUsuIdResponsavelNavigation { get; set; }

}