using Jint;
using Jint.Native;
using Jint.Native.Json;
using Jint.Runtime;
using Newtonsoft.Json;
using System;

namespace Executor;
internal class Execute
{
    private const string LocalPath = "/app/data";

    public static async Task<long> ExecuteJS(long id, string file, string[] data, string fun)
    {
        string resp;
        StringContent content = new StringContent(JsonConvert.SerializeObject(new
        {
            id,
            status = 1,
            time = DateTime.UtcNow,
            data = string.Empty
        }));

        HttpClient client = new HttpClient();
        content.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("application/json");
        var p = client.PutAsync("http://api:80/Execute", content);

        try
        {
            string code = await File.ReadAllTextAsync(Path.Combine(LocalPath, file));
            // Timeout de 10 min
            CancellationTokenSource cancellationToken = new CancellationTokenSource(TimeSpan.FromMinutes(10));

            var t = Task.Run(() =>
            {
                var eg = new Engine(options =>
                {
                    options.LimitRecursion(1000000); // Previnir recursividade infinita
                    options.MaxStatements(1000000); // Previne while(true) infinito
                    options.DisableStringCompilation(); // Previne eval
                    options.DebuggerStatementHandling(Jint.Runtime.Debugger.DebuggerStatementHandling.Ignore); // Ignora debugger
                    options.LimitMemory(209715200); // 200MB
                });

                var j = eg.Execute(code).Invoke(fun, TransformValue(data, eg));

                return GetRetorno(j, eg);
            }, cancellationToken.Token);

            await Task.WhenAll(t, p);

            if (t.Exception != null)
                throw t.Exception;

            resp = await t;
        }
        catch (OperationCanceledException)
        {
            resp = "Timeout no processo.";
        }
        catch (AggregateException ex)
        {
            resp = ex.Message;
        }
        catch (StatementsCountOverflowException)
        {
            resp = "Estouro no limite de pilha.";
        }
        catch (Exception ex)
        {
            resp = $"Ocorreu uma falha durante o processamento.\r\n" + ex.Message;
        }

        content = new StringContent(JsonConvert.SerializeObject(new
        {
            id,
            status = 2,
            time = DateTime.UtcNow,
            data = resp
        }));

        content.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("application/json");

        await client.PutAsync("http://api:80/Execute", content);

        return id;
    }

    static JsValue[] TransformValue(string[] json, Engine eg)
    {
        List<JsValue> values = new List<JsValue>();
        foreach (string j in json)
        {
            if ((j.StartsWith('{') && j.EndsWith('}')) || (j.StartsWith('[') && j.EndsWith(']')))
            {
                try
                {
                    values.Add(new Jint.Native.Json.JsonParser(eg).Parse(j));
                }
                catch
                {
                    values.Add(eg.Execute("var d = " + j).GetValue("d"));
                }

                continue;
            }

            if (double.TryParse(j, out var d))
                values.Add(JsValue.FromObject(eg, d));
            else
                values.Add(JsValue.FromObject(eg, j));
        }
        return values.ToArray();
    }

    static string GetRetorno(JsValue j, Engine eg)
    {
        if (j.IsString() || j.IsNumber() || j.IsBoolean() || j.IsNull() || j.IsUndefined())
            return j.ToString();
        return new Jint.Native.Json.JsonSerializer(eg).Serialize(j).ToString();
    }
}
