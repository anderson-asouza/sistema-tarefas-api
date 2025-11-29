using SistemaTarefas.Enums;

namespace SistemaTarefas.DTO.Query
{
    public class TramitesSQL
    {
        public int TraId { get; set; }
        public int TraStatus { get; set; }
        public int TraOrdem { get; set; }
        public DateTime TraDataInicio { get; set; }
        public DateTime TraDataPrevisaoTermino { get; set; }
        public DateTime? TraDataExecucao { get; set; } = null;
        public DateTime? TraDataRevisao { get; set; } = null;
        public string? TraNotaTramitador { get; set; }
        public string? TraNotaRevisor { get; set; }
        public bool TraRepetido { get; set; } = false;
        public int? TraUsuIdRevisor { get; set; }
        public int? TraUsuIdTramitador { get; set; }        
        public int TraTarId { get; set; }
        public int TraMtraId { get; set; }
        public string? UsuNomeResponsavel { get; set; } = null;
        public string? UsuNomeTramitador { get; set; } = null;
        public string? UsuNomeRevisor { get; set; } = null;
        public int TarMtarId { get; set; }
        public string? MtarNome { get; set; }
        public string MtarDescricao { get; set; } = string.Empty;
        public int? tarUsuIdResponsavelTarefa { get; set; }
        public string? TarNomeTarefa { get; set; }
        public string? TarDescricao { get; set; }
        public string? TarStatus { get; set; }
        public int? TarFlaId { get; set; }
        public string? FlaRotulo { get; set; }
        public string? FlaCor { get; set; }

        public DateTime? TarDataComeco { get; set; }
        public DateTime? TarDataFinalPrevista { get; set; }
        public string MtraNome { get; set; } = string.Empty;
        public string MtraDescricao { get; set; } = string.Empty;
    }
}
