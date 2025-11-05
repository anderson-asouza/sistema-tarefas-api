using SistemaTarefas.DTO.Request;
using SistemaTarefas.DTO.Response;
using SistemaTarefas.Models;

namespace SistemaTarefas.Repositorios.Interfaces
{
    public interface IModelosTramiteRepositorio
    {
        Task<List<ModeloTramiteResponse>> BuscarVarios(int idModeloTarefa = 0, string? nomeModeloTarefa = null);
        Task<ModeloTramiteResponse> BuscarUm(int idModeloTramite);
        Task<ModeloTramiteResponse> Cadastrar(ModeloTramiteRequest modeloTramiteRequest);
        Task<ModeloTramiteResponse> Atualizar(ModeloTramiteUpdRequest modeloTramiteRequest, int idModeloTramite);
        Task<ResponseModel> Apagar(int idModeloTramite);
    }
}
