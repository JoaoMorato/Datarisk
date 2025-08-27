namespace Datarisk.Models.Request;

public class UpdateStatusExecutionRequest
{
    public long Id { get; set; }
    public short Status { get; set; }
    public DateTime Time { get; set; }
    public string Data { get; set; } = string.Empty;
}
