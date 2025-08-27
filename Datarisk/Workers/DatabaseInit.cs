
using SQLRepository.Models;
using System.Diagnostics;

namespace Datarisk.Workers;

public class DatabaseInit : IHostedService
{
    private readonly IServiceProvider _serviceProvider;

    public DatabaseInit(IServiceProvider _serviceProvider)
    {
        this._serviceProvider = _serviceProvider;
    }

    public async Task StartAsync(CancellationToken cancellationToken)
    {
        using (var scope = _serviceProvider.CreateScope())
        {
            var context = scope.ServiceProvider.GetRequiredService<DatariskContext>();

            try
            {
                await context.Database.EnsureCreatedAsync(cancellationToken);
                // OU, para migrações: await context.Database.MigrateAsync(cancellationToken);
            }
            catch (Exception ex)
            {
                var logger = scope.ServiceProvider.GetRequiredService<ILogger<Program>>();
                logger.LogError(ex, "Ocorreu um erro ao verificar o banco de dados.");
                // Considere re-lançar a exceção ou parar a aplicação se o banco de dados for crítico
                throw;
            }
        }
    }

    public Task StopAsync(CancellationToken cancellationToken) => Task.CompletedTask;
}
