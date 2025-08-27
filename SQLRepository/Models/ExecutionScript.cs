using System;
using System.Collections.Generic;

namespace SQLRepository.Models;

public partial class ExecutionScript
{
    public long Id { get; set; }

    public long ScriptId { get; set; }

    public short Status { get; set; }

    public DateTime StartTime { get; set; }

    public DateTime EndTime { get; set; }

    public string Response { get; set; } = null!;

    public List<string> Request { get; set; } = null!;

    public virtual Script Script { get; set; } = null!;
}
