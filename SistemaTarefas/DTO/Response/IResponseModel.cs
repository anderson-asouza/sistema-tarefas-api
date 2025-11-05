using SistemaTarefas.Enums;

namespace SistemaTarefas.DTO.Response
{
    public interface IResponseModel
    {
        string RM { get; set; }
        ResponseCode RC { get; set; }
        string errorCode { get; set; }
        bool OK { get; set; }
    }
}
