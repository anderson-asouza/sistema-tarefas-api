using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ApplicationModels;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion.Internal;
using Microsoft.Identity.Client;
using SistemaTarefas.DTO.Response;
using SistemaTarefas.Enums;
using SistemaTarefas.Models;
using SistemaTarefas.Servicos;
using SistemaTarefas.Util;
using System;
using System.Reflection.Metadata;

namespace SistemaTarefas.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class Controladores : ControllerBase
    {
        private static string GetApiErroCode(ResponseCode code)
        {
            if ((int)code >= 200 && (int)code <= 299)
                return "";

            switch (code)
            {
                case ResponseCode.RegistroNaoEncontrado:
                    return "REGISTRO_NAO_ENCONTRADO";

                case ResponseCode.BadRequest:
                    return "REQUISICAO_INCORRETA";

                case ResponseCode.EntidadeNaoProcessavel:
                    return "ENTIDADE_NAO_PROCESSAVEL";

                case ResponseCode.Conflito:
                    return "CONFLITO_ENTIDADE";

                case ResponseCode.Excecao:
                default:
                    return "MSG_EXCEPTION";
            }
        }

        public static ActionResult<T> Retorno<T>(ControllerBase controller, T model, ResponseCode code = ResponseCode.OK, string? mensagem = "", string? uri = "") where T : class, IResponseModel, new()
        {
            #region model nulo
            if (model == null)
            {
                model = new T
                {
                    RM = !string.IsNullOrWhiteSpace(mensagem) ? mensagem : "",
                    RC = code,
                    OK = false
                };
            }
            #endregion

            #region Tratando RC/RM/OK

            if ((ResponseCode)model.RC! != ResponseCode.Nulo)
            {
                code = (ResponseCode)model.RC;
            }
            else
            {
                model.RC = code;
            }

            if (!string.IsNullOrWhiteSpace(model.RM))
            {
                mensagem = model.RM;
            }
            else
            {
                model.RM = !string.IsNullOrWhiteSpace(mensagem) ? mensagem : "";
            }

            model.RM ??= "";
            model.errorCode ??= "";

            model.OK = ((int)code >= 200 && (int)code <= 299);

            if (code != ResponseCode.OK && model.RM == "")
            {
                model.RM = EnumExtensions.GetDescription(code);
            }

            #endregion

            #region Casos de Sucesso

            if (model.OK)
            {
                uri ??= "";

                switch (model.RC)
                {
                    case ResponseCode.OK:
                        return controller.Ok(model);

                    case ResponseCode.CadastradoSucesso:
                        return (uri != "") ? controller.Created(uri, model) : controller.Ok(model);

                    case ResponseCode.SucessoSemConteudo:
                        return (model.RM == "")
                                ? controller.NoContent()
                                : controller.StatusCode((int)code, model);

                    case ResponseCode.AceitoParaProcessamento:
                        return (uri != "") ? controller.Accepted(uri, model) : controller.Accepted(model);

                }
                return controller.StatusCode((int)model.RC, model);
            }

            #endregion

            #region Casos de Erro

            if (string.IsNullOrWhiteSpace(model.errorCode))
                model.errorCode = GetApiErroCode(code);

            if (string.IsNullOrWhiteSpace(model.RM))
                model.RM = Servico.MSG_EXCEPTION;

            switch (model.RC)
            {
                case ResponseCode.Conflito:
                    return controller.Conflict(model);

                case ResponseCode.NaoAutorizado:
                    return controller.Unauthorized(model);

                case ResponseCode.ForbidAcessoNegado:
                    return (model.RM == "")
                            ? controller.Forbid()
                            : controller.StatusCode((int)code, model);

                case ResponseCode.RegistroNaoEncontrado:
                    return controller.NotFound(model);

                case ResponseCode.EntidadeNaoProcessavel:
                    return controller.UnprocessableEntity(model);

                case ResponseCode.BadRequest:
                case ResponseCode.Nulo:
                    return controller.BadRequest(model);

                case ResponseCode.Excecao:
                    return controller.StatusCode((int)model.RC, model);

                default:
                    return controller.StatusCode((int)model.RC, model);
            }

            #endregion
        }

        public static ActionResult<List<T>> RetornoLista<T>(ControllerBase controller, IEnumerable<T> models, ResponseCode code = ResponseCode.OK, string? mensagem = "", string? uri = "") where T : class, IResponseModel, new()
        {
            try
            {
                string? codidoErro = "";

                if (models is ICollection<T> col && col.Count == 1 && col.First().RC != ResponseCode.Nulo && !string.IsNullOrWhiteSpace(col.First().RM) && ((int)col.First().RC! < 200 || (int)col.First().RC! > 299))
                {
                    var item = col.First();
                    code = (ResponseCode)item.RC!;
                    mensagem = item.RM!;
                    codidoErro = !string.IsNullOrWhiteSpace(item.errorCode) ? item.errorCode : "";
                }
                else
                if (models is ICollection<T> col1 && col1.Count > 0)
                {
                    var item = col1.First();

                    if ((ResponseCode)item.RC! != ResponseCode.Nulo)
                        code = (ResponseCode)item.RC;
                    if (!string.IsNullOrWhiteSpace(item.RM))
                        mensagem = item.RM;
                }

                if (code != ResponseCode.OK && string.IsNullOrWhiteSpace(mensagem))
                {
                    mensagem = EnumExtensions.GetDescription(code);
                }

                mensagem ??= "";

                if ((int)code! >= 200 && (int)code <= 299)
                {
                    if (models != null)
                    {
                        foreach (var model in models)
                        {
                            model.RM = mensagem;
                            model.RC = code;
                            model.errorCode ??= "";
                            model.OK = true;
                        }
                    }

                    #region Casos de Sucesso

                    uri ??= "";

                    switch (code)
                    {
                        case ResponseCode.OK:
                            return controller.Ok(models);

                        case ResponseCode.CadastradoSucesso:
                            return (uri != "") ? controller.Created(uri, models) : controller.Ok(models);

                        case ResponseCode.SucessoSemConteudo:
                            return (mensagem == "")
                                    ? controller.NoContent()
                                    : controller.StatusCode((int)code, models);

                        case ResponseCode.AceitoParaProcessamento:
                            return (uri != "") ? controller.Accepted(uri, models) : controller.Accepted(models);
                    }
                    return controller.StatusCode((int)code, models);

                    #endregion

                }
                else
                {
                    #region Casos de Erro

                    if (string.IsNullOrWhiteSpace(codidoErro))
                        codidoErro = GetApiErroCode(code);

                    if (string.IsNullOrWhiteSpace(mensagem))
                        mensagem = Servico.MSG_EXCEPTION;

                    switch (code)
                    {
                        case ResponseCode.Conflito:
                            return controller.Conflict(new { RM = mensagem, errorCode = codidoErro, RC = code, OK = false });

                        case ResponseCode.NaoAutorizado:
                            return controller.Unauthorized(new { RM = mensagem, errorCode = codidoErro, RC = code, OK = false });

                        case ResponseCode.ForbidAcessoNegado:
                            return (mensagem == "")
                                    ? controller.Forbid()
                                    : controller.StatusCode((int)code, new { RM = mensagem, errorCode = codidoErro, RC = code, OK = false });

                        case ResponseCode.RegistroNaoEncontrado:
                            return controller.NotFound(new { RM = mensagem, errorCode = codidoErro, RC = code, OK = false });

                        case ResponseCode.EntidadeNaoProcessavel:
                            return controller.UnprocessableEntity(new { RM = mensagem, errorCode = codidoErro, RC = code, OK = false });

                        case ResponseCode.BadRequest:
                        case ResponseCode.Nulo:
                            return controller.BadRequest(new { RM = mensagem, errorCode = codidoErro, RC = code, OK = false });

                        case ResponseCode.Excecao:
                            return controller.StatusCode((int)code, new { RM = mensagem, errorCode = codidoErro, RC = code, OK = false });

                        default:
                            return controller.StatusCode((int)code, new { RM = mensagem, errorCode = codidoErro, RC = code, OK = false });
                    }

                    #endregion
                }
            }
            catch (Exception ex)
            {
                Servico.GravaLog($"Exceção na função RetornoSucessoLista({(int)code})", ex);
                return controller.StatusCode((int)ResponseCode.Excecao, new { message = mensagem });
            }            
        }
    }
}
