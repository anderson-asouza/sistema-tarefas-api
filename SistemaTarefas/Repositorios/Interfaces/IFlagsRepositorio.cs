using SistemaTarefas.DTO.Request;
using SistemaTarefas.DTO.Response;
using SistemaTarefas.Models;

namespace SistemaTarefas.Repositorios.Interfaces
{
    public interface IFlagsRepositorio
    {
        Task<List<FlagResponse>> BuscarVarios(string? rotuloFlag = null);
        Task<FlagResponse> BuscarUm(int id, string? rotuloFlag = null);
        Task<FlagResponse> Cadastrar(FlagRequest flagRequest);
        Task<FlagResponse> Atualizar(FlagRequest flagRequest, int id);
        Task<ResponseModel> Apagar(int id);
    }
}
