using SistemaTarefas.Enums;

namespace SistemaTarefas.DTO.Request
{
    public class ModeloTarefaRequest : IRequestModel
    {
        public string MtarNome { get; set; } = string.Empty;
        public string MtarDescricao { get; set; } = string.Empty;
    }
}