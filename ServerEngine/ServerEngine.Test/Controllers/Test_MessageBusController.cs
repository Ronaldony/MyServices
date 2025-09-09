using Microsoft.AspNetCore.Mvc;

namespace ServerEngine.Test.Controllers
{
    using GameCore.Datas.Player;
    using RabbitMQ.Client;
    using RabbitMQ.Client.Events;
    using ServerEngine.Core.Services.Interfaces;
    using ServerEngine.Core.Util;
    using ServerEngine.Test.Database.DataObject;
    using System.Text;

    /// <summary>
    /// Test_Controller.
    /// </summary>
    public class Test_MessageBusController : Controller
    {
        private readonly ILogger<Test_MessageBusController> _logger;
        private readonly PlayerInfoObejct _playerInfoObject;
        private readonly IDataSerializer _dataSerializer;

        public Test_MessageBusController(
            ILogger<Test_MessageBusController> logger, 
            PlayerInfoObejct playerInfoObject,
            IDataSerializer dataSerializer)
        {
            _logger = logger;
            _playerInfoObject = playerInfoObject;
            _dataSerializer = dataSerializer;
        }

        [HttpGet]
        [Route("test-messagebus")]
        public async Task<string> Process()
        {
            var factory = new ConnectionFactory { HostName = "localhost" };
            using var connection = await factory.CreateConnectionAsync();
            using var channel = await connection.CreateChannelAsync();

            await channel.QueueDeclareAsync(queue: "hello", durable: false, exclusive: false, autoDelete: false,
                arguments: null);

            Console.WriteLine(" [*] Waiting for messages.");

            var consumer = new AsyncEventingBasicConsumer(channel);
            consumer.ReceivedAsync += (model, ea) =>
            {
                var body = ea.Body.ToArray();
                var message = Encoding.UTF8.GetString(body);
                Console.WriteLine($" [x] Received {message}");
                return Task.CompletedTask;
            };

            await channel.BasicConsumeAsync("hello", autoAck: true, consumer: consumer);

            return default;
        }
    }
}
