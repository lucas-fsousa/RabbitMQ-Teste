using PublicUtility.Extension;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace RabbitMQ.Consumer.Workers;

public class FanoutMessageWorker: BaseWorker {
  public FanoutMessageWorker(ILogger<FanoutMessageWorker> logger, IConfiguration configuration) : base(logger, configuration) { }

  private const string EXCHANGE = "logs";

  protected override Task ExecuteAsync(CancellationToken stoppingToken) {
    var channel = _factory.CreateConnection().CreateModel();

    channel.QueueDeclare(queue: "text", durable: false, exclusive: false, autoDelete: false, arguments: null);
    channel.QueueBind("text", EXCHANGE, "");

    channel.QueueDeclare(queue: "extern", durable: false, exclusive: false, autoDelete: false, arguments: null);
    channel.QueueBind("extern", EXCHANGE, "");


    var consumer = new EventingBasicConsumer(channel);
    var consumer2 = new EventingBasicConsumer(channel);

   consumer.Received += async (model, ea) => {
      try {
        var message = ea.Body.ToArray().AsString();
        _log.LogInformation($"C1 -> New message from {ea.Exchange} -> {message}");

        channel.BasicAck(ea.DeliveryTag, false);
      } catch {
        channel.BasicNack(ea.DeliveryTag, false, true);
      }
     await Task.Delay(1500);
   };

    consumer2.Received += async (model, ea) => {
      try {
        var message = ea.Body.ToArray().AsString();
        _log.LogInformation($"C2 -> New message from {ea.Exchange} -> {message}");

        channel.BasicAck(ea.DeliveryTag, false);
      } catch {
        channel.BasicNack(ea.DeliveryTag, false, true);
      }
      await Task.Delay(1500);
    };

    channel.BasicConsume(queue: "text", autoAck: false, consumer: consumer);
    channel.BasicConsume(queue: "extern", autoAck: false, consumer: consumer2);
    return Task.CompletedTask;
  }
}
