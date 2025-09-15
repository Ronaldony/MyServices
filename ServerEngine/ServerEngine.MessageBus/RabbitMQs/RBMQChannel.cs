using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace ServerEngine.MessageBus.RabbitMQs
{
    using ServerEngine.Core.Services.Interfaces;

    /// <summary>
    /// RBMQChannel.
    /// </summary>
    public class RBMQChannel
    {
        private readonly ILogger<RBMQChannel> _logger;

        private readonly IDataSerializer _dataSerializer;

        private IChannel _channel;

        private AsyncEventingBasicConsumer _consumer;

        public RBMQChannel(IServiceProvider serviceProvider, IChannel channel)
        {
            _logger = serviceProvider.GetRequiredService<ILogger<RBMQChannel>>();

            _dataSerializer = serviceProvider.GetRequiredService<IDataSerializer>();

            _channel = channel;
        }

        /// <summary>
        /// Create exchange.
        /// </summary>
        public async Task CreateExchangeAsync(string exchange)
        {
            await _channel.ExchangeDeclareAsync(exchange, ExchangeType.Direct, autoDelete: false);
        }

        /// <summary>
        /// Create queue.
        /// </summary>
        public async Task<string> CreateQueueAsync(string queue = null)
        {
            QueueDeclareOk queueDeclare;

            // Declare queue.
            // durable: 소비자 사망 시, 작업 손실되지 않음.
            // exclusive: 다른 채널에서도 해당 큐 접근.
            // autoDelete: 소비자가 없는 경우, 자동 삭제.
            if (string.IsNullOrEmpty(queue))
            {
                queueDeclare = await _channel.QueueDeclareAsync(durable: false, exclusive: false, autoDelete: true, arguments: null);
            }
            else
            {
                queueDeclare = await _channel.QueueDeclareAsync(queue: queue, durable: false, exclusive: false, autoDelete: true, arguments: null);
            }

            return queueDeclare.QueueName;
        }

        /// <summary>
        /// Bind queue.
        /// </summary>
        public async Task BindQueueAsync(string queue, string exchange, string key)
        {
            await _channel.QueueBindAsync(queue, exchange, key);
        }

        /// <summary>
        /// Publish
        /// </summary>
        public async Task PublishAsync<T>(string exchange, string key, T message) where T : class
        {
            // Produce.
            var body = _dataSerializer.Serialize(message);

            await _channel.BasicPublishAsync(exchange: exchange, routingKey: key, body: body, mandatory: true);
        }

        /// <summary>
        /// Register consumer.
        /// </summary>
        public async Task<bool> RegisterConsumerAsync(string queue, Action<byte[]> callback)
        {
            if (null != _consumer)
            {
                // 이미 Consumer 등록.
                return false;
            }

            // Consume.
            var consumer = new AsyncEventingBasicConsumer(_channel);

            consumer.ReceivedAsync += async (model, ea) =>
            {
                var body = ea.Body.ToArray();
                callback(body);

                // Ack.
                await _channel.BasicAckAsync(deliveryTag: ea.DeliveryTag, multiple: false);
            };

            // Consumer 등록.
            // autoAck: 수동 Ack. 소비자가 응답없이 죽어도 메시지를 잃지 않기 위함.
            await _channel.BasicConsumeAsync(queue, autoAck: false, consumer: consumer);

            _consumer = consumer;

            return true;
        }

        /// <summary>
        /// Register consumer.
        /// </summary>
        public async Task<bool> RegisterConsumerAsync(string queue, Func<byte[], Task> callback)
        {
            if (null != _consumer)
            {
                // 이미 Consumer 등록.
                return false;
            }

            // Consume.
            var consumer = new AsyncEventingBasicConsumer(_channel);

            consumer.ReceivedAsync += async (model, ea) =>
            {
                var body = ea.Body.ToArray();
                await callback(body);

                // Ack.
                await _channel.BasicAckAsync(deliveryTag: ea.DeliveryTag, multiple: false);
            };

            // Consumer 등록.
            // autoAck: 수동 Ack. 소비자가 응답없이 죽어도 메시지를 잃지 않기 위함.
            await _channel.BasicConsumeAsync(queue, autoAck: false, consumer: consumer);

            _consumer = consumer;

            return true;
        }
    }
}
