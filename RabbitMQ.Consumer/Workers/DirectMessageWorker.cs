using PublicUtility.Extension;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace RabbitMQ.Consumer.Workers;

public class DirectMessageWorker: BaseWorker {
  public DirectMessageWorker(ILogger<DirectMessageWorker> logger, IConfiguration configuration) : base(logger, configuration) { }

  private const string EXCHANGE = "image";

  protected override Task ExecuteAsync(CancellationToken stoppingToken) {
    var fila1 = "imageProcess";
    var fila2 = "imageArchive";
    var routingKey1 = "crop";
    var routingKey2 = "resize";

    var channel = _factory.CreateConnection().CreateModel();
    channel.BasicQos(0, 5, false);

    channel.QueueDeclare(queue: fila1, durable: false, exclusive: false, autoDelete: false, arguments: null);
    channel.QueueBind(fila1, EXCHANGE, routingKey1);

    channel.QueueDeclare(queue: fila2, durable: false, exclusive: false, autoDelete: false, arguments: null);
    channel.QueueBind(fila2, EXCHANGE, routingKey2);


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

    channel.BasicConsume(queue: fila1, autoAck: false, consumer: consumer);
    channel.BasicConsume(queue: fila2, autoAck: false, consumer: consumer2);
    return Task.CompletedTask;
  }
}
