namespace Datarisk.Models.Request;

public class ExecuteScriptRequest
{
    /// <summary>
    /// Nome do script a ser executado
    /// </summary>
    public string ScriptName { get; set; } = string.Empty;
    /// <summary>
    /// Nome da função a ser chamada, caso nulo ou vazio é procurada uma função com o mesmo nome do script
    /// </summary>
    public string? Function { get; set; } = null;
    /// <summary>
    /// Lista com os parametros a ser usado, ex: ["Param1", "Param2"]
    /// </summary>
    public List<string>? Data { get; set; } = [];
}
