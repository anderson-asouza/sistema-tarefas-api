using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using SistemaTarefas.Controllers;
using SistemaTarefas.DTO.Request;
using SistemaTarefas.DTO.Response;
using SistemaTarefas.Enums;
using SistemaTarefas.Models;
using SistemaTarefas.Repositorios.Interfaces;
using SistemaTarefas.Servicos;
using SistemaTarefas.Util;
using System;
using System.Collections.Generic;
using System.Diagnostics.Eventing.Reader;
using System.Globalization;
using System.IdentityModel.Tokens.Jwt;
using System.Text;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace SistemaTarefas.Controllers
{
    //[Microsoft.AspNetCore.Authorization.Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class ModelosTramiteController : ControllerBase
    {
        private readonly IModelosTramiteRepositorio _TramitesModelosTarefaRepositorio;
        private readonly IMapper _mapper;
        public ModelosTramiteController(IModelosTramiteRepositorio TramitesModelosTarefaRepositorio, IMapper mapper)
        {
            _TramitesModelosTarefaRepositorio = TramitesModelosTarefaRepositorio;
            _mapper = mapper;
        }
        private static bool ValidaRequisicao(ModeloTramiteRequest modeloResquet, ModeloTramiteResponse resposta, bool verificaMtraMtarId = true)
        {
            try
            {
                List<string> erros = new();
                List<string> errosCode = new();


                if (string.IsNullOrWhiteSpace(modeloResquet.MtraNomeTramite))
                {
                    erros.Add("Deve informar um Nome para Modelo Trâmite.");
                    errosCode.Add("MODELOS_TRAMITE_NOME_OBRIGATORIO");
                }

                if (modeloResquet.MtraDuracaoPrevistaDias < 1)
                {
                    erros.Add("Deve informar da duração para o Modelo Trâmite.");
                    errosCode.Add("MODELOS_TRAMITE_DURACAO_OBRIGATORIA");
                }

                if (verificaMtraMtarId && modeloResquet.MtraMtarId < 1)
                {
                    erros.Add("Deve informar um Modelo Tarefa.");
                    errosCode.Add("MODELOS_TRAMITE_OBRIGATORIO_MODELO_TAREFA");
                }

                if (modeloResquet.MtraUsuIdRevisor > 0 && modeloResquet.MtraUsuIdRevisor == modeloResquet.MtraUsuIdIndicacao)
                {
                    erros.Add("Usuário Revisor deve ser diferente do Tramitador.");
                    errosCode.Add("MODELOS_TRAMITE_REVISOR_TRAMITADOR_IGUAIS");
                }

                if (erros.Any())
                {
                    resposta.RM = string.Join(Environment.NewLine, erros);
                    resposta.errorCode = string.Join(", ", errosCode);
                    resposta.RC = ResponseCode.BadRequest;
                    return false;
                }

                return true;
            }
            catch (Exception ex)
            {
                Servico.GravaLog($"{nameof(ModelosTramiteController)}.{nameof(ValidaRequisicao)}", ex);
                resposta.RM = Servico.MSG_EXCEPTION;
                resposta.errorCode = "MSG_EXCEPTION";
                resposta.RC = ResponseCode.Excecao;
                return false;
            }
        }

        [Authorize(Policy = "NivelAcesso1a2")]
        [HttpGet]
        [ProducesResponseType(typeof(List<ModeloTramiteResponse>), 200)]
        [ProducesResponseType(typeof(List<ModeloTramiteResponse>), 400)]
        [ProducesResponseType(typeof(List<ModeloTramiteResponse>), 403)]
        [ProducesResponseType(typeof(List<ModeloTramiteResponse>), 404)]
        [ProducesResponseType(typeof(List<ModeloTramiteResponse>), 500)]
        public async Task<ActionResult<List<ModeloTramiteResponse>>> BuscarVarios([FromQuery] int idModeloTarefa = 0, [FromQuery] string? nomeModeloTarefa = null)
        {
            try
            {
                List<ModeloTramiteResponse> tramitesModelosTarefa = await _TramitesModelosTarefaRepositorio.BuscarVarios(idModeloTarefa, nomeModeloTarefa);

                return Controladores.RetornoLista(this, tramitesModelosTarefa);
            }
            catch (Exception ex)
            {
                Servico.GravaLog($"{nameof(ModelosTramiteController)}.{nameof(BuscarVarios)}", ex);
                return new List<ModeloTramiteResponse>
                {
                    new ModeloTramiteResponse
                    {
                        RM = Servico.MSG_EXCEPTION,
                        RC = ResponseCode.Excecao,
                        OK = false
                    }
                };
            }
        }

        [Authorize(Policy = "NivelAcesso1a2")]
        [HttpGet("{idModeloTramite}")]
        [ProducesResponseType(typeof(ModeloTramiteResponse), 200)]
        [ProducesResponseType(typeof(ModeloTramiteResponse), 400)]
        [ProducesResponseType(typeof(ModeloTramiteResponse), 403)]
        [ProducesResponseType(typeof(ModeloTramiteResponse), 404)]
        [ProducesResponseType(typeof(ModeloTramiteResponse), 500)]
        public async Task<ActionResult<ModeloTramiteResponse>> BuscarUm(int idModeloTramite)
        {
            ModeloTramiteResponse tramiteModeloTarefa = new ModeloTramiteResponse();

            if (idModeloTramite < 1)
            {
                return Controladores.Retorno(this, tramiteModeloTarefa, ResponseCode.BadRequest, "Parâmetro incorreto para Buscar um Trâmite.");
            }

            try
            {
                tramiteModeloTarefa = await _TramitesModelosTarefaRepositorio.BuscarUm(idModeloTramite);
                return Controladores.Retorno(this, tramiteModeloTarefa);
            }
            catch (Exception ex)
            {
                Servico.GravaLog($"{nameof(ModelosTramiteController)}.{nameof(BuscarUm)}", ex);
                return new ModeloTramiteResponse
                {
                    RM = Servico.MSG_EXCEPTION,
                    RC = ResponseCode.Excecao,
                    OK = false
                };
            }
        }

        [Authorize(Policy = "NivelAcesso1")]
        [HttpPost]
        [ProducesResponseType(typeof(ModeloTramiteResponse), 200)]
        [ProducesResponseType(typeof(ModeloTramiteResponse), 400)]
        [ProducesResponseType(typeof(ModeloTramiteResponse), 403)]
        [ProducesResponseType(typeof(ModeloTramiteResponse), 404)]
        [ProducesResponseType(typeof(ModeloTramiteResponse), 409)]
        [ProducesResponseType(typeof(ModeloTramiteResponse), 500)]
        public async Task<ActionResult<ModeloTramiteResponse>> Cadastrar([FromBody] ModeloTramiteRequest modeloTramiteRequest)
        {
            try
            {
                ModeloTramiteResponse resposta = new ModeloTramiteResponse();

                if (!ValidaRequisicao(modeloTramiteRequest, resposta))
                {
                    return Controladores.Retorno(this, resposta);
                }                

                resposta = await _TramitesModelosTarefaRepositorio.Cadastrar(modeloTramiteRequest);

                string? uri = Url.Action(nameof(BuscarUm), "TramitesModelosTarefa", new { id = resposta.MtraId });

                return Controladores.Retorno(this, resposta, ResponseCode.CadastradoSucesso, "", uri);
            }
            catch (Exception ex)
            {
                Servico.GravaLog($"{nameof(ModelosTramiteController)}.{nameof(Cadastrar)}", ex);
                return new ModeloTramiteResponse
                {
                    RM = Servico.MSG_EXCEPTION,
                    RC = ResponseCode.Excecao,
                    OK = false
                };
            }
        }

        [Authorize(Policy = "NivelAcesso1")]
        [HttpPut("{idModeloTramite}")]
        [ProducesResponseType(typeof(ModeloTramiteResponse), 200)]
        [ProducesResponseType(typeof(ModeloTramiteResponse), 400)]
        [ProducesResponseType(typeof(ModeloTramiteResponse), 403)]
        [ProducesResponseType(typeof(ModeloTramiteResponse), 404)]
        [ProducesResponseType(typeof(ModeloTramiteResponse), 409)]
        [ProducesResponseType(typeof(ModeloTramiteResponse), 500)]
        public async Task<ActionResult<ModeloTramiteResponse>> Atualizar([FromBody] ModeloTramiteUpdRequest modeloTramiteRequest, int idModeloTramite)
        {
            try
            {
                if (modeloTramiteRequest == null || idModeloTramite < 1)
                {
                    return Controladores.Retorno(this, new ModeloTramiteResponse
                    {
                        RM = "Parâmetro inválido para requisição.",
                        errorCode = "MODELOS_TRAMITE_OBRIGATORIO_MODELO_TAREFA",
                        RC = ResponseCode.BadRequest,
                        OK = false
                    });
                }

                ModeloTramiteResponse resposta = new ModeloTramiteResponse();

                if (!ValidaRequisicao(_mapper.Map<ModeloTramiteRequest>(modeloTramiteRequest), resposta, false))
                {
                    return Controladores.Retorno(this, resposta);
                }

                resposta = await _TramitesModelosTarefaRepositorio.Atualizar(modeloTramiteRequest, idModeloTramite);

                return Controladores.Retorno(this, resposta);
            }
            catch (Exception ex)
            {
                Servico.GravaLog($"{nameof(ModelosTramiteController)}.{nameof(Atualizar)}", ex);
                return new ModeloTramiteResponse
                {
                    RM = Servico.MSG_EXCEPTION,
                    RC = ResponseCode.Excecao,
                    OK = false
                };
            }
        }

        [Authorize(Policy = "NivelAcesso1")]        
        [HttpDelete("{idModeloTramite}")]
        [ProducesResponseType(typeof(ResponseModel), 200)]
        [ProducesResponseType(typeof(ResponseModel), 400)]
        [ProducesResponseType(typeof(ResponseModel), 403)]
        [ProducesResponseType(typeof(ResponseModel), 404)]
        [ProducesResponseType(typeof(ResponseModel), 409)]
        [ProducesResponseType(typeof(ResponseModel), 500)]
        public async Task<ActionResult<ResponseModel>> Apagar(int idModeloTramite)
        {
            try
            {
                if (idModeloTramite < 1)
                {
                    return Controladores.Retorno(this, new ResponseModel
                    {
                        RM = "Parâmetro inválido para requisição.",
                        RC = ResponseCode.BadRequest,
                        OK = false
                    });
                }

                ResponseModel retorno;

                retorno = await _TramitesModelosTarefaRepositorio.Apagar(idModeloTramite);

                return Controladores.Retorno(this, retorno);
            }
            catch (Exception ex)
            {
                Servico.GravaLog($"{nameof(ModelosTramiteController)}.{nameof(Apagar)}", ex);
                return new ResponseModel
                {
                    RM = Servico.MSG_EXCEPTION,
                    RC = ResponseCode.Excecao,
                    OK = false
                };
            }
        }
    }
}
