using SistemaTarefas.DTO.Response;
using SistemaTarefas.Enums;
using System.ComponentModel.DataAnnotations;

namespace SistemaTarefas.DTO.Request
{
    public class FlagRequest : IRequestModel
    {
        public string FlaRotulo { get; set; } = string.Empty;       
        public string FlaCor { get; set; } = string.Empty;
    }
}