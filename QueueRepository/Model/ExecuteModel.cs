using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QueueRepository.Model;
public class ExecuteModel
{
    public long Id { get; set; }
    public string ScriptName { get; set; } = string.Empty;
    public List<string> Data { get; set; } = [];
    public string Function { get; set; } = string.Empty;
}
