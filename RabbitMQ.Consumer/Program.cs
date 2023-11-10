using RabbitMQ.Consumer.Workers;

var host = Host.CreateDefaultBuilder(args);
var build = host.ConfigureServices(s => {
  s.AddHostedService<ClassicMessageWorker>();
  s.AddHostedService<FanoutMessageWorker>();
  s.AddHostedService<DirectMessageWorker>();
  s.AddHostedService<TopicMessageWorker>();
}).Build();

build.Run();
