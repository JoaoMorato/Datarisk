namespace Datarisk.Models.Response;

public class StatusScriptResponse
{
    public string Name { get; set; } = string.Empty;
    public List<string>? Errors { get; set; } = null;
    public string Status { get; set; } = string.Empty;
}
