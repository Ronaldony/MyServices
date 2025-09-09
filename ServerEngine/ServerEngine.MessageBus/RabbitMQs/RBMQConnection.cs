
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using RabbitMQ.Client;
using System.Collections.Concurrent;

namespace ServerEngine.MessageBus.RabbitMQs
{
    /// <summary>
    /// RBMQConnection.
    /// RabbitMQ.
    /// </summary>
    public class RBMQConnection
    {
        private readonly ILogger<RBMQConnection> _logger;

        private readonly IServiceProvider _serviceProvider;

        private IConnection _connection;

        private ConcurrentQueue<RBMQChannel> _channels;

        public RBMQConnection(IServiceProvider serviceProvider)
        {
            _logger = serviceProvider.GetRequiredService<ILogger<RBMQConnection>>();

            _serviceProvider = serviceProvider;

            _channels = new ConcurrentQueue<RBMQChannel>();
        }

        /// <summary>
        /// Initialize.
        /// </summary>
        public async Task InitializeAsync(string host, string user, string password, CancellationToken ct)
        {
            var factory = new ConnectionFactory 
            { 
                HostName = host,
                UserName = user,
                Password = password
            };

            _connection = await factory.CreateConnectionAsync(ct);
        }

        /// <summary>
        /// Create channel.
        /// </summary>
        public async Task<RBMQChannel> CreateChannelAsync()
        {
            var channel = await _connection.CreateChannelAsync();
            var rbmqChannel = new RBMQChannel(_serviceProvider, channel);

            // Enqueue channel.
            _channels.Enqueue(rbmqChannel);

            return rbmqChannel;
        }
    }
}
