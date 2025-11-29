using SistemaTarefas.DTO.Response;
using SistemaTarefas.Enums;

namespace SistemaTarefas.Models;
    
public partial class TarefaResponse : IResponseModel
{
    public int TarId { get; set; }
    public string TarNomeTarefa { get; set; } = string.Empty;
    public string TarDescricao { get; set; } = string.Empty;
    public DateTime TarDataComeco { get; set; }
    public DateTime TarDataFinalPrevista { get; set; }
    public DateTime? TarDataFinal { get; set; }
    public string? TarStatus { get; set; }
    public string? TarStatusDescricao => StatusTarefaExtensions.GetTarefaDescricao(TarStatus ?? "", out var status) ? status.ToDescricao() : null;
    public int TarMtarId { get; set; }
    public string? MtarNome { get; set; }
    public string? MtarDescricao { get; set; }
    public int TarUsuIdResponsavelTarefa { get; set; }
    public string? UsuNomeUsuarioResponsavelTarefa { get; set; }

    public int TarFlaId { get; set; }
    public string? FlaRotulo { get; set; }
    public string? FlaCor { get; set; }

    public string RM { get; set; } = string.Empty;
    public ResponseCode RC { get; set; } = ResponseCode.Nulo;
    public string errorCode { get; set; } = string.Empty;
    public bool OK { get; set; } = false;
}
