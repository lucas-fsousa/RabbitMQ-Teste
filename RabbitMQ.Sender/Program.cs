using RabbitMQ.Sender;

await Collection.SendClassic();
await Collection.SendDirect();
await Collection.SendTopic();
await Collection.SendFanout();

Console.ReadKey();