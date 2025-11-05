using SistemaTarefas.Enums;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SistemaTarefas.DTO.Request
{
    public class TramiteRequest : IRequestModel
    {
    }
    public class TramiteNotaRequest : IRequestModel
    {
        public int IdTramite { get; set; }
        public string? Nota { get; set; }
    }

}