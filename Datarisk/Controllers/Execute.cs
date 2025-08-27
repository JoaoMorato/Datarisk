using Datarisk.Models;
using Datarisk.Models.Request;
using Datarisk.Models.Response;
using Datarisk.Services;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace Datarisk.Controllers;

[ApiController]
[Route("[controller]")]
public class Execute : ControllerBase
{
    public IExecuteService ExecuteService { get; }

    public Execute(IExecuteService executeService)
    {
        ExecuteService = executeService;
    }

    /// <summary>
    /// Execute uma função específica de um script com determinados valores.
    /// </summary>
    /// <remarks>
    /// <para>
    /// Exemplo:
    /// {
    ///     "ScriptName": "Script",
    ///     "Function": "Fun", // Caso seja nulo ou vazio, será procurado uma função com o mesmo nome que ScriptName
    ///     "Data": ["Param1", "Param2", ...] // Parametros da função chamada
    /// }
    /// </para>
    /// </remarks>
    /// <returns>Id para buscar o status/resposta da execução</returns>
    [HttpPost]
    [ProducesResponseType(typeof(BaseResponse<long>), 200)]
    [ProducesResponseType(typeof(BaseResponseError), 422)]
    public async Task<IActionResult> ExecuteScript([FromBody] ExecuteScriptRequest model)
    {
        return await ExecuteService.ExecuteScript(model);
    }

    [HttpPut]
    [ApiExplorerSettings(IgnoreApi = true)]
    public async Task<IActionResult> UpdateStatusExecution([FromBody] UpdateStatusExecutionRequest model)
    {
        return await ExecuteService.UpdateStatus(model);
    }

    /// <summary>
    /// Verifica o status de execução.
    /// </summary>
    /// <param name="id">Id de execução</param>
    /// <returns>Status atual da execução</returns>
    [HttpGet("Status/{id:long}")]
    [ProducesResponseType(typeof(BaseResponse<StatusExecutionResponse>), 200)]
    [ProducesResponseType(typeof(BaseResponseError), 422)]
    public async Task<IActionResult> GetStatus(long id)
    {
        return await ExecuteService.GetStatus(id);
    }

    /// <summary>
    /// Retorna a resposta de uma execução finalizada.
    /// </summary>
    /// <param name="id">Id de execução</param>
    /// <returns>Resposta do script executado</returns>
    [HttpGet("{id:long}")]
    [ProducesResponseType(typeof(BaseResponse<string>), 200)]
    [ProducesResponseType(typeof(BaseResponseError), 422)]
    public async Task<IActionResult> Get(long id)
    {
        return await ExecuteService.Get(id);
    }
}
