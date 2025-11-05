using System.ComponentModel;
using System.Reflection;


namespace SistemaTarefas.Enums
{
    public enum ResponseCode
    {
        [Description("Nulo")]
        Nulo = 0,
        [Description("Sucesso!")]
        OK = 200,
        [Description("Cadastrado com Sucesso.")]
        CadastradoSucesso = 201,
        [Description("Requisição aceita, mas ainda não processada.")]
        AceitoParaProcessamento = 202,
        [Description("Sucesso sem conteúdo útil, mas corpo base presente.")]
        SucessoSemConteudo = 204,
        [Description("Múltiplos status.")]
        MultiStatus = 207,
        [Description("Requisição inválida.")]   
        BadRequest = 400,
        [Description("Não autorizado (ex: token ausente/inválido).")]
        NaoAutorizado = 401,
        [Description("Forbid - Acesso Negado.")]
        ForbidAcessoNegado = 403,
        [Description("Registro não encontrado.")]
        RegistroNaoEncontrado = 404,
        [Description("Conflito de estado (violação de regra ou integridade).")]
        Conflito = 409,
        [Description("Entidade Não Processável.")]
        EntidadeNaoProcessavel = 422,
        [Description("Muitas requisições.")]
        MuitasRequisicoes = 429,
        [Description("Exceção.")]
        Excecao = 500,
        [Description("Serviço indisponível.")]
        ServicoIndisponivel = 503,

        #region Demais Códigos

        [Description("Método HTTP não permitido para este recurso")]
        MetodoNaoPermitido = 405,
        [Description("Tipo de mídia não suportado")]
        TipoMidiaNaoSuportado = 415,
        [Description("Pré-condição falhou")]
        PreCondicaoFalhou = 412,
        [Description("Requisição expirou (timeout do cliente)")]
        TimeoutRequisicao = 408,

        #endregion
    }
    public static class EnumExtensions
    {
        public static string GetDescription(this Enum value)
        {
            FieldInfo? field = value.GetType().GetField(value.ToString());

            if (field != null)
            {
                DescriptionAttribute? attribute =
                    Attribute.GetCustomAttribute(field, typeof(DescriptionAttribute)) as DescriptionAttribute;

                if (attribute != null)
                {
                    return attribute.Description;
                }
            }

            return value.ToString();
        }

        public static string GetDescription(ResponseCode codigo)
        {
            FieldInfo? field = codigo.GetType().GetField(codigo.ToString());

            if (field != null)
            {
                DescriptionAttribute? attribute =
                    Attribute.GetCustomAttribute(field, typeof(DescriptionAttribute)) as DescriptionAttribute;

                if (attribute != null)
                {
                    return attribute.Description;
                }
            }

            return codigo.ToString();
        }
    }
}