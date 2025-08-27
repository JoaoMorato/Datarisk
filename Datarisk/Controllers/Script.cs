using Datarisk.Models;
using Datarisk.Models.Request;
using Datarisk.Models.Response;
using Datarisk.Services;
using Datarisk.Validators;
using Microsoft.AspNetCore.Mvc;

namespace Datarisk.Controllers;

[ApiController]
[Route("[Controller]")]
public class Script : ControllerBase
{
    private readonly IScriptService scriptService;

    public Script(IScriptService scriptService)
    {
        this.scriptService = scriptService;
    }

    /// <summary>
    /// Envia um script para análise.
    /// </summary>
    [HttpPost("{name}")]
    [ProducesResponseType(typeof(BaseResponse), 200)]
    [ProducesResponseType(typeof(BaseResponseError), 422)]
    public async Task<IActionResult> AddScript(string name, IFormFile file)
    {
        var model = new NewScriptRequest
        {
            ScriptName = name,
            Overwrite = true
        };

        var result = new NewScriptValidator().Validate(model);
        if (!result.IsValid)
            return new BaseResponseError
            {
                Errors = result.Errors.Select(e => e.ErrorMessage).ToList()
            };

        var s = new StreamReader(file.OpenReadStream());
        model.Arquivo = await s.ReadToEndAsync();

        return await scriptService.SubmitScript(model);
    }

    [HttpPut]
    [ApiExplorerSettings(IgnoreApi =true)]
    public async Task<IActionResult> UpdateStatus([FromBody] UpdateStatusScriptRequest model)
    {
        return await scriptService.UpdateStatus(model);
    }

    /// <summary>
    /// Verifica o status de um script.
    /// </summary>
    /// <param name="name">Nome do script</param>
    [HttpGet("Status/{name}")]
    [ProducesResponseType(typeof(BaseResponse<StatusScriptResponse>), 200)]
    [ProducesResponseType(typeof(BaseResponseError), 422)]
    public async Task<IActionResult> GetStatus(string name)
    {
        return await scriptService.Status(name);
    }

    /// <summary>
    /// Lista os scripts.
    /// </summary>
    [HttpGet]
    [ProducesResponseType(typeof(BaseResponse<ListaScriptsResponse>), 200)]
    [ProducesResponseType(typeof(BaseResponseError), 422)]
    public async Task<IActionResult> List()
    {
        return await scriptService.ListScripts();
    }

    /// <summary>
    /// Busca um script no sistema e retorna seu conteúdo.
    /// </summary>
    /// <param name="name">Nome do script</param>
    [HttpGet("{name:minlength(1)}")]
    [ProducesResponseType(typeof(BaseResponse<IFormFile>), 200)]
    [ProducesResponseType(typeof(BaseResponseError), 422)]
    public async Task<IActionResult?> Get(string name)
    {
        var bts = await scriptService.GetScript(name);
        if (bts == null)
            return NotFound();
        return File(bts, "application/octet-stream", name + ".js");
    }
}
