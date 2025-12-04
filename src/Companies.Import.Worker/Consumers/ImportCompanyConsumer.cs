
using System.Text;
using System.Text.Json;
using Companies.Application.Abstractions.Handlers;
using Companies.Application.Features.Companies.Commands.CreateCompany;
using Companies.Application.Features.Companies.Models;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace Companies.Import.Worker.Consumers;

public class ImportCompanyConsumer : BackgroundService
{
    private readonly ConnectionFactory _connectionFactory;
    private readonly string _queueName = "import-companies-queue";
    private readonly ILogger<ImportCompanyConsumer> _logger;
    private readonly IServiceProvider _serviceProvider;


    public ImportCompanyConsumer(
        ILogger<ImportCompanyConsumer> logger,
        IConfiguration configuration,
        IServiceProvider serviceProvider)
    {
        _logger = logger;
        _serviceProvider = serviceProvider;

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

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        await using var connection = await _connectionFactory.CreateConnectionAsync(stoppingToken);
        await using var channel = await connection.CreateChannelAsync(cancellationToken: stoppingToken);

        await channel.QueueDeclareAsync(
            queue: _queueName,
            durable: false,
            exclusive: false,
            autoDelete: false,
            arguments: null,
            cancellationToken: stoppingToken
        );

        var consumer = new AsyncEventingBasicConsumer(channel);

        consumer.ReceivedAsync += async (sender, eventArgs) =>
        {
            var contentArray = eventArgs.Body.ToArray();
            var contentString = Encoding.UTF8.GetString(contentArray);
            var createCompany = JsonSerializer.Deserialize<CreateCompanyCommand>(contentString);

            if (createCompany is null)
                return;

            _logger.LogInformation(
                "Received message: CreateCompanyCommand with CNPJ: {cnpj}",
                createCompany.Cnpj
                );

            using (var scope = _serviceProvider.CreateScope())
            {
                var createCompanyHandler = scope.ServiceProvider
                    .GetRequiredService<ICommandHandler<CreateCompanyCommand, CompanyDetails>>();

                var result = await createCompanyHandler.HandleAsync(createCompany);

                if (result.IsFailure)
                {
                    _logger.LogWarning(
                        "Error while create company with CNPJ {cnpj}: \n {errors}",
                        createCompany.Cnpj,
                        JsonSerializer.Serialize(result.Notifications)
                        );
                }
                else
                {
                    _logger.LogInformation(
                        "Company with CNPJ {cnpj} created with success. Id: {id}",
                        createCompany.Cnpj,
                        result.Data!.Id
                        );
                }
            }

            await channel.BasicAckAsync(eventArgs.DeliveryTag, false, stoppingToken);
        };

        await channel.BasicConsumeAsync(_queueName, false, consumer, stoppingToken);

        while (!stoppingToken.IsCancellationRequested)
        {
            await Task.Delay(1000, stoppingToken);
        }
    }
}
