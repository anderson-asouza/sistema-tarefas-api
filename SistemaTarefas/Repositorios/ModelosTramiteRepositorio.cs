using AutoMapper;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using SistemaTarefas.Controllers;
using SistemaTarefas.Data;
using SistemaTarefas.DTO.Query;
using SistemaTarefas.DTO.Request;
using SistemaTarefas.DTO.Response;
using SistemaTarefas.Enums;
using SistemaTarefas.Models;
using SistemaTarefas.Repositorios.Interfaces;
using SistemaTarefas.Servicos;

namespace SistemaTarefas.Repositorios
{
    public class ModelosTramiteRepositorio(
        IServiceProvider _serviceProvider,
        SistemaTarefasDBContex _dbContext,
        IMapper _mapper
    ) : IModelosTramiteRepositorio
    {
        private async Task<bool> IntegridadeReferencial(int id, ResponseModel resposta)
        {
            try
            {
                ModelosTramite? tramitesModelosTarefas = await _dbContext.ModelosTramite.FirstOrDefaultAsync(x => x.MtraId == id);

                if (tramitesModelosTarefas == null)
                {
                    resposta.RM = "Trâmite Modelo Tarefa não foi encontrado.";
                    resposta.errorCode = "MODELOS_TRAMITE_NAO_ENCONTRADO";
                    resposta.RC = ResponseCode.EntidadeNaoProcessavel;
                    resposta.OK = false;
                    return false;
                }

                bool existeTramitePosterior = await _dbContext.ModelosTramite.AnyAsync(x => x.MtraMtarId == tramitesModelosTarefas.MtraMtarId && x.MtraOrdem > tramitesModelosTarefas.MtraOrdem);

                if (existeTramitePosterior)
                {
                    resposta.RM = "Só é possível excluir um Trâmite Modelo Tarefa que esteja em última posição.";
                    resposta.errorCode = "MODELOS_TRAMITE_SO_PODE_EXCLUIR_ULTIMO";
                    resposta.RC = ResponseCode.EntidadeNaoProcessavel;
                    resposta.OK = false;
                    return false;
                }

                return true;
            }
            catch (Exception ex)
            {
                resposta.RC = ResponseCode.Excecao;
                Servico.GravaLog($"{nameof(ModelosTramiteRepositorio)}.{nameof(IntegridadeReferencial)} Id[{id}]", ex);
                return false;
            }
        }
        public async Task<List<ModeloTramiteResponse>> BuscarVarios(int idModeloTarefa = 0, string? nomeModeloTarefa = null)
        {
            try
            {
                #region monta SQL

                string sql = @"SELECT 
                                t.MTRA_ID AS MtraId,
                                t.MTRA_NomeTramite AS MtraNomeTramite,
                                t.MTRA_DescricaoTramite AS MtraDescricaoTramite,
                                t.MTRA_MTAR_ID AS MtraMtarId,
                                t.MTRA_USU_ID_Indicacao AS MtraUsuIdIndicacao,
                                t.MTRA_USU_ID_Revisor AS MtraUsuIdRevisor,
                                t.MTRA_DuracaoPrevistaDias AS MtraDuracaoPrevistaDias,
                                t.MTRA_Ordem AS MtraOrdem,
                                tt.MTAR_Nome AS MtarNome,
                                tt.MTAR_Descricao AS MtarDescricao,
                                ISNULL(ui.USU_Nome, '') AS UsuNomeIndicacao,
                                ISNULL(ur.USU_Nome, '') AS UsuNomeRevisor
                                FROM ModelosTramite t INNER JOIN ModelosTarefa tt ON t.MTRA_MTAR_ID = tt.MTAR_ID LEFT JOIN Usuarios ui ON t.MTRA_USU_ID_Indicacao = ui.USU_ID LEFT JOIN Usuarios ur ON t.MTRA_USU_ID_Revisor = ur.USU_ID ";

                var parametros = new List<object>();

                if (idModeloTarefa > 0)
                {
                    sql += "WHERE t.MTRA_MTAR_ID = @idModeloTarefa ";
                    parametros.Add(new SqlParameter("@idModeloTarefa", idModeloTarefa));
                }
                else if (!string.IsNullOrWhiteSpace(nomeModeloTarefa))
                {
                    sql += "UPPER(tt.MTAR_Nome) COLLATE Latin1_General_CI_AI LIKE '%' + UPPER(@nomeModeloTarefa) + '%'";
                    parametros.Add(new SqlParameter("@nomeModeloTarefa", nomeModeloTarefa));
                }

                sql += " ORDER BY MTAR_Nome, MTRA_Ordem";

                #endregion

                #region Executa SQL

                List<ModelosTramiteSQL> resultado = await _dbContext
                        .ModelosTramiteSQL
                        .FromSqlRaw(sql, parametros.ToArray())
                        .ToListAsync();

                if (!resultado.Any())
                {
                    return new List<ModeloTramiteResponse>
                    {
                        new ModeloTramiteResponse
                        {
                            RM = "Nenhum registro encontrado.",
                            RC = ResponseCode.RegistroNaoEncontrado,
                            OK = false
                        }
                    };
                }

                #endregion

                List<ModeloTramiteResponse> resposta = _mapper.Map<List<ModeloTramiteResponse>>(resultado);

                return resposta;
            }
            catch (Exception ex)
            {
                Servico.GravaLog($"{nameof(ModelosTramiteRepositorio)}.{nameof(BuscarVarios)}", ex);
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
        public async Task<ModeloTramiteResponse> BuscarUm(int idModeloTramite)
        {
            try
            {
                #region monta SQL

                string sql = @"SELECT 
                                t.MTRA_ID AS MtraId,
                                t.MTRA_NomeTramite AS MtraNomeTramite,
                                t.MTRA_DescricaoTramite AS MtraDescricaoTramite,
                                t.MTRA_MTAR_ID AS MtraMtarId,
                                t.MTRA_USU_ID_Indicacao AS MtraUsuIdIndicacao,
                                t.MTRA_USU_ID_Revisor AS MtraUsuIdRevisor,
                                t.MTRA_DuracaoPrevistaDias AS MtraDuracaoPrevistaDias,
                                t.MTRA_Ordem AS MtraOrdem,
                                tt.MTAR_Nome AS MtarNome,
                                tt.MTAR_Descricao AS MtarDescricao,
                                ISNULL(ui.USU_Nome, '') AS UsuNomeIndicacao,
                                ISNULL(ur.USU_Nome, '') AS UsuNomeRevisor
                                FROM ModelosTramite t INNER JOIN ModelosTarefa tt ON t.MTRA_MTAR_ID = tt.MTAR_ID LEFT JOIN Usuarios ui ON t.MTRA_USU_ID_Indicacao = ui.USU_ID LEFT JOIN Usuarios ur ON t.MTRA_USU_ID_Revisor = ur.USU_ID 
                                WHERE t.MTRA_ID = @idModeloTramite";

                #endregion

                #region Executa SQL

                var parametro = new SqlParameter("@idModeloTramite", idModeloTramite);

                ModelosTramiteSQL? resultado = await _dbContext
                        .ModelosTramiteSQL
                        .FromSqlRaw(sql, parametro)
                        .FirstOrDefaultAsync();

                if (resultado == null)
                {
                    return new ModeloTramiteResponse
                    {
                        RM = "Nenhum Trâmite de Modelo Tarefa foi encontrado.",
                        RC = ResponseCode.RegistroNaoEncontrado,
                        OK = false
                    };
                }

                #endregion

                ModeloTramiteResponse resposta = _mapper.Map<ModeloTramiteResponse>(resultado);

                return resposta;
            }
            catch (Exception ex)
            {
                Servico.GravaLog($"{nameof(ModelosTramiteRepositorio)}.{nameof(BuscarUm)}", ex);
                return new ModeloTramiteResponse
                {
                    RM = Servico.MSG_EXCEPTION,
                    RC = ResponseCode.Excecao,
                    OK = false
                };
            }
        }
        private async Task<bool> ValidaGravacao(ModeloTramiteResponse resposta, string MtraNomeTramite, string? MtraDescricaoTramite, int MtraMtarId, int MtraUsuIdRevisor, int MtraUsuIdIndicacao, int id = 0)
        {
            try
            {
                if (MtraUsuIdRevisor > 0 && MtraUsuIdIndicacao == MtraUsuIdRevisor)
                {
                    resposta.RM = "Usuário revisor não pode ser o mesmo usuário Tramitador.";
                    resposta.errorCode = "MODELOS_TRAMITE_REVISOR_TRAMITADOR_IGUAIS";
                    resposta.RC = ResponseCode.EntidadeNaoProcessavel;
                    return false;
                }

                List<string> erro = new List<string>();
                List<string> erroCode = new List<string>();

                if (string.IsNullOrWhiteSpace(MtraNomeTramite))
                {
                    erro.Add("É obrigatório preencher o Nome Trâmite.");
                    erroCode.Add("PREECHIMENTO_OBRIGAORIO");
                }

                if (MtraNomeTramite.Length > Servico.TAM_NOMES || MtraDescricaoTramite?.Length > Servico.TAM_NOTASDESCRICAO)
                {
                    erro.Add("Excedido o tamanho máximo para o campo.");
                    erroCode.Add("MSG_EXCEDE_TAMANHO");
                }

                using var scope1 = _serviceProvider.CreateScope();
                using var scope2 = _serviceProvider.CreateScope();
                using var scope3 = _serviceProvider.CreateScope();
                using var scope4 = _serviceProvider.CreateScope();

                var db1 = scope1.ServiceProvider.GetRequiredService<SistemaTarefasDBContex>();
                var db2 = scope2.ServiceProvider.GetRequiredService<SistemaTarefasDBContex>();
                var db3 = scope3.ServiceProvider.GetRequiredService<SistemaTarefasDBContex>();
                var db4 = scope4.ServiceProvider.GetRequiredService<SistemaTarefasDBContex>();

                var tasks = new List<Task>();
                Task<bool>? existeModeloTarefa = null;
                Task<bool>? usuarioRevisor = null;
                Task<bool>? usuarioIndicacao = null;

                var modelosTramiteNome = db1.ModelosTramite.AnyAsync(x => x.MtraId != id && x.MtraMtarId == MtraMtarId && x.MtraNomeTramite.ToUpper().Trim() == MtraNomeTramite.ToUpper().Trim());
                
                if (id < 1)
                {
                    existeModeloTarefa = db2.ModelosTarefa.AnyAsync(x => x.MtarId == MtraMtarId);
                    tasks.Add(existeModeloTarefa);
                }

                if (MtraUsuIdRevisor > 0)
                {
                    usuarioRevisor = db3.Usuarios.AnyAsync(x => x.UsuId == MtraUsuIdRevisor);
                    tasks.Add(usuarioRevisor);
                }

                if (MtraUsuIdIndicacao > 0)
                {
                    usuarioIndicacao = db4.Usuarios.AnyAsync(x => x.UsuId == MtraUsuIdIndicacao);
                    tasks.Add(usuarioIndicacao);
                }

                await Task.WhenAll(tasks);

                if (existeModeloTarefa != null &&!await existeModeloTarefa)
                {
                    erro.Add("Modelo Tarefa não existe para associar ao Trâmite.");
                    erroCode.Add("MODELOS_TRAMITE_NAO_EXISTE_MODELO_TAREFA");
                }

                if (await modelosTramiteNome)
                {
                    erro.Add("Nome já usado para Trâmite no mesmo Modelo de Trâmite.");
                    erroCode.Add("MODELOS_TRAMITE_NOME_PARA_TRAMITE_EM_USO_NA_TAREFA");
                }

                if (usuarioIndicacao != null && !await usuarioIndicacao)
                {
                    erro.Add("Usuário Indicado para Tramitador do Trâmite no Modelo da Tarefa não encontrado.");
                    erroCode.Add("MODELOS_TRAMITE_USUARIO_TRAMITADOR_NAO_ENCONTRADO");
                }

                if (usuarioRevisor != null &&!await usuarioRevisor)
                {
                    erro.Add("Usuário Responsável pelo Trâmite no Modelo da Tarefa não encontrado.");
                    erroCode.Add("MODELOS_TRAMITE_USUARIO_RESPONSAVEL_NAO_ENCONTRADO");
                }

                if (erro.Any())
                {
                    resposta.RM = string.Join(Environment.NewLine, erro);
                    resposta.errorCode = string.Join(", ", erroCode);
                    resposta.RC = ResponseCode.EntidadeNaoProcessavel;
                    resposta.OK = false;
                    return false;
                }

                return true;
            }
            catch (Exception ex)
            {                
                Servico.GravaLog($"{nameof(ModelosTramiteRepositorio)}.{nameof(ValidaGravacao)} Id[{id}]", ex);
                resposta.RC = ResponseCode.Excecao;
                return false;
            }            
        }
        public async Task<ModeloTramiteResponse> Cadastrar(ModeloTramiteRequest modeloTramiteRequest)
        {
            try
            {
                #region Validações

                ModeloTramiteResponse resposta = new ModeloTramiteResponse();

                if (!await ValidaGravacao(resposta, modeloTramiteRequest.MtraNomeTramite, modeloTramiteRequest.MtraDescricaoTramite, modeloTramiteRequest.MtraMtarId, modeloTramiteRequest.MtraUsuIdRevisor, modeloTramiteRequest.MtraUsuIdIndicacao))
                {
                    return resposta;
                }

                modeloTramiteRequest.MtraDescricaoTramite ??= "";

                #endregion

                #region Cálculo Ordem
                int ordem = await _dbContext.ModelosTramite
                                  .Where(x => x.MtraMtarId == modeloTramiteRequest.MtraMtarId)
                                  .MaxAsync(x => (int?)x.MtraOrdem) ?? 0;

                ordem++;

                #endregion

                #region Atribuindo Valores

                ModelosTramite modeloTramite = new();

                _mapper.Map(modeloTramiteRequest, modeloTramite);

                if (modeloTramite.MtraUsuIdIndicacao == 0)
                    modeloTramite.MtraUsuIdIndicacao = null;

                if (modeloTramite.MtraUsuIdRevisor == 0)
                    modeloTramite.MtraUsuIdRevisor = null;

                modeloTramite.MtraOrdem = ordem;

                #endregion

                await _dbContext.ModelosTramite.AddAsync(modeloTramite);
                await _dbContext.SaveChangesAsync();

                resposta = _mapper.Map<ModeloTramiteResponse>(modeloTramite);              

                resposta.RM = "";
                resposta.RC = ResponseCode.CadastradoSucesso;
                resposta.OK = true;

                return resposta;
            }
            catch (Exception ex)
            {
                Servico.GravaLog($"{nameof(ModelosTramiteRepositorio)}.{nameof(Cadastrar)}", ex);
                return new ModeloTramiteResponse
                {
                    RM = Servico.MSG_EXCEPTION,
                    RC = ResponseCode.Excecao,
                    OK = false
                };
            }
        }
        public async Task<ModeloTramiteResponse> Atualizar(ModeloTramiteUpdRequest modeloTramiteRequest, int idModeloTramite)
        {
            try
            {
                #region Validações

                ModelosTramite? modeloTramite = await _dbContext.ModelosTramite.FirstOrDefaultAsync(x => x.MtraId == idModeloTramite);

                if (modeloTramite == null)
                {
                    return new ModeloTramiteResponse
                    {
                        RM = "Falha na atualização. Trâmite Modelo Tarefa não localizado!",
                        RC = ResponseCode.RegistroNaoEncontrado,
                        OK = false
                    };
                }

                ModeloTramiteResponse resposta = new ModeloTramiteResponse();

                if (!await ValidaGravacao(resposta, modeloTramiteRequest.MtraNomeTramite, modeloTramiteRequest.MtraDescricaoTramite, modeloTramite.MtraMtarId, modeloTramiteRequest.MtraUsuIdRevisor, modeloTramiteRequest.MtraUsuIdIndicacao, idModeloTramite))
                {
                    return resposta;
                }

                modeloTramiteRequest.MtraDescricaoTramite ??= "";

                #endregion

                _mapper.Map(modeloTramiteRequest, modeloTramite);

                if (modeloTramite.MtraUsuIdIndicacao < 1)
                    modeloTramite.MtraUsuIdIndicacao = null;

                if (modeloTramite.MtraUsuIdRevisor < 1)
                    modeloTramite.MtraUsuIdRevisor = null;

                await _dbContext.SaveChangesAsync();

                resposta = _mapper.Map<ModeloTramiteResponse>(modeloTramite);

                resposta.RM = "";
                resposta.RC = ResponseCode.CadastradoSucesso;
                resposta.OK = true;

                return resposta;
            }
            catch (Exception ex)
            {
                Servico.GravaLog($"{nameof(ModelosTramiteRepositorio)}.{nameof(Atualizar)}", ex);
                return new ModeloTramiteResponse
                {
                    RM = Servico.MSG_EXCEPTION,
                    RC = ResponseCode.Excecao,
                    OK = false
                };
            }
        }

        public async Task<ResponseModel> Apagar(int id)
        {
            ResponseModel resposta = new();

            try
            {
                ModelosTramite? entidadeModeloTramite = await _dbContext.ModelosTramite.FirstOrDefaultAsync(x => x.MtraId == id);

                if (entidadeModeloTramite == null)
                {
                    return new ResponseModel
                    {
                        RM = "Registro para exclusão não foi encontrado.",
                        RC = ResponseCode.RegistroNaoEncontrado,
                        OK = false
                    };
                }

                if (!await IntegridadeReferencial(id, resposta))
                {
                    return resposta;
                }

                _dbContext.ModelosTramite.Remove(entidadeModeloTramite);
                await _dbContext.SaveChangesAsync();

                resposta.RM = "Registro apagado.";
                resposta.RC = ResponseCode.OK;
                resposta.OK = true;
            }
            catch (Exception ex)
            {
                Servico.GravaLog($"{nameof(ModelosTramiteRepositorio)}.{nameof(Apagar)} Id[{id}]", ex);
                resposta.RM = Servico.MSG_EXCEPTION;
                resposta.RC = ResponseCode.Excecao;
                resposta.OK = false;
            }

            return resposta;
        }
    }
}
