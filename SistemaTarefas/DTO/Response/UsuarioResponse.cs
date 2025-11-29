using SistemaTarefas.Enums;

namespace SistemaTarefas.DTO.Response
{
    public class UsuarioResponse : IResponseModel
    {
        public int usuId { get; set; }
        public string usuLogin { get; set; } = string.Empty;
        public int usuNivel { get; set; }
        public string usuNome { get; set; } = string.Empty;
        public string usuEmail { get; set; } = string.Empty;
        public string usuMatricula { get; set; } = string.Empty;

        public string RM { get; set; } = string.Empty;
        public ResponseCode RC { get; set; } = ResponseCode.Nulo;
        public string errorCode { get; set; } = string.Empty;
        public bool OK { get; set; } = false;
    }
    public class UsuarioLoginResponse : IResponseModel
    {
        public int usuId { get; set; }
        public string usuLogin { get; set; } = string.Empty;
        public int usuNivel { get; set; }
        public string usuNome { get; set; } = string.Empty;
        public string usuMatricula { get; set; } = string.Empty;
        public string? usuToken { get; set; } = string.Empty;
        public string? usuImagemPerfil { get; set; } = string.Empty;

        public string RM { get; set; } = string.Empty;
        public ResponseCode RC { get; set; } = ResponseCode.Nulo;
        public string errorCode { get; set; } = string.Empty;
        public bool OK { get; set; } = false;
    }

    public class UsuarioImagemResponse : IResponseModel
    {
        public string usuImagemPerfil { get; set; } = string.Empty;
        public string RM { get; set; } = string.Empty;
        public ResponseCode RC { get; set; } = ResponseCode.Nulo;
        public string errorCode { get; set; } = string.Empty;
        public bool OK { get; set; } = false;
    }
    public class UsuarioAlterarSenhaResponse : UsuarioLoginResponse
    {
    }
}