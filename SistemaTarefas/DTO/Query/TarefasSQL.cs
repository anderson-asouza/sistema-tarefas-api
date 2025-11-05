namespace SistemaTarefas.DTO.Query
{
    public class TarefasSQL
    {
        public int TarId { get; set; }
        public string TarNomeTarefa { get; set; } = string.Empty;
        public string TarDescricao { get; set; } = string.Empty;
        public DateTime TarDataComeco { get; set; }
        public DateTime TarDataFinalPrevista { get; set; }
        public DateTime? TarDataFinal { get; set; }
        public string TarStatus { get; set; } = string.Empty;

        public int? TarFlaId { get; set; }
        public string? FlaRotulo { get; set; }
        public string? FlaCor { get; set; }

        public int TarMtarId { get; set; }
        public string MtarNome { get; set; } = string.Empty;
        public string MtarDescricao { get; set; } = string.Empty;
        public int TarUsuIdResponsavelTarefa { get; set; }
        public string UsuNomeUsuarioResponsavelTarefa { get; set; } = string.Empty;
    }
}
