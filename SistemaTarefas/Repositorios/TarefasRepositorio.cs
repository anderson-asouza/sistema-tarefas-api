using AutoMapper;
using Azure.Core;
using Azure.Identity;
using Microsoft.AspNetCore.Authorization.Infrastructure;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Conventions;
using SistemaTarefas;
using SistemaTarefas.Controllers;
using SistemaTarefas.Data;
using SistemaTarefas.DTO.Query;
using SistemaTarefas.DTO.Request;
using SistemaTarefas.DTO.Response;
using SistemaTarefas.Enums;
using SistemaTarefas.Models;
using SistemaTarefas.Repositorios.Interfaces;
using SistemaTarefas.Servicos;
using SistemaTarefas.Util;
using System.Collections.Generic;
using System.ComponentModel;
using System.Reflection.Emit;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace SistemaTarefas.Repositorios
{
    public class TarefasRepositorio(
        IServiceProvider _serviceProvider,
        SistemaTarefasDBContex _dbContext,
        IMapper _mapper    
    ) : ITarefasRepositorio
    {
        private ITramitesRepositorio? _tramitesRepositorio;
        private ITramitesRepositorio spTramitesRepositorio => _tramitesRepositorio ??= _serviceProvider.GetRequiredService<ITramitesRepositorio>();
        public async Task<List<TarefaResponse>> BuscarVarios(string? status = null, string? nomeTarefa = null, bool ordenarPelaDataInicial = false)
        {
            try
            {
                #region monta SQL

                string sql = @"SELECT 
                                t.TAR_ID AS TarId, 
                                t.TAR_Nome AS TarNomeTarefa, 
                                t.TAR_Descricao AS TarDescricao, 
                                t.TAR_DataComeco AS TarDataComeco, 
                                t.TAR_DataFinalPrevista AS TarDataFinalPrevista,
                                t.TAR_DataFinal AS TarDataFinal, 
                                t.TAR_Status AS TarStatus, 
                                t.TAR_USU_ID_Responsavel AS TarUsuIdResponsavelTarefa, 
                                t.TAR_MTAR_ID AS TarMtarId, 
                                t.TAR_FLA_ID AS TarFlaId, 
                                ISNULL(f.FLA_Rotulo, '') AS FlaRotulo, 
                                ISNULL(f.FLA_Cor, '') AS FlaCor, 
                                u.USU_Nome AS UsuNomeUsuarioResponsavelTarefa, 
                                tt.MTAR_Nome AS MtarNome, 
                                tt.MTAR_Descricao AS MtarDescricao 
                                FROM Tarefas t 
                                     INNER JOIN Usuarios u ON t.TAR_USU_ID_Responsavel = u.USU_ID 
                                     INNER JOIN ModelosTarefa tt ON t.TAR_MTAR_ID = tt.MTAR_ID 
                                     LEFT JOIN Flags f ON t.TAR_FLA_ID = f.FLA_ID ";

                var filtros = new List<string>();
                var parametros = new List<SqlParameter>();

                if (!string.IsNullOrWhiteSpace(nomeTarefa))
                {
                    filtros.Add("UPPER(t.TAR_Nome) COLLATE Latin1_General_CI_AI LIKE '%' + UPPER(@nomeTarefa) + '%'");
                    parametros.Add(new SqlParameter("@nomeTarefa", nomeTarefa));
                }

                if (!string.IsNullOrWhiteSpace(status))
                {
                    filtros.Add("t.TAR_Status = @status");
                    parametros.Add(new SqlParameter("@status", status));
                }

                if (filtros.Count > 0)
                {
                    sql += " WHERE " + string.Join(" AND ", filtros);
                }

                sql += " ORDER BY ";

                if (ordenarPelaDataInicial)
                {
                    sql += "t.TAR_DataComeco, ";
                }
                
                sql += "t.TAR_Nome";

                List<TarefasSQL> resultado = await _dbContext
                        .TarefasSQL
                        .FromSqlRaw(sql, parametros.ToArray())
                        .ToListAsync();

                #endregion

                if (!resultado.Any())
                {
                    return new List<TarefaResponse>
                    {
                        new TarefaResponse
                        {
                            RM = "Nenhum registro encontrado.",
                            RC = ResponseCode.RegistroNaoEncontrado,
                            OK = false
                        }
                    };
                }

                List<TarefaResponse> resposta = _mapper.Map<List<TarefaResponse>>(resultado);

                return resposta;
            }
            catch (Exception ex)
            {
                Servico.GravaLog($"{nameof(TarefasRepositorio)}.{nameof(BuscarVarios)}", ex);
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
        public async Task<TarefaResponse> BuscarUm(int id, string? nomeTarefa = null)
        {
            try
            {
                #region monta SQL

                string sql = @"SELECT 
                                t.TAR_ID AS TarId, 
                                t.TAR_Nome AS TarNomeTarefa, 
                                t.TAR_Descricao AS TarDescricao, 
                                t.TAR_DataComeco AS TarDataComeco, 
                                t.TAR_DataFinalPrevista AS TarDataFinalPrevista,
                                t.TAR_DataFinal AS TarDataFinal, 
                                t.TAR_Status AS TarStatus, 
                                t.TAR_USU_ID_Responsavel AS TarUsuIdResponsavelTarefa, 
                                t.TAR_MTAR_ID AS TarMtarId, 
                                t.TAR_FLA_ID AS TarFlaId, 
                                ISNULL(f.FLA_Rotulo, '') AS FlaRotulo, 
                                ISNULL(f.FLA_Cor, '') AS FlaCor, 
                                u.USU_Nome AS UsuNomeUsuarioResponsavelTarefa, 
                                tt.MTAR_Nome AS MtarNome, 
                                tt.MTAR_Descricao AS MtarDescricao 
                                FROM Tarefas t 
                                     INNER JOIN Usuarios u ON t.TAR_USU_ID_Responsavel = u.USU_ID 
                                     INNER JOIN ModelosTarefa tt ON t.TAR_MTAR_ID = tt.MTAR_ID 
                                     LEFT JOIN Flags f ON t.TAR_FLA_ID = f.FLA_ID ";

                var parametros = new List<SqlParameter>();

                if (id > 0)
                {
                    sql += "WHERE t.TAR_ID = @id ";
                    parametros.Add(new SqlParameter("@id", id));
                }
                else
                if (!string.IsNullOrWhiteSpace(nomeTarefa))
                {
                    sql += "WHERE UPPER(t.TAR_Nome) = UPPER(@nomeTarefa) ";
                    parametros.Add(new SqlParameter("@nomeTarefa", nomeTarefa));
                }

                TarefasSQL? resultado = await _dbContext
                        .TarefasSQL
                        .FromSqlRaw(sql, parametros.ToArray())
                        .FirstOrDefaultAsync();

                #endregion

                if (resultado == null)
                {
                    return new TarefaResponse
                    {
                        RM = "Tarefa não foi encontrada.",
                        RC = ResponseCode.RegistroNaoEncontrado,
                        OK = false
                    };
                }

                TarefaResponse resposta = _mapper.Map<TarefaResponse>(resultado);

                return resposta;
            }
            catch (Exception ex)
            {
                Servico.GravaLog($"{nameof(TarefasRepositorio)}.{nameof(BuscarUm)}", ex);
                return new TarefaResponse
                {
                    RM = Servico.MSG_EXCEPTION,
                    RC = ResponseCode.BadRequest,
                    OK = false
                };
            }
        }
        private async Task<bool>  ValidaGravacao(TarefaResponse resposta, string TarNomeTarefa, int TarMtarId, int id = 0)
        {
            try
            {
                using var scope1 = _serviceProvider.CreateScope();
                using var scope2 = _serviceProvider.CreateScope();

                var db1 = scope1.ServiceProvider.GetRequiredService<SistemaTarefasDBContex>();
                var db2 = scope2.ServiceProvider.GetRequiredService<SistemaTarefasDBContex>();

                var tasks = new List<Task>();

                var duplicado = db1.Tarefas.AnyAsync(u => u.TarId != id && (u.TarNomeTarefa.ToUpper().Trim() == TarNomeTarefa.ToUpper().Trim()));
                var modeloTarefa = db2.ModelosTarefa.AnyAsync(x => x.MtarId == TarMtarId);

                tasks.Add(duplicado);
                tasks.Add(modeloTarefa);

                await Task.WhenAll(tasks);

                List<string> erro = new List<string>();
                List<string> erroCode = new List<string>();

                if (await duplicado)
                {
                    erro.Add("Nome já usado para uma Tarefa.");
                    erroCode.Add("TAREFAS_VALIDACAO_NOME_TAREFA_EM_USO");
                }

                if (!await modeloTarefa)
                {
                    erro.Add("Modelo de Tarefa informado não foi encontrado.");
                    erroCode.Add("TAREFAS_VALIDACAO_MODELO_TAREFA_NAO_ENCONTRADO");
                }

                if (erro.Any())
                {
                    resposta.RM = string.Join(Environment.NewLine, erro);
                    resposta.errorCode = string.Join(", ", erroCode);
                    resposta.RC = ResponseCode.EntidadeNaoProcessavel;
                    return false;
                }

                return true;
            }
            catch (Exception ex)
            {
                Servico.GravaLog($"{nameof(TarefasRepositorio)}.{nameof(ValidaGravacao)} Id[{id}]", ex);
                resposta.RC = ResponseCode.Excecao;
                return false;
            }
        }
        public async Task<TarefaResponse> Cadastrar(TarefaRequest tarefaRequest, int usuarioID)
        {
            try
            {
                TarefaResponse resposta = new TarefaResponse();

                if (!await ValidaGravacao(resposta, tarefaRequest.TarNomeTarefa, tarefaRequest.TarMtarId))
                {
                    return resposta;
                }

                int dias = await _dbContext.ModelosTramite.Where(x => x.MtraMtarId == tarefaRequest.TarMtarId).SumAsync(x => x.MtraDuracaoPrevistaDias);

                if (dias < 1)
                {
                    return new TarefaResponse
                    {
                        RM = "Não há Trâmites válidos neste Modelo de Tarefa.",
                        errorCode = "TAREFAS_CADASTRAR_NAO_HA_TRAMITES_VALIDOS_MODELO",
                        RC = ResponseCode.EntidadeNaoProcessavel,
                        OK = false
                    };
                }

                using var transaction = await _dbContext.Database.BeginTransactionAsync();

                Tarefas tarefa = _mapper.Map<Tarefas>(tarefaRequest);

                tarefa.TarUsuIdResponsavelTarefa = usuarioID;
                tarefa.TarStatus = StatusTarefa.Aberta;
                tarefa.TarDataComeco = DateTime.Now.Date;
                tarefa.TarDataFinalPrevista = tarefa.TarDataComeco.AddDays(dias);
                tarefa.TarDataFinal = null;
                tarefa.TarFlaId = null;

                await _dbContext.Tarefas.AddAsync(tarefa);
                await _dbContext.SaveChangesAsync();

                _mapper.Map(tarefa, resposta);

                resposta.RM = "";
                resposta.RC = ResponseCode.CadastradoSucesso;
                resposta.OK = true;

                TramiteResponse respostaInclusao = await spTramitesRepositorio.IncluirTramite(tarefa.TarId);

                if (!respostaInclusao.OK)
                {                    
                    Servico.GravaLog($"Trâmite Inicial não foi gravado | Modelo Tarefa Id[{tarefa.TarMtarId}] | "+respostaInclusao.RM);

                    await transaction.RollbackAsync();

                    return new TarefaResponse
                    {
                        RM = "Erro ao cadastrar a tarefa: o trâmite inicial não pôde ser gravado.",
                        errorCode = "TAREFAS_INICIAL_NAO_GRAVADO",
                        RC = respostaInclusao.RC,
                        OK = false
                    };
                }

                await transaction.CommitAsync();
                return resposta;
            }
            catch (Exception ex)
            {
                Servico.GravaLog($"{nameof(TarefasRepositorio)}.{nameof(Cadastrar)}", ex);
                return new TarefaResponse
                {
                    RC = ResponseCode.Excecao,
                    OK = false
                };
            }
        }
        public async Task<TarefaResponse> Atualizar(TarefaUpdRequest tarefaUpdRequest, int idTarefa)
        {
            try
            {
                Tarefas? tarefa = await _dbContext.Tarefas.FirstOrDefaultAsync(x => x.TarId == idTarefa);

                if (tarefa == null)
                {
                    return new TarefaResponse
                    {
                        RM = "Não foi encontrada a Tarefa para atualização do registro.",
                        errorCode = "TAREFAS_ATUALIZAR_REGISTRO_NAO_ENCONTRADO",
                        RC = ResponseCode.RegistroNaoEncontrado,
                        OK = false
                    };
                }

                TarefaResponse resposta = new TarefaResponse();

                if (await ValidaGravacao(resposta, tarefaUpdRequest.TarNomeTarefa, tarefa.TarMtarId, idTarefa))
                {
                    return resposta;
                }

                tarefa.TarNomeTarefa = tarefaUpdRequest.TarNomeTarefa;
                tarefa.TarDescricao = tarefaUpdRequest.TarDescricao;

                await _dbContext.SaveChangesAsync();

                resposta = _mapper.Map<TarefaResponse>(tarefa);

                resposta.RM = "Tarefa atualizada com sucesso.";
                resposta.RC = ResponseCode.CadastradoSucesso;
                resposta.OK = true;

                return resposta;
            }
            catch (Exception ex)
            {
                Servico.GravaLog($"{nameof(TarefasRepositorio)}.{nameof(Atualizar)}", ex);
                return new TarefaResponse
                {
                    RM = Servico.MSG_EXCEPTION,
                    errorCode = "MSG_EXCEPTION",
                    RC = ResponseCode.Excecao,
                    OK = false
                };
            }
        }
        private async Task<bool> IntegridadeReferencialTarefa(ResponseModel resposta, int id)
        {
            try
            {
                bool tramites = await _dbContext.Tramites.AnyAsync(x => x.TraTarId == id && (x.TraStatus != StatusTramite.TerminadoOK && x.TraStatus != StatusTramite.TerminadoFalha));

                List<string> erro = new List<string>();
                List<string> erroCode = new List<string>();

                if (tramites)
                {
                    erro.Add("Tramites");
                    erroCode.Add("TRAMITES");
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
                Servico.GravaLog($"{nameof(TarefasRepositorio)}.{nameof(IntegridadeReferencialTarefa)} Id[{id}]", ex);
                resposta.RC = ResponseCode.Excecao;
                return false;
            }            
        }
        public async Task<ResponseModel> Apagar(int idTarefa)
        {
            try
            {
                Tarefas? tarefa = await _dbContext.Tarefas.FirstOrDefaultAsync(x => x.TarId == idTarefa);

                if (tarefa == null)
                {
                    return new ResponseModel
                    {
                        RM = "Registro para exclusão não foi encontrado.",
                        RC = ResponseCode.RegistroNaoEncontrado,
                        OK = false
                    };
                }

                if (tarefa.TarStatus != StatusTarefa.Desativada)
                {
                    ResponseModel resposta = new ResponseModel();

                    if (!await IntegridadeReferencialTarefa(resposta, idTarefa))
                    {
                        return resposta;
                    }
                }

                int usuarioID = Servico.Claims().usuarioID;

                _dbContext.Tarefas.Remove(tarefa);
                await _dbContext.SaveChangesAsync();

                Servico.GravaLog($"{nameof(TarefasRepositorio)}.{nameof(Apagar)} Tarefa Apagada Tarefa Id[{idTarefa}] Usuário Id[{usuarioID}] ");

                return new ResponseModel
                {
                    RM = "Registro apagado.",
                    RC = ResponseCode.OK,
                    OK = true
                };
            }
            catch (Exception ex)
            {
                Servico.GravaLog($"{nameof(TarefasRepositorio)}.{nameof(Apagar)} Id[{idTarefa}]", ex);
                return new ResponseModel
                {
                    RM = Servico.MSG_EXCEPTION,
                    RC = ResponseCode.Excecao,
                    OK = false
                };
            }
        }
        public async Task<ResponseModel> MarcarFlagNaTarefa(int idTarefa, int idFlag, int usuarioID)
        {
            try
            {
                Tarefas? tarefa = await _dbContext.Tarefas.FirstOrDefaultAsync(x => x.TarId == idTarefa);

                if (tarefa == null)
                {
                    return new ResponseModel
                    {
                        RM = "Tarefa não foi encontrada.",
                        RC = ResponseCode.RegistroNaoEncontrado,
                        OK = false
                    };
                }

                if (tarefa.TarUsuIdResponsavelTarefa != usuarioID)
                {
                    return new ResponseModel
                    {
                        RM = "Flag não alterada. Somente o resposável da Tarefa pode mudar a Flag.",
                        errorCode = "TAREFAS_SOMENTE_RESPONSAVEL_MUDAR_FLAG",
                        RC = ResponseCode.EntidadeNaoProcessavel,
                        OK = false
                    };
                }

                Flags? flag = new Flags();

                if (idFlag > 0)
                {
                    flag = await _dbContext.Flags.FirstOrDefaultAsync(x => x.FlaId == idFlag);

                    if (flag == null)
                    {
                        return new ResponseModel
                        {
                            RM = "Flag não foi encontrada.",
                            RC = ResponseCode.RegistroNaoEncontrado,
                            OK = false
                        };
                    }
                }

                tarefa.TarFlaId = (idFlag > 0) ? idFlag : null;
                await _dbContext.SaveChangesAsync();

                return new ResponseModel
                {
                    RM = (idFlag > 0) ? $"Flag [{flag.FlaRotulo}] marcada na Tarefa." : "Flag desmarcada na Tarefa.",
                    RC = ResponseCode.OK,
                    OK = true
                };
            }
            catch (Exception ex)
            {
                Servico.GravaLog($"{nameof(TarefasRepositorio)}.{nameof(MarcarFlagNaTarefa)}", ex);
                return new ResponseModel
                {
                    RM = Servico.MSG_EXCEPTION,
                    RC = ResponseCode.Excecao,
                    OK = false
                };
            }
        }
        public async Task<ResponseModel> FecharTarefa(int idTarefa)
        {
            try
            {
                ResponseModel resposta = new ResponseModel();

                if (!await IntegridadeReferencialTarefa(resposta, idTarefa))
                {
                    return resposta;
                }

                Tarefas? tarefa = await _dbContext.Tarefas.FirstOrDefaultAsync(x => x.TarId == idTarefa);

                if (tarefa == null)
                {
                    return new ResponseModel
                    {
                        RM = "Registro não foi encontrado.",
                        RC = ResponseCode.RegistroNaoEncontrado,
                        OK = false
                    };
                }

                if (tarefa.TarStatus == StatusTarefa.Finalizada)
                {
                    return new ResponseModel
                    {
                        RM = "Tarefa já está fechada.",
                        RC = ResponseCode.BadRequest,
                        OK = false
                    };
                }

                tarefa.TarStatus = StatusTarefa.Finalizada;
                tarefa.TarDataFinal = DateTime.Now.Date;

                await _dbContext.SaveChangesAsync();

                string rm = "Tarefa Fechada com sucesso!";

                if (tarefa.TarDataFinal < tarefa.TarDataFinalPrevista)
                {
                    int diasAntecipado = (int)(tarefa.TarDataFinalPrevista.Value - tarefa.TarDataFinal.Value).TotalDays;

                    rm += Environment.NewLine + $" Finalizada com {diasAntecipado} dia{(diasAntecipado > 1 ? "s" : "")} de antecedência.";
                }
                else if (tarefa.TarDataFinal > tarefa.TarDataFinalPrevista)
                {
                    int diasPosteriores = (int)(tarefa.TarDataFinal.Value - tarefa.TarDataFinalPrevista.Value).TotalDays;

                    rm += Environment.NewLine + $" Finalizada com {diasPosteriores} dia{(diasPosteriores > 1 ? "s" : "")} de ATRASO.";
                }

                return new ResponseModel
                {
                    RM = rm,
                    RC = ResponseCode.OK,
                    OK = true
                };
            }
            catch (Exception ex)
            {
                Servico.GravaLog($"{nameof(TarefasRepositorio)}.{nameof(FecharTarefa)}", ex);
                return new ResponseModel
                {
                    RM = Servico.MSG_EXCEPTION,
                    RC = ResponseCode.Excecao,
                    OK = false
                };
            }
        }
        public async Task<ResponseModel> AtivarDesativarTarefa(int idTarefa, bool ativar = false)
        {
            try
            {
                Tarefas? tarefa = await _dbContext.Tarefas.FirstOrDefaultAsync(x => x.TarId == idTarefa);

                if (tarefa == null)
                {
                    return new ResponseModel
                    {
                        RM = "Registro não foi encontrado.",
                        RC = ResponseCode.RegistroNaoEncontrado,
                        OK = false
                    };
                }

                if (tarefa.TarStatus == StatusTarefa.Finalizada)
                {
                    return new ResponseModel
                    {
                        RM = "Tarefa está Finalizada não pode ser reativada.",
                        errorCode = "TAREFAS_FINALIZADA_NAO_PODE_ATIVAR",
                        RC = ResponseCode.EntidadeNaoProcessavel,
                        OK = false
                    };
                }

                if ((ativar && tarefa.TarStatus == StatusTarefa.Aberta) || (!ativar && tarefa.TarStatus == StatusTarefa.Desativada))
                {
                    return new ResponseModel
                    {
                        RM = $"Tarefa já está {tarefa.TarStatus.ToDescricao()}. Nenhuma ação efetuada.",
                        RC = ResponseCode.EntidadeNaoProcessavel,
                        OK = false
                    };
                }

                tarefa.TarStatus = (ativar) ? StatusTarefa.Aberta : StatusTarefa.Desativada;
                tarefa.TarDataFinal = null;

                await _dbContext.SaveChangesAsync();

                return new ResponseModel
                {
                    RM = $"Tarefa {tarefa.TarStatus.ToDescricao()} com sucesso.",
                    RC = ResponseCode.OK,
                    OK = true
                };
            }
            catch (Exception ex)
            {
                Servico.GravaLog($"{nameof(TarefasRepositorio)}.{nameof(AtivarDesativarTarefa)}", ex);
                return new ResponseModel
                {
                    RM = Servico.MSG_EXCEPTION,
                    RC = ResponseCode.Excecao,
                    OK = false
                };
            }
        }
        public async Task<ResponseModel> AjustarPrazoTarefa(int idTarefa, int ajusteEmDias)
        {
            try
            {
                Tarefas? tarefa = await _dbContext.Tarefas.FirstOrDefaultAsync(x => x.TarId == idTarefa);

                if (tarefa == null)
                {
                    return new ResponseModel
                    {
                        RM = "Registro não foi encontrado.",
                        RC = ResponseCode.RegistroNaoEncontrado,
                        OK = false
                    };
                }

                if (tarefa.TarStatus != StatusTarefa.Aberta)
                {
                    return new ResponseModel
                    {
                        RM = "Tarefa deve estar aberta.",
                        RC = ResponseCode.EntidadeNaoProcessavel,
                        OK = false
                    };
                }

                tarefa.TarDataFinalPrevista = Convert.ToDateTime(tarefa.TarDataFinalPrevista).AddDays(ajusteEmDias).Date;
                tarefa.TarDataFinal = null;

                await _dbContext.SaveChangesAsync();

                return new ResponseModel
                {
                    RM = "Prazo da tarefa atualizado com sucesso.",
                    RC = ResponseCode.OK,
                    OK = true
                };
            }
            catch (Exception ex)
            {
                Servico.GravaLog($"{nameof(TarefasRepositorio)}.{nameof(AjustarPrazoTarefa)}", ex);
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