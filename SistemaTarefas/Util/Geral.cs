using SistemaTarefas.Enums;
using System.Globalization;
using System.Text;
using System.Text.RegularExpressions;

namespace SistemaTarefas.Util
{
    public class Geral
    {
        public static bool EmailEhValido(string? email)
        {
            if (string.IsNullOrWhiteSpace(email))
                return false;

            return Regex.IsMatch(email, @"^[^@\s]+@[^@\s]+\.[^@\s]+$");
        }

        public static DateTime ProximoDiaUtil(DateTime data, bool incluirFeriados = true, List<DateTime>? lstFeriadosEspecificos = null)
        { // Retorna a Próxima data útil, ou a própria data definida se já for útil.

            // Cálculo Páscoa
            // A Páscoa é celebrada no primeiro domingo após a primeira lua cheia que ocorre depois do equinócio da Primavera(no hemisfério norte, outono no hemisfério sul), ou seja, 
            // é equivalente à antiga regra de que seria o primeiro Domingo após o 14º dia do mês lunar de Nissan.
            int X = 0;
            int Y = 0;

            // Não existia Páscoa antes de 1582

            if (data.Year >= 1582 && data.Year <= 1699)
            {
                X = 22;
                Y = 2;
            }
            else
            if (data.Year >= 1700 && data.Year <= 1799)
            {
                X = 23;
                Y = 3;
            }
            else
            if (data.Year >= 1800 && data.Year <= 1899)
            {
                X = 23;
                Y = 4;
            }
            else
            if (data.Year >= 1900 && data.Year <= 2099)
            {
                X = 24;
                Y = 5;
            }
            else
            if (data.Year >= 2100 && data.Year <= 2199)
            {
                X = 24;
                Y = 6;
            }
            else
            if (data.Year >= 2200 && data.Year <= 2299)
            {
                X = 25;
                Y = 7;
            }
            else
            { // Sem precisão
                X = 25;
                Y = 8;
            }

            int a = data.Year % 19;
            int b = data.Year % 4;
            int c = data.Year % 7;
            int d = ((19 * a) + X) % 30;
            int e = ((2 * b) + (4 * c) + (6 * d) + Y) % 7;

            int dia;
            int mes;

            if (d + e < 10)
            {
                dia = (d + e + 22);
                mes = 3;
            }
            else
            {
                dia = (d + e - 9);
                mes = 4;
            }

            if (dia == 26 && mes == 4) // exceções
            {
                dia = 19;
            }
            else
            if (dia == 25 && mes == 4 && d == 28 && a > 10) // exceções
            {
                dia = 18;
            }

            DateTime pascoa = new DateTime(data.Year, mes, dia);
            // fim Cálculo Páscoa // 


            if (lstFeriadosEspecificos == null)
            {
                lstFeriadosEspecificos = new List<DateTime>();
            }

            if (incluirFeriados)
            {   // Para calcular a Terça-feira de Carnaval, basta subtrair 47 dias do Domingo de Páscoa. Para calcular a Quinta-feira de Corpus Christi, soma-se 60 dias ao Domingo de Páscoa.

                lstFeriadosEspecificos.Add(pascoa);//Domingo de Páscoa
                //lstFeriadosEspecificos.Add(pascoa.AddDays(-2));//Sexta Feira da Paixão de Cristo. NÃO é feriado nacional (Apenas se estiver em decreto municipal).
                lstFeriadosEspecificos.Add(pascoa.AddDays(-47));//Terça Feira de Carnaval
                lstFeriadosEspecificos.Add(pascoa.AddDays(-46));//Quarta Feira de Cinzas (1/2 expediente inclusive bancário) 
                //lstFeriadosEspecificos.Add(pascoa.AddDays(60));//Quinta-feira de Corpus Christi. NÃO é feriado nacional (Apenas se estiver em decreto municipal).
            }

            bool modificou = true;

            while (modificou)
            {
                modificou = false;

                if (data.DayOfWeek == DayOfWeek.Saturday)
                {
                    data = data.AddDays(2);
                    modificou = true;
                }
                else
                if (data.DayOfWeek == DayOfWeek.Sunday)
                {
                    data = data.AddDays(1);
                    modificou = true;
                }

                if (incluirFeriados) // Feriados fixos do Brasil e mundiais.
                {
                    if (
                        (data.Day == 1 && data.Month == 1) ||
                        (data.Day == 21 && data.Month == 4) ||
                        (data.Day == 1 && data.Month == 5) ||
                        (data.Day == 7 && data.Month == 9) ||
                        (data.Day == 12 && data.Month == 10) ||
                        (data.Day == 2 && data.Month == 11) ||
                        (data.Day == 15 && data.Month == 11) ||
                        (data.Day == 25 && data.Month == 12)
                       )
                    {
                        data = data.AddDays(1);
                        modificou = true;
                    }

                }

                if (lstFeriadosEspecificos != null)// Lista personalidade de Feriados
                {
                    for (int i = 0; i < lstFeriadosEspecificos.Count(); i++)
                    {
                        if (lstFeriadosEspecificos[i].Year == 1 && lstFeriadosEspecificos[i].Month == data.Month && lstFeriadosEspecificos[i].Day == data.Day)
                        { // Feriados que ocorrem todo ano, Defina Year == 1
                            data = data.AddDays(1);
                            modificou = true;
                        }
                        else
                        if (lstFeriadosEspecificos[i].Year > 1 && data.Date == lstFeriadosEspecificos[i].Date)
                        { // Feriados que ocorrem apenas no ano específico, especifique Year correto.
                            data = data.AddDays(1);
                            modificou = true;
                        }

                    }
                }

            }

            return data;
        }
    }
}
