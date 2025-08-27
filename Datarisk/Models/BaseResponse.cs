using Microsoft.AspNetCore.Mvc;
using Microsoft.VisualBasic;
using Newtonsoft.Json;
using System.Buffers;
using System.Text;

namespace Datarisk.Models;

public class BaseResponse : IActionResult
{
    public string Identifier { get; private set; } = string.Empty;
    public string? Information { get; set; } = null;
    protected int _statusCode = 200; 

    public virtual async Task ExecuteResultAsync(ActionContext context)
    {
        Identifier = context.HttpContext.TraceIdentifier;
        context.HttpContext.Response.StatusCode = _statusCode;
        context.HttpContext.Response.ContentType = "application/json";
        var resp = JsonConvert.SerializeObject(this, Formatting.Indented, new JsonSerializerSettings
        {
            NullValueHandling = NullValueHandling.Ignore,
        });
        await context.HttpContext.Response.WriteAsync(resp, Encoding.UTF8);
    }
}

public class BaseResponse<T> : BaseResponse
{
    public T? Data { get; set; }
}

public class BaseResponseError : BaseResponse
{
    public List<string>? Errors { get; set; } = null;

    public override Task ExecuteResultAsync(ActionContext context)
    {
        _statusCode = Errors == null || Errors.Count == 0 ? 200 : 422;
        return base.ExecuteResultAsync(context);
    }
}

public class BaseResponseError<T> : BaseResponse<T> 
{
    public List<string>? Errors { get; set; } = null;

    public override Task ExecuteResultAsync(ActionContext context)
    {
        _statusCode = Errors == null || Errors.Count == 0 ? 200 : 422;
        return base.ExecuteResultAsync(context);
    }
}
