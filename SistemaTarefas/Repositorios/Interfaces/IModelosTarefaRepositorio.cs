using SistemaTarefas.DTO.Request;
using SistemaTarefas.DTO.Response;
using SistemaTarefas.Models;

namespace SistemaTarefas.Repositorios.Interfaces
{
    public interface IModelosTarefaRepositorio
    {
        Task<List<ModeloTarefaResponse>> BuscarVarios(string? nomeModeloTarefa = null);
        Task<ModeloTarefaResponse> BuscarUm(int idModeloTarefa, string? nomeModeloTarefa = null);
        Task<ModeloTarefaResponse> Cadastrar(ModeloTarefaRequest modeloTarefaRequest);
        Task<ModeloTarefaResponse> Atualizar(ModeloTarefaRequest modeloTarefaRequest, int idModeloTarefa);
        Task<ResponseModel> Apagar(int id);
    }
}
