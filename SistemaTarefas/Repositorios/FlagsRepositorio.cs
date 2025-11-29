using AutoMapper;
using Azure.Core;
using Azure.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Conventions;
using SistemaTarefas;
using SistemaTarefas.Data;
using SistemaTarefas.DTO.Request;
using SistemaTarefas.DTO.Response;
using SistemaTarefas.Enums;
using SistemaTarefas.Models;
using SistemaTarefas.Repositorios.Interfaces;
using SistemaTarefas.Servicos;
using SistemaTarefas.Util;
using System.Collections.Generic;
using System.ComponentModel;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace SistemaTarefas.Repositorios
{
    public class FlagsRepositorio(
        SistemaTarefasDBContex _dbContext,
        IMapper _mapper
    ) : IFlagsRepositorio
    {

        public async Task<List<FlagResponse>> BuscarVarios(string? rotuloFlag = null)
        {
            try
            {
                var flags = new List<Flags>();

                if (!string.IsNullOrWhiteSpace(rotuloFlag))
                {
                   flags = await _dbContext.Flags.Where(x => x.FlaRotulo!.ToUpper().Contains(rotuloFlag.ToUpper().Trim())).OrderBy(x => x.FlaRotulo).ToListAsync();
                }
                else
                {
                    flags = await _dbContext.Flags.OrderBy(x => x.FlaRotulo).ToListAsync();
                }

                if (!flags.Any())
                {
                    return new List<FlagResponse>
                    {
                        new FlagResponse
                        {
                            RM = "Nenhum registro encontrado.",
                            RC = ResponseCode.RegistroNaoEncontrado,
                            OK = false
                        }
                    };
                }

                List<FlagResponse> resposta = _mapper.Map<List<FlagResponse>>(flags) ?? new List<FlagResponse>();

                return resposta;
            }
            catch (Exception ex)
            {
                Servico.GravaLog($"{nameof(FlagsRepositorio)}.{nameof(BuscarVarios)}", ex);
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

        public async Task<FlagResponse> BuscarUm(int id, string? rotuloFlag = null)
        {
            try
            {
                Flags? flag = null;

                if (id > 0)
                    flag = await _dbContext.Flags.FirstOrDefaultAsync(x => x.FlaId == id);

                if (flag == null && !string.IsNullOrWhiteSpace(rotuloFlag))
                    flag = await _dbContext.Flags.FirstOrDefaultAsync(x => x.FlaRotulo.ToUpper().Trim() == rotuloFlag.ToUpper().Trim());

                if (flag == null)
                {
                    return new FlagResponse
                    {
                        RM = "Flag não encontrada.",
                        RC = ResponseCode.RegistroNaoEncontrado,
                        OK = false
                    };
                }

                FlagResponse resposta = _mapper.Map<FlagResponse>(flag);

                resposta.RM = "";
                resposta.RC = ResponseCode.OK;
                resposta.OK = true;

                return resposta;
            }
            catch (Exception ex)
            {
                Servico.GravaLog($"{nameof(FlagsRepositorio)}.{nameof(BuscarUm)}", ex);
                return new FlagResponse
                {
                    RM = Servico.MSG_EXCEPTION,
                    RC = ResponseCode.Excecao,
                    OK = false
                };
            }
        }
        private async Task<bool> ValidaGracao(FlagRequest flagRequest, FlagResponse resposta, int id = 0)
        {
            try
            {
                bool duplicado = await _dbContext.Flags.AnyAsync(u => u.FlaId != id && (u.FlaRotulo.ToUpper().Trim() == flagRequest.FlaRotulo.ToUpper().Trim()));

                if (duplicado)
                {
                    resposta.RM = "Já existe uma Flag definida com este rótulo.";
                    resposta.errorCode = "NOME_EM_USO";
                    resposta.RC = ResponseCode.EntidadeNaoProcessavel;
                    resposta.OK = false;
                    return false;
                }

                return true;
            }
            catch (Exception ex)
            {
                Servico.GravaLog($"{nameof(FlagsRepositorio)}.{nameof(ValidaGracao)}", ex);
                return false;
            }
        }
        public async Task<FlagResponse> Cadastrar(FlagRequest flagRequest)
        {
            try
            {
                FlagResponse resposta = new();

                if (!await ValidaGracao(flagRequest, resposta))
                {
                    return resposta;
                }

                Flags flag = _mapper.Map<Flags>(flagRequest);

                await _dbContext.Flags.AddAsync(flag);
                await _dbContext.SaveChangesAsync();

                resposta = _mapper.Map<FlagResponse>(flag);

                resposta.RM = "";
                resposta.RC = ResponseCode.CadastradoSucesso;
                resposta.OK = true;

                return resposta;
            }
            catch (Exception ex)
            {
                Servico.GravaLog($"{nameof(FlagsRepositorio)}.{nameof(Cadastrar)}", ex);
                return new FlagResponse
                {
                    RM = Servico.MSG_EXCEPTION,
                    RC = ResponseCode.Excecao,
                    OK = false
                };
            }
        }
        public async Task<FlagResponse> Atualizar(FlagRequest flagRequest, int id)
        {
            try
            {
                FlagResponse resposta = new();

                if (!await ValidaGracao(flagRequest, resposta, id))
                {
                    return resposta;
                }

                Flags? flag = await _dbContext.Flags.FirstOrDefaultAsync(x => x.FlaId == id);

                if (flag == null)
                {
                    resposta.RM = "Registro para atualização não foi encontrado.";
                    resposta.RC = ResponseCode.RegistroNaoEncontrado;
                    resposta.OK = false;
                    return resposta;
                }

                _mapper.Map(flagRequest, flag);
                await _dbContext.SaveChangesAsync();


                _mapper.Map(flag, resposta);

                resposta.RM = "Flag atualizada com sucesso.";
                resposta.RC = ResponseCode.CadastradoSucesso;
                resposta.OK = true;

                return resposta;
            }
            catch (Exception ex)
            {
                Servico.GravaLog($"{nameof(FlagsRepositorio)}.{nameof(Atualizar)}", ex);
                return new FlagResponse
                {
                    RM = Servico.MSG_EXCEPTION,
                    RC = ResponseCode.Excecao,
                    OK = false
                };
            }
        }
        private async Task<string> IntegridadeReferencialFlag(int id)
        {
            try
            {
                string retorno = "";

                bool tarefas = await _dbContext.Tarefas.AnyAsync(x => x.TarFlaId == id);

                if (tarefas)
                {
                    retorno += "Tarefas";
                }

                return retorno;
            }
            catch (Exception ex)
            {
                Servico.GravaLog($"{nameof(FlagsRepositorio)}.{nameof(IntegridadeReferencialFlag)} Id[{id}]", ex);
                return Servico.MSG_EXCEPTION;
            }
        }
        public async Task<ResponseModel> Apagar(int id)
        {
            try
            {
                string msg = await IntegridadeReferencialFlag(id);

                if (!string.IsNullOrWhiteSpace(msg))
                {
                    return new ResponseModel
                    {
                        RM = "Flag não pode ser excluída. Há vínculos com:"+Environment.NewLine+msg,
                        RC = ResponseCode.Conflito,
                        OK = false
                    };
                }

                Flags? tipoTarefa = await _dbContext.Flags.FirstOrDefaultAsync(x => x.FlaId == id);

                if (tipoTarefa == null)
                {
                    return new ResponseModel
                    {
                        RM = "Registro para exclusão não foi encontrado.",
                        RC = ResponseCode.RegistroNaoEncontrado,
                        OK = false
                    };
                }

                _dbContext.Flags.Remove(tipoTarefa);
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
                Servico.GravaLog($"{nameof(FlagsRepositorio)}.{nameof(Apagar)} Id[{id}]", ex);
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
