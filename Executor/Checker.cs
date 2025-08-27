using Acornima;
using Acornima.Ast;
using Jint;
using Newtonsoft.Json;
using QueueRepository.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Executor;
internal class Checker
{
    private const string LocalPath = "/app/data";

    public static async Task<long> CheckScript(ScriptModel model)
    {
        try
        {
            Parser parser = new Parser();
            var node = parser.ParseScript(File.ReadAllText(Path.Combine(LocalPath, model.FileName)));
            model.Errors = ParseScript(node, new List<string>());
            model.Accept = model.Errors.Count == 0;
        }
        catch(Exception)
        {
            model.Accept = false;
            model.Errors = ["Falha ao tentar ler o conteudo do arquivo."];
        }
        var c = new StringContent(JsonConvert.SerializeObject(model));
        c.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("application/json");

        await new HttpClient().PutAsync("http://api:80/Script", c);

        return model.Id;
    }

    private static List<string> ParseScript(Node node, List<string> lista)
    {
        foreach (var n in node.ChildNodes)
        {
            string erro = string.Empty;
            List<string> help;

            switch (n.Type)
            {
                case NodeType.Identifier:
                    help = ["eval", "console", "Console", "document", "Document", "XMLHttpRequest", "fetch", "window", "Window"];
                    erro = help.FirstOrDefault(e => e == ((Identifier)n).Name) ?? string.Empty;
                    break;
                case NodeType.ImportExpression:
                    erro = "import";
                    break;
            }

            if (!string.IsNullOrEmpty(erro))
                lista.Add($"'{erro}' não permitido [{n.Location.Start.Line}, {n.Location.Start.Column}]");
            lista = ParseScript(n, lista);
        }

        return lista;
    }
}
