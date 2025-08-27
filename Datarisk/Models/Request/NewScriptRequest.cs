using Newtonsoft.Json;

namespace Datarisk.Models.Request;

public class NewScriptRequest
{
    /// <summary>
    /// Nome do script.
    /// </summary>
    public string ScriptName { get; set; } = string.Empty;
    /// <summary>
    /// Sobrescrever caso já exista.
    /// </summary>
    public bool Overwrite { get; set; } = false;
    [JsonIgnore]
    public string Arquivo { get; set; }
}
