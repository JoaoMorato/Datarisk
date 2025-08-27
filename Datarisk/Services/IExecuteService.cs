using Datarisk.Models;
using Datarisk.Models.Request;
using Datarisk.Models.Response;

namespace Datarisk.Services;

public interface IExecuteService
{
    Task<BaseResponse<long>> ExecuteScript(ExecuteScriptRequest model);
    Task<BaseResponse> UpdateStatus(UpdateStatusExecutionRequest model);
    Task<BaseResponse<StatusExecutionResponse>> GetStatus(long id);
    Task<BaseResponse<string>> Get(long id);
}