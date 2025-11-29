using SistemaTarefas.DTO.Request;
using SistemaTarefas.DTO.Response;
using SistemaTarefas.Models;

namespace SistemaTarefas.Repositorios.Interfaces
{
    public interface IUsuariosRepositorio
    {
        Task<List<UsuarioResponse>> BuscarVarios(string? nomeUsuario = null);
        Task<UsuarioResponse> BuscarUm(int idUsuario, string? nomeUsuario = null, string? matriculaUsuario = null, string? login = null);
        Task<UsuarioResponse> Cadastrar(UsuarioRequest usuarioRequest);
        Task<UsuarioResponse> Atualizar(UsuarioUpdRequest usuarioRequest, int idUsuario);
        Task<ResponseModel> Apagar(int idUsuario);
        Task<UsuarioAlterarSenhaResponse> AlterarSenha(string Login, string SenhaAtual, string NovaSenha, string ConfirmacaoNovaSenha, bool trocarSenhaPeloAdm = false);
        Task<UsuarioLoginResponse> Logar(string Login, string Senha, string? NovaSenha = null, string? ConfirmacaoNovaSenha = null);
        Task<UsuarioImagemResponse> UploadImagemPerfil(UploadImagemPerfilRequest arq, int usuarioID);
        Task<ResponseModel> RemoverImagemPerfil(int usuarioID);
        Task<bool> ExisteAlgumAdmin();
    }
}
