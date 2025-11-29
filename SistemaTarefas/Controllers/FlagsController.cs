using Azure.Core;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Serilog;
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
using System.Runtime.ConstrainedExecution;
using System.Text;
using System.Text.RegularExpressions;

namespace SistemaTarefas.Controllers
{
    //[Microsoft.AspNetCore.Authorization.Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class FlagsController : ControllerBase
    {
        private readonly IFlagsRepositorio _flagRepositorio;
        public FlagsController(IFlagsRepositorio flagRepositorio)
        {
            _flagRepositorio = flagRepositorio;
        }

        private static bool ValidarRequisicao(FlagResponse resposta, FlagRequest flagRequest)
        {
            try
            {
                List<string> erro = new List<string>();
                List<string> erroCode = new List<string>();

                if (string.IsNullOrWhiteSpace(flagRequest.FlaRotulo))
                {
                    erro.Add("Campo obrigatório não foi preenchido.");
                    erroCode.Add("PREECHIMENTO_OBRIGAORIO");
                }

                if (flagRequest.FlaRotulo?.Length > Servico.TAM_NOMES)
                {
                    erro.Add(Servico.MSG_EXCEDE_TAMANHO);
                    erroCode.Add("MSG_EXCEDE_TAMANHO");
                }

                if (!ServicoFuncoes.ValidaCorRGB(flagRequest.FlaCor))
                {
                    erro.Add("Cor inválida! Formato incorreto.");
                    erroCode.Add("FLAGS_COR_INCORRETA");
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
                Servico.GravaLog($"{nameof(FlagsController)}.{nameof(ValidarRequisicao)}", ex);
                resposta.RC = ResponseCode.Excecao;
                return false;
            }
        }

        [Authorize(Policy = "NivelAcesso1a2")]
        [HttpGet]
        [ProducesResponseType(typeof(List<FlagResponse>), 200)]
        [ProducesResponseType(typeof(List<FlagResponse>), 400)]
        [ProducesResponseType(typeof(List<FlagResponse>), 403)]
        [ProducesResponseType(typeof(List<FlagResponse>), 404)]
        [ProducesResponseType(typeof(List<FlagResponse>), 500)]
        public async Task<ActionResult<List<FlagResponse>>> BuscarVarios([FromQuery] string? rotuloFlag = null)
        {
            List<FlagResponse> flag = new List<FlagResponse>();

            try
            {
                flag = await _flagRepositorio.BuscarVarios(rotuloFlag);

                return Controladores.RetornoLista(this, flag, ResponseCode.OK);
            }
            catch (Exception ex)
            {
                Servico.GravaLog($"{nameof(FlagsController)}.{nameof(BuscarVarios)}", ex);
                return new List<FlagResponse>
                {
                    new FlagResponse
                    {
                        RM = Servico.MSG_EXCEPTION,
                        RC = ResponseCode.Excecao,
                        OK = false
                    }
                };
            }
        }

        [Authorize(Policy = "NivelAcesso1a2")]
        [HttpGet("{id}")]
        [ProducesResponseType(typeof(FlagResponse), 200)]
        [ProducesResponseType(typeof(FlagResponse), 400)]
        [ProducesResponseType(typeof(FlagResponse), 403)]
        [ProducesResponseType(typeof(FlagResponse), 404)]
        [ProducesResponseType(typeof(FlagResponse), 500)]
        public async Task<ActionResult<FlagResponse>> BuscarUm(
            [FromRoute] int id,
            [FromQuery] string? rotuloFlag = null)
        {
            FlagResponse flag = new FlagResponse();

            try
            {
                if (id < 1 && string.IsNullOrWhiteSpace(rotuloFlag))
                {
                    return Controladores.Retorno(this, flag, ResponseCode.BadRequest, "Parâmetros incorretos para Buscar um registro.");
                }

                flag = await _flagRepositorio.BuscarUm(id, rotuloFlag);
                return Controladores.Retorno(this, flag);
            }
            catch (Exception ex)
            {
                Servico.GravaLog($"{nameof(FlagsController)}.{nameof(BuscarUm)}", ex);
                return Controladores.Retorno(this, new FlagResponse
                {
                    RM = Servico.MSG_EXCEPTION,
                    RC = ResponseCode.Excecao,
                    OK = false
                });
            }
        }

        [Authorize(Policy = "NivelAcesso1")]
        [HttpPost]
        [ProducesResponseType(typeof(FlagResponse), 200)]
        [ProducesResponseType(typeof(FlagResponse), 400)]
        [ProducesResponseType(typeof(FlagResponse), 403)]
        [ProducesResponseType(typeof(FlagResponse), 404)]
        [ProducesResponseType(typeof(FlagResponse), 409)]
        [ProducesResponseType(typeof(FlagResponse), 500)]
        public async Task<ActionResult<FlagResponse>> Cadastrar([FromBody] FlagRequest flagRequest)
        {
            try
            {
                FlagResponse resposta = new FlagResponse();

                if (!ValidarRequisicao(resposta, flagRequest))
                {
                    return Controladores.Retorno(this, resposta);
                }

                resposta = await _flagRepositorio.Cadastrar(flagRequest);             

                string? uri = Url.Action(nameof(BuscarUm), "Flags", new { id = resposta.flaId });

                return Controladores.Retorno(this, resposta, ResponseCode.CadastradoSucesso, "", uri);
            }
            catch (Exception ex)
            {
                Servico.GravaLog($"{nameof(FlagsController)}.{nameof(Cadastrar)}", ex);
                return Controladores.Retorno(this, new FlagResponse
                {
                    RM = Servico.MSG_EXCEPTION,
                    RC = ResponseCode.Excecao,
                    OK = false
                });
            }
        }

        [Authorize(Policy = "NivelAcesso1")]
        [HttpPut("{id}")]
        [ProducesResponseType(typeof(FlagResponse), 200)]
        [ProducesResponseType(typeof(FlagResponse), 400)]
        [ProducesResponseType(typeof(FlagResponse), 403)]
        [ProducesResponseType(typeof(FlagResponse), 404)]
        [ProducesResponseType(typeof(FlagResponse), 409)]
        [ProducesResponseType(typeof(FlagResponse), 500)]
        public async Task<ActionResult<FlagResponse>> Atualizar([FromBody] FlagRequest flagRequest, [FromRoute] int id)
        {
            try
            {
                FlagResponse resposta = new FlagResponse();

                if (!ValidarRequisicao(resposta, flagRequest))
                {
                    return Controladores.Retorno(this, resposta);
                }

                resposta = await _flagRepositorio.Atualizar(flagRequest, id);

                return Controladores.Retorno(this, resposta);
            }
            catch (Exception ex)
            {
                Servico.GravaLog($"{nameof(FlagsController)}.{nameof(Atualizar)}", ex);
                return Controladores.Retorno(this, new FlagResponse
                {
                    RM = Servico.MSG_EXCEPTION,
                    RC = ResponseCode.Excecao,
                    OK = false
                });
            }
        }

        [Authorize(Policy = "NivelAcesso1")]
        [HttpDelete("{id}")]
        [ProducesResponseType(typeof(ResponseModel), 200)]
        [ProducesResponseType(typeof(ResponseModel), 400)]
        [ProducesResponseType(typeof(ResponseModel), 403)]
        [ProducesResponseType(typeof(ResponseModel), 404)]
        [ProducesResponseType(typeof(ResponseModel), 409)]
        [ProducesResponseType(typeof(ResponseModel), 500)]
        public async Task<ActionResult<ResponseModel>> Apagar(int id)
        {
            ResponseModel retorno = new ResponseModel();

            try
            {
                if (id < 1)
                {
                    return Controladores.Retorno(this, retorno, ResponseCode.BadRequest, "Parâmetro incorreto para Exclusão de registro.");
                }

                retorno = await _flagRepositorio.Apagar(id);

                return Controladores.Retorno(this, retorno);
            }
            catch (Exception ex)
            {
                Servico.GravaLog($"{nameof(FlagsController)}.{nameof(Apagar)}", ex);
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
