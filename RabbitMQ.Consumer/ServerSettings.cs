namespace RabbitMQ.Consumer;
public class ServerSettings {
  public string HostName { get; set; } = null!;

  public int Port { get; set; }

  public string UserName { get; set; } = null!;

  public string Password { get; set; } = null!;

  public string VirtualHost { get; set; } = null!;
}
