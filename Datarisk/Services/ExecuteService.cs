using Datarisk.Models;
using Datarisk.Models.Request;
using Datarisk.Models.Response;
using Microsoft.EntityFrameworkCore;
using QueueRepository;
using QueueRepository.Model;
using SQLRepository.Models;

namespace Datarisk.Services;

public class ExecuteService : IExecuteService
{
    private readonly DatariskContext context;

    public ExecuteService(DatariskContext context)
    {
        this.context = context;
    }

    public async Task<BaseResponse<long>> ExecuteScript(ExecuteScriptRequest model)
    {
        var s = await context.Scripts.FirstOrDefaultAsync(e => e.Name == model.ScriptName);
        if (s == null)
            return new BaseResponseError<long> { Errors = ["ScriptName não encontrado."] };

        ExecutionScript es = new ExecutionScript
        {
            StartTime = DateTime.MinValue,
            EndTime = DateTime.MinValue,
            Request = model.Data ?? [],
            Id = 0,
            Response = string.Empty,
            Status = 0,
            ScriptId = s.Id
        };

        await context.ExecutionScripts.AddAsync(es);

        await context.SaveChangesAsync();

        if (string.IsNullOrEmpty(model.Function))
            model.Function = model.ScriptName;

        RabbitConnection connection = new RabbitConnection();
        await connection.SendExecute(new ExecuteModel
        {
            ScriptName = s.ScriptName,
            Id = es.Id,
            Data = model.Data ?? [],
            Function = model.Function
        });

        return new BaseResponse<long>
        {
            Data = es.Id,
            Information = $"Consulte 'Execute/Status/{es.Id}' para verificar o status ou 'Execute/{es.Id}' para pegar a resposta de execução."
        };
    }

    public async Task<BaseResponse<string>> Get(long id)
    {
        var es = await context.ExecutionScripts.FirstOrDefaultAsync(e => e.Id == id);
        if (es == null)
            return new BaseResponseError<string> { Errors = ["Id não encontrado."] };

        return new BaseResponse<string> { Data = es.Response };
    }

    public async Task<BaseResponse<StatusExecutionResponse>> GetStatus(long id)
    {
        var es = await context.ExecutionScripts.FirstOrDefaultAsync(e => e.Id == id);
        if (es == null)
            return new BaseResponseError<StatusExecutionResponse> { Errors = ["Id não encontrado."] };

        return new BaseResponse<StatusExecutionResponse>
        {
            Data = new StatusExecutionResponse
            {
                RequetsId = id,
                Status = es.Status switch
                {
                    1 => "Em andamento",
                    2 => "Finalizado",
                    _ => "Na fila"
                },
                StartTime = es.StartTime == DateTime.MinValue ? null : es.StartTime,
                EndTime = es.EndTime == DateTime.MinValue ? null : es.EndTime,
            }
        };
    }

    public async Task<BaseResponse> UpdateStatus(UpdateStatusExecutionRequest model)
    {
        var es = await context.ExecutionScripts.FirstOrDefaultAsync(e => e.Id == model.Id);
        if (es == null)
            return new BaseResponse();

        switch (model.Status)
        {
            case 1:
                es.StartTime = model.Time;
                break;
            case 2:
                es.EndTime = model.Time;
                if (!string.IsNullOrEmpty(model.Data))
                    es.Response = model.Data;
                break;
        }

        es.Status = model.Status;
        context.ExecutionScripts.Update(es);
        await context.SaveChangesAsync();

        return new BaseResponse();
    }
}
