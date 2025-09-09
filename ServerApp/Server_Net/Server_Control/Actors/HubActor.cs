using Akka.Actor;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using ServerEngine.Core.Services.Interfaces;
using ServerEngine.MessageBus.RabbitMQs;

namespace Server_Control.Actors
{
    using Framework.Config.ConfigManager;
    using Framework.Config.MessageBus;
    using Framework.Core.Hubs;
    using Framework.Core.Types;
    using Framework.Core.Utilities;
    using Server_Control.Hubs;
    using Server_Control.Hubs.Msg;

    /// <summary>
    /// HubActor.
    /// </summary>
    public class HubActor : ReceiveActor
    {
        private readonly ILogger<HubActor> _logger;

        private readonly IServiceProvider _serviceProvider;
        private readonly IDataSerializer _dataSerializer;

        // Providers.
        private readonly HubActorProvider _hubActorProvider;
        private readonly PlayHubProvider _playHubProvider;

        // PlayHub channel.
        private RBMQChannel _playhubChannel;

        /// <summary>
        /// Channels.
        /// key: queue name.
        /// value: RabbitMQ channel.
        /// </summary>
        private readonly Dictionary<string, RBMQChannel> _channelDict;

        // Playhubs.
        private readonly Config_MessageBusData _playHubData;

        public HubActor()
        {
            var serviceProvider = ServiceUtility.GetServiceProvider();

            _logger = serviceProvider.GetRequiredService<ILogger<HubActor>>();

            _serviceProvider = serviceProvider;
            _dataSerializer = serviceProvider.GetRequiredService<IDataSerializer>();

            _hubActorProvider = serviceProvider.GetRequiredService<HubActorProvider>();

            _channelDict = new Dictionary<string, RBMQChannel>();

            _playHubData = GameConfigManager.GetMessageBusData("PlayHub");

            // Receives.
            ReceiveAsync<string>(async msg =>
            {
                switch (msg)
                {
                    case "initialize":
                        await InitializeAsync();
                        break;
                }
            });

            // Claim post.
            ReceiveAsync<PlayHubMsg_ClaimPost>(msg => PublishMsg<PlayHubMsg_ClaimPost>(msg.ZoneId, msg));

            // initialize.
            _hubActorProvider.Tell("initialize");
        }

        /// <summary>
        /// Initialize.
        /// </summary>
        public async Task InitializeAsync()
        {
            var rbmqConnection = _serviceProvider.GetRequiredService<RBMQConnection>();

            await InitializePlayHubAsync(rbmqConnection);

            _logger.LogInformation($"HubActor initialize.");
        }

        /// <summary>
        /// Initialize PlayHub.
        /// </summary>
        private async Task InitializePlayHubAsync(RBMQConnection connection)
        {
            // PlayHub 채널 생성.
            _playhubChannel = await connection.CreateChannelAsync();

            await _playhubChannel.CreateExchangeAsync(_playHubData.Router);
            await _playhubChannel.CreateQueueAsync(_playHubData.Queue);
            await _playhubChannel.BindQueueAsync(_playHubData.Queue, _playHubData.Router, _playHubData.Queue);

            // Consume.
            await _playhubChannel.RegisterConsumerAsync(_playHubData.Queue, ConsumePlayHubMsgAsync);
        }

        /// <summary>
        /// Consume play hub.
        /// </summary>
        private async Task ConsumePlayHubMsgAsync(byte[] bytes)
        {
            var msg = _dataSerializer.Deserialize<PlayHubMsg>(bytes);

            switch (msg.MsgType)
            {
                case Type_PlayHubMsg.AddQueue:
                    var addQueue = _dataSerializer.Deserialize<PlayHubMsg_AddQueue>(msg.Data);

                    // Add queue.
                    await AddChannelAsync(addQueue.Queue);
                    break;
                case Type_PlayHubMsg.FailClaimPost:     
                    var failClaimPost = _dataSerializer.Deserialize<PlayHubMsg_FailClaimPost>(msg.Data);

                    // 우편물 수령 실패.
                    _playHubProvider.Tell<HubMsg_FailClaimPost>(new HubMsg_FailClaimPost
                    {
                        PostUid = failClaimPost.PostUid,
                        TargetPid = failClaimPost.Pid
                    });
                    break;
            }
        }

        /// <summary>
        /// Add queue.
        /// </summary>
        private async Task AddChannelAsync(string queue)
        {
            var rbmqConnection = _serviceProvider.GetRequiredService<RBMQConnection>();
            var channel = await rbmqConnection.CreateChannelAsync();

            _channelDict.Add(queue, channel);
        }

        /// <summary>
        /// Publish msg.
        /// </summary>
        private async Task PublishMsg<T>(string queue, T msg) where T : class
        {
            if (false == _channelDict.ContainsKey(queue))
            {
                return;
            }

            var channel = _channelDict[queue];
            await channel.PublishAsync(_playHubData.Router, _playHubData.Queue, msg);
        }
    }
}
