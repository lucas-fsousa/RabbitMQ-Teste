using RabbitMQ.Client;

namespace RabbitMQ.Consumer.Workers {
  public class BaseWorker: BackgroundService {
    protected readonly ILogger<BaseWorker> _log;
    protected readonly ConnectionFactory _factory;
    
    protected BaseWorker(ILogger<BaseWorker> logger, IConfiguration configuration) {
      var serverInfos = configuration.GetSection("ServerSettings").Get<ServerSettings>()!;
      _factory = new ConnectionFactory {
        HostName = serverInfos.HostName,
        Port = serverInfos.Port,
        UserName = serverInfos.UserName,
        Password = serverInfos.Password,
        VirtualHost = serverInfos.VirtualHost
      };

      _log = logger;
    }

    protected override Task ExecuteAsync(CancellationToken stoppingToken) => throw new NotImplementedException();
  }
}
