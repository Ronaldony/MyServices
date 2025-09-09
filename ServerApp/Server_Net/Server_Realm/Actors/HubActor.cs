using Akka.Actor;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using ServerEngine.Core.Services.Interfaces;
using ServerEngine.MessageBus.RabbitMQs;

namespace Server_Realm.Actors
{
    using Framework.Config.ConfigManager;
    using Framework.Config.MessageBus;
    using Framework.Core.Hubs;
    using Framework.Core.Types;
    using Framework.Core.Utilities;
    using Server_Realm.Worlds.Msg;
    using Server_Realm.Zones.Providers;

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
        private readonly HuntZoneProcessorProvider _huntZoneProcessorProvider;

        // MessageBus.
        private RBMQChannel _playhubChannel;
        private RBMQChannel _huntZoneChannel;

        // Playhubs.
        private readonly Config_MessageBusData _playHubData;

        public HubActor()
        {
            var serviceProvider = ServiceUtility.GetServiceProvider();

            _logger = serviceProvider.GetRequiredService<ILogger<HubActor>>();

            _serviceProvider = serviceProvider;
            _dataSerializer = serviceProvider.GetRequiredService<IDataSerializer>();

            _hubActorProvider = serviceProvider.GetRequiredService<HubActorProvider>();
            _huntZoneProcessorProvider = serviceProvider.GetRequiredService<HuntZoneProcessorProvider>();

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

            ReceiveAsync<PlayHubMsg_FailClaimPost>(async msg => await FailClaimPost(msg));

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
            await InitializeHuntZoneAsync(rbmqConnection);

            _logger.LogInformation($"HubActor initialize.");
        }

        /// <summary>
        /// Initialize PlayHub.
        /// </summary>
        private async Task InitializePlayHubAsync(RBMQConnection connection)
        {
            // PlayHub 채널 생성.
            _playhubChannel = await connection.CreateChannelAsync();
        }

        /// <summary>
        /// Initialize HuntZone.
        /// </summary>
        private async Task InitializeHuntZoneAsync(RBMQConnection connection)
        {
            // HubZone 채널 생성.
            _huntZoneChannel = await connection.CreateChannelAsync();

            // Queue 생성.
            var huntZoneQueue = await _huntZoneChannel.CreateQueueAsync(Global.HuntZoneId);

            await _huntZoneChannel.BindQueueAsync(huntZoneQueue, _playHubData.Router, huntZoneQueue);
            await _huntZoneChannel.RegisterConsumerAsync(huntZoneQueue, ConsumePlayHubMsg);

            await AddQueueToHubAsync(huntZoneQueue);
        }

        /// <summary>
        /// Consume play hub.
        /// </summary>
        private void ConsumePlayHubMsg(byte[] bytes)
        {
            var msg = _dataSerializer.Deserialize<PlayHubMsg>(bytes);

            switch (msg.MsgType)
            {
                case Type_PlayHubMsg.ClaimPost:
                    var claimPost = _dataSerializer.Deserialize<PlayHubMsg_ClaimPost>(msg.Data);

                    _huntZoneProcessorProvider.Tell<WorldMsg_ClaimPost>(new WorldMsg_ClaimPost
                    {
                        Pid = claimPost.Pid,
                        Post = claimPost.Post
                    });
                    break;
            }
        }

        /// <summary>
        /// 우편물 수령 실패.
        /// </summary>
        private async Task FailClaimPost(PlayHubMsg_FailClaimPost msg)
        {
            var msgData = _dataSerializer.Serialize(msg);

            await _playhubChannel.PublishAsync(_playHubData.Router, _playHubData.Queue,
                new PlayHubMsg
                {
                    MsgType = Type_PlayHubMsg.FailClaimPost,
                    Data = msgData
                });
        }

        /// <summary>
        /// PlayHub에 Queue 추가 알림.
        /// </summary>
        private async Task AddQueueToHubAsync(string queue)
        {
            var addQueueMsg = new PlayHubMsg_AddQueue
            {
                Queue = queue
            };
            var msgData = _dataSerializer.Serialize(addQueueMsg);

            await _playhubChannel.PublishAsync(_playHubData.Router, _playHubData.Queue, 
                new PlayHubMsg
                {
                    MsgType = Type_PlayHubMsg.AddQueue,
                    Data = msgData
                });
        }
    }
}
