using System;
using System.Collections.Generic;

namespace SQLRepository.Models;

public partial class Script
{
    public long Id { get; set; }

    public string Name { get; set; } = null!;

    public string ScriptName { get; set; } = null!;

    public DateTime DateAdd { get; set; }

    public bool? Accepted { get; set; }

    public string? Errors { get; set; }

    public virtual ICollection<ExecutionScript> ExecutionScripts { get; set; } = new List<ExecutionScript>();
}
