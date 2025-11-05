using SistemaTarefas.DTO.Response;
using SistemaTarefas.Enums;
using System.ComponentModel.DataAnnotations;

namespace SistemaTarefas.DTO.Request
{
    public class UsuarioRequest : IRequestModel
    {
        public string usuLogin { get; set; } = string.Empty;
        public string UsuSenha { get; set; } = string.Empty;
        public int usuNivel { get; set; }
        public string usuNome { get; set; } = string.Empty;
        public string usuEmail { get; set; } = string.Empty;
        public string usuMatricula { get; set; } = string.Empty;
    }

    public class UsuarioUpdRequest : IRequestModel
    {
        public int usuNivel { get; set; }
        public string usuNome { get; set; } = string.Empty;
        public string usuEmail { get; set; } = string.Empty;
        public string usuMatricula { get; set; } = string.Empty;
    }
    public class UsuarioLoginRequest : IRequestModel
    {
        public string Login { get; set; } = string.Empty;
        public string Senha { get; set; } = string.Empty;
        public string? NovaSenha { get; set; } = null;
        public string? ConfirmacaoNovaSenha { get; set; } = null;
    }

    public class UploadImagemPerfilRequest
    {
        [Required]
        public IFormFile? Imagem { get; set; }
    }
    public class UsuarioAlterarSenhaRequest : UsuarioLoginRequest
    {
    }
}