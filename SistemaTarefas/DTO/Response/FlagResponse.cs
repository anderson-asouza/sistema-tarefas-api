using SistemaTarefas.Enums;

namespace SistemaTarefas.DTO.Response
{
    public class FlagResponse : IResponseModel
    {
        public int flaId { get; set; }
        public string flaRotulo { get; set; } = string.Empty;
        public string flaCor { get; set; } = string.Empty;

        public string RM { get; set; } = string.Empty;
        public ResponseCode RC { get; set; } = ResponseCode.Nulo;
        public string errorCode { get; set; } = string.Empty;
        public bool OK { get; set; } = false;
    }
}
