using AutoMapper;
using Microsoft.AspNetCore.Http.HttpResults;
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
    public class TramitesRepositorio(
        IServiceProvider _serviceProvider,
        SistemaTarefasDBContex _dbContext,
        IMapper _mapper
    ) : ITramitesRepositorio
    {
        private ITarefasRepositorio? _tarefasRepositorio;
        private ITarefasRepositorio spTarefasRepositorio => _tarefasRepositorio ??= _serviceProvider.GetRequiredService<ITarefasRepositorio>();
        public async Task<List<TramiteResponse>> BuscarVarios(int idTarefa = 0, int statusTramite = 0, string? statusTarefa = null, bool ordenarPelaDataComecoTarefa = false)
        {
            try
            {
                #region monta SQL

                string sql = @"SELECT 
                                tra.TRA_ID as TraId, 
                                tra.TRA_Status as TraStatus, 
                                tra.TRA_Ordem as TraOrdem, 
                                tra.TRA_DataInicio as TraDataInicio, 
                                tra.TRA_DataPrevisaoTermino as TraDataPrevisaoTermino, 
                                tra.TRA_DataExecucao as TraDataExecucao,
                                tra.TRA_DataRevisao as TraDataRevisao,
                                tra.TRA_TramiteRepetido as TraRepetido, 

                                tar.TAR_USU_ID_Responsavel as tarUsuIdResponsavelTarefa, 
                                urr.USU_Nome AS UsuNomeResponsavel, 

                                tra.TRA_USU_ID_Revisor as TraUsuIdRevisor, 
                                ISNULL(ur.USU_Nome, '') AS UsuNomeRevisor, 
                                tra.TRA_NotaRevisor as TraNotaRevisor, 

                                tra.TRA_USU_ID_Tramitador as TraUsuIdTramitador, 
                                ISNULL(ut.USU_Nome, '') AS UsuNomeTramitador,
                                tra.TRA_NotaTramitador as TraNotaTramitador, 
                               
                                tra.TRA_TAR_ID AS TraTarId,
                                tar.TAR_Nome AS TarNomeTarefa,
                                tar.TAR_Descricao AS TarDescricao,
                                tar.TAR_DataComeco AS TarDataComeco,
                                tar.TAR_DataFinalPrevista AS tarDataFinalPrevista,
                                tar.TAR_MTAR_ID AS TarMtarId,
                                tar.TAR_Status AS TarStatus,

                                tar.TAR_FLA_ID AS TarFlaId, 
                                ISNULL(f.FLA_Rotulo, '') AS FlaRotulo, 
                                ISNULL(f.FLA_Cor, '') AS FlaCor, 

                                tra.TRA_MTRA_ID as TraMtraId, 
                                mtra.MTRA_NomeTramite AS MtraNome,
                                mtra.MTRA_DescricaoTramite AS MtraDescricao, 
                               
                                mtar.MTAR_Nome AS MtarNome,
                                mtar.MTAR_Descricao AS MtarDescricao

                                FROM Tramites as tra
                                 INNER JOIN Tarefas tar ON tra.TRA_TAR_ID = tar.TAR_ID
                                 LEFT JOIN Flags f ON tar.TAR_FLA_ID = f.FLA_ID 
                                 LEFT JOIN Usuarios ur ON tra.TRA_USU_ID_Revisor = ur.USU_ID 
                                 LEFT JOIN Usuarios ut ON tra.TRA_USU_ID_Tramitador = ut.USU_ID 
                                 LEFT JOIN Usuarios urr ON tar.TAR_USU_ID_Responsavel = urr.USU_ID
                                 INNER JOIN ModelosTramite mtra ON tra.TRA_MTRA_ID = mtra.MTRA_ID
                                 INNER JOIN ModelosTarefa mtar ON tar.TAR_MTAR_ID = mtar.MTAR_ID ";

                var filtros = new List<string>();
                var parametros = new List<SqlParameter>();

                if (idTarefa > 0)
                {
                    filtros.Add("tra.TRA_TAR_ID  = @idTarefa");
                    parametros.Add(new SqlParameter("@idTarefa", idTarefa));
                }

                if (statusTramite > 0)
                {
                    filtros.Add("UPPER(tra.TRA_Status) = UPPER(@statusTramite)");
                    parametros.Add(new SqlParameter("@statusTramite", statusTramite));
                }

                if (!string.IsNullOrWhiteSpace(statusTarefa))
                {
                    filtros.Add("UPPER(tar.TAR_Status) = UPPER(@statusTarefa)");
                    parametros.Add(new SqlParameter("@statusTarefa", statusTarefa));
                }

                if (filtros.Count > 0)
                {
                    sql += " WHERE " + string.Join(" AND ", filtros);
                }

                sql += " ORDER BY ";

                if (ordenarPelaDataComecoTarefa)
                    sql += "tar.TAR_DataComeco, ";

                sql += "tar.TAR_Nome, tra.TRA_Ordem";

                #endregion

                #region Executa SQL

                List<TramitesSQL> resultado = await _dbContext
                        .TramitesSQL
                        .FromSqlRaw(sql, parametros.ToArray())
                        .ToListAsync();

                #endregion

                if (!resultado.Any())
                {
                    return new List<TramiteResponse>
                    {
                        new TramiteResponse
                        {
                            RM = "Nenhum registro encontrado.",
                            RC = ResponseCode.RegistroNaoEncontrado,
                            OK = false
                        }
                    };
                }

                List<TramiteResponse> resposta = _mapper.Map<List<TramiteResponse>>(resultado);

                return resposta;
            }
            catch (Exception ex)
            {
                Servico.GravaLog($"{nameof(TramitesRepositorio)}.{nameof(BuscarVarios)}", ex);
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
        public async Task<TramiteResponse> BuscarUm(int idTramite)
        {
            try
            {
                #region monta SQL

                string sql = @"SELECT 
                                tra.TRA_ID as TraId, 
                                tra.TRA_Status as TraStatus, 
                                tra.TRA_Ordem as TraOrdem, 
                                tra.TRA_DataInicio as TraDataInicio, 
                                tra.TRA_DataPrevisaoTermino as TraDataPrevisaoTermino, 
                                tra.TRA_DataExecucao as TraDataExecucao,
                                tra.TRA_DataRevisao as TraDataRevisao,
                                tra.TRA_TramiteRepetido as TraRepetido, 

                                tar.TAR_USU_ID_Responsavel as tarUsuIdResponsavelTarefa, 
                                urr.USU_Nome AS UsuNomeResponsavel, 

                                tra.TRA_USU_ID_Revisor as TraUsuIdRevisor, 
                                ISNULL(ur.USU_Nome, '') AS UsuNomeRevisor, 
                                tra.TRA_NotaRevisor as TraNotaRevisor, 

                                tra.TRA_USU_ID_Tramitador as TraUsuIdTramitador, 
                                ISNULL(ut.USU_Nome, '') AS UsuNomeTramitador,
                                tra.TRA_NotaTramitador as TraNotaTramitador, 
                               
                                tra.TRA_TAR_ID AS TraTarId,
                                tar.TAR_Nome AS TarNomeTarefa,
                                tar.TAR_Descricao AS TarDescricao,
                                tar.TAR_DataComeco AS TarDataComeco,
                                tar.TAR_DataFinalPrevista AS tarDataFinalPrevista,
                                tar.TAR_MTAR_ID AS TarMtarId,
                                tar.TAR_Status AS TarStatus,

                                tar.TAR_FLA_ID AS TarFlaId, 
                                ISNULL(f.FLA_Rotulo, '') AS FlaRotulo, 
                                ISNULL(f.FLA_Cor, '') AS FlaCor, 

                                tra.TRA_MTRA_ID as TraMtraId, 
                                mtra.MTRA_NomeTramite AS MtraNome,
                                mtra.MTRA_DescricaoTramite AS MtraDescricao, 
                               
                                mtar.MTAR_Nome AS MtarNome,
                                mtar.MTAR_Descricao AS MtarDescricao

                                FROM Tramites as tra
                                 INNER JOIN Tarefas tar ON tra.TRA_TAR_ID = tar.TAR_ID
                                 LEFT JOIN Flags f ON tar.TAR_FLA_ID = f.FLA_ID 
                                 LEFT JOIN Usuarios ur ON tra.TRA_USU_ID_Revisor = ur.USU_ID 
                                 LEFT JOIN Usuarios ut ON tra.TRA_USU_ID_Tramitador = ut.USU_ID 
                                 LEFT JOIN Usuarios urr ON tar.TAR_USU_ID_Responsavel = urr.USU_ID
                                 INNER JOIN ModelosTramite mtra ON tra.TRA_MTRA_ID = mtra.MTRA_ID
                                 INNER JOIN ModelosTarefa mtar ON tar.TAR_MTAR_ID = mtar.MTAR_ID 

                                 WHERE tra.TRA_ID = @idTramite";

                #endregion

                #region Executa SQL

                SqlParameter parametro = new SqlParameter("@idTramite", idTramite);

                TramitesSQL? resultado = await _dbContext
                        .TramitesSQL
                        .FromSqlRaw(sql, parametro).FirstOrDefaultAsync();

                if (resultado == null)
                {
                    return new TramiteResponse
                    {
                        RM = "Nenhum Trâmite foi encontrado.",
                        RC = ResponseCode.RegistroNaoEncontrado,
                        OK = false
                    };
                }

                #endregion

                TramiteResponse resposta = _mapper.Map<TramiteResponse>(resultado);

                return resposta;
            }
            catch (Exception ex)
            {
                Servico.GravaLog($"{nameof(TramitesRepositorio)}.{nameof(BuscarUm)}", ex);
                return new TramiteResponse
                {
                    RM = Servico.MSG_EXCEPTION,
                    RC = ResponseCode.Excecao,
                    OK = false
                };
            }
        }
        public async Task<List<TramiteResponse>> BuscarTramitesPorTipoDeUsuario(int idUsuario, TipoUsuarioTramite tipoUsuarioTramite, bool ordenarDataComeco, string? statusTarefa = "A")
        {
            try
            {
                #region monta SQL

                string sql = @"SELECT 
                                tra.TRA_ID as TraId, 
                                tra.TRA_Status as TraStatus, 
                                tra.TRA_Ordem as TraOrdem, 
                                tra.TRA_DataInicio as TraDataInicio, 
                                tra.TRA_DataPrevisaoTermino as TraDataPrevisaoTermino, 
                                tra.TRA_DataExecucao as TraDataExecucao,
                                tra.TRA_DataRevisao as TraDataRevisao,
                                tra.TRA_TramiteRepetido as TraRepetido, 

                                tar.TAR_USU_ID_Responsavel as tarUsuIdResponsavelTarefa, 
                                urr.USU_Nome AS UsuNomeResponsavel, 

                                tra.TRA_USU_ID_Revisor as TraUsuIdRevisor, 
                                ISNULL(ur.USU_Nome, '') AS UsuNomeRevisor, 
                                tra.TRA_NotaRevisor as TraNotaRevisor, 

                                tra.TRA_USU_ID_Tramitador as TraUsuIdTramitador, 
                                ISNULL(ut.USU_Nome, '') AS UsuNomeTramitador,
                                tra.TRA_NotaTramitador as TraNotaTramitador, 
                               
                                tra.TRA_TAR_ID AS TraTarId,
                                tar.TAR_Nome AS TarNomeTarefa,
                                tar.TAR_Descricao AS TarDescricao,
                                tar.TAR_DataComeco AS TarDataComeco,
                                tar.TAR_DataFinalPrevista AS tarDataFinalPrevista,
                                tar.TAR_MTAR_ID AS TarMtarId,
                                tar.TAR_Status AS TarStatus,

                                tar.TAR_FLA_ID AS TarFlaId, 
                                ISNULL(f.FLA_Rotulo, '') AS FlaRotulo, 
                                ISNULL(f.FLA_Cor, '') AS FlaCor, 

                                tra.TRA_MTRA_ID as TraMtraId, 
                                mtra.MTRA_NomeTramite AS MtraNome,
                                mtra.MTRA_DescricaoTramite AS MtraDescricao, 
                               
                                mtar.MTAR_Nome AS MtarNome,
                                mtar.MTAR_Descricao AS MtarDescricao

                                FROM Tramites as tra
                                 INNER JOIN Tarefas tar ON tra.TRA_TAR_ID = tar.TAR_ID
                                 LEFT JOIN Flags f ON tar.TAR_FLA_ID = f.FLA_ID 
                                 LEFT JOIN Usuarios ur ON tra.TRA_USU_ID_Revisor = ur.USU_ID 
                                 LEFT JOIN Usuarios ut ON tra.TRA_USU_ID_Tramitador = ut.USU_ID 
                                 LEFT JOIN Usuarios urr ON tar.TAR_USU_ID_Responsavel = urr.USU_ID
                                 INNER JOIN ModelosTramite mtra ON tra.TRA_MTRA_ID = mtra.MTRA_ID
                                 INNER JOIN ModelosTarefa mtar ON tar.TAR_MTAR_ID = mtar.MTAR_ID ";


                var filtros = new List<string>();
                var parametros = new List<SqlParameter>();

                if (tipoUsuarioTramite == TipoUsuarioTramite.Responsavel || tipoUsuarioTramite == TipoUsuarioTramite.Todos)
                {
                    filtros.Add("(tar.TAR_USU_ID_Responsavel = @idUsuario AND tra.TRA_Status BETWEEN 1 AND 3)");
                } 
                
                if (tipoUsuarioTramite == TipoUsuarioTramite.Revisor || tipoUsuarioTramite == TipoUsuarioTramite.Todos) {
                    filtros.Add("(tra.TRA_USU_ID_Revisor = @idUsuario AND tra.TRA_Status = 3) ");
                }
                
                if (tipoUsuarioTramite == TipoUsuarioTramite.Tramitador || tipoUsuarioTramite == TipoUsuarioTramite.Todos) {
                    filtros.Add("(tra.TRA_USU_ID_Tramitador = @idUsuario AND tra.TRA_Status IN (1, 2))");
                }
                
                if (tipoUsuarioTramite == TipoUsuarioTramite.Todos)
                {
                    filtros.Add("(tra.TRA_USU_ID_Tramitador IS NULL AND tra.TRA_Status = 1)");
                }

                parametros.Add(new SqlParameter("@idUsuario", idUsuario));

                if (filtros.Count > 0)
                {
                    sql += " WHERE (" + string.Join(" OR ", filtros) + ")";
                }

                if (!string.IsNullOrWhiteSpace(statusTarefa))
                {
                    sql += " AND tar.TAR_Status = @statusTarefa";
                    parametros.Add(new SqlParameter("@statusTarefa", statusTarefa.ToUpper()));
                }

                sql += " ORDER BY ";

                if (ordenarDataComeco)
                    sql += "tar.TAR_DataComeco, ";

                sql += " tar.TAR_Nome, tra.TRA_Ordem ";

                #endregion

                #region Executa SQL

                List<TramitesSQL> resultado = await _dbContext
                        .TramitesSQL
                        .FromSqlRaw(sql, parametros.ToArray())
                        .ToListAsync();

                #endregion

                if (!resultado.Any())
                {
                    return new List<TramiteResponse>
                    {
                        new TramiteResponse
                        {
                            RM = "Nenhum registro encontrado.",
                            RC = ResponseCode.RegistroNaoEncontrado,
                            OK = false
                        }
                    };
                }

                List<TramiteResponse> resposta = _mapper.Map<List<TramiteResponse>>(resultado);

                return resposta;
            }
            catch (Exception ex)
            {
                Servico.GravaLog($"{nameof(TramitesRepositorio)}.{nameof(BuscarTramitesPorTipoDeUsuario)} Id[{idUsuario}]", ex);
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
        public async Task<TramiteResponse> IncluirTramite(int idTarefa)
        {           
            try
            {
                #region Tarefa Existe?

                Tarefas? tarefa = await _dbContext.Tarefas.FirstOrDefaultAsync(x => x.TarId == idTarefa);

                if (tarefa == null)
                {
                    return new TramiteResponse
                    {
                        RM = "Tarefa não encontrada.",
                        RC = ResponseCode.RegistroNaoEncontrado,
                        OK = false
                    };
                }
                #endregion

                #region Último Trâmite

                Tramites? ultimoTramite = await _dbContext.Tramites.Where(x => x.TraTarId == idTarefa).OrderByDescending(x => x.TraOrdem).FirstOrDefaultAsync();

                if (ultimoTramite != null && ultimoTramite.TraStatus != StatusTramite.TerminadoOK && ultimoTramite.TraStatus != StatusTramite.TerminadoFalha)
                {
                    return new TramiteResponse
                    {
                        RM = "Não pode incluir mais um Trâmite. O Trâmite atual está com Status = "+ultimoTramite.TraStatus.GetDescriptionTramite(),
                        errorCode = "TRAMITES_NAO_PODE_INCLUIR_TRAMITE",
                        RC = ResponseCode.EntidadeNaoProcessavel,
                        OK = false
                    };
                }

                int idMtra = (ultimoTramite != null) ? ultimoTramite.TraMtraId : 0;
                #endregion

                #region Atributos Modelo Trâmite

                ModelosTramite? modeloTramite = await _dbContext.ModelosTramite.Where(x => x.MtraMtarId == tarefa.TarMtarId && x.MtraId > idMtra).OrderBy(x => x.MtraOrdem).FirstOrDefaultAsync();

                if (modeloTramite == null)
                {
                    ResponseModel respostaTarefa = await spTarefasRepositorio.FecharTarefa(idTarefa);
                    return new TramiteResponse
                    {
                        RM = respostaTarefa.RM,
                        RC = respostaTarefa.RC,
                        OK = respostaTarefa.OK
                    };
                }

                #endregion

                #region Gravação

                Tramites tramite = new Tramites();

                tramite.TraTarId = tarefa.TarId;
                tramite.TraMtraId = modeloTramite.MtraId;
                tramite.TraStatus = StatusTramite.AFazer;
                tramite.TraUsuIdTramitador = modeloTramite.MtraUsuIdIndicacao;
                tramite.TraUsuIdRevisor = modeloTramite.MtraUsuIdRevisor;
                tramite.TraDataInicio = DateTime.Now.Date;
                tramite.TraDataPrevisaoTermino = tramite.TraDataInicio.AddDays(modeloTramite.MtraDuracaoPrevistaDias);
                tramite.TraDataExecucao = null;
                tramite.TraDataRevisao = null;
                tramite.TraOrdem = (ultimoTramite != null) ? ultimoTramite.TraOrdem +1 : 1;
                tramite.TraRepetido = false;
                tramite.TraNotaTramitador = "";
                tramite.TraNotaRevisor = "";

                await _dbContext.Tramites.AddAsync(tramite);
                await _dbContext.SaveChangesAsync();

                TramiteResponse resposta = _mapper.Map<TramiteResponse>(tramite);

                resposta.TarMtarId = tarefa.TarMtarId;
                resposta.MtraNome = modeloTramite.MtraNomeTramite;
                resposta.MtraDescricao = modeloTramite.MtraDescricaoTramite;
                resposta.TarFlaId = tarefa.TarFlaId;
                resposta.TarNomeTarefa = tarefa.TarNomeTarefa;
                resposta.TarDescricao = tarefa.TarDescricao;
                resposta.TarStatus = tarefa.TarStatus.ToString();

                resposta.RM = "Inclusão de Trâmite realizada com sucesso.";
                resposta.RC = ResponseCode.CadastradoSucesso;
                resposta.OK = true;
                return resposta;

                #endregion
            }
            catch (Exception ex)
            {
                Servico.GravaLog($"{nameof(TramitesRepositorio)}.{nameof(IncluirTramite)}", ex);
                return new TramiteResponse
                {
                    RM = "Falha ao incluir Trâmite.",
                    RC = ResponseCode.Excecao,
                    OK = false
                };
            }
        }
        public async Task<ResponseModel> AssumirTramite(int idTramite, int idUsuario)
        {
            try
            {
                Tramites? tramite = await _dbContext.Tramites.FirstOrDefaultAsync(x => x.TraId == idTramite);

                if (tramite == null)
                {
                    return new ResponseModel
                    {
                        RM = "Trâmite não foi encontrado.",
                        RC = ResponseCode.RegistroNaoEncontrado,
                        OK = false
                    };
                }

                if (tramite.TraUsuIdTramitador > 0)
                {
                    return new ResponseModel
                    {
                        RM = "Trâmite já está associado a um usuário.",
                        errorCode = "TRAMITES_TRAMITE_JA_ASSOCIADO_UM_USUARIO",
                        RC = ResponseCode.EntidadeNaoProcessavel,
                        OK = false
                    };
                }

                if (idUsuario == tramite.TraUsuIdRevisor)
                {
                    return new ResponseModel
                    {
                        RM = "O usuário não pode ser Tramidador e Revisor no mesmo Trâmite.",
                        errorCode = "TRAMITES_REVISOR_TRAMITADOR_IGUAIS",
                        RC = ResponseCode.EntidadeNaoProcessavel,
                        OK = false
                    };
                }

                tramite.TraUsuIdTramitador = idUsuario;
                await _dbContext.SaveChangesAsync();

                return new ResponseModel
                {
                    RM = "Trâmite atribuído ao usuário.",
                    RC = ResponseCode.OK,
                    OK = true
                };
            }
            catch (Exception ex)
            {
                Servico.GravaLog($"{nameof(TramitesRepositorio)}.{nameof(AssumirTramite)}", ex);
                return new ResponseModel
                {
                    RM = Servico.MSG_EXCEPTION,
                    RC = ResponseCode.Excecao,
                    OK = false
                };
            }
        }
        public async Task<ResponseModel> ComecarExecucaoTramite(int idTramite, int idUsuario)
        {
            try
            {
                Tramites? tramite = await _dbContext.Tramites.FirstOrDefaultAsync(x => x.TraId == idTramite);

                if (tramite == null)
                {
                    return new ResponseModel
                    {
                        RM = "Trâmite não foi encontrado.",
                        RC = ResponseCode.RegistroNaoEncontrado,
                        OK = false
                    };
                }

                if (tramite.TraUsuIdTramitador != idUsuario)
                {
                    return new ResponseModel
                    {
                        RM = "Usuário não é o Tramitador deste Trâmite.",
                        errorCode = "TRAMITES_USUARIO_NAO_TRAMITADOR",
                        RC = ResponseCode.EntidadeNaoProcessavel,
                        OK = false
                    };
                }

                if (tramite.TraStatus != StatusTramite.AFazer)
                {
                    return new ResponseModel
                    {
                        RM = $"O Trâmite já está com Status = {EnumTramiteExtensions.GetDescriptionTramite((tramite.TraStatus == StatusTramite.EmAndamento) ? 
                                                                                                            StatusTramite.EmAndamento : StatusTramite.AFazer) }.",
                        errorCode = "TRAMITES_TRAMITE_JA_ESTA_COM_O_STATUS",
                        RC = ResponseCode.EntidadeNaoProcessavel,
                        OK = false
                    };
                }

                tramite.TraStatus = StatusTramite.EmAndamento;
                await _dbContext.SaveChangesAsync();

                return new ResponseModel
                {
                    RM = $"Atribuído ao Trâmite o Status = {EnumTramiteExtensions.GetDescriptionTramite(StatusTramite.EmAndamento)}.",
                    RC = ResponseCode.OK,
                    OK = true
                };
            }
            catch (Exception ex)
            {
                Servico.GravaLog($"{nameof(TramitesRepositorio)}.{nameof(ComecarExecucaoTramite)}", ex);
                return new ResponseModel
                {
                    RM = Servico.MSG_EXCEPTION,
                    RC = ResponseCode.Excecao,
                    OK = false
                };
            }
        }
        public async Task<ResponseModel> FinalizarExecucaoTramite(int idTramite, string notaTramitador, int idUsuario)
        {
            try
            {
                Tramites? tramite = await _dbContext.Tramites.FirstOrDefaultAsync(x => x.TraId == idTramite);

                if (tramite == null)
                {
                    return new ResponseModel
                    {
                        RM = "Trâmite não foi encontrado.",
                        RC = ResponseCode.RegistroNaoEncontrado,
                        OK = false
                    };
                }

                if (tramite.TraUsuIdTramitador != idUsuario)
                {
                    return new ResponseModel
                    {
                        RM = "Usuário não é o Tramitador deste Trâmite.",
                        RC = ResponseCode.EntidadeNaoProcessavel,
                        errorCode = "TRAMITES_USUARIO_NAO_TRAMITADOR",
                        OK = false
                    };
                }

                if (tramite.TraStatus != StatusTramite.EmAndamento)
                {
                    if (tramite.TraStatus == StatusTramite.TerminadoOK)
                        return new ResponseModel
                        {
                            RM = $"O Trâmite já está finalizado Status = {EnumTramiteExtensions.GetDescriptionTramite(tramite.TraStatus)}",
                            RC = ResponseCode.EntidadeNaoProcessavel,
                            errorCode = "TRAMITES_TRAMITE_JA_FINALIZADO",
                            OK = false
                        };

                    return new ResponseModel
                    {
                        RM = $"O Trâmite deve estar com Status = {EnumTramiteExtensions.GetDescriptionTramite(StatusTramite.EmAndamento)}",
                        errorCode = "TRAMITES_TRAMITE_DEVE_ESTAR_COM_STATUS_EM_ANDAMENTO",
                        RC = ResponseCode.EntidadeNaoProcessavel,
                        OK = false
                    };
                }

                using var transaction = await _dbContext.Database.BeginTransactionAsync();
                tramite.TraDataExecucao = DateTime.Now.Date;
                tramite.TraNotaTramitador = notaTramitador.Trim();

                bool avanca = false;

                if (tramite.TraUsuIdRevisor > 0)
                {
                    tramite.TraStatus = StatusTramite.AguardandoRevisao;
                }
                else
                {
                    tramite.TraStatus = StatusTramite.TerminadoOK;
                    avanca = true;
                    tramite.TraDataRevisao = tramite.TraDataExecucao;
                }

                await _dbContext.SaveChangesAsync();

                if (avanca)
                {
                    TramiteResponse resposta = await IncluirTramite(tramite.TraTarId);

                    if (!resposta.OK)
                    {
                        Servico.GravaLog($"Trâmite não finalizado. Houve erro ao incluir o próximo Trâmite | Tarefa Id[{tramite.TraTarId}] Trâmite Id[{tramite.TraId}] | "+resposta.RM);
                        await transaction.RollbackAsync();

                        return new ResponseModel
                        {
                            RM = "Trâmite não finalizado. Houve erro ao incluir o próximo Trâmite.",
                            errorCode = "TRAMITES_PROXIMO_NAO_GRAVADO",
                            RC = resposta.RC,
                            OK = false
                        };
                    }
                }

                await transaction.CommitAsync();

                return new ResponseModel
                {
                    RM = $"Finalizada a Execução do Trâmite. Status = {EnumTramiteExtensions.GetDescriptionTramite(tramite.TraStatus)}.",
                    RC = ResponseCode.OK,
                    OK = true
                };
            }
            catch (Exception ex)
            {
                Servico.GravaLog($"{nameof(TramitesRepositorio)}.{nameof(FinalizarExecucaoTramite)}", ex);
                return new ResponseModel
                {
                    RM = Servico.MSG_EXCEPTION,
                    RC = ResponseCode.Excecao,
                    OK = false
                };
            }
        }
        public async Task<ResponseModel> RevisarTramite(bool aprovado, int idTramite, string notaRevisor, int idUsuario)
        {
            try
            {
                Tramites? tramite = await _dbContext.Tramites.FirstOrDefaultAsync(x => x.TraId == idTramite);

                if (tramite == null)
                {
                    return new ResponseModel
                    {
                        RM = "Trâmite não foi encontrado.",
                        RC = ResponseCode.RegistroNaoEncontrado,
                        OK = false
                    };
                }

                if (tramite.TraStatus == StatusTramite.TerminadoOK || tramite.TraStatus == StatusTramite.TerminadoFalha)
                {
                    return new ResponseModel
                    {
                        RM = "Este trâmite já foi terminado.",
                        RC = ResponseCode.EntidadeNaoProcessavel,
                        OK = false
                    };
                }

                if (tramite.TraUsuIdRevisor != idUsuario)
                {
                    return new ResponseModel
                    {
                        RM = "Usuário não é o Revisor deste Trâmite.",
                        errorCode = "TRAMITES_USUARIO_NAO_REVISOR",
                        RC = ResponseCode.EntidadeNaoProcessavel,
                        OK = false
                    };
                }

                if (tramite.TraStatus != StatusTramite.AguardandoRevisao)
                {
                    if (tramite.TraStatus == StatusTramite.TerminadoOK)
                        return new ResponseModel
                        {
                            RM = $"O Trâmite já está finalizado Status = {EnumTramiteExtensions.GetDescriptionTramite(StatusTramite.TerminadoOK)}",
                            errorCode = "TRAMITES_TRAMITE_JA_FINALIZADO",
                            RC = ResponseCode.EntidadeNaoProcessavel,
                            OK = false
                        };

                    return new ResponseModel
                    {
                        RM = $"O Trâmite deve estar com Status = {EnumTramiteExtensions.GetDescriptionTramite(StatusTramite.AguardandoRevisao)}",
                        errorCode = "TRAMITES_TRAMITE_DEVE_ESTAR_COM_STATUS_AGUARDANDO_REVISAO",
                        RC = ResponseCode.EntidadeNaoProcessavel,
                        OK = false
                    };
                }

                if (aprovado)
                {
                    tramite.TraStatus = StatusTramite.TerminadoOK;
                }
                else
                {
                    tramite.TraStatus = StatusTramite.TerminadoFalha;
                }

                using var transaction = await _dbContext.Database.BeginTransactionAsync();
                tramite.TraNotaRevisor = notaRevisor.Trim();
                tramite.TraDataRevisao = DateTime.Now.Date;
                await _dbContext.SaveChangesAsync();

                if (aprovado)
                {
                    TramiteResponse resposta = await IncluirTramite(tramite.TraTarId);

                    if (!resposta.OK)
                    {
                        Servico.GravaLog($"Trâmite atual finalizado. Porém houve erro ao incluir o próximo Trâmite. | Tarefa Id[{tramite.TraTarId}] Trâmite Id[{tramite.TraId}] | "+resposta.RM);
                        await transaction.RollbackAsync();

                        return new ResponseModel
                        {
                            RM = "Trâmite não finalizado. Houve erro ao incluir o próximo Trâmite.",
                            errorCode = "TRAMITES_PROXIMO_NAO_GRAVADO",
                            RC = resposta.RC,
                            OK = false
                        };
                    }
                }
                else
                {
                    ResponseModel resposta = await RepetirTramite(tramite);

                    if (!resposta.OK)
                    {
                        Servico.GravaLog($"Trâmite atual finalizado. Porém houve erro ao repetir o próximo Trâmite. | Tarefa Id[{tramite.TraTarId}] Trâmite Id[{tramite.TraId}] | "+resposta.RM);
                        await transaction.RollbackAsync();

                        return new ResponseModel
                        {
                            RM = "Trâmite não finalizado. Houve erro ao repetir o Trâmite.",
                            errorCode = "TRAMITES_REPETIDO_NAO_GRAVADO",
                            RC = resposta.RC,
                            OK = false
                        };
                    }
                }

                await transaction.CommitAsync();

                return new ResponseModel
                {
                    RM = $"Finalizada a Revisão do Trâmite. Status = {EnumTramiteExtensions.GetDescriptionTramite(tramite.TraStatus)}.",
                    RC = ResponseCode.CadastradoSucesso,
                    OK = true
                };
            }
            catch (Exception ex)
            {
                Servico.GravaLog($"{nameof(TramitesRepositorio)}.{nameof(RevisarTramite)}", ex);
                return new ResponseModel
                {
                    RM = Servico.MSG_EXCEPTION,
                    RC = ResponseCode.Excecao,
                    OK = false
                };
            }
        }
        private async Task<ResponseModel> RepetirTramite(Tramites tramiteAtual)
        {
            try
            {
                #region Modelo Trâmite

                ModelosTramite? modeloTramite = await _dbContext.ModelosTramite.Where(x => x.MtraId == tramiteAtual.TraMtraId).FirstOrDefaultAsync();

                if (modeloTramite == null)
                {
                    return new ResponseModel
                    {
                        RM = "Modelo do Trâmite não foi encontrado.",
                        errorCode = "TRAMITES_MODELOSTRAMITE_NAO_ENCONTRADO",
                        RC = ResponseCode.RegistroNaoEncontrado,
                        OK = false
                    };
                }

                #endregion

                #region Gravação

                Tramites tramite = new Tramites();

                tramite.TraTarId = tramiteAtual.TraTarId;
                tramite.TraMtraId = modeloTramite.MtraId;
                tramite.TraStatus = StatusTramite.AFazer;
                tramite.TraUsuIdTramitador = modeloTramite.MtraUsuIdIndicacao;
                tramite.TraUsuIdRevisor = modeloTramite.MtraUsuIdRevisor;
                tramite.TraDataInicio = DateTime.Now.Date;
                tramite.TraDataPrevisaoTermino = tramite.TraDataInicio.AddDays(modeloTramite.MtraDuracaoPrevistaDias);
                tramite.TraDataExecucao = null;
                tramite.TraDataRevisao = null;
                tramite.TraOrdem = tramiteAtual.TraOrdem +1;
                tramite.TraRepetido = true;
                tramite.TraNotaTramitador = "";
                tramite.TraNotaRevisor = "";

                await _dbContext.Tramites.AddAsync(tramite);
                await _dbContext.SaveChangesAsync();

                return new ResponseModel
                {
                    RM = "Inclusão de Trâmite Repedito realizada com sucesso.",
                    RC = ResponseCode.CadastradoSucesso,
                    OK = true
                };

                #endregion
            }
            catch (Exception ex)
            {
                Servico.GravaLog($"{nameof(TramitesRepositorio)}.{nameof(RepetirTramite)}", ex);
                return new ResponseModel
                {
                    RM = Servico.MSG_EXCEPTION,
                    RC = ResponseCode.Excecao,
                    OK = false
                };
            }
        }
        private async Task<bool> IntegridadeReferencialRetrocederTramite(ResponseModel resposta, int idTramite, int TraTarId)
        {
            try
            {
                Tramites? ultimoTramite = await _dbContext.Tramites.Where(x => x.TraTarId == TraTarId).OrderByDescending(x => x.TraOrdem).FirstOrDefaultAsync();

                if (ultimoTramite == null)
                {
                    resposta.RC = ResponseCode.Excecao;
                    return false;
                }

                if (ultimoTramite.TraId != idTramite)
                {
                    resposta.RM = "Retroceder não realizado. Só é possível Retroceder a partir do último trâmite de uma Tarefa.";
                    resposta.errorCode = "TRAMITES_SO_PODE_RETROCEDER_ULTIMO";
                    resposta.RC = ResponseCode.EntidadeNaoProcessavel;
                    return false;
                }

                return true;
            }
            catch (Exception ex)
            {
                Servico.GravaLog($"{nameof(TramitesRepositorio)}.{nameof(IntegridadeReferencialRetrocederTramite)} Id[{idTramite}]", ex);
                resposta.RC = ResponseCode.Excecao;
                return false;
            }
        }
        public async Task<ResponseModel> Retroceder(int idTramite)
        {
            try
            {
                Tramites? tramite = await _dbContext.Tramites.FirstOrDefaultAsync(x => x.TraId == idTramite);

                if (tramite == null)
                {
                    return new ResponseModel
                    {
                        RM = "Trâmite não foi encontrado.",
                        RC = ResponseCode.RegistroNaoEncontrado,
                        OK = false
                    };
                }

                ResponseModel resposta = new ResponseModel();

                if (!await IntegridadeReferencialRetrocederTramite(resposta, idTramite, tramite.TraTarId))
                {
                    return resposta;
                }

                if (tramite.TraStatus == StatusTramite.AFazer && string.IsNullOrWhiteSpace(tramite.TraNotaTramitador))
                {
                    _dbContext.Tramites.Remove(tramite);
                }
                else if (tramite.TraStatus != StatusTramite.AFazer)
                {
                    tramite.TraStatus = StatusTramite.AFazer;
                    tramite.TraUsuIdTramitador = null;
                    tramite.TraDataExecucao = null;
                    tramite.TraDataRevisao = null;
                }
                else
                {
                    return new ResponseModel
                    {
                        RM = "Não é possível Retroceder pois o Trâmite já foi iniciado.",
                        errorCode = "TRAMITES_TRAMITE_INICIADO_NAO_PODE_RETROCEDER",
                        RC = ResponseCode.EntidadeNaoProcessavel,
                        OK = false
                    };
                }

                await _dbContext.SaveChangesAsync();

                return new ResponseModel
                {
                    RM = "Trâmite Retrocedido.",
                    RC = ResponseCode.OK,
                    OK = true
                };
            }
            catch (Exception ex)
            {
                Servico.GravaLog($"{nameof(TramitesRepositorio)}.{nameof(Retroceder)} Id[{idTramite}]", ex);
                return new ResponseModel
                {
                    RM = Servico.MSG_EXCEPTION,
                    RC = ResponseCode.Excecao,
                    OK = true
                };
            }
        }
    }
}
