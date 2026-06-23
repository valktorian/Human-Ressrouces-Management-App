namespace Infrastructure.Api.Messaging;

public interface IKafkaConsumer
{
    Task StartAsync(CancellationToken cancellationToken);

    Task StopAsync(CancellationToken cancellationToken);
}
