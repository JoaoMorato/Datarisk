using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QueueRepository.Model;
public class ScriptModel
{
    public long Id { get; set; }
    public string FileName { get; set; } = string.Empty;
    public List<string>? Errors { get; set; } = null;
    public bool Accept { get; set; } = false;
}
