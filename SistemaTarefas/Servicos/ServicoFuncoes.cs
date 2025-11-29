using System.Text.RegularExpressions;

namespace SistemaTarefas.Servicos
{
    public class ServicoFuncoes
    {   public static bool ValidaCorRGB(string cor)
        {
            if (string.IsNullOrWhiteSpace(cor))
                return false;

            return Regex.IsMatch(cor, "^#[0-9A-Fa-f]{6}$");
        }
    }
}
