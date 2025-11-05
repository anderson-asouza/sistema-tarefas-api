using SistemaTarefas.Enums;

namespace SistemaTarefas.DTO.Request
{
    public class ModeloTramiteRequest : IRequestModel
    {
        public string MtraNomeTramite { get; set; } = string.Empty;
        public string MtraDescricaoTramite { get; set; } = string.Empty;
        public int MtraDuracaoPrevistaDias { get; set; }
        public int MtraUsuIdRevisor { get; set; }
        public int MtraUsuIdIndicacao { get; set; }
        public int MtraMtarId { get; set; }
    }

    public class ModeloTramiteUpdRequest : IRequestModel
    {
        public string MtraNomeTramite { get; set; } = string.Empty;
        public string MtraDescricaoTramite { get; set; } = string.Empty;
        public int MtraDuracaoPrevistaDias { get; set; }
        public int MtraUsuIdRevisor { get; set; }
        public int MtraUsuIdIndicacao { get; set; }
    }
}