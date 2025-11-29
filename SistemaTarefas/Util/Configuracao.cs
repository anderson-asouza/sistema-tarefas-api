namespace SistemaTarefas.Util
{
    public class ServidorConfiguracao
    {
        public string ServidorOrigem1 { get; set; } = string.Empty;
        public string ServidorOrigem2 { get; set; } = string.Empty;
        public string ChaveHttps { get; set; } = string.Empty;
        public string PathCertificadoPfx { get; set; } = string.Empty;
        public int PortaHttp { get; set; }
        public int PortaHttps { get; set; }
    }
    public class TokenConfiguracao
    {
        public string NomeEmpresa { get; set; } = string.Empty;
        public string NomeAplicacao { get; set; } = string.Empty;
        public string ChaveSecreta { get; set; } = string.Empty;
        public int TempoVidaMinutos { get; set; }
    }
    public class BancoConfiguracao
    {
        public string ConnectionString { get; set; } = string.Empty;
        public int ExpiracaoSenhaDias { get; set; }
    }
    public class CriptografiaConfiguracao
    {
        public string AES_256_32bytes { get; set; } = string.Empty;
        public string AES_16bytes { get; set; } = string.Empty;
    }
}
