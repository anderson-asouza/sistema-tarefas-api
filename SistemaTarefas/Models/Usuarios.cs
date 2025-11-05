using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SistemaTarefas.Models;

public partial class Usuarios
{
    [Key]
    [Column("USU_ID")]
    public int UsuId { get; set; }

    [Required]
    [Column("USU_Login")]
    public string UsuLogin { get; set; } = string.Empty;

    [Required]
    [Column("USU_Senha")]
    public string UsuSenha { get; set; } = string.Empty;

    [Column("USU_Nivel")]
    public int UsuNivel { get; set; }

    [Column("USU_Nome")]
    public string UsuNome { get; set; } = string.Empty;

    [Column("USU_Email")]
    public string UsuEmail { get; set; } = string.Empty;

    [Required]
    [Column("USU_Matricula")]
    public string UsuMatricula { get; set; } = string.Empty;

    [Required]
    [Column("USU_ImagemPerfil")]
    public string UsuImagemPerfil { get; set; } = string.Empty;

    [Required]
    [Column("USU_DataMudancaSenha")]
    public DateTime UsuDataMudancaSenha { get; set; }

    public virtual ICollection<Tarefas> Tarefas { get; set; } = new List<Tarefas>();
    public virtual ICollection<Tramites> TramitesTramitador { get; set; } = new List<Tramites>();
    public virtual ICollection<Tramites> TramitesRevisor { get; set; } = new List<Tramites>();
    public virtual ICollection<ModelosTramite> TramitesTipoTarefaIndicacao { get; set; } = new List<ModelosTramite>();
    public virtual ICollection<ModelosTramite> TramitesTipoTarefaResponsavel { get; set; } = new List<ModelosTramite>();
}