using Datarisk.Models;
using Datarisk.Models.Request;
using Datarisk.Models.Response;

namespace Datarisk.Services;

public interface IScriptService
{
    Task<BaseResponse> SubmitScript(NewScriptRequest model);
    Task<BaseResponse> UpdateStatus(UpdateStatusScriptRequest model);
    Task<BaseResponse<StatusScriptResponse>> Status(string name);
    Task<byte[]?> GetScript(string name);
    Task<BaseResponse<List<ListaScriptsResponse>>> ListScripts();
}