using SistemaTarefas.Enums;

namespace SistemaTarefas.DTO.Response
{
    public class ModeloTarefaResponse : IResponseModel
    {
        public int MtarId { get; set; }
        public string MtarNome { get; set; } = string.Empty;
        public string MtarDescricao { get; set; } = string.Empty;

        public string RM { get; set; } = string.Empty;
        public ResponseCode RC { get; set; } = ResponseCode.Nulo;
        public string errorCode { get; set; } = string.Empty;
        public bool OK { get; set; } = false;
    }
}
