using SistemaTarefas.Enums;

namespace SistemaTarefas.DTO.Response
{
    public class ResponseModel : IResponseModel
    {
        public string RM { get; set; } = string.Empty;
        public ResponseCode RC { get; set; } = ResponseCode.Nulo;
        public string errorCode { get; set; } = string.Empty;
        public bool OK { get; set; } = false;
    }
}
