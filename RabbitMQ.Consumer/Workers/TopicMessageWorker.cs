using PublicUtility.Extension;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace RabbitMQ.Consumer.Workers;

public class TopicMessageWorker: BaseWorker {
  public TopicMessageWorker(ILogger<TopicMessageWorker> logger, IConfiguration configuration) : base(logger, configuration) { }

  protected override Task ExecuteAsync(CancellationToken stoppingToken) {
    var fila1 = "email";
    var fila2 = "sms";
    var fila3 = "external";

    var channel = _factory.CreateConnection().CreateModel();
    channel.BasicQos(0, 5, false);

    channel.QueueDeclare(queue: fila1, durable: false, exclusive: false, autoDelete: false, arguments: null);
    channel.QueueDeclare(queue: fila2, durable: false, exclusive: false, autoDelete: false, arguments: null);
    channel.QueueDeclare(queue: fila3, durable: false, exclusive: false, autoDelete: false, arguments: null);

    var consumer = new EventingBasicConsumer(channel);
    var consumer2 = new EventingBasicConsumer(channel);
    var consumer3 = new EventingBasicConsumer(channel);

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

    consumer3.Received += async (model, ea) => {
      try {
        var message = ea.Body.ToArray().AsString();
        _log.LogInformation($"C3 -> New message from {ea.Exchange} -> {message}");

        channel.BasicAck(ea.DeliveryTag, false);
      } catch {
        channel.BasicNack(ea.DeliveryTag, false, true);
      }
      await Task.Delay(1500);
    };

    channel.BasicConsume(queue: fila1, autoAck: false, consumer: consumer);
    channel.BasicConsume(queue: fila2, autoAck: false, consumer: consumer2);
    channel.BasicConsume(queue: fila3, autoAck: false, consumer: consumer3);

    return Task.CompletedTask;
  }
}
