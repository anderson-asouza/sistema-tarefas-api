using SistemaTarefas.DTO.Request;
using SistemaTarefas.DTO.Response;
using SistemaTarefas.Models;

namespace SistemaTarefas.Repositorios.Interfaces
{
    public interface ITarefasRepositorio
    {
        Task<List<TarefaResponse>> BuscarVarios(string? statusBusca = null, string? nomeTarefa = null, bool ordenarPelaDataInicial = false);
        Task<TarefaResponse> BuscarUm(int idTarefa, string? nomeTarefa = null);
        Task<TarefaResponse> Cadastrar(TarefaRequest tarefaRequest, int usuarioID);
        Task<TarefaResponse> Atualizar(TarefaUpdRequest tarefaUpdRequest, int idTarefa);
        Task<ResponseModel> FecharTarefa(int idTarefa);
        Task<ResponseModel> AtivarDesativarTarefa(int idTarefa, bool ativar = false);
        Task<ResponseModel> Apagar(int idTarefa);
        Task<ResponseModel> MarcarFlagNaTarefa(int idTarefa, int idFlag, int usuarioID);
        Task<ResponseModel> AjustarPrazoTarefa(int idTarefa, int ajusteEmDias);
    }
}
