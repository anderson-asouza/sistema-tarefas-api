using SistemaTarefas.Enums;

namespace SistemaTarefas.DTO.Query
{
    public class ModelosTramiteSQL
    {
        public int MtraId { get; set; }
        public string MtraNomeTramite { get; set; } = string.Empty;
        public string MtraDescricaoTramite { get; set; } = string.Empty;
        public int MtraDuracaoPrevistaDias { get; set; }
        public int MtraOrdem { get; set; }

        public int MtraMtarId { get; set; }
        public string MtarNome { get; set; } = string.Empty;
        public string MtarDescricao { get; set; } = string.Empty;

        public int? MtraUsuIdRevisor { get; set; }
        public string UsuNomeRevisor { get; set; } = string.Empty;
        public int? MtraUsuIdIndicacao { get; set; }
        public string UsuNomeIndicacao { get; set; } = string.Empty;
    }
}
