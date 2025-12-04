using System.Text;
using System.Text.Json;
using Companies.Application.Abstractions.Messaging;
using Microsoft.Extensions.Configuration;
using RabbitMQ.Client;

namespace Companies.Infrastructure.Services.Messaging;

public class MessageBroker : IMessageBroker
{
    private readonly ConnectionFactory _connectionFactory;
    private readonly string _queueName = "import-companies-queue";

    public MessageBroker(IConfiguration configuration)
    {
        _connectionFactory = new ConnectionFactory
        {
            HostName = configuration["RabbitMQ:Host"] ??
                throw new InvalidOperationException("RabbitMQ:Host configuration is required"),
            UserName = configuration["RabbitMQ:Username"] ??
                throw new InvalidOperationException("RabbitMQ:Username configuration is required"),
            Password = configuration["RabbitMQ:Password"] ??
                throw new InvalidOperationException("RabbitMQ:Password configuration is required")
        };
    }

    public async Task PostMessageAsync<TMessage>(TMessage message)
    {
        await using var connection = await _connectionFactory.CreateConnectionAsync();
        await using var channel = await connection.CreateChannelAsync();

        await channel.QueueDeclareAsync(
            queue: _queueName,
            durable: false,
            exclusive: false,
            autoDelete: false,
            arguments: null
        );

        var messageAsString = JsonSerializer.Serialize(message);
        var messageInBytes = Encoding.UTF8.GetBytes(messageAsString);

        await channel.BasicPublishAsync(
            exchange: "",
            routingKey: _queueName,
            body: messageInBytes
        );
    }
}
