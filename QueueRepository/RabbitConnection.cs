using Newtonsoft.Json;
using QueueRepository.Model;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QueueRepository;
public class RabbitConnection
{
    private readonly ConnectionFactory cnnFactory;

    public RabbitConnection()
    {
        cnnFactory = new ConnectionFactory { HostName = "rabbitmq", Port = 5672 };
    }

    public async Task SendExecute(ExecuteModel model)
    {
        var cnn = await cnnFactory.CreateConnectionAsync();
        var channel = await cnn.CreateChannelAsync();
        await channel.BasicPublishAsync(
            exchange: "datarisk",
            routingKey: "execute",
            mandatory: false,
            body: Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(model)));
        await channel.CloseAsync();
        await cnn.CloseAsync();
    }

    public async Task SendScript(ScriptModel model)
    {
        var cnn = await cnnFactory.CreateConnectionAsync();
        var channel = await cnn.CreateChannelAsync();
        await channel.BasicPublishAsync(
            exchange: "datarisk",
            routingKey: "script",
            mandatory: false,
            body: Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(model)));
        await channel.CloseAsync();
        await cnn.CloseAsync();
    }

    public async Task<ScriptModel?> ReceiveScript()
    {
        var cnn = await cnnFactory.CreateConnectionAsync();
        var channel = await cnn.CreateChannelAsync();

        var t = await channel.BasicGetAsync(
            queue: "datarisk_queue_script",
            autoAck: true);

        ScriptModel? r = null;

        if (t != null && !t.Redelivered)
            r = JsonConvert.DeserializeObject<ScriptModel>(Encoding.UTF8.GetString(t.Body.ToArray()));

        await channel.CloseAsync();
        await cnn.CloseAsync();

        return r;
    }

    public async Task<ExecuteModel?> ReceiveExecute()
    {
        var cnn = await cnnFactory.CreateConnectionAsync();
        var channel = await cnn.CreateChannelAsync();

        var t = await channel.BasicGetAsync(
            queue: "datarisk_queue_execute",
            autoAck: true);

        ExecuteModel? r = null;

        if (t != null && !t.Redelivered)
            r = JsonConvert.DeserializeObject<ExecuteModel>(Encoding.UTF8.GetString(t.Body.ToArray()));

        await channel.CloseAsync();
        await cnn.CloseAsync();

        return r;
    }

    public async Task CheckRabbit()
    {
        var cnn = await cnnFactory.CreateConnectionAsync();
        var channel = await cnn.CreateChannelAsync();
        await channel.ExchangeDeclareAsync(
            exchange: "datarisk",
            type: ExchangeType.Direct,
            durable: true,
            autoDelete: false,
            arguments: null,
            noWait: false);

        await channel.QueueDeclareAsync(
            queue: "datarisk_queue_execute",
            durable: true,
            exclusive: false,
            autoDelete: false,
            arguments: null,
            noWait: false);

        await channel.QueueBindAsync(
            queue: "datarisk_queue_execute",
            exchange: "datarisk",
            routingKey: "execute",
            arguments: null,
            noWait: false);

        await channel.QueueDeclareAsync(
            queue: "datarisk_queue_script",
            durable: true,
            exclusive: false,
            autoDelete: false,
            arguments: null,
            noWait: false);

        await channel.QueueBindAsync(
            queue: "datarisk_queue_script",
            exchange: "datarisk",
            routingKey: "script",
            arguments: null,
            noWait: false);

        await channel.CloseAsync();
        await cnn.CloseAsync();
    }
}
