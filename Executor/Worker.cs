using Newtonsoft.Json;
using QueueRepository;
using System.Text;

namespace Executor;

public class Worker : BackgroundService
{
    private readonly ILogger<Worker> logger;
    private readonly RabbitConnection rc;
    List<Task<long>> _listE = new List<Task<long>>();
    List<Task<long>> _listS = new List<Task<long>>();

    public Worker(ILogger<Worker> logger)
    {
        rc = new RabbitConnection();
        this.logger = logger;
    }

    public override async Task StartAsync(CancellationToken cancellationToken)
    {
        await rc.CheckRabbit();
        await base.StartAsync(cancellationToken);
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {        
        while (!stoppingToken.IsCancellationRequested)
        {
            await CheckScript();
            await CheckExecutor();
            await Task.Delay(200, stoppingToken);
        }
    }

    private async Task CheckScript()
    {
        for (int i = 0; i < _listS.Count; i++)
        {
            if (!_listS[i].IsCompleted)
                continue;

            logger.LogInformation($"Analise {_listS[i].Result} finalizada.");

            _listS.RemoveAt(i);
        }

        if (_listS.Count == 20)
            return;

        var model = await rc.ReceiveScript();
        if (model == null)
            return;

        logger.LogInformation($"Iniciando analise de script: {model.Id}.");

        _listS.Add(Checker.CheckScript(model));
    }

    private async Task CheckExecutor()
    {
        for (int i = 0; i < _listE.Count; i++)
        {
            if (!_listE[i].IsCompleted)
                continue;

            logger.LogInformation($"Processamento {_listE[i].Result} finalizado.");

            _listE.RemoveAt(i);
        }

        if (_listE.Count == 20)
            return;

        var model = await rc.ReceiveExecute();
        if (model == null)
            return;

        logger.LogInformation($"Iniciando o processamento de dados: {model.Id}.");

        _listE.Add(Execute.ExecuteJS(model.Id, model.ScriptName, model.Data.ToArray(), model.Function));
    }
}
