using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using SistemaTarefas.Controllers;
using SistemaTarefas.DTO.Request;
using SistemaTarefas.DTO.Response;
using SistemaTarefas.Enums;
using SistemaTarefas.Models;
using SistemaTarefas.Repositorios;
using SistemaTarefas.Repositorios.Interfaces;
using SistemaTarefas.Servicos;
using SistemaTarefas.Util;
using System;
using System.Diagnostics.Eventing.Reader;
using System.Globalization;
using System.IdentityModel.Tokens.Jwt;
using System.Text;

namespace SistemaTarefas.Controllers
{
    //[Microsoft.AspNetCore.Authorization.Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class TramitesController(ITramitesRepositorio tramitesRepositorio) : ControllerBase
    {
        private readonly ITramitesRepositorio _tramitesRepositorio = tramitesRepositorio;

        [Authorize(Policy = "NivelAcesso1a4")]
        [HttpGet]
        [ProducesResponseType(typeof(List<TramiteResponse>), 200)]
        [ProducesResponseType(typeof(List<TramiteResponse>), 400)]
        [ProducesResponseType(typeof(List<TramiteResponse>), 403)]
        [ProducesResponseType(typeof(List<TramiteResponse>), 404)]
        [ProducesResponseType(typeof(List<TramiteResponse>), 500)]
        public async Task<ActionResult<List<TramiteResponse>>> BuscarVarios([FromQuery] int idTarefa = 0, [FromQuery] int statusTramite = 0, [FromQuery] string? statusTarefa = null, [FromQuery] bool ordenarPelaDataComecoTarefa = false)
        {           
            try
            {
                List<TramiteResponse> tramite = new List<TramiteResponse>();

                if (statusTramite != 0 && !Enum.IsDefined(typeof(StatusTramite), statusTramite))
                {
                    int maxStatus = Enum.GetValues(typeof(StatusTramite))
                    .Cast<int>()
                    .Max();

                    return Controladores.RetornoLista(this, tramite, ResponseCode.BadRequest, $"Parâmetro 'Status Trâmite' inválido. Use 0 (todos) ou um valor entre 1 e {maxStatus}.");
                }

                if (!StatusTarefaExtensions.CodigoStatusTarefaValido(statusTarefa))
                    return Controladores.RetornoLista(this, tramite, ResponseCode.BadRequest, "Parâmetro 'Status Tarefa' inválido.");

                tramite = await _tramitesRepositorio.BuscarVarios(idTarefa, statusTramite, statusTarefa, ordenarPelaDataComecoTarefa);

                return Controladores.RetornoLista(this, tramite);
            }
            catch (Exception ex)
            {
                Servico.GravaLog($"{nameof(TramitesController)}.{nameof(BuscarVarios)}", ex);
                return new List<TramiteResponse>
                {
                    new TramiteResponse
                    {
                        RM = Servico.MSG_EXCEPTION,
                        RC = ResponseCode.Excecao,
                        OK = false
                    }
                };
            }
        }

        [Authorize(Policy = "NivelAcesso1a4")]
        [HttpGet("{idTramite}")]
        [ProducesResponseType(typeof(TramiteResponse), 200)]
        [ProducesResponseType(typeof(TramiteResponse), 400)]
        [ProducesResponseType(typeof(TramiteResponse), 403)]
        [ProducesResponseType(typeof(TramiteResponse), 404)]
        [ProducesResponseType(typeof(TramiteResponse), 500)]
        public async Task<ActionResult<TramiteResponse>> BuscarUm([FromRoute] int idTramite)
        {           
            try
            {
                TramiteResponse tramite = new();

                if (idTramite < 1)
                {
                    return Controladores.Retorno(this, tramite, ResponseCode.BadRequest, "Parâmetro de busca incorreto.");
                }

                tramite = await _tramitesRepositorio.BuscarUm(idTramite);
                return Controladores.Retorno(this, tramite);
            }
            catch (Exception ex)
            {
                Servico.GravaLog($"{nameof(TramitesController)}.{nameof(BuscarUm)}", ex);                
                return Controladores.Retorno(this, new TramiteResponse
                {
                    RM = Servico.MSG_EXCEPTION,
                    RC = ResponseCode.Excecao,
                    OK = false
                });
            }
        }

        [Authorize(Policy = "NivelAcesso1a4")]
        [HttpGet("buscar-tramites-usuario")]
        [ProducesResponseType(typeof(List<TramiteResponse>), 200)]
        [ProducesResponseType(typeof(List<TramiteResponse>), 400)]
        [ProducesResponseType(typeof(List<TramiteResponse>), 403)]
        [ProducesResponseType(typeof(List<TramiteResponse>), 404)]
        [ProducesResponseType(typeof(List<TramiteResponse>), 500)]
        public async Task<ActionResult<List<TramiteResponse>>> BuscarTramitesUsuario([FromQuery] bool ordenarDataComeco = false, [FromQuery] int idUsuario = 0, [FromQuery] string? statusTarefa = "A")
        {
            try
            {
                int id = (idUsuario == 0) ? Servico.Claims().usuarioID : idUsuario;

                if (id < 1)
                {
                    return new List<TramiteResponse>
                    {
                        new TramiteResponse
                        {
                            RM = "Id de usuário é inválido.",
                            RC = ResponseCode.BadRequest,
                            OK = false
                        }
                    };
                }

                List<TramiteResponse> tramite;
                tramite = await _tramitesRepositorio.BuscarTramitesPorTipoDeUsuario(id, TipoUsuarioTramite.Todos, ordenarDataComeco, statusTarefa);

                return Controladores.RetornoLista(this, tramite);
            }
            catch (Exception ex)
            {
                Servico.GravaLog($"{nameof(TramitesController)}.{nameof(BuscarTramitesUsuario)}", ex);
                return new List<TramiteResponse>
                {
                    new TramiteResponse
                    {
                        RM = Servico.MSG_EXCEPTION,
                        RC = ResponseCode.Excecao,
                        OK = false
                    }
                };
            }
        }

        [Authorize(Policy = "NivelAcesso1a4")]
        [HttpGet("buscar-tramites-responsavel")]
        [ProducesResponseType(typeof(List<TramiteResponse>), 200)]
        [ProducesResponseType(typeof(List<TramiteResponse>), 400)]
        [ProducesResponseType(typeof(List<TramiteResponse>), 403)]
        [ProducesResponseType(typeof(List<TramiteResponse>), 404)]
        [ProducesResponseType(typeof(List<TramiteResponse>), 500)]
        public async Task<ActionResult<List<TramiteResponse>>> BuscarTramitesResponsavel([FromQuery] bool ordenarDataComeco = false, [FromQuery] int idUsuario = 0, [FromQuery] string? statusTarefa = "A")
        {
            try
            {
                int id = (idUsuario == 0) ? Servico.Claims().usuarioID : idUsuario;

                if (id < 1)
                {
                    return new List<TramiteResponse>
                    {
                        new TramiteResponse
                        {
                            RM = "Id de usuário é inválido.",
                            RC = ResponseCode.BadRequest,
                            OK = false
                        }
                    };
                }

                List<TramiteResponse> tramite;
                tramite = await _tramitesRepositorio.BuscarTramitesPorTipoDeUsuario(id, TipoUsuarioTramite.Responsavel, ordenarDataComeco, statusTarefa);

                return Controladores.RetornoLista(this, tramite);
            }
            catch (Exception ex)
            {
                Servico.GravaLog($"{nameof(TramitesController)}.{nameof(BuscarTramitesRevisor)}", ex);
                return new List<TramiteResponse>
                {
                    new TramiteResponse
                    {
                        RM = Servico.MSG_EXCEPTION,
                        RC = ResponseCode.Excecao,
                        OK = false
                    }
                };
            }
        }

        [Authorize(Policy = "NivelAcesso1a4")]
        [HttpGet("buscar-tramites-revisor")]
        [ProducesResponseType(typeof(List<TramiteResponse>), 200)]
        [ProducesResponseType(typeof(List<TramiteResponse>), 400)]
        [ProducesResponseType(typeof(List<TramiteResponse>), 403)]
        [ProducesResponseType(typeof(List<TramiteResponse>), 404)]
        [ProducesResponseType(typeof(List<TramiteResponse>), 500)]
        public async Task<ActionResult<List<TramiteResponse>>> BuscarTramitesRevisor([FromQuery] bool ordenarDataComeco = false, [FromQuery] int idUsuario = 0, [FromQuery] string? statusTarefa = "A")
        {
            try
            {
                int id = (idUsuario == 0) ? Servico.Claims().usuarioID : idUsuario;

                if (id < 1)
                {
                    return new List<TramiteResponse>
                    {
                        new TramiteResponse
                        {
                            RM = "Id de usuário é inválido.",
                            RC = ResponseCode.BadRequest,
                            OK = false
                        }
                    };
                }

                List<TramiteResponse> tramite;
                tramite = await _tramitesRepositorio.BuscarTramitesPorTipoDeUsuario(id, TipoUsuarioTramite.Revisor, ordenarDataComeco, statusTarefa);

                return Controladores.RetornoLista(this, tramite);
            }
            catch (Exception ex)
            {
                Servico.GravaLog($"{nameof(TramitesController)}.{nameof(BuscarTramitesRevisor)}", ex);
                return new List<TramiteResponse>
                {
                    new TramiteResponse
                    {
                        RM = Servico.MSG_EXCEPTION,
                        RC = ResponseCode.Excecao,
                        OK = false
                    }
                };
            }
        }

        [Authorize(Policy = "NivelAcesso1a4")]
        [HttpGet("buscar-tramites-tramitador")]
        [ProducesResponseType(typeof(List<TramiteResponse>), 200)]
        [ProducesResponseType(typeof(List<TramiteResponse>), 400)]
        [ProducesResponseType(typeof(List<TramiteResponse>), 403)]
        [ProducesResponseType(typeof(List<TramiteResponse>), 404)]
        [ProducesResponseType(typeof(List<TramiteResponse>), 500)]
        public async Task<ActionResult<List<TramiteResponse>>> BuscarTramitesTramitador([FromQuery] bool ordenarDataComeco = false, [FromQuery] int idUsuario = 0, [FromQuery] string? statusTarefa = "A")
        {
            try
            {
                int id = (idUsuario == 0) ? Servico.Claims().usuarioID : idUsuario;

                if (id < 1)
                {
                    return new List<TramiteResponse>
                    {
                        new TramiteResponse
                        {
                            RM = "Id de usuário é inválido.",
                            RC = ResponseCode.BadRequest,
                            OK = false
                        }
                    };
                }

                List<TramiteResponse> tramite;
                tramite = await _tramitesRepositorio.BuscarTramitesPorTipoDeUsuario(id, TipoUsuarioTramite.Tramitador, ordenarDataComeco, statusTarefa);

                return Controladores.RetornoLista(this, tramite);
            }
            catch (Exception ex)
            {
                Servico.GravaLog($"{nameof(TramitesController)}.{nameof(BuscarTramitesTramitador)}", ex);
                return new List<TramiteResponse>
                {
                    new TramiteResponse
                    {
                        RM = Servico.MSG_EXCEPTION,
                        RC = ResponseCode.Excecao,
                        OK = false
                    }
                };
            }
        }

        [Authorize(Policy = "NivelAcesso1")]
        [HttpPost("incluir-tramite/{idTarefa}")]
        [ProducesResponseType(typeof(ResponseModel), 200)]
        [ProducesResponseType(typeof(ResponseModel), 400)]
        [ProducesResponseType(typeof(ResponseModel), 403)]
        [ProducesResponseType(typeof(ResponseModel), 404)]
        [ProducesResponseType(typeof(ResponseModel), 409)]
        [ProducesResponseType(typeof(ResponseModel), 500)]
        public async Task<ActionResult<TramiteResponse>> IncluirTramite([FromRoute] int idTarefa)
        {           
            try
            {
                TramiteResponse retorno = new();

                if (idTarefa < 1)
                {
                    return Controladores.Retorno(this, retorno, ResponseCode.BadRequest, "Parâmetro incorreto.");
                }

                retorno = await _tramitesRepositorio.IncluirTramite(idTarefa);

                return Controladores.Retorno(this, retorno);
            }
            catch (Exception ex)
            {
                Servico.GravaLog($"{nameof(TramitesController)}.{nameof(IncluirTramite)}", ex);
                return Controladores.Retorno(this, new TramiteResponse
                {
                    RM = Servico.MSG_EXCEPTION,
                    RC = ResponseCode.Excecao,
                    OK = false
                });
            }
        }

        [Authorize(Policy = "NivelAcesso1a3")]
        [HttpPost("assumir-tramite/{idTramite}")]
        [ProducesResponseType(typeof(ResponseModel), 200)]
        [ProducesResponseType(typeof(ResponseModel), 400)]
        [ProducesResponseType(typeof(ResponseModel), 403)]
        [ProducesResponseType(typeof(ResponseModel), 404)]
        [ProducesResponseType(typeof(ResponseModel), 409)]
        [ProducesResponseType(typeof(ResponseModel), 500)]
        public async Task<ActionResult<ResponseModel>> AssumirTramite([FromRoute] int idTramite)
        {
            try
            {
                ResponseModel retorno = new();

                if (idTramite < 1)
                {
                    return Controladores.Retorno(this, retorno, ResponseCode.BadRequest, "Parâmetro incorreto.");
                }

                retorno = await _tramitesRepositorio.AssumirTramite(idTramite, Servico.Claims().usuarioID);

                return Controladores.Retorno(this, retorno);
            }
            catch (Exception ex)
            {
                Servico.GravaLog($"{nameof(TramitesController)}.{nameof(AssumirTramite)}", ex);
                return Controladores.Retorno(this, new ResponseModel
                {
                    RM = Servico.MSG_EXCEPTION,
                    RC = ResponseCode.Excecao,
                    OK = false
                });
            }
        }

        [Authorize(Policy = "NivelAcesso1a3")]
        [HttpPost("comecar-execucao-tramite/{idTramite}")]
        [ProducesResponseType(typeof(ResponseModel), 200)]
        [ProducesResponseType(typeof(ResponseModel), 400)]
        [ProducesResponseType(typeof(ResponseModel), 403)]
        [ProducesResponseType(typeof(ResponseModel), 404)]
        [ProducesResponseType(typeof(ResponseModel), 409)]
        [ProducesResponseType(typeof(ResponseModel), 500)]
        public async Task<ActionResult<ResponseModel>> ComecarExecucaoTramite([FromRoute] int idTramite)
        {
            try
            {
                ResponseModel retorno = new();

                if (idTramite < 1)
                {
                    return Controladores.Retorno(this, retorno, ResponseCode.BadRequest, "Parâmetro incorreto.");
                }

                retorno = await _tramitesRepositorio.ComecarExecucaoTramite(idTramite, Servico.Claims().usuarioID);

                return Controladores.Retorno(this, retorno);
            }
            catch (Exception ex)
            {
                Servico.GravaLog($"{nameof(TramitesController)}.{nameof(ComecarExecucaoTramite)}", ex);
                return Controladores.Retorno(this, new ResponseModel
                {
                    RM = Servico.MSG_EXCEPTION,
                    RC = ResponseCode.Excecao,
                    OK = false
                });
            }
        }

        [Authorize(Policy = "NivelAcesso1a3")]
        [HttpPost("finalizar-execucao-tramite")]
        [ProducesResponseType(typeof(ResponseModel), 200)]
        [ProducesResponseType(typeof(ResponseModel), 400)]
        [ProducesResponseType(typeof(ResponseModel), 403)]
        [ProducesResponseType(typeof(ResponseModel), 404)]
        [ProducesResponseType(typeof(ResponseModel), 409)]
        [ProducesResponseType(typeof(ResponseModel), 500)]
        public async Task<ActionResult<ResponseModel>> FinalizarExecucaoTramite([FromBody] TramiteNotaRequest tramiteNotaRequest)
        {
            try
            {
                ResponseModel retorno = new();

                if (tramiteNotaRequest.IdTramite < 1 || string.IsNullOrWhiteSpace(tramiteNotaRequest.Nota))
                {
                    return Controladores.Retorno(this, retorno, ResponseCode.BadRequest, "Parâmetro incorreto.");
                }

                if (tramiteNotaRequest.Nota.Trim().Length > Servico.TAM_NOTASDESCRICAO)
                {
                    retorno.errorCode = "MSG_EXCEDE_TAMANHO";
                    return Controladores.Retorno(this, retorno, ResponseCode.BadRequest, $"Tamanho da nota excedeu o limite de {Servico.TAM_NOTASDESCRICAO} caracteres.");
                }

                retorno = await _tramitesRepositorio.FinalizarExecucaoTramite(tramiteNotaRequest.IdTramite, tramiteNotaRequest.Nota.Trim(), Servico.Claims().usuarioID);

                return Controladores.Retorno(this, retorno);
            }
            catch (Exception ex)
            {
                Servico.GravaLog($"{nameof(TramitesController)}.{nameof(FinalizarExecucaoTramite)}", ex);
                return Controladores.Retorno(this, new ResponseModel
                {
                    RM = Servico.MSG_EXCEPTION,
                    RC = ResponseCode.Excecao,
                    OK = false
                });
            }
        }

        [Authorize(Policy = "NivelAcesso1a2")]
        [HttpPost("revisar-tramite/{aprovado}")]
        [ProducesResponseType(typeof(ResponseModel), 200)]
        [ProducesResponseType(typeof(ResponseModel), 400)]
        [ProducesResponseType(typeof(ResponseModel), 403)]
        [ProducesResponseType(typeof(ResponseModel), 404)]
        [ProducesResponseType(typeof(ResponseModel), 409)]
        [ProducesResponseType(typeof(ResponseModel), 500)]
        public async Task<ActionResult<ResponseModel>> RevisarTramite([FromRoute] bool aprovado, [FromBody] TramiteNotaRequest tramiteNotaRequest)
        {
            try
            {
                ResponseModel retorno = new();

                if (tramiteNotaRequest.IdTramite < 1 || string.IsNullOrWhiteSpace(tramiteNotaRequest.Nota))
                {
                    return Controladores.Retorno(this, retorno, ResponseCode.BadRequest, "Parâmetro incorreto.");
                }

                if (tramiteNotaRequest.Nota.Length > Servico.TAM_NOTASDESCRICAO)
                {
                    retorno.errorCode = "MSG_EXCEDE_TAMANHO";
                    return Controladores.Retorno(this, retorno, ResponseCode.BadRequest, $"Tamanho da nota excedeu o limite de {Servico.TAM_NOTASDESCRICAO} caracteres.");
                }

                retorno = await _tramitesRepositorio.RevisarTramite(aprovado, tramiteNotaRequest.IdTramite, tramiteNotaRequest.Nota.Trim(), Servico.Claims().usuarioID);

                return Controladores.Retorno(this, retorno);
            }
            catch (Exception ex)
            {
                Servico.GravaLog($"{nameof(TramitesController)}.{nameof(RevisarTramite)}", ex);
                return Controladores.Retorno(this, new ResponseModel
                {
                    RM = Servico.MSG_EXCEPTION,
                    RC = ResponseCode.Excecao,
                    OK = false
                });
            }
        }

        [Authorize(Policy = "NivelAcesso1")]
        [HttpPost("retroceder-tramite/{idTramite}")]
        [ProducesResponseType(typeof(ResponseModel), 200)]
        [ProducesResponseType(typeof(ResponseModel), 400)]
        [ProducesResponseType(typeof(ResponseModel), 403)]
        [ProducesResponseType(typeof(ResponseModel), 404)]
        [ProducesResponseType(typeof(ResponseModel), 409)]
        [ProducesResponseType(typeof(ResponseModel), 500)]
        public async Task<ActionResult<ResponseModel>> Retroceder([FromRoute] int idTramite)
        {           
            try
            {
                ResponseModel retorno = new();

                if (idTramite < 1)
                {
                    return Controladores.Retorno(this, retorno, ResponseCode.BadRequest, "Parâmetro incorreto para exclusão.");
                }

                retorno = await _tramitesRepositorio.Retroceder(idTramite);

                return Controladores.Retorno(this, retorno);
            }
            catch (Exception ex)
            {
                Servico.GravaLog($"{nameof(TramitesController)}.{nameof(Retroceder)}", ex);
                return Controladores.Retorno(this, new ResponseModel
                {
                    RM = Servico.MSG_EXCEPTION,
                    RC = ResponseCode.Excecao,
                    OK = false
                });
            }
        }
    }
}