using Acornima.Ast;
using Datarisk.Models;
using Datarisk.Models.Request;
using Datarisk.Models.Response;
using Microsoft.EntityFrameworkCore;
using QueueRepository;
using SQLRepository.Models;

namespace Datarisk.Services;

public class ScriptService : IScriptService
{
    private readonly static string LocalPath = "/app/data";
    private readonly DatariskContext context;
    private readonly RabbitConnection rabbitConnection;

    public ScriptService(DatariskContext context)
    {
        this.context = context;
        this.rabbitConnection = new RabbitConnection();
    }

    public async Task<BaseResponse> SubmitScript(NewScriptRequest model)
    {
        var s = await context.Scripts.FirstOrDefaultAsync(e => e.Name == model.ScriptName);

        if (s != null && s.Accepted == null)
            return new BaseResponseError
            {
                Errors = ["Este arquivo está sob análise, espere terminar para enviar novamente."]
            };

        if (s != null && !model.Overwrite)
            return new BaseResponseError
            {
                Errors = ["Script com o mesmo nome já existe, torne o campo 'Overwrite' verdadeiro para sobrescrever."]
            };

        Directory.CreateDirectory(LocalPath);
        s ??= new SQLRepository.Models.Script
        {
            Id = 0,
            DateAdd = DateTime.UtcNow,
            ScriptName = Path.GetRandomFileName() + DateTime.UtcNow.ToString("yyyyMMdd") + ".js",
            Name = model.ScriptName
        };

        s.Accepted = null;
        s.Errors = null;

        await File.WriteAllTextAsync(Path.Combine(LocalPath, s.ScriptName), model.Arquivo);

        if (s.Id != 0)
            context.Scripts.Update(s);
        else
            await context.Scripts.AddAsync(s);

        await context.SaveChangesAsync();

        await rabbitConnection.SendScript(new QueueRepository.Model.ScriptModel
        {
            FileName = s.ScriptName,
            Id = s.Id,
        });

        return new BaseResponse
        {
            Information = $"Checar a rota Script/Status/{s.Name}"
        };
    }

    public async Task<BaseResponse<StatusScriptResponse>> Status(string name)
    {
        var ob = await context.Scripts.FirstOrDefaultAsync(e => e.Name == name);
        if (ob == null)
            return new BaseResponseError<StatusScriptResponse>
            {
                Errors = ["Nome de arquivo não encontrado."]
            };

        return new BaseResponse<StatusScriptResponse>
        {
            Data = new StatusScriptResponse
            {
                Errors = string.IsNullOrEmpty(ob.Errors) ? null : ob.Errors?.Split("\r\n").ToList(),
                Name = name,
                Status = ob.Accepted switch
                {
                    true => "Aceito",
                    false => "Recusado",
                    _ => "Em análise"
                }
            }
        };
    }

    public async Task<BaseResponse> UpdateStatus(UpdateStatusScriptRequest model)
    {
        var ob = await context.Scripts.FirstOrDefaultAsync(e => e.Id == model.Id);
        if (ob == null)
            return new BaseResponseError { Errors = ["Id não encontrado."] };
        
        ob.Accepted = model.Accept;
        ob.Errors = string.Join("\r\n", model.Errors ?? [""]);

        context.Scripts.Update(ob);

        await context.SaveChangesAsync();

        return new BaseResponse();
    }

    public async Task<byte[]?> GetScript(string name)
    {
        var ob = await context.Scripts.FirstOrDefaultAsync(e => e.Name == name);
        if (ob == null)
            return null;

        return await File.ReadAllBytesAsync(Path.Combine(LocalPath, ob.ScriptName));
    }

    public async Task<BaseResponse<List<ListaScriptsResponse>>> ListScripts()
    {
        var obs = context.Scripts.Where(e => e.Accepted == true).Select(e => new ListaScriptsResponse
        {
            Name = e.Name,
            Status = e.Accepted == null ? "Em análise" : ((bool)e.Accepted ? "Aceito" : "Recusado")
        });
        return new BaseResponse<List<ListaScriptsResponse>>
        {
            Data = await obs.ToListAsync()
        };
    }
}
