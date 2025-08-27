namespace Datarisk.Models.Response;

public class StatusExecutionResponse
{
    public long RequetsId { get; set; }
    public string Status { get; set; } = string.Empty;
    public DateTime? StartTime { get; set; }
    public DateTime? EndTime { get; set; } = null;
}
