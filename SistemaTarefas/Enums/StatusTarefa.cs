using System.ComponentModel;
using System.Reflection;

namespace SistemaTarefas.Enums
{
    public enum StatusTarefa
    {
        [Description("Aberta/Ativa")]
        Aberta,
        [Description("Desativada")]
        Desativada,
        [Description("Finalizada")]
        Finalizada
    }

    public static class StatusTarefaExtensions
    {
        public static string ToCodigo(this StatusTarefa status)
        {
            return status switch
            {
                StatusTarefa.Aberta => "A",
                StatusTarefa.Desativada => "D",
                StatusTarefa.Finalizada => "F",
                _ => throw new ArgumentOutOfRangeException()
            };
        }

        public static StatusTarefa FromCodigo(string codigo)
        {
            return codigo switch
            {
                "A" => StatusTarefa.Aberta,
                "D" => StatusTarefa.Desativada,
                "F" => StatusTarefa.Finalizada,
                _ => throw new ArgumentOutOfRangeException(nameof(codigo), codigo, null)
            };
        }
        public static string ToDescricao(this StatusTarefa status)
        {
            return status.GetType()
                    .GetField(status.ToString())
                    ?.GetCustomAttribute<DescriptionAttribute>()
                    ?.Description ?? status.ToString();
        }

        public static bool GetTarefaDescricao(string codigo, out StatusTarefa status)
        {
            switch (codigo)
            {
                case "A":
                    status = StatusTarefa.Aberta;
                    return true;
                case "D":
                    status = StatusTarefa.Desativada;
                    return true;
                case "F":
                    status = StatusTarefa.Finalizada;
                    return true;
                default:
                    status = default;
                    return false;
            }
        }

        public static bool CodigoStatusTarefaValido(string? codigo)
        {
            if (string.IsNullOrWhiteSpace(codigo))
                return true;

            return Enum.GetValues(typeof(StatusTarefa))
                       .Cast<StatusTarefa>()
                       .Select(e => e.ToCodigo())
                       .Contains(codigo);
        }
    }

    public enum StatusTramite
    {
        [Description("A fazer")]
        AFazer = 1,
        [Description("Em andamento")]
        EmAndamento = 2,
        [Description("Aguardando Revisão")]
        AguardandoRevisao = 3,
        [Description("Falha")]
        TerminadoFalha = 4,
        [Description("OK")]
        TerminadoOK = 5
    }
    public static class EnumTramiteExtensions
    {
        public static string GetDescriptionTramite(this Enum value)
        {
            return value.GetType()
                .GetField(value.ToString())?
                .GetCustomAttribute<DescriptionAttribute>()?
                .Description ?? value.ToString();
        }
    }
}