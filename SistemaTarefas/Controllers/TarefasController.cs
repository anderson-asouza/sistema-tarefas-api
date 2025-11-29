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
    public class TarefasController(ITarefasRepositorio tarefasRepositorio) : ControllerBase
    {
        private readonly ITarefasRepositorio _tarefasRepositorio = tarefasRepositorio;

        private bool ValidaRequisicao(TarefaResponse resposta, bool registroNovo, int TarMtarId, string? nome, string? descricao, int idTarefa = 0)
        {
            try
            {
                List<string> erro = new List<string>();
                List<string> erroCode = new List<string>();

                if (registroNovo && TarMtarId < 1)
                {
                    erro.Add("Tarefa deve se associada a um Modelo de Tarefa.");
                    erroCode.Add("PREECHIMENTO_OBRIGAORIO");
                }
                else if (!registroNovo && idTarefa < 1)
                {
                    erro.Add("Parâmetros incorretos para atualizar o registro.");
                    erroCode.Add("PREECHIMENTO_OBRIGAORIO");
                }

                if (string.IsNullOrWhiteSpace(nome))
                {
                    erro.Add("Nome Tarefa deve ser preenchido.");
                    erroCode.Add("PREECHIMENTO_OBRIGAORIO");                    
                }

                if (nome?.Length > Servico.TAM_NOMES || descricao?.Length > Servico.TAM_NOTASDESCRICAO)
                {
                    erro.Add("Campo para Tarefa excedeu o tamanho limite.");
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
                Servico.GravaLog($"{nameof(TarefasController)}.{nameof(ValidaRequisicao)}", ex);
                resposta.RC = ResponseCode.Excecao;
                return false;
            }
        }

        [Authorize(Policy = "NivelAcesso1a4")]
        [HttpGet]
        [ProducesResponseType(typeof(List<TarefaResponse>), 200)]
        [ProducesResponseType(typeof(List<TarefaResponse>), 400)]
        [ProducesResponseType(typeof(List<TarefaResponse>), 403)]
        [ProducesResponseType(typeof(List<TarefaResponse>), 404)]
        [ProducesResponseType(typeof(List<TarefaResponse>), 500)]
        public async Task<ActionResult<List<TarefaResponse>>> BuscarVarios([FromQuery] string? statusBusca = null, [FromQuery] string? nomeTarefa = null, [FromQuery] bool ordenarPelaDataInicial = false)
        {
            try
            {
                if (!string.IsNullOrWhiteSpace(statusBusca) && !StatusTarefaExtensions.GetTarefaDescricao(statusBusca, out StatusTarefa status))
                {
                    Servico.GravaLog($"{nameof(TarefasController)}.{nameof(BuscarVarios)} | Status Inválido.");
                    return new List<TarefaResponse>
                    {
                        new TarefaResponse
                        {
                            RM = "Falha. Status Incorreto para Tarefa.",
                            RC = ResponseCode.BadRequest,
                            OK = false
                        }
                    };
                }

                List<TarefaResponse> tarefa;

                tarefa = await _tarefasRepositorio.BuscarVarios(statusBusca, nomeTarefa, ordenarPelaDataInicial);

                return Controladores.RetornoLista(this, tarefa);
            }
            catch (Exception ex)
            {
                Servico.GravaLog($"{nameof(TarefasController)}.{nameof(BuscarVarios)}", ex);
                return new List<TarefaResponse>
                {
                    new TarefaResponse
                    {
                        RM = Servico.MSG_EXCEPTION,
                        RC = ResponseCode.Excecao,
                        OK = false
                    }
                };
            }
        }

        [Authorize(Policy = "NivelAcesso1a4")]
        [HttpGet("{idTarefa}")]
        [ProducesResponseType(typeof(TarefaResponse), 200)]
        [ProducesResponseType(typeof(TarefaResponse), 400)]
        [ProducesResponseType(typeof(TarefaResponse), 403)]
        [ProducesResponseType(typeof(TarefaResponse), 404)]
        [ProducesResponseType(typeof(TarefaResponse), 500)]
        public async Task<ActionResult<TarefaResponse>> BuscarUm(
            [FromRoute] int idTarefa,
            [FromQuery] string? nomeTarefa = null)
        {
            TarefaResponse tarefa = new();

            try
            {
                if (idTarefa < 1 && string.IsNullOrWhiteSpace(nomeTarefa))
                {
                    return Controladores.Retorno(this, tarefa, ResponseCode.BadRequest, "Parâmetros de busca incorretos.");
                }

                tarefa = await _tarefasRepositorio.BuscarUm(idTarefa, nomeTarefa);
                return Controladores.Retorno(this, tarefa);
            }
            catch (Exception ex)
            {
                Servico.GravaLog($"{nameof(TarefasController)}.{nameof(BuscarUm)}", ex);
                return Controladores.Retorno(this, new TarefaResponse
                {
                    RM = Servico.MSG_EXCEPTION,
                    RC = ResponseCode.Excecao,
                    OK = false
                });
            }
        }

        [Authorize(Policy = "NivelAcesso1a2")]
        [HttpPost]
        [ProducesResponseType(typeof(TarefaResponse), 200)]
        [ProducesResponseType(typeof(TarefaResponse), 400)]
        [ProducesResponseType(typeof(TarefaResponse), 403)]
        [ProducesResponseType(typeof(TarefaResponse), 404)]
        [ProducesResponseType(typeof(TarefaResponse), 409)]
        [ProducesResponseType(typeof(TarefaResponse), 500)]
        public async Task<ActionResult<TarefaResponse>> Cadastrar([FromBody] TarefaRequest tarefa)
        {
            try
            {
                TarefaResponse resposta = new();

                if (!ValidaRequisicao(resposta, true, tarefa.TarMtarId, tarefa.TarNomeTarefa, tarefa.TarDescricao))
                {
                    return Controladores.Retorno(this, resposta);
                }

                int usuarioID = Servico.Claims().usuarioID;

                resposta = await _tarefasRepositorio.Cadastrar(tarefa, usuarioID);

                string? uri = Url.Action(nameof(BuscarUm), "Tarefas", new { id = resposta.TarId });

                return Controladores.Retorno(this, resposta, ResponseCode.CadastradoSucesso, "", uri);

            }
            catch (Exception ex)
            {
                Servico.GravaLog($"{nameof(TarefasController)}.{nameof(Cadastrar)}", ex);
                return Controladores.Retorno(this, new TarefaResponse
                {
                    RM = Servico.MSG_EXCEPTION,
                    RC = ResponseCode.Excecao,
                    OK = false
                });
            }
        }

        [Authorize(Policy = "NivelAcesso1a2")]
        [HttpPut("{idTarefa}")]
        [ProducesResponseType(typeof(TarefaResponse), 200)]
        [ProducesResponseType(typeof(TarefaResponse), 400)]
        [ProducesResponseType(typeof(TarefaResponse), 403)]
        [ProducesResponseType(typeof(TarefaResponse), 404)]
        [ProducesResponseType(typeof(TarefaResponse), 409)]
        [ProducesResponseType(typeof(TarefaResponse), 500)]
        public async Task<ActionResult<TarefaResponse>> Atualizar([FromBody] TarefaUpdRequest tarefaUpdRequest, [FromRoute] int idTarefa)
        {
            try
            {
                TarefaResponse resposta = new TarefaResponse();

                if (!ValidaRequisicao(resposta, false, 0, tarefaUpdRequest.TarNomeTarefa, tarefaUpdRequest.TarDescricao, idTarefa))
                {
                    return Controladores.Retorno(this, resposta);
                }

                resposta = await _tarefasRepositorio.Atualizar(tarefaUpdRequest, idTarefa);

                return Controladores.Retorno(this, resposta);
            }
            catch (Exception ex)
            {
                Servico.GravaLog($"{nameof(TarefasController)}.{nameof(Atualizar)}", ex);
                return Controladores.Retorno(this, new TarefaResponse
                {
                    RM = Servico.MSG_EXCEPTION,
                    RC = ResponseCode.Excecao,
                    OK = false
                });
            }
        }

        [Authorize(Policy = "NivelAcesso1a2")]
        [HttpDelete("{idTarefa}")]
        [ProducesResponseType(typeof(ResponseModel), 200)]
        [ProducesResponseType(typeof(ResponseModel), 400)]
        [ProducesResponseType(typeof(ResponseModel), 403)]
        [ProducesResponseType(typeof(ResponseModel), 404)]
        [ProducesResponseType(typeof(ResponseModel), 409)]
        [ProducesResponseType(typeof(ResponseModel), 500)]
        public async Task<ActionResult<ResponseModel>> Apagar([FromRoute] int idTarefa)
        {            
            try
            {
                ResponseModel resposta = new();

                if (idTarefa < 1)
                {
                    return Controladores.Retorno(this, resposta, ResponseCode.BadRequest, "Parâmetro incorreto para exclusão.");
                }

                resposta = await _tarefasRepositorio.Apagar(idTarefa);

                return Controladores.Retorno(this, resposta);
            }
            catch (Exception ex)
            {
                Servico.GravaLog($"{nameof(TarefasController)}.{nameof(Apagar)}", ex);
                return Controladores.Retorno(this, new ResponseModel
                {
                    RM = Servico.MSG_EXCEPTION,
                    RC = ResponseCode.Excecao,
                    OK = false
                });
            }
        }

        [Authorize(Policy = "NivelAcesso1a2")]
        [HttpPost("ativar-desativar/{idTarefa}")]
        [ProducesResponseType(typeof(TarefaResponse), 200)]
        [ProducesResponseType(typeof(TarefaResponse), 400)]
        [ProducesResponseType(typeof(TarefaResponse), 403)]
        [ProducesResponseType(typeof(TarefaResponse), 404)]
        [ProducesResponseType(typeof(TarefaResponse), 409)]
        [ProducesResponseType(typeof(TarefaResponse), 500)]
        public async Task<ActionResult<ResponseModel>> AtivarDesativarTarefa([FromRoute] int idTarefa, [FromQuery] bool ativar = false)
        {
            ResponseModel resposta = new ResponseModel();

            try
            {
                if (idTarefa < 1)
                {
                    return Controladores.Retorno(this, resposta, ResponseCode.BadRequest, "Parâmetro incorreto para Ativar/Paralizar Tarefa.");
                }

                resposta = await _tarefasRepositorio.AtivarDesativarTarefa(idTarefa, ativar);

                return Controladores.Retorno(this, resposta);
            }
            catch (Exception ex)
            {
                Servico.GravaLog($"{nameof(TarefasController)}.{nameof(AtivarDesativarTarefa)}", ex);
                return Controladores.Retorno(this, new ResponseModel
                {
                    RM = Servico.MSG_EXCEPTION,
                    RC = ResponseCode.Excecao,
                    OK = false
                });
            }
        }

        [Authorize(Policy = "NivelAcesso1a2")]
        [HttpPost("marcar-flag/{idTarefa}")]
        [ProducesResponseType(typeof(TarefaResponse), 200)]
        [ProducesResponseType(typeof(TarefaResponse), 400)]
        [ProducesResponseType(typeof(TarefaResponse), 403)]
        [ProducesResponseType(typeof(TarefaResponse), 404)]
        [ProducesResponseType(typeof(TarefaResponse), 409)]
        [ProducesResponseType(typeof(TarefaResponse), 500)]
        public async Task<ActionResult<ResponseModel>> MarcarFlagNaTarefa([FromRoute] int idTarefa, [FromQuery] int idFlag = 0)
        {
            ResponseModel resposta = new ResponseModel();

            try
            {
                if (idTarefa < 1)
                {
                    return Controladores.Retorno(this, resposta, ResponseCode.BadRequest, "Deve informar a Tarefa para marcar a Flag.");
                }

                int usuarioID = Servico.Claims().usuarioID;

                resposta = await _tarefasRepositorio.MarcarFlagNaTarefa(idTarefa, idFlag, usuarioID);

                return Controladores.Retorno(this, resposta);
            }
            catch (Exception ex)
            {
                Servico.GravaLog($"{nameof(TarefasController)}.{nameof(MarcarFlagNaTarefa)}", ex);
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