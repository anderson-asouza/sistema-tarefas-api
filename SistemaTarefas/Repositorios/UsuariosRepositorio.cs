using AutoMapper;
using Azure.Core;
using Azure.Identity;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Conventions;
using Microsoft.Extensions.DependencyInjection;
using SistemaTarefas;
using SistemaTarefas.Controllers;
using SistemaTarefas.Data;
using SistemaTarefas.DTO.Request;
using SistemaTarefas.DTO.Response;
using SistemaTarefas.Enums;
using SistemaTarefas.Models;
using SistemaTarefas.Repositorios.Interfaces;
using SistemaTarefas.Servicos;
using SistemaTarefas.Util;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using static SistemaTarefas.Repositorios.UsuariosRepositorio;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace SistemaTarefas.Repositorios
{
    public class UsuariosRepositorio(
        IServiceProvider _serviceProvider,
        SistemaTarefasDBContex _dbContext,
        IMapper _mapper,
        IWebHostEnvironment _env,
        BancoConfiguracao _configBanco
    ) : IUsuariosRepositorio
    {
        public async Task<bool> ExisteAlgumAdmin()
        {
            return await _dbContext.Usuarios.AnyAsync(u => u.UsuNivel == 1);
        }
        private async Task<bool> ValidaGravacao(string usuLogin, string usuMatricula, UsuarioResponse resposta, int id = 0)
        {
            try
            {
                using var scope1 = _serviceProvider.CreateScope();
                using var scope2 = _serviceProvider.CreateScope();

                var db1 = scope1.ServiceProvider.GetRequiredService<SistemaTarefasDBContex>();
                var db2 = scope2.ServiceProvider.GetRequiredService<SistemaTarefasDBContex>();

                var tasks = new List<Task>();

                var loginDuplicado = db1.Usuarios.AnyAsync(u => u.UsuId != id && u.UsuLogin.ToUpper().Trim() == usuLogin.ToUpper().Trim());
                var matriculaDuplicada = db2.Usuarios.AnyAsync(u => u.UsuId != id && u.UsuMatricula.Trim() == usuMatricula.Trim());

                tasks.Add(loginDuplicado);
                tasks.Add(matriculaDuplicada);

                await Task.WhenAll(tasks);

                List<string> erro = new List<string>();
                List<string> erroCode = new List<string>();

                if (await loginDuplicado)
                {
                    erro.Add("Login já existe para outro usuário.");
                    erroCode.Add("USUARIOS_LOGIN_JA_EXISTE");
                }

                if (await matriculaDuplicada)
                {
                    erro.Add("Matrícula já existe para outro usuário.");
                    erroCode.Add("USUARIOS_MATRICULA_JA_EXISTE");
                }                    

                if (erro.Any())
                {
                    resposta.RM = "Não pode gravar o usuário."+Environment.NewLine+string.Join(Environment.NewLine, erro);
                    resposta.errorCode = string.Join(", ", erroCode);
                    resposta.RC = ResponseCode.EntidadeNaoProcessavel;
                    resposta.OK = false;
                    return false;
                }

                return true;
            }
            catch (Exception ex)
            {
                Servico.GravaLog($"{nameof(UsuariosRepositorio)}.{nameof(ValidaGravacao)}", ex);
                resposta.RC = ResponseCode.Excecao;
                return false;
            }
        }
        public async Task<List<UsuarioResponse>> BuscarVarios(string? nomeUsuario = null)
        {
            try
            {
                var usuarios = new List<Usuarios>();

                if (!string.IsNullOrWhiteSpace(nomeUsuario))
                {
                    usuarios = await _dbContext.Usuarios.Where(x => x.UsuNome!.ToUpper().Contains(nomeUsuario.ToUpper().Trim())).OrderBy(x => x.UsuNome).ToListAsync();
                }
                else
                {
                    usuarios = await _dbContext.Usuarios.OrderBy(x => x.UsuNome).ToListAsync();
                }

                if (!usuarios.Any())
                {
                    return new List<UsuarioResponse>
                    {
                        new UsuarioResponse
                        {
                            RM = "Nenhum registro encontrado.",
                            RC = ResponseCode.RegistroNaoEncontrado,
                            OK = false
                        }
                    };
                }

                List<UsuarioResponse> resposta = _mapper.Map<List<UsuarioResponse>>(usuarios) ?? new List<UsuarioResponse>();

                return resposta;
            }
            catch (Exception ex)
            {
                Servico.GravaLog($"{nameof(UsuariosRepositorio)}.{nameof(BuscarVarios)}", ex);
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
        public async Task<UsuarioResponse> BuscarUm(int id, string? nome = null, string? matricula = null, string? login = null)
        {
            try
            {
                Usuarios? usuario = null;

                if (id > 0)
                    usuario = await _dbContext.Usuarios.FirstOrDefaultAsync(x => x.UsuId == id);

                if (usuario == null && !string.IsNullOrWhiteSpace(nome))
                    usuario = await _dbContext.Usuarios.FirstOrDefaultAsync(x => x.UsuNome == nome);

                if (usuario == null && !string.IsNullOrWhiteSpace(matricula))
                    usuario = await _dbContext.Usuarios.FirstOrDefaultAsync(x => x.UsuMatricula == matricula);

                if (usuario == null && !string.IsNullOrWhiteSpace(login))
                    usuario = await _dbContext.Usuarios.FirstOrDefaultAsync(x => x.UsuLogin == login);
              
                if (usuario == null)
                {
                    return new UsuarioResponse
                    {
                        RM = "Usuário não encontrado.",
                        RC = ResponseCode.RegistroNaoEncontrado,
                        OK = false
                    };
                }

                UsuarioResponse resposta = _mapper.Map<UsuarioResponse>(usuario);

                resposta.RM = "";
                resposta.RC = ResponseCode.OK;
                resposta.OK = true;

                return resposta;
            }
            catch (Exception ex)
            {
                Servico.GravaLog($"{nameof(UsuariosRepositorio)}.{nameof(BuscarUm)}", ex);
                return new UsuarioResponse
                {
                    RM = Servico.MSG_EXCEPTION,
                    RC = ResponseCode.Excecao,
                    OK = false
                };
            }
        }
        public async Task<UsuarioResponse> Cadastrar(UsuarioRequest usuarioRequest)
        {
            try
            {
                UsuarioResponse resposta = new();

                if (!await ValidaGravacao(usuarioRequest.usuLogin, usuarioRequest.usuMatricula, resposta))
                {
                    return resposta;
                }

                Usuarios usuario = _mapper.Map<Usuarios>(usuarioRequest);
                usuario.UsuDataMudancaSenha = DateTime.Now.Date.AddDays( -(_configBanco.ExpiracaoSenhaDias +1));
                usuario.UsuSenha = BCrypt.Net.BCrypt.HashPassword(usuarioRequest.UsuSenha);
                usuario.UsuImagemPerfil = "";

                await _dbContext.Usuarios.AddAsync(usuario);
                await _dbContext.SaveChangesAsync();

                resposta = _mapper.Map<UsuarioResponse>(usuario);

                resposta.RM = "";
                resposta.RC = ResponseCode.CadastradoSucesso;
                resposta.OK = true;

                return resposta;
            }
            catch (Exception ex)
            {
                Servico.GravaLog($"{nameof(UsuariosRepositorio)}.{nameof(Cadastrar)}", ex);
                return new UsuarioResponse
                {
                    RM = Servico.MSG_EXCEPTION,
                    RC = ResponseCode.Excecao,
                    OK = false
                };
            }
        }
        public async Task<UsuarioResponse> Atualizar(UsuarioUpdRequest usuarioRequest, int id)
        {
            try
            {
                Usuarios? usuario = await _dbContext.Usuarios.FirstOrDefaultAsync(x => x.UsuId == id);

                if (usuario == null)
                {
                    return new UsuarioResponse
                    {
                        RM = "Registro para atualização não foi encontrado.",
                        RC = ResponseCode.RegistroNaoEncontrado,
                        OK = false
                    };
                }

                UsuarioResponse resposta = new();

                if (!await ValidaGravacao(".", usuarioRequest.usuMatricula, resposta, id))
                {
                    return resposta;
                }

                _mapper.Map(usuarioRequest, usuario);
                await _dbContext.SaveChangesAsync();

                resposta = _mapper.Map<UsuarioResponse>(usuario);

                resposta.RM = "Atualizado com sucesso.";
                resposta.RC = ResponseCode.CadastradoSucesso;
                resposta.OK = true;
                return resposta;
            }
            catch (Exception ex)
            {
                Servico.GravaLog($"{nameof(UsuariosRepositorio)}.{nameof(Atualizar)}", ex);
                return new UsuarioResponse
                {
                    RM = Servico.MSG_EXCEPTION,
                    RC = ResponseCode.Excecao,
                    OK = false
                };
            }            
        }
        private async Task<bool> IntegridadeReferencialUsuario(int id, ResponseModel resposta)
        {
            try
            {
                using var scope1 = _serviceProvider.CreateScope();
                using var scope2 = _serviceProvider.CreateScope();
                using var scope3 = _serviceProvider.CreateScope();

                var db1 = scope1.ServiceProvider.GetRequiredService<SistemaTarefasDBContex>();
                var db2 = scope2.ServiceProvider.GetRequiredService<SistemaTarefasDBContex>();
                var db3 = scope3.ServiceProvider.GetRequiredService<SistemaTarefasDBContex>();

                var tarefasTask = db1.Tarefas.AnyAsync(x => x.TarUsuIdResponsavelTarefa == id);
                var tramitesTask = db2.Tramites.AnyAsync(x => x.TraUsuIdTramitador == id || x.TraUsuIdRevisor == id);
                var tramitesTipoTarefaTask = db3.ModelosTramite.AnyAsync(x => x.MtraUsuIdIndicacao == id);

                await Task.WhenAll(tarefasTask, tramitesTask, tramitesTipoTarefaTask);

                List<string> erro = new List<string>();
                List<string> erroCode = new List<string>();

                if (await tarefasTask)
                {
                    erro.Add("Tarefas");
                    erroCode.Add("TAREFAS");
                }

                if (await tramitesTask)
                {
                    erro.Add("Trâmites");
                    erroCode.Add("TRAMITES");
                }

                if (await tramitesTipoTarefaTask)
                {
                    erro.Add("Trâmites do Modelo Tarefa");
                    erroCode.Add("TRAMITES_MODELOS_TAREFA");
                }

                if (erro.Any())
                {
                    resposta.RM = "Compromete a Integridade Referencial com:" +Environment.NewLine+ string.Join(Environment.NewLine, erro);
                    resposta.errorCode = "INTEGRIDADE_REFERENCIAL, " + string.Join(", ", erroCode);
                    resposta.RC = ResponseCode.EntidadeNaoProcessavel;
                    return false;
                }

                return true;
            }
            catch (Exception ex)
            {
                resposta.RM = Servico.MSG_EXCEPTION;
                resposta.RC = ResponseCode.Excecao;
                Servico.GravaLog($"{nameof(UsuariosRepositorio)}.{nameof(IntegridadeReferencialUsuario)} Id[{id}]", ex);
                return false;
            }
        }
        public async Task<ResponseModel> Apagar(int id)
        {
            try
            {
                ResponseModel resposta = new ResponseModel();

                if (!await IntegridadeReferencialUsuario(id, resposta))
                {
                    return resposta;
                }

                Usuarios? usuario = await _dbContext.Usuarios.FirstOrDefaultAsync(x => x.UsuId == id);

                if (usuario == null)
                {
                    return new ResponseModel
                    {
                        RM = "Registro para exclusão não foi encontrado.",
                        RC = ResponseCode.RegistroNaoEncontrado,
                        OK = false
                    };
                }

                _dbContext.Usuarios.Remove(usuario);
                await _dbContext.SaveChangesAsync();

                return new ResponseModel
                {
                    RM = "Registro apagado.",
                    RC = ResponseCode.OK,
                    OK = true
                };
            }
            catch (Exception ex)
            {
                Servico.GravaLog($"{nameof(UsuariosRepositorio)}.{nameof(Apagar)} Id[{id}]", ex);
                return new ResponseModel
                {
                    RM = Servico.MSG_EXCEPTION,
                    RC = ResponseCode.Excecao,
                    OK = false
                };
            }
        }
        public async Task<UsuarioAlterarSenhaResponse> AlterarSenha(string Login, string SenhaAtual, string NovaSenha, string ConfirmacaoNovaSenha, bool trocarSenhaPeloAdm = false)
        {
            try
            {
                if (SenhaAtual.Trim() == NovaSenha.Trim())
                {
                    return new UsuarioAlterarSenhaResponse
                    {
                        RM = "Nova senha deve ser diferente da senha atual.",
                        errorCode = "USUARIOS_NOVASENHA_DEVE_SER_DIFERENTE_SENHAATUAL",
                        RC = ResponseCode.EntidadeNaoProcessavel,
                        OK = false
                    };
                }

                if (string.IsNullOrWhiteSpace(NovaSenha))
                {
                    return new UsuarioAlterarSenhaResponse
                    {
                        RM = "Informe uma Nova Senha.",
                        errorCode = "USUARIOS_INFORME_NOVASENHA",
                        RC = ResponseCode.EntidadeNaoProcessavel,
                        OK = false
                    };
                }

                if (NovaSenha != ConfirmacaoNovaSenha)
                {
                    return new UsuarioAlterarSenhaResponse
                    {
                        RM = "Nova senha diferente de sua confirmação. Senha NÃO foi alterada.",
                        errorCode = "USUARIOS_NOVASENHA_DIFERENTE_CONFIRMACAO",
                        RC = ResponseCode.EntidadeNaoProcessavel,
                        OK = false
                    };
                }

                (int admID, int nivel) = Servico.Claims();

                if (trocarSenhaPeloAdm && nivel != 1)
                {
                    Servico.GravaLog($"{Servico.ERRO_USO_API} usuário [{admID}] tentou forçar alteração de senha.");
                    return new UsuarioAlterarSenhaResponse
                    {
                        RM = "Usuário logado não possui perfil de Administrador para forçar alteração de senha.",
                        RC = ResponseCode.EntidadeNaoProcessavel,
                        OK = false
                    };
                }

                Usuarios? usuario = await _dbContext.Usuarios.FirstOrDefaultAsync(x => x.UsuLogin == Login);

                if (usuario == null || (!trocarSenhaPeloAdm && !BCrypt.Net.BCrypt.Verify(SenhaAtual, usuario.UsuSenha)))
                {
                    return new UsuarioAlterarSenhaResponse
                    {
                        RM = "Credencias Inválidas! Verifique login e senha.",
                        RC = ResponseCode.NaoAutorizado,
                        OK = false
                    };
                }

                UsuarioAlterarSenhaResponse retorno = new UsuarioAlterarSenhaResponse();

                retorno = _mapper.Map<UsuarioAlterarSenhaResponse>(usuario);

                usuario.UsuSenha = BCrypt.Net.BCrypt.HashPassword(NovaSenha);

                _dbContext.Usuarios.Update(usuario);
                usuario.UsuDataMudancaSenha = (trocarSenhaPeloAdm) ? DateTime.Now.Date.AddDays(-(_configBanco.ExpiracaoSenhaDias +1)) : DateTime.Now.Date;
                await _dbContext.SaveChangesAsync();

                if (trocarSenhaPeloAdm)
                {                    
                    Servico.GravaLog($"Alteração de senha pelo Administrador [{admID}] para o usuário [{usuario.UsuId}].");

                    retorno.usuImagemPerfil = null;
                    retorno.usuToken = null;
                }

                retorno.RM = "Senha alterada com sucesso.";
                retorno.RC = ResponseCode.OK;
                retorno.OK = true;
                return retorno;
            }
            catch (Exception ex)
            {
                Servico.GravaLog($"{nameof(UsuariosRepositorio)}.{nameof(AlterarSenha)}", ex);
                return new UsuarioAlterarSenhaResponse
                {
                    RM = Servico.MSG_EXCEPTION,
                    RC = ResponseCode.Excecao,
                    OK = false
                };
            }
        }
        public async Task<UsuarioLoginResponse> Logar(string Login, string Senha, string? NovaSenha = null, string? ConfirmacaoNovaSenha = null)
        {
            try 
            {
                #region Credenciais

                Usuarios? usuario = await _dbContext.Usuarios.FirstOrDefaultAsync(x => x.UsuLogin.ToLower() == Login.ToLower());

                if (usuario == null || !BCrypt.Net.BCrypt.Verify(Senha, usuario.UsuSenha))
                {
                    return new UsuarioLoginResponse
                    {
                        RM = "Credencias Inválidas! Verifique usuário e senha.",
                        errorCode = "USUARIOS_CREDENCIAIS_INVALIDAS",
                        RC = ResponseCode.NaoAutorizado,
                        OK = false
                    };
                }

                if (usuario.UsuNivel < 1)
                {
                    return new UsuarioLoginResponse
                    {
                        RM = "Usuário Bloqueado.",
                        errorCode = "USUARIOS_BLOQUEADO",
                        RC = ResponseCode.NaoAutorizado,
                        OK = false
                    };

                }

                #endregion

                #region Troca de Senha

                UsuarioLoginResponse retorno = _mapper.Map<UsuarioLoginResponse>(usuario);

                if (!string.IsNullOrWhiteSpace(NovaSenha) || !string.IsNullOrWhiteSpace(ConfirmacaoNovaSenha))
                {
                    retorno = await AlterarSenha(Login, Senha, NovaSenha!, ConfirmacaoNovaSenha!);
                    return retorno;
                }

                #endregion

                #region Forçar Trocar Senha 

                if (usuario.UsuLogin.ToLower().Trim() == "admin" && string.IsNullOrWhiteSpace(NovaSenha) && BCrypt.Net.BCrypt.Verify(Senha, Servico.SENHA_INICIAL_ADMIN))
                {
                    return new UsuarioLoginResponse
                    {
                        RM = "A senha do usuário deve ser trocada agora.",
                        errorCode = "USUARIOS_USUARIO_DEVE_TROCAR_SENHA",
                        RC = ResponseCode.NaoAutorizado,
                        OK = false
                    };
                }

                if (string.IsNullOrWhiteSpace(NovaSenha) && DateTime.Now.Date > usuario.UsuDataMudancaSenha.AddDays(_configBanco.ExpiracaoSenhaDias))
                {
                    return new UsuarioLoginResponse
                    {
                        RM = "A Senha Expirou! O usuário deve alterar a senha.",
                        errorCode = "USUARIOS_SENHA_EXPIROU",
                        RC = ResponseCode.NaoAutorizado,
                        OK = false
                    };
                }

                #endregion

                retorno.RC = ResponseCode.OK;
                retorno.OK = true;
                return retorno;
            }
            catch (Exception ex)
            {
                Servico.GravaLog($"{nameof(UsuariosRepositorio)}.{nameof(Logar)}", ex);
                return new UsuarioLoginResponse
                {
                    RM = Servico.MSG_EXCEPTION,
                    RC = ResponseCode.Excecao,
                    OK = false
                };
            }
        }
        public async Task<UsuarioImagemResponse> UploadImagemPerfil(UploadImagemPerfilRequest arq, int usuarioID)
        {
            try
            {
                var usuario = await _dbContext.Usuarios.FindAsync(usuarioID);

                if (usuario == null)
                {
                    return new UsuarioImagemResponse
                    {
                        RM = "Usuário não encontrado.",
                        RC = ResponseCode.RegistroNaoEncontrado,
                        OK = false
                    };
                }

                if (!string.IsNullOrEmpty(usuario.UsuImagemPerfil))
                {
                    var pastaUploadsDel = Path.Combine(_env.WebRootPath, Servico.PATH_IMG_PERFIL);
                    var caminhoArquivoDel = Path.Combine(pastaUploadsDel, usuario.UsuImagemPerfil);

                    if (File.Exists(caminhoArquivoDel))
                    {
                        try
                        {
                            File.Delete(caminhoArquivoDel);
                        }
                        catch (Exception ex)
                        {
                            Servico.GravaLog(
                                    $"{nameof(UsuariosRepositorio)}.{nameof(RemoverImagemPerfil)} " +
                                    $"Falha ao excluir arquivo físico. Id[{usuarioID}] Path[{caminhoArquivoDel}]",
                                    ex);
                        }
                    }
                }

                var nomeArquivo = $"{Guid.NewGuid()}_{Path.GetFileName(arq.Imagem!.FileName)}";
                var pastaUploads = Path.Combine(_env.WebRootPath, Servico.PATH_IMG_PERFIL);

                if (!Directory.Exists(pastaUploads))
                    Directory.CreateDirectory(pastaUploads);

                var caminhoArquivo = Path.Combine(pastaUploads, nomeArquivo);

                using (var stream = new FileStream(caminhoArquivo, FileMode.Create))
                {
                    await arq.Imagem.CopyToAsync(stream);
                }

                usuario.UsuImagemPerfil = nomeArquivo;
                await _dbContext.SaveChangesAsync();

                return new UsuarioImagemResponse
                {
                    usuImagemPerfil = usuario.UsuImagemPerfil,
                    RM = "Imagem de perfil atualizada com sucesso.",
                    RC = ResponseCode.OK,
                    OK = true
                };
            }
            catch (Exception ex)
            {
                Servico.GravaLog($"{nameof(UsuariosRepositorio)}.{nameof(UploadImagemPerfil)}", ex);
                return new UsuarioImagemResponse
                {
                    RM = Servico.MSG_EXCEPTION,
                    RC = ResponseCode.Excecao,
                    OK = false
                };
            }
        }
        public async Task<ResponseModel> RemoverImagemPerfil(int usuarioID)
        {
            try
            {
                var usuario = await _dbContext.Usuarios.FindAsync(usuarioID);

                if (usuario == null)
                {
                    return new ResponseModel
                    {
                        RM = "Usuário não encontrado.",
                        RC = ResponseCode.RegistroNaoEncontrado,
                        OK = false
                    };
                }

                if (!string.IsNullOrEmpty(usuario.UsuImagemPerfil))
                {
                    var pastaUploads = Path.Combine(_env.WebRootPath, Servico.PATH_IMG_PERFIL);
                    var caminhoArquivo = Path.Combine(pastaUploads, usuario.UsuImagemPerfil);

                    if (File.Exists(caminhoArquivo))
                    {
                        try
                        {
                            File.Delete(caminhoArquivo);
                        }
                        catch (Exception ex)
                        {
                            Servico.GravaLog(
                                    $"{nameof(UsuariosRepositorio)}.{nameof(RemoverImagemPerfil)} " +
                                    $"Falha ao excluir arquivo físico. Id[{usuarioID}] Path[{caminhoArquivo}]",
                                    ex);
                        }
                    }
                }

                usuario.UsuImagemPerfil = "";
                await _dbContext.SaveChangesAsync();

                return new ResponseModel
                {
                    RM = "Imagem removida do perfil.",
                    RC = ResponseCode.OK,
                    OK = true
                };
            }
            catch (Exception ex)
            {
                Servico.GravaLog($"{nameof(UsuariosRepositorio)}.{nameof(RemoverImagemPerfil)} Id[{usuarioID}]", ex);
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
