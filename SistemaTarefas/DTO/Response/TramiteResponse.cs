using SistemaTarefas.Enums;
using SistemaTarefas.Util;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SistemaTarefas.DTO.Response
{
    public class TramiteResponse : IResponseModel
    {
        public int TraId { get; set; }
        public int TraStatus { get; set; }
        public string TraStatusDescricao
        {
            get
            {
                if (Enum.IsDefined(typeof(StatusTramite), TraStatus))
                {
                    var statusEnum = (StatusTramite)TraStatus;
                    return statusEnum.GetDescriptionTramite();
                }
                return "Status Desconhecido";
            }
        }

        public int TraOrdem { get; set; }
        public DateTime TraDataInicio { get; set; }
        public DateTime TraDataPrevisaoTermino { get; set; }
        public DateTime? TraDataExecucao { get; set; } = null;
        public DateTime? TraDataRevisao { get; set; } = null;
        public string? TraNotaTramitador { get; set; }
        public string? TraNotaRevisor { get; set; }
        public bool TraRepetido { get; set; } = false;

        public int tarUsuIdResponsavelTarefa { get; set; }
        public string? UsuNomeResponsavel { get; set; } = null;

        public int TraUsuIdRevisor { get; set; }
        public string? UsuNomeRevisor { get; set; } = null;

        public int TraUsuIdTramitador { get; set; }
        public string? UsuNomeTramitador { get; set; } = null;

        public int TraTarId { get; set; }
        public string? TarNomeTarefa { get; set; }
        public string? TarDescricao { get; set; }
        public string? TarStatus { get; set; }
        public string? TarStatusDescricao => StatusTarefaExtensions.GetTarefaDescricao(TarStatus ?? "", out var status) ? status.ToDescricao() : null;
        public DateTime TarDataComeco { get; set; }
        public DateTime? TarDataFinalPrevista { get; set; }

        public int TarMtarId { get; set; }
        public string? MtarNome { get; set; }
        public string? MtarDescricao { get; set; }

        public int TraMtraId { get; set; }
        public string MtraNome { get; set; } = string.Empty;
        public string MtraDescricao { get; set; } = string.Empty;

        public int? TarFlaId { get; set; }
        public string? FlaRotulo { get; set; }
        public string? FlaCor { get; set; }

        public string RM { get; set; } = string.Empty;
        public ResponseCode RC { get; set; } = ResponseCode.Nulo;
        public string errorCode { get; set; } = string.Empty;
        public bool OK { get; set; } = false;
    }

    public class IncluirTramiteResponse : IResponseModel
    {
        public int NovoTramiteId { get; set; }
        public string RM { get; set; } = string.Empty;
        public ResponseCode RC { get; set; } = ResponseCode.Nulo;
        public string errorCode { get; set; } = string.Empty;
        public bool OK { get; set; } = false;
    }
}
