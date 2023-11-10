using PublicUtility.Extension;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace RabbitMQ.Consumer.Workers;

public class ClassicMessageWorker: BaseWorker {
  public ClassicMessageWorker(ILogger<ClassicMessageWorker> logger, IConfiguration configuration) : base(logger, configuration) { }

  private const string ROUTINGKEY = "MessageWorker";

  protected override Task ExecuteAsync(CancellationToken stoppingToken) {
    var channel = _factory.CreateConnection().CreateModel();
    channel.QueueDeclare(queue: ROUTINGKEY, exclusive: false, autoDelete: false);

    var consumer = new EventingBasicConsumer(channel);
    consumer.Received += (model, ea) => {
      try {
        var message = ea.Body.ToArray().AsString();
        _log.LogInformation($"New message from {ea.Exchange} -> {message}");

        channel.BasicAck(ea.DeliveryTag, false);
      } catch {
        channel.BasicNack(ea.DeliveryTag, false, true);
      }
    };

    channel.BasicConsume(queue: ROUTINGKEY, autoAck: false, consumer: consumer);
    return Task.CompletedTask;
  }
}
