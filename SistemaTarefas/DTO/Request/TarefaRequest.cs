using SistemaTarefas.Enums;

namespace SistemaTarefas.DTO.Request;

public class TarefaRequest : IRequestModel
{
    public string TarNomeTarefa { get; set; } = null!;
    public string? TarDescricao { get; set; }    
    public int TarMtarId { get; set; }
}

public class TarefaUpdRequest : IRequestModel
{
    public string TarNomeTarefa { get; set; } = null!;
    public string? TarDescricao { get; set; }
}