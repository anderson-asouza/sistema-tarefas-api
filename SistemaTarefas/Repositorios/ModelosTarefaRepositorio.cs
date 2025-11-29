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
    public class ModelosTarefaRepositorio(
        SistemaTarefasDBContex _dbContext,
        IMapper _mapper
    ) : IModelosTarefaRepositorio
    {    
        public async Task<List<ModeloTarefaResponse>> BuscarVarios(string? nomeModeloTarefa = null)
        {
            try
            {
                List<ModelosTarefa> modelosTarefa = new List<ModelosTarefa>();

                if (!string.IsNullOrWhiteSpace(nomeModeloTarefa))
                {
                    modelosTarefa = await _dbContext.ModelosTarefa.Where(x => x.MtarNome!.ToUpper().Contains(nomeModeloTarefa.ToUpper().Trim())).OrderBy(x => x.MtarNome).ToListAsync();
                }
                else
                {
                    modelosTarefa = await _dbContext.ModelosTarefa.OrderBy(x => x.MtarNome).ToListAsync();
                }

                if (!modelosTarefa.Any())
                {
                    return new List<ModeloTarefaResponse>
                    {
                        new ModeloTarefaResponse
                        {
                            RM = "Nenhum registro encontrado.",
                            RC = ResponseCode.RegistroNaoEncontrado,
                            OK = false
                        }
                    };
                }

                List<ModeloTarefaResponse> resposta = _mapper.Map<List<ModeloTarefaResponse>>(modelosTarefa) ?? new List<ModeloTarefaResponse>();

                return resposta;
            }
            catch (Exception ex)
            {                
                Servico.GravaLog($"{nameof(ModelosTarefaRepositorio)}.{nameof(BuscarVarios)}", ex);
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
        public async Task<ModeloTarefaResponse> BuscarUm(int id, string? nomeModeloTarefa = null)
        {
            try
            {
                ModelosTarefa? modelosTarefa = null;

                if (id > 0)
                    modelosTarefa = await _dbContext.ModelosTarefa.FirstOrDefaultAsync(x => x.MtarId == id);

                if (modelosTarefa == null && !string.IsNullOrWhiteSpace(nomeModeloTarefa))
                    modelosTarefa = await _dbContext.ModelosTarefa.FirstOrDefaultAsync(x => x.MtarNome.ToUpper().Trim() == nomeModeloTarefa.ToUpper().Trim());

                if (modelosTarefa == null)
                {
                    return new ModeloTarefaResponse
                    {
                        RM = "Modelo Tarefa não encontrado.",
                        RC = ResponseCode.RegistroNaoEncontrado,
                        OK = false
                    };
                }

                ModeloTarefaResponse resposta = _mapper.Map<ModeloTarefaResponse>(modelosTarefa);

                resposta.RM = "";
                resposta.RC = ResponseCode.OK;
                resposta.OK = true;

                return resposta;
            }
            catch (Exception ex)
            {
                Servico.GravaLog($"{nameof(ModelosTarefaRepositorio)}.{nameof(BuscarUm)}", ex);
                return new ModeloTarefaResponse
                {
                    RM = Servico.MSG_EXCEPTION,
                    RC = ResponseCode.Excecao,
                    OK = false
                };
            }
        }
        private async Task<bool> ValidaGravacao(ModeloTarefaRequest modeloTarefaRequest, ModeloTarefaResponse resposta, int id = 0)
        {
            try
            {
                bool duplicado = await _dbContext.ModelosTarefa.AnyAsync(u => u.MtarId != id && (u.MtarNome.ToUpper().Trim() == modeloTarefaRequest.MtarNome.ToUpper().Trim()));

                if (duplicado)
                {
                    resposta.RM = "Já existe um Modelo de Tarefa definido com este nome.";
                    resposta.errorCode = "NOME_EM_USO";
                    resposta.RC = ResponseCode.EntidadeNaoProcessavel;
                    resposta.OK = false;
                    return false;
                }

                return true;
            }
            catch (Exception ex)
            {
                Servico.GravaLog($"{nameof(ModelosTarefaRepositorio)}.{nameof(ValidaGravacao)}", ex);
                return false;
            }
        }
        public async Task<ModeloTarefaResponse> Cadastrar(ModeloTarefaRequest modeloTarefaRequest)
        {
            try
            {
                ModeloTarefaResponse resposta = new();

                if (!await ValidaGravacao(modeloTarefaRequest, resposta))
                {
                    return resposta;
                }

                ModelosTarefa modeloTarefa = _mapper.Map<ModelosTarefa>(modeloTarefaRequest);

                await _dbContext.ModelosTarefa.AddAsync(modeloTarefa);
                await _dbContext.SaveChangesAsync();

                resposta = _mapper.Map<ModeloTarefaResponse>(modeloTarefa);

                resposta.RM = "";
                resposta.RC = ResponseCode.CadastradoSucesso;
                resposta.OK = true;

                return resposta;
            }
            catch (Exception ex)
            {
                Servico.GravaLog($"{nameof(ModelosTarefaRepositorio)}.{nameof(Cadastrar)}", ex);
                return new ModeloTarefaResponse
                {
                    RM = Servico.MSG_EXCEPTION,
                    RC = ResponseCode.Excecao,
                    OK = false
                };
            }
        }
        public async Task<ModeloTarefaResponse> Atualizar(ModeloTarefaRequest modeloTarefaRequest, int id)
        {
            try
            {
                ModeloTarefaResponse resposta = new();

                if (!await ValidaGravacao(modeloTarefaRequest, resposta, id))
                {
                    return resposta;
                }

                ModelosTarefa? modeloTarefa = await _dbContext.ModelosTarefa.FirstOrDefaultAsync(x => x.MtarId == id);

                if (modeloTarefa == null)
                {
                    resposta.RM = "Registro para atualização não foi encontrado.";
                    resposta.RC = ResponseCode.RegistroNaoEncontrado;
                    resposta.OK = false;
                    return resposta;
                }

                _mapper.Map(modeloTarefaRequest, modeloTarefa);
                await _dbContext.SaveChangesAsync();


                _mapper.Map(modeloTarefa, resposta);

                resposta.RM = "Modelo Tarefa atualizado com sucesso.";
                resposta.RC = ResponseCode.CadastradoSucesso;
                resposta.OK = true;

                return resposta;
            }
            catch (Exception ex)
            {
                Servico.GravaLog($"{nameof(ModelosTarefaRepositorio)}.{nameof(Atualizar)}", ex);
                return new ModeloTarefaResponse
                {
                    RM = Servico.MSG_EXCEPTION,
                    RC = ResponseCode.Excecao,
                    OK = false
                };
            }
        }
        private async Task<bool> IntegridadeReferencialModeloTarefa(int id, ResponseModel resposta)
        {            
            try
            {
                List<string> erro = new List<string>();
                List<string> erroCode = new List<string>();

                bool tarefas = await _dbContext.Tarefas.AnyAsync(x => x.TarMtarId == id);

                if (tarefas)
                {
                    erro.Add("Tarefas");
                    erroCode.Add("TAREFAS");
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
                Servico.GravaLog($"{nameof(ModelosTarefaRepositorio)}.{nameof(Atualizar)} Id[{id}]", ex);
                resposta.RC = ResponseCode.Excecao;
                return false;
            }            
        }
        public async Task<ResponseModel> Apagar(int id)
        {
            try
            {
                ResponseModel resposta = new ResponseModel();

                if (!await IntegridadeReferencialModeloTarefa(id, resposta))
                {
                    return resposta;
                }

                ModelosTarefa? modeloTarefa = await _dbContext.ModelosTarefa.FirstOrDefaultAsync(x => x.MtarId == id);

                if (modeloTarefa == null)
                {
                    return new ResponseModel
                    {
                        RM = "Registro para exclusão não foi encontrado.",
                        RC = ResponseCode.RegistroNaoEncontrado,
                        OK = false
                    };
                }

                _dbContext.ModelosTarefa.Remove(modeloTarefa);
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
                Servico.GravaLog($"{nameof(ModelosTarefaRepositorio)}.{nameof(Apagar)} Id[{id}]", ex);
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
