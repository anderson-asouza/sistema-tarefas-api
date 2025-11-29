using AutoMapper;
using SistemaTarefas.DTO.Request;
using SistemaTarefas.DTO.Response;
using SistemaTarefas.DTO.Query;

namespace SistemaTarefas.Models
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<UsuarioRequest, Usuarios>();
            CreateMap<UsuarioUpdRequest, UsuarioRequest>();
            CreateMap<UsuarioUpdRequest, Usuarios>();
            CreateMap<Usuarios, UsuarioResponse>();
            CreateMap<Usuarios, UsuarioLoginResponse>();
            CreateMap<Usuarios, UsuarioAlterarSenhaResponse>();

            CreateMap<ModeloTarefaRequest, ModelosTarefa>();
            CreateMap<ModelosTarefa, ModeloTarefaResponse>();

            CreateMap<ModeloTramiteRequest, ModelosTramite>();
            CreateMap<ModeloTramiteUpdRequest, ModeloTramiteRequest>();
            CreateMap<ModeloTramiteUpdRequest, ModelosTramite>();
            CreateMap<ModelosTramite, ModeloTramiteResponse>();
            CreateMap<ModelosTramiteSQL, ModeloTramiteResponse>();

            CreateMap<FlagRequest, Flags>();
            CreateMap<Flags, FlagResponse>();

            CreateMap<TarefaRequest, Tarefas>();
            CreateMap<Tarefas, TarefaResponse>();
            CreateMap<TarefasSQL, TarefaResponse>();

            CreateMap<TramiteRequest, Tramites>();
            CreateMap<Tramites, TramiteResponse>();
            CreateMap<TramitesSQL, TramiteResponse>();
        }
    }
}