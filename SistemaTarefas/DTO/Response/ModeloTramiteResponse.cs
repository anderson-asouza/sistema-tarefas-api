using SistemaTarefas.Enums;

namespace SistemaTarefas.DTO.Response
{
    public class ModeloTramiteResponse : IResponseModel
    {
        public int MtraId { get; set; }
        public string MtraNomeTramite { get; set; } = string.Empty;
        public string MtraDescricaoTramite { get; set; } = string.Empty;
        public int MtraDuracaoPrevistaDias { get; set; }
        public int MtraOrdem { get; set; }

        public int MtraMtarId { get; set; }
        public string? TtarNome { get; set; }
        public string? TtarDescricao { get; set; }

        public int MtraUsuIdIndicacao { get; set; }
        public string? UsuNomeIndicacao { get; set; }
        public int MtraUsuIdRevisor { get; set; }
        public string? UsuNomeRevisor { get; set; }


        public string RM { get; set; } = string.Empty;
        public ResponseCode RC { get; set; } = ResponseCode.Nulo;
        public string errorCode { get; set; } = string.Empty;
        public bool OK { get; set; } = false;
    }
}