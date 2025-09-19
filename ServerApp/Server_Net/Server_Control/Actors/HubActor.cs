using Akka.Actor;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using ServerEngine.Core.Services.Interfaces;
using ServerEngine.MessageBus.RabbitMQs;

namespace Server_Control.Actors
{
    using Framework.Config.ConfigManager;
    using Framework.Config.MessageBus;
    using Framework.Core.Database.Actors;
    using Framework.Core.Hubs;
    using Framework.Core.Types;
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

        public HubActor(IServiceProvider serviceProvider)
        {
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
            ReceiveAsync<ControlMsg_ClaimPost>(msg => PublishMsg<ControlMsg_ClaimPost>(msg.ZoneId, msg));

            // Initialize.
            _hubActorProvider.Tell("initialize");
        }

        /// <summary>
        /// Props.
        /// </summary>
        public static Props Props(IServiceProvider serviceProvider)
        {
            return Akka.Actor.Props.Create(() => new DataInfoSaveActor(serviceProvider));
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
            var msg = _dataSerializer.Deserialize<ControlMsg>(bytes);

            switch (msg.MsgType)
            {
                case Type_ControlMsg.AddQueue:
                    var addQueue = _dataSerializer.Deserialize<ControlMsg_AddQueue>(msg.Data);

                    // Add queue.
                    await AddChannelAsync(addQueue.Queue);
                    break;
                case Type_ControlMsg.ClaimPostFail:     
                    var failClaimPost = _dataSerializer.Deserialize<ControlMsg_ClaimPostFail>(msg.Data);

                    // 우편물 수령 실패.
                    _playHubProvider.Tell<HubMsg_ClaimPostFail>(new HubMsg_ClaimPostFail
                    {
                        PostUid = failClaimPost.PostUid,
                        TargetPid = failClaimPost.Pid,
                        FailCount = failClaimPost.FailCount,
                    });
                    break;
                case Type_ControlMsg.ClaimPostComplete:
                    var completeClaimPost = _dataSerializer.Deserialize<ControlMsg_ClaimPostComplete>(msg.Data);

                    // 우편물 수령 완료.
                    _playHubProvider.Tell<HubMsg_ClaimPostComplete>(new HubMsg_ClaimPostComplete
                    {
                        PostUid = completeClaimPost.PostUid,
                        TargetPid = completeClaimPost.Pid,
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
