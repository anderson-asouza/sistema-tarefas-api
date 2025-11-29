using SistemaTarefas.DTO.Request;
using SistemaTarefas.DTO.Response;
using SistemaTarefas.Enums;
using SistemaTarefas.Models;

namespace SistemaTarefas.Repositorios.Interfaces
{
    public interface ITramitesRepositorio
    {
        Task<List<TramiteResponse>> BuscarVarios(int idTarefa, int statusTramite, string? statusTarefa, bool ordenarPelaDataComecoTarefa);
        Task<TramiteResponse> BuscarUm(int id);
        Task<List<TramiteResponse>> BuscarTramitesPorTipoDeUsuario(int idUsuario, TipoUsuarioTramite tipoUsuarioTramite, bool ordenarDataComeco, string? statusTarefa = "A");
        Task<TramiteResponse> IncluirTramite(int idTarefa);
        Task<ResponseModel> AssumirTramite(int idTramite, int idUsuario);
        Task<ResponseModel> ComecarExecucaoTramite(int idTramite, int idUsuario);
        Task<ResponseModel> FinalizarExecucaoTramite(int idTramite, string notaTramitador, int idUsuario);
        Task<ResponseModel> RevisarTramite(bool aprovado, int idTramite, string notaRevisor, int idUsuario);
        Task<ResponseModel> Retroceder(int idTramite);
    }
}