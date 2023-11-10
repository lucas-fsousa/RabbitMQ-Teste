using PublicUtility.Extension;
using RabbitMQ.Client;
using System.Text;

namespace RabbitMQ.Sender {
  public class Collection {
    private static IModel GetModel() {
      var factory = new ConnectionFactory { HostName = "172.29.166.52", Port = 5672, UserName = "dev", Password = "dev123", VirtualHost = "teste" };
      var connection = factory.CreateConnection();
      return connection.CreateModel();
    }

    public static async Task SendTopic() {
      using var channel = GetModel();
      var exchange = "alertas";

      channel.ExchangeDeclare(exchange, type: ExchangeType.Topic, durable: true);

      channel.QueueDeclare(queue: "email", durable: false, exclusive: false, autoDelete: false, arguments: null);
      channel.QueueBind("email", exchange, "*.web.*");

      channel.QueueDeclare(queue: "sms", durable: false, exclusive: false, autoDelete: false, arguments: null);
      channel.QueueBind("sms", exchange, "*.mobile.*");

      channel.QueueDeclare(queue: "external", durable: false, exclusive: false, autoDelete: false, arguments: null);
      channel.QueueBind("external", exchange, "gaming.#");

      for(var i = 0; i < 1000; i++) {
        string message = $"enviando mensagem de log! {DateTime.Now.Ticks:x2}";

        var rng = new Random().Next(1, 5);
        switch(rng) {
          case 1:
            channel.BasicPublish(exchange: exchange, routingKey: "payment.web.exec", basicProperties: null, body: ("payment web -> " + message).AsByteArray(Encoding.UTF8));
            break;
          case 2:
            channel.BasicPublish(exchange: exchange, routingKey: "orders.web.exec", basicProperties: null, body: ("orders web -> " + message).AsByteArray(Encoding.UTF8));
            break;
          case 3:
            channel.BasicPublish(exchange: exchange, routingKey: "gaming.desktop.exec", basicProperties: null, body: ("gaming desktop -> " + message).AsByteArray(Encoding.UTF8));
            break;
          case 4:
            channel.BasicPublish(exchange: exchange, routingKey: "gaming.mobile.exec", basicProperties: null, body: ("gaming mobile -> " + message).AsByteArray(Encoding.UTF8));
            break;
          default:
            break;
        }
        Console.WriteLine($" [x] Sent {message}");
        await Task.Delay(200);
      }
    }


    public static async Task SendFanout() {
      using var channel = GetModel();
      var exchange = "logs";

      channel.ExchangeDeclare(exchange, type: ExchangeType.Fanout, durable: true);

      channel.QueueDeclare(queue: "text", durable: false, exclusive: false, autoDelete: false, arguments: null);
      channel.QueueBind("text", exchange, "");

      channel.QueueDeclare(queue: "extern", durable: false, exclusive: false, autoDelete: false, arguments: null);
      channel.QueueBind("extern", exchange, "");

      for(var i = 0; i < 1000; i++) {
        string message = $"enviando mensagem de log! {DateTime.Now.Ticks:x2}";
        channel.BasicPublish(exchange: exchange, routingKey: "", basicProperties: null, body: message.AsByteArray(Encoding.UTF8));
        Console.WriteLine($" [x] Sent {message}");
        await Task.Delay(200);
      }
    }

    public static async Task SendClassic() {
      using var channel = GetModel();
      var routingKey = "ClassicMessage";
      string message = $"enviando mensagem de Hello World!";

      channel.QueueDeclare(queue: routingKey, durable: false, exclusive: false, autoDelete: false, arguments: null);
      channel.BasicPublish(exchange: "", routingKey: routingKey, basicProperties: null, body: message.AsByteArray(Encoding.UTF8));

      Console.WriteLine($" [x] Sent {message}");
      await Task.Delay(500);
    }

    public static async Task SendDirect() {
      using var channel = GetModel();
      var exchange = "image";
      var fila1 = "imageProcess";
      var fila2 = "imageArchive";

      var routingKey1 = "crop";
      var routingKey2 = "resize";

      channel.ExchangeDeclare(exchange, type: ExchangeType.Direct, durable: true);

      channel.QueueDeclare(queue: fila1, durable: false, exclusive: false, autoDelete: false, arguments: null);
      channel.QueueBind(fila1, exchange, routingKey1);

      channel.QueueDeclare(queue: fila2, durable: false, exclusive: false, autoDelete: false, arguments: null);
      channel.QueueBind(fila2, exchange, routingKey2);
      channel.QueueBind(fila2, exchange, routingKey1);

      for(var i = 0; i < 1000; i++) {
        string message = $"imagem {i + 1} -> {DateTime.Now.Ticks:x2}";

        channel.BasicPublish(exchange: exchange, routingKey: routingKey1, basicProperties: null, body: ("CROP -> " + message).AsByteArray());
        channel.BasicPublish(exchange: exchange, routingKey: routingKey2, basicProperties: null, body: ("REIZE -> " + message).AsByteArray());

        Console.WriteLine($"Sent {message}");
        await Task.Delay(200);
      }
    }
  }
}
