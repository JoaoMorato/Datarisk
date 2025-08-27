using Datarisk.Services;
using Datarisk.Workers;
using Microsoft.EntityFrameworkCore;
using QueueRepository;
using SQLRepository.Models;
using System.Reflection;

internal class Program
{
    private static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        // Add services to the container.

        builder.Services.AddControllers();
        // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddSwaggerGen(op =>
        {
            var xmlFilename = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
            op.IncludeXmlComments(Path.Combine(AppContext.BaseDirectory, xmlFilename));
        });

        builder.Services.AddTransient<IScriptService, ScriptService>();
        builder.Services.AddTransient<IExecuteService, ExecuteService>();

        builder.Configuration.AddJsonFile(Path.Combine(AppContext.BaseDirectory, "appsettings.json"));

        builder.Services.AddDbContext<DatariskContext>((e) =>
        {
            var cnn = builder.Configuration.GetConnectionString("PostgreSQL");
            e.UseNpgsql(cnn);
        });

        builder.Services.AddHostedService<DatabaseInit>();

        var app = builder.Build();

        new RabbitConnection().CheckRabbit().Wait();

        // Configure the HTTP request pipeline.
        app.UseSwagger();
        app.UseSwaggerUI();

        app.UseHttpsRedirection();

        app.UseAuthorization();

        app.MapControllers();

        app.Run();
    }
}