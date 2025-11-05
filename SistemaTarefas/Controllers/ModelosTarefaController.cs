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
using SistemaTarefas.Repositorios.Interfaces;
using SistemaTarefas.Servicos;
using SistemaTarefas.Util;
using System;
using System.ComponentModel;
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
    public class ModelosTarefaController : ControllerBase
    {
        private readonly IModelosTarefaRepositorio _modelosTarefaRepositorio;
        public ModelosTarefaController(IModelosTarefaRepositorio modelosTarefaRepositorio)
        {
            _modelosTarefaRepositorio = modelosTarefaRepositorio;
        }

        private static bool ValidarRequisicao(ModeloTarefaResponse resposta, ModeloTarefaRequest request)
        {
            try
            {
                List<string> erro = new List<string>();
                List<string> erroCode = new List<string>();

                if (string.IsNullOrWhiteSpace(request.MtarNome) || request.MtarDescricao == null)
                {
                    erro.Add("Campo obrigatório não foi preenchido.");
                    erroCode.Add("PREECHIMENTO_OBRIGAORIO");
                }

                if (request.MtarNome?.Length > Servico.TAM_NOMES || request.MtarDescricao?.Length > Servico.TAM_NOTASDESCRICAO)
                {
                    erro.Add(Servico.MSG_EXCEDE_TAMANHO);
                    erroCode.Add("MSG_EXCEDE_TAMANHO");
                }

                if (erro.Any())
                {
                    resposta.RM = string.Join(Environment.NewLine, erro);
                    resposta.errorCode = string.Join(", ", erroCode);
                    resposta.RC = ResponseCode.BadRequest;
                    return false;
                }

                return true;
            }
            catch (Exception ex)
            {
                Servico.GravaLog($"{nameof(ModelosTarefaController)}.{nameof(ValidarRequisicao)}", ex);
                resposta.RC = ResponseCode.Excecao;
                return false;
            }
        }

        [Authorize(Policy = "NivelAcesso1a2")]
        [HttpGet]
        [ProducesResponseType(typeof(List<ModeloTarefaResponse>), 200)]
        [ProducesResponseType(typeof(List<ModeloTarefaResponse>), 400)]
        [ProducesResponseType(typeof(List<ModeloTarefaResponse>), 403)]
        [ProducesResponseType(typeof(List<ModeloTarefaResponse>), 404)]
        [ProducesResponseType(typeof(List<ModeloTarefaResponse>), 500)]
        public async Task<ActionResult<List<ModeloTarefaResponse>>> BuscarVarios([FromQuery] string? nomeModeloTarefa = null)
        {
            List<ModeloTarefaResponse> modelosTarefa = new List<ModeloTarefaResponse>();

            try
            {
                modelosTarefa = await _modelosTarefaRepositorio.BuscarVarios(nomeModeloTarefa);

                return Controladores.RetornoLista(this, modelosTarefa, ResponseCode.OK);
            }
            catch (Exception ex)
            {
                Servico.GravaLog($"{nameof(ModelosTarefaController)}.{nameof(BuscarVarios)}", ex);
                return new List<ModeloTarefaResponse>
                {
                    new ModeloTarefaResponse
                    {
                        RM = Servico.MSG_EXCEPTION,
                        RC = ResponseCode.Excecao,
                        OK = false
                    }
                };
            }
        }

        [Authorize(Policy = "NivelAcesso1a2")]
        [HttpGet("{idModeloTarefa}")]
        [ProducesResponseType(typeof(ModeloTarefaResponse), 200)]
        [ProducesResponseType(typeof(ModeloTarefaResponse), 400)]
        [ProducesResponseType(typeof(ModeloTarefaResponse), 403)]
        [ProducesResponseType(typeof(ModeloTarefaResponse), 404)]
        [ProducesResponseType(typeof(ModeloTarefaResponse), 500)]
        public async Task<ActionResult<ModeloTarefaResponse>> BuscarUm(
            [FromRoute] int idModeloTarefa,
            [FromQuery] string? nomeModeloTarefa = null)
        {
            ModeloTarefaResponse modeloTarefa = new ModeloTarefaResponse();

            try
            {
                if (idModeloTarefa < 1 && string.IsNullOrWhiteSpace(nomeModeloTarefa))
                {
                    return Controladores.Retorno(this, modeloTarefa, ResponseCode.BadRequest, "Parâmetros incorretos para Buscar um registro.");
                }

                modeloTarefa = await _modelosTarefaRepositorio.BuscarUm(idModeloTarefa, nomeModeloTarefa);
                return Controladores.Retorno(this, modeloTarefa);
            }
            catch (Exception ex)
            {
                Servico.GravaLog($"{nameof(ModelosTarefaController)}.{nameof(BuscarUm)}", ex);
                return new ModeloTarefaResponse
                {
                    RM = Servico.MSG_EXCEPTION,
                    RC = ResponseCode.Excecao,
                    OK = false
                };
            }
        }

        [Authorize(Policy = "NivelAcesso1")]
        [HttpPost]
        [ProducesResponseType(typeof(ModeloTarefaResponse), 200)]
        [ProducesResponseType(typeof(ModeloTarefaResponse), 400)]
        [ProducesResponseType(typeof(ModeloTarefaResponse), 403)]
        [ProducesResponseType(typeof(ModeloTarefaResponse), 404)]
        [ProducesResponseType(typeof(ModeloTarefaResponse), 409)]
        [ProducesResponseType(typeof(ModeloTarefaResponse), 500)]
        public async Task<ActionResult<ModeloTarefaResponse>> Cadastrar([FromBody] ModeloTarefaRequest modeloTarefa)
        {
            try
            {
                ModeloTarefaResponse retorno = new ModeloTarefaResponse();

                if (!ValidarRequisicao(retorno, modeloTarefa))
                {
                    return Controladores.Retorno(this, retorno);
                }

                retorno = await _modelosTarefaRepositorio.Cadastrar(modeloTarefa);

                string? uri = Url.Action(nameof(BuscarUm), "ModelosTarefa", new { id = retorno.MtarId });

                return Controladores.Retorno(this, retorno, ResponseCode.CadastradoSucesso, "", uri);
            }
            catch (Exception ex)
            {
                Servico.GravaLog($"{nameof(ModelosTarefaController)}.{nameof(Cadastrar)}", ex);
                return new ModeloTarefaResponse
                {
                    RM = Servico.MSG_EXCEPTION,
                    RC = ResponseCode.Excecao,
                    OK = false
                };
            }
        }

        [Authorize(Policy = "NivelAcesso1")]
        [HttpPut("{idModeloTarefa}")]
        [ProducesResponseType(typeof(ModeloTarefaResponse), 200)]
        [ProducesResponseType(typeof(ModeloTarefaResponse), 400)]
        [ProducesResponseType(typeof(ModeloTarefaResponse), 403)]
        [ProducesResponseType(typeof(ModeloTarefaResponse), 404)]
        [ProducesResponseType(typeof(ModeloTarefaResponse), 409)]
        [ProducesResponseType(typeof(ModeloTarefaResponse), 500)]
        public async Task<ActionResult<ModeloTarefaResponse>> Atualizar([FromBody] ModeloTarefaRequest modeloTarefa, [FromRoute] int idModeloTarefa)
        {
            try
            {
                ModeloTarefaResponse retorno = new ModeloTarefaResponse();

                if (idModeloTarefa < 1)
                {
                    return Controladores.Retorno(this, retorno, ResponseCode.BadRequest, "Parâmetro incorreto para Atualização de registro.");
                }

                if (!ValidarRequisicao(retorno, modeloTarefa))
                {
                    return Controladores.Retorno(this, retorno);
                }

                retorno = await _modelosTarefaRepositorio.Atualizar(modeloTarefa, idModeloTarefa);

                return Controladores.Retorno(this, retorno);
            }
            catch (Exception ex)
            {
                Servico.GravaLog($"{nameof(ModelosTarefaController)}.{nameof(Atualizar)}", ex);
                return new ModeloTarefaResponse
                {
                    RM = Servico.MSG_EXCEPTION,
                    RC = ResponseCode.Excecao,
                    OK = false
                };
            }
        }

        [Authorize(Policy = "NivelAcesso1")]
        [HttpDelete("{idModeloTarefa}")]
        [ProducesResponseType(typeof(ResponseModel), 200)]
        [ProducesResponseType(typeof(ResponseModel), 400)]
        [ProducesResponseType(typeof(ResponseModel), 403)]
        [ProducesResponseType(typeof(ResponseModel), 404)]
        [ProducesResponseType(typeof(ResponseModel), 409)]
        [ProducesResponseType(typeof(ResponseModel), 500)]
        public async Task<ActionResult<ResponseModel>> Apagar(int idModeloTarefa)
        {
            try
            {
                ResponseModel retorno = new ResponseModel();

                if (idModeloTarefa < 1)
                {
                    return Controladores.Retorno(this, retorno, ResponseCode.BadRequest, "Parâmetro incorreto para Exclusão de registro.");
                }

                retorno = await _modelosTarefaRepositorio.Apagar(idModeloTarefa);

                return Controladores.Retorno(this, retorno);
            }
            catch (Exception ex)
            {
                Servico.GravaLog($"{nameof(ModelosTarefaController)}.{nameof(Apagar)}", ex);
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
