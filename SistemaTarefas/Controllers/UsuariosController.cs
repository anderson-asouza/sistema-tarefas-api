using Azure.Core;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Conventions;
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
using System.Linq;
using System.Text;
using static System.Net.Mime.MediaTypeNames;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace SistemaTarefas.Controllers
{
    //[Microsoft.AspNetCore.Authorization.Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class UsuariosController : ControllerBase
    {
        private readonly IUsuariosRepositorio _usuarioRepositorio;
        private readonly TokenConfiguracao _configToken;
        public UsuariosController(IUsuariosRepositorio usuarioRepositorio, TokenConfiguracao configToken)
        {
            _usuarioRepositorio = usuarioRepositorio;
            _configToken = configToken;
        }

        private bool ValidaRequisicao(UsuarioResponse resposta,  int id, int nivel, string? nome = null, string? matricula = null, string? email = null, string? login = null, string? senhaInicial = null)
        {
            try
            {
                List<string> erro = new List<string>();
                List<string> erroCode = new List<string>();

                if ((nome?.Length ?? 0) > 100 || (matricula?.Length ?? 0) > 30 || (email?.Length ?? 0) > 100 || (id == 0 && ((login?.Length ?? 0) > 50 || (senhaInicial?.Length ?? 0) > 60)))
                {
                    erro.Add(Servico.MSG_EXCEDE_TAMANHO);
                    erroCode.Add("MSG_EXCEDE_TAMANHO");
                }

                if (id < 0)
                { 
                    erro.Add("Id irregular para solicitação.");
                    erroCode.Add("ID_IRREGULAR");
                }

                if ((id == 0 && string.IsNullOrWhiteSpace(login)) || string.IsNullOrWhiteSpace(nome) || string.IsNullOrWhiteSpace(matricula) || !Geral.EmailEhValido(email))
                { 
                    erro.Add("Campo obrigatório não foi preenchido.");
                    erroCode.Add("PREECHIMENTO_OBRIGATORIO");
                }

                if (id == 0 && string.IsNullOrWhiteSpace(senhaInicial))
                {
                    erro.Add("Uma Senha Inicial deve ser definida.");
                    erroCode.Add("USUARIOS_SENHA_INICIAL");
                }

                if (nivel < 0 || nivel > 4)
                {
                    erro.Add("Nível inválido para o usuário.");
                    erroCode.Add("USUARIOS_NIVEL_INVALIDO");
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
                Servico.GravaLog($"{nameof(UsuariosController)}.{nameof(ValidaRequisicao)}", ex);
                resposta.RC = ResponseCode.Excecao;
                return false;
            }
        }

        [Authorize(Policy = "NivelAcesso1")]
        [HttpGet]
        [ProducesResponseType(typeof(List<UsuarioResponse>), 200)]
        [ProducesResponseType(typeof(List<UsuarioResponse>), 400)]
        [ProducesResponseType(typeof(List<UsuarioResponse>), 403)]
        [ProducesResponseType(typeof(List<UsuarioResponse>), 404)]
        [ProducesResponseType(typeof(List<UsuarioResponse>), 500)]
        public async Task<ActionResult<List<UsuarioResponse>>> BuscarVarios([FromQuery] string? nomeUsuario = null)
        {
            List<UsuarioResponse> usuarios = new List<UsuarioResponse>();

            try
            {
                usuarios = await _usuarioRepositorio.BuscarVarios(nomeUsuario);

                return Controladores.RetornoLista(this, usuarios, ResponseCode.OK);
            }
            catch (Exception ex)
            {
                Servico.GravaLog($"{nameof(UsuariosController)}.{nameof(BuscarVarios)}", ex);
                return new List<UsuarioResponse>
                {
                    new UsuarioResponse
                    {
                        RM = Servico.MSG_EXCEPTION,
                        RC = ResponseCode.Excecao,
                        OK = false
                    }
                };
            }
        }

        [Authorize(Policy = "NivelAcesso1")]
        [HttpGet("{idUsuario}")]
        [ProducesResponseType(typeof(List<UsuarioResponse>), 200)]
        [ProducesResponseType(typeof(List<UsuarioResponse>), 400)]
        [ProducesResponseType(typeof(List<UsuarioResponse>), 403)]
        [ProducesResponseType(typeof(List<UsuarioResponse>), 404)]
        [ProducesResponseType(typeof(List<UsuarioResponse>), 500)]
        public async Task<ActionResult<UsuarioResponse>> BuscarUm(
            [FromRoute] int idUsuario,
            [FromQuery] string nomeUsuario = "",
            [FromQuery] string matriculaUsuario = "",
            [FromQuery] string login = "")
        {
            try
            {
                UsuarioResponse usuario = new UsuarioResponse();

                if (idUsuario < 1 && string.IsNullOrWhiteSpace(nomeUsuario) && string.IsNullOrWhiteSpace(matriculaUsuario) && string.IsNullOrWhiteSpace(login))
                {
                    return Controladores.Retorno(this, usuario, ResponseCode.BadRequest, "Parâmetros incorretos para Buscar um registro.");
                }

                usuario = await _usuarioRepositorio.BuscarUm(idUsuario, nomeUsuario, matriculaUsuario, login);
                return Controladores.Retorno(this, usuario);
            }
            catch (Exception ex)
            {                
                Servico.GravaLog($"{nameof(UsuariosController)}.{nameof(BuscarUm)}", ex);
                return Controladores.Retorno(this, new UsuarioResponse
                {
                    RM = Servico.MSG_EXCEPTION,
                    RC = ResponseCode.Excecao,
                    OK = false
                });
            }
        }

#if DEBUG
        [AllowAnonymous]
#else
        [Authorize(Policy = "NivelAcesso1")]
#endif
        [HttpPost]
        [ProducesResponseType(typeof(List<UsuarioResponse>), 200)]
        [ProducesResponseType(typeof(List<UsuarioResponse>), 400)]
        [ProducesResponseType(typeof(List<UsuarioResponse>), 403)]
        [ProducesResponseType(typeof(List<UsuarioResponse>), 404)]
        [ProducesResponseType(typeof(List<UsuarioResponse>), 409)]
        [ProducesResponseType(typeof(List<UsuarioResponse>), 500)]
        public async Task<ActionResult<UsuarioResponse>> Cadastrar([FromBody] UsuarioRequest usuarioRequest)
        {          
            try
            {
                bool testarClaims = true;

#if DEBUG
                testarClaims = await _usuarioRepositorio.ExisteAlgumAdmin();
#endif
                UsuarioResponse resposta = new UsuarioResponse();

                if (testarClaims)
                {
                    var (usuarioID, nivelUsuario) = Servico.Claims();

                    if (nivelUsuario != 1)
                    {
                        resposta.RM = $"Usuário: [{usuarioID}] não tem permissão para esta operação.";
                        resposta.RC = ResponseCode.NaoAutorizado;                        
                        Servico.GravaLog($"{nameof(UsuariosController)}.{nameof(Cadastrar)} | {resposta.RM}");
                        return Controladores.Retorno(this, resposta);
                    }
                }

                if (!ValidaRequisicao(resposta, 0, usuarioRequest.usuNivel, usuarioRequest.usuNome, usuarioRequest.usuMatricula, usuarioRequest.usuEmail, usuarioRequest.usuLogin, usuarioRequest.UsuSenha))
                {
                    return Controladores.Retorno(this, resposta);
                }

                resposta = await _usuarioRepositorio.Cadastrar(usuarioRequest);

                string? uri = Url.Action(nameof(BuscarUm), "Usuarios", new { id = resposta.usuId });

                return Controladores.Retorno(this, resposta, ResponseCode.CadastradoSucesso, "", uri);
            }
            catch (Exception ex)
            {                
                Servico.GravaLog($"{nameof(UsuariosController)}.{nameof(Cadastrar)}", ex);
                return Controladores.Retorno(this, new UsuarioResponse
                {
                    RM = Servico.MSG_EXCEPTION,
                    RC = ResponseCode.Excecao,
                    OK = false
                });
            }
        }

        [Authorize(Policy = "NivelAcesso1")]
        [HttpPut("{idUsuario}")]
        [ProducesResponseType(typeof(List<UsuarioResponse>), 200)]
        [ProducesResponseType(typeof(List<UsuarioResponse>), 403)]
        [ProducesResponseType(typeof(List<UsuarioResponse>), 404)]
        [ProducesResponseType(typeof(List<UsuarioResponse>), 409)]
        [ProducesResponseType(typeof(List<UsuarioResponse>), 500)]
        public async Task<ActionResult<UsuarioResponse>> Atualizar([FromBody] UsuarioUpdRequest usuarioRequest, [FromRoute] int idUsuario)
        {
            try
            {
                UsuarioResponse resposta = new UsuarioResponse();

                if (idUsuario < 1)
                    return Controladores.Retorno(this, new UsuarioResponse(), ResponseCode.BadRequest, "Id irregular para solicitação.");

                if (!ValidaRequisicao(resposta, idUsuario, usuarioRequest.usuNivel, usuarioRequest.usuNome, usuarioRequest.usuMatricula, usuarioRequest.usuEmail))
                {
                    return Controladores.Retorno(this, resposta);
                }

                resposta = await _usuarioRepositorio.Atualizar(usuarioRequest, idUsuario);

                return Controladores.Retorno(this, resposta);
            }
            catch (Exception ex)
            {
                Servico.GravaLog($"{nameof(UsuariosController)}.{nameof(Atualizar)}", ex);
                return Controladores.Retorno(this, new UsuarioResponse
                {
                    RM = Servico.MSG_EXCEPTION,
                    RC = ResponseCode.Excecao,
                    OK = false
                });
            }
        }

        [Authorize(Policy = "NivelAcesso1")]
        [HttpDelete("{idUsuario}")]
        [ProducesResponseType(typeof(List<ResponseModel>), 200)]
        [ProducesResponseType(typeof(List<ResponseModel>), 400)]
        [ProducesResponseType(typeof(List<ResponseModel>), 403)]
        [ProducesResponseType(typeof(List<ResponseModel>), 404)]
        [ProducesResponseType(typeof(List<ResponseModel>), 409)]
        [ProducesResponseType(typeof(List<ResponseModel>), 500)]
        public async Task<ActionResult<ResponseModel>> Apagar([FromRoute] int idUsuario)
        {
            try
            {
                ResponseModel retorno = new ResponseModel();

                if (idUsuario < 1)
                {
                    return Controladores.Retorno(this, retorno, ResponseCode.BadRequest, "Parâmetro incorreto para Exclusão de registro.");
                }

                int usuarioID = Servico.Claims().usuarioID;

                if (usuarioID == idUsuario)
                {
                    retorno.RM = $"Usuário: [{usuarioID}] não apagar o próprio o próprio cadastro.";
                    retorno.RC = ResponseCode.Conflito;
                    Servico.GravaLog($"Controller Apagar Usuário | {retorno.RM}");
                    return Controladores.Retorno(this, retorno);
                }

                retorno = await _usuarioRepositorio.Apagar(idUsuario);

                return Controladores.Retorno(this, retorno);
            }
            catch (Exception ex)
            {
                Servico.GravaLog($"{nameof(UsuariosController)}.{nameof(Apagar)}", ex);
                return Controladores.Retorno(this, new ResponseModel
                {
                    RM = Servico.MSG_EXCEPTION,
                    RC = ResponseCode.Excecao,
                    OK = false
                });
            }
        }

        private static bool ValidarParametrosAcesso(bool trocarSenhaPeloAdm, string login, string senha, string novaSenha, string confirmacao, out string resposta, out string erroCode)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(login) || (!trocarSenhaPeloAdm && string.IsNullOrWhiteSpace(senha)))
                {
                    resposta = "Informe o Login e a Senha.";
                    erroCode = "USUARIOS_LOGIN_SENHA";
                    return false;
                }

                if (novaSenha != confirmacao)
                {
                    resposta = "Nova Senha diferente de sua Confirmação.";
                    erroCode = "USUARIOS_NOVASENHA_DIFERENTE_CONFIRMACAO";
                    return false;
                }

                resposta = "";
                erroCode = "";
                return true;
            }
            catch (Exception)
            {
                resposta = Servico.MSG_EXCEPTION;
                erroCode = "MSG_EXCEPTION";
                return false;
            }
        }

        [Authorize(Policy = "NivelAcesso1a4")]
        [HttpPost("alterar-senha")]
        [ProducesResponseType(typeof(List<UsuarioLoginResponse>), 200)]
        [ProducesResponseType(typeof(List<UsuarioLoginResponse>), 403)]
        [ProducesResponseType(typeof(List<UsuarioLoginResponse>), 404)]
        [ProducesResponseType(typeof(List<UsuarioLoginResponse>), 500)]
        public async Task<ActionResult<UsuarioAlterarSenhaResponse>> AlterarSenha([FromBody] UsuarioAlterarSenhaRequest request, [FromQuery] bool trocarSenhaPeloAdm = false)
        {
            try
            {
                if (request == null || string.IsNullOrWhiteSpace(request.NovaSenha))
                {
                    return Controladores.Retorno(this, new UsuarioAlterarSenhaResponse
                    {
                        RM = "Requisição inválida.",
                        RC = ResponseCode.BadRequest,
                        OK = false
                    });
                }

                if (!ValidarParametrosAcesso(trocarSenhaPeloAdm, request.Login, request.Senha, request.NovaSenha ?? "", request.ConfirmacaoNovaSenha ?? "", out string resposta, out string erroCode))
                {
                    return Controladores.Retorno(this, new UsuarioAlterarSenhaResponse
                    {
                        RM = resposta,
                        errorCode = erroCode,
                        RC = ResponseCode.BadRequest,
                        OK = false
                    });
                }

                UsuarioAlterarSenhaResponse usuario = new UsuarioAlterarSenhaResponse();

                usuario = await _usuarioRepositorio.AlterarSenha(request.Login, request.Senha, request.NovaSenha!, request.ConfirmacaoNovaSenha!, trocarSenhaPeloAdm);

                return Controladores.Retorno(this, usuario);
            }
            catch (Exception ex)
            {
                Servico.GravaLog($"{nameof(UsuariosController)}.{nameof(AlterarSenha)}", ex);
                return Controladores.Retorno(this, new UsuarioAlterarSenhaResponse
                {
                    RM = Servico.MSG_EXCEPTION,
                    RC = ResponseCode.Excecao,
                    OK = false
                });
            }
        }

        [AllowAnonymous]
        [HttpPost("logar")]
        [ProducesResponseType(typeof(List<UsuarioLoginResponse>), 200)]
        [ProducesResponseType(typeof(List<UsuarioLoginResponse>), 400)]
        [ProducesResponseType(typeof(List<UsuarioLoginResponse>), 403)]
        [ProducesResponseType(typeof(List<UsuarioLoginResponse>), 404)]
        [ProducesResponseType(typeof(List<UsuarioLoginResponse>), 500)]
        public async Task<ActionResult<UsuarioLoginResponse>> Logar([FromBody] UsuarioLoginRequest request)
        {
            try
            {
                if (request == null)
                {
                    return Controladores.Retorno(this, new UsuarioLoginResponse
                    {
                        RM = "Requisição inválida.",
                        RC = ResponseCode.BadRequest,
                        OK = false
                    });
                }

                if (!ValidarParametrosAcesso(false, request.Login, request.Senha, request.NovaSenha ?? "", request.ConfirmacaoNovaSenha ?? "", out string resposta, out string erroCode))
                {
                    return Controladores.Retorno(this, new UsuarioLoginResponse
                    {
                        RM = resposta,
                        errorCode = erroCode,
                        RC = ResponseCode.BadRequest,
                        OK = false
                    });
                }

                UsuarioLoginResponse usuario = usuario = await _usuarioRepositorio.Logar(request.Login, request.Senha, request.NovaSenha, request.ConfirmacaoNovaSenha);

                if (!usuario.OK)
                {
                    return Controladores.Retorno(this, usuario);
                }

                usuario.usuToken = Servico.GerarTokenJWT(_configToken, usuario.usuId, usuario.usuNivel);

                return Controladores.Retorno(this, usuario);
            }
            catch (Exception ex)
            {                
                Servico.GravaLog($"{nameof(UsuariosController)}.{nameof(Logar)}", ex);
                return Controladores.Retorno(this, new UsuarioLoginResponse
                {
                    RM = Servico.MSG_EXCEPTION,
                    RC = ResponseCode.Excecao,
                    OK = false
                });
            }
        }
        private bool ValidaAssinaturaImagem(IFormFile arquivo)
        {
            try
            {
                byte[] pngSignature = new byte[] { 137, 80, 78, 71, 13, 10, 26, 10 };
                byte[] jpegSignature1 = new byte[] { 0xFF, 0xD8, 0xFF, 0xE0 };
                byte[] jpegSignature2 = new byte[] { 0xFF, 0xD8, 0xFF, 0xE1 };

                using var stream = arquivo.OpenReadStream();
                byte[] buffer = new byte[8];
                int bytesRead = stream.Read(buffer, 0, buffer.Length);

                if (bytesRead < 4)
                    return false;

                if (bytesRead >= pngSignature.Length && buffer.Take(pngSignature.Length).SequenceEqual(pngSignature))
                    return true;

                if (bytesRead >= jpegSignature1.Length && buffer.Take(jpegSignature1.Length).SequenceEqual(jpegSignature1))
                    return true;

                if (bytesRead >= jpegSignature2.Length && buffer.Take(jpegSignature2.Length).SequenceEqual(jpegSignature2))
                    return true;

                return false;
            }
            catch (Exception ex)
            {
                Servico.GravaLog($"{nameof(UsuariosController)}.{nameof(ValidaAssinaturaImagem)}", ex);
                return false;
            }
        }
        private string ValidaImagem(UploadImagemPerfilRequest arq)
        {
            try
            {
                if (arq == null || arq.Imagem == null || arq.Imagem.Length == 0)
                    return "Imagem inválida.";

                var extensoesValidas = new List<string> { ".jpg", ".jpeg", ".png" };
                var extensao = Path.GetExtension(arq.Imagem.FileName).ToLower();

                if (!extensoesValidas.Contains(extensao))
                    return "Formato de arquivo inválido. Apenas imagens são permitidas.";

                if (arq.Imagem.Length > Servico.TAM_MAX_IMAGEM_PERFIL)
                    return "Arquivo muito grande. Limite é 512 KB.";

                if (!ValidaAssinaturaImagem(arq.Imagem))
                    return "Tipo de conteúdo inválido.";

                return "";
            }
            catch (Exception ex)
            {
                Servico.GravaLog($"{nameof(UsuariosController)}.{nameof(ValidaImagem)}", ex);
                return Servico.MSG_EXCEPTION;
            }
        }

        [Authorize(Policy = "NivelAcesso1a4")]
        [HttpPost("upload-imagem-perfil")]
        [Consumes("multipart/form-data")]
        [ProducesResponseType(typeof(List<UsuarioImagemResponse>), 200)]
        [ProducesResponseType(typeof(List<UsuarioImagemResponse>), 400)]
        [ProducesResponseType(typeof(List<UsuarioImagemResponse>), 403)]
        [ProducesResponseType(typeof(List<UsuarioImagemResponse>), 404)]
        [ProducesResponseType(typeof(List<UsuarioImagemResponse>), 409)]
        [ProducesResponseType(typeof(List<UsuarioImagemResponse>), 500)]
        public async Task<ActionResult<UsuarioImagemResponse>> UploadImagemPerfil([FromForm] UploadImagemPerfilRequest imagem)
        {
            try
            {
                string erro = ValidaImagem(imagem);

                if (!string.IsNullOrWhiteSpace(erro))
                    return Controladores.Retorno(this, new UsuarioImagemResponse(), ResponseCode.BadRequest, erro);

                int usuarioID = Servico.Claims().usuarioID;

                UsuarioImagemResponse response = await _usuarioRepositorio.UploadImagemPerfil(imagem, usuarioID);

                return Controladores.Retorno(this, response);
            }
            catch (Exception ex)
            {
                Servico.GravaLog($"{nameof(UsuariosController)}.{nameof(UploadImagemPerfil)}", ex);
                return Controladores.Retorno(this, new UsuarioImagemResponse
                {
                    RM = Servico.MSG_EXCEPTION,
                    RC = ResponseCode.Excecao,
                    OK = false
                });
            }
        }

        [Authorize(Policy = "NivelAcesso1a4")]
        [HttpDelete("remover-imagem-perfil")]
        [ProducesResponseType(typeof(List<ResponseModel>), 200)]
        [ProducesResponseType(typeof(List<ResponseModel>), 400)]
        [ProducesResponseType(typeof(List<ResponseModel>), 403)]
        [ProducesResponseType(typeof(List<ResponseModel>), 404)]
        [ProducesResponseType(typeof(List<ResponseModel>), 409)]
        [ProducesResponseType(typeof(List<ResponseModel>), 500)]
        public async Task<ActionResult<ResponseModel>> RemoverImagemPerfil()
        {
            try
            {
                int usuarioID = Servico.Claims().usuarioID;

                ResponseModel response = await _usuarioRepositorio.RemoverImagemPerfil(usuarioID);

                return Controladores.Retorno(this, response);
            }
            catch (Exception ex)
            {
                Servico.GravaLog($"{nameof(UsuariosController)}.{nameof(RemoverImagemPerfil)}", ex);
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
