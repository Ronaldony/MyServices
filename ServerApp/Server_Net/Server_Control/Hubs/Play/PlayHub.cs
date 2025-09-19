using Akka.Actor;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using ServerEngine.Core.Services.Interfaces;
using System.Threading.Tasks.Dataflow;

namespace Server_Control.Hubs.Play
{
    using Framework.Core.Database.DAO;
    using GameCore.HubPacket;
    using Server_Control.Actors;
    using Server_Control.Hubs;
    using Server_Control.Hubs.Msg;
    using Server_Control.Hubs.Play.Datas;
    using Server_Control.Server.Hub;

    /// <summary>
    /// PlayHub.
    /// </summary>
    public partial class PlayHub : ReceiveActor
    {
        private readonly ILogger<PlayHub> _logger;

        // Services.
        private readonly IDataSerializer _dataSerializer;

        // DB object.
        private readonly DBPostInfo _dbPostInfo;
        private readonly DBPost _dbPost;

        // Server.
        private readonly HubServer _hubServer;

        // Providers.
        private readonly PlayHubProvider _hubProvider;
        private readonly HubActorProvider _hubActorProvider;

        // 도메인 그룹.
        public PlayHubGroup _hubGroup;

        public PlayHub(IServiceProvider serviceProvider)
        {
            _logger = serviceProvider.GetRequiredService<ILogger<PlayHub>>();

            _dataSerializer = serviceProvider.GetRequiredService<IDataSerializer>();

            _dbPostInfo = serviceProvider.GetRequiredService<DBPostInfo>();
            _dbPost = serviceProvider.GetRequiredService<DBPost>();

            // Servers.
            _hubServer = serviceProvider.GetRequiredService<HubServer>();

            // Providers.
            _hubProvider = serviceProvider.GetRequiredService<PlayHubProvider>();
            _hubActorProvider = serviceProvider.GetRequiredService<HubActorProvider>();

            _hubGroup = new PlayHubGroup();

            RegisterReceive();
        }

        /// <summary>
        /// Props.
        /// </summary>
        public static Props Props(IServiceProvider serviceProvider)
        {
            return Akka.Actor.Props.Create(() => new PlayHub(serviceProvider));
        }

        /// <summary>
        /// Receive 등록.
        /// </summary>
        private void RegisterReceive()
        {
            // Hubmsg.
            Receive<HubMsg_Enter>(msg => DispatchHubMsg(msg));
            Receive<HubMsg_EnterZone>(msg => DispatchHubMsg(msg));
            Receive<HubMsg_Leave>(msg => DispatchHubMsg(msg));
            Receive<HubMsg_PushPost>(msg => DispatchHubMsg(msg));
            Receive<HubMsg_ClaimPost>(msg => DispatchHubMsg(msg));
            Receive<HubMsg_ClaimPostComplete>(msg => DispatchHubMsg(msg));
            Receive<HubMsg_ClaimPostFail>(msg => DispatchHubMsg(msg));
            Receive<HubMsg_DeletePost>(msg => DispatchHubMsg(msg));
        }

        /// <summary>
        /// Dispatch hub message.
        /// </summary>
        private void DispatchHubMsg(HubMsgBase msg)
        {
            switch (msg)
            {
                case HubMsg_PushPost:    // 우편물 푸시.
                    var postPush = msg as HubMsg_PushPost;

                    foreach (var pair in postPush.Posts)
                    {
                        var pid = pair.Key;

                        // Hub 그룹에 Pid 정보 추가.
                        if (false == _hubGroup.DataSets.ContainsKey(pid))
                        {
                            AddToHubGroup(pid);
                        }

                        // 메시지 전달.
                        var domainData = _hubGroup.DataSets[pid];
                        domainData.HubMsgPipeline.Post((domainData, msg));
                    }
                    break;
                default:
                    // Hub 그룹에 Pid 정보 추가.
                    if (false == _hubGroup.DataSets.ContainsKey(msg.TargetPid))
                    {
                        AddToHubGroup(msg.TargetPid);
                    }

                    // 메시지 전달.
                    var dataSet = _hubGroup.DataSets[msg.TargetPid];
                    dataSet.HubMsgPipeline.Post((dataSet, msg));
                    break;
            }
        }

        /// <summary>
        /// Process domain.
        /// </summary>
        private async Task ProcessAsync((PlayHubDataSet dataSet, HubMsgBase msg) tuple)
        {
            switch (tuple.msg)
            {
                case HubMsg_Enter:
                    await EnterAsync(tuple.dataSet, tuple.msg as HubMsg_Enter);
                    break;
                case HubMsg_Leave:
                    Leave(tuple.dataSet, tuple.msg as HubMsg_Leave);
                    break;
                case HubMsg_EnterZone:
                    EnterZone(tuple.dataSet, tuple.msg as HubMsg_EnterZone);
                    break;
                case HubMsg_PushPost:
                    await PushPostAsync(tuple.dataSet, tuple.msg as HubMsg_PushPost);
                    break;
                case HubMsg_ClaimPost:
                    await ClaimPostAsync(tuple.dataSet, tuple.msg as HubMsg_ClaimPost);
                    break;
                case HubMsg_ClaimPostFail:
                    ClaimPostFail(tuple.dataSet, tuple.msg as HubMsg_ClaimPostFail);
                    break;
                case HubMsg_ClaimPostComplete:
                    ClaimPostComplete(tuple.dataSet, tuple.msg as HubMsg_ClaimPostComplete);
                    break;
                case HubMsg_DeletePost:
                    DeletePost(tuple.dataSet, tuple.msg as HubMsg_DeletePost);
                    break;
            }
        }

        /// <summary>
        /// Add player to domain group.
        /// </summary>
        private void AddToHubGroup(string pid)
        {
            _hubGroup.DataSets.Add(pid, new PlayHubDataSet
            {
                Pid = pid,
                HubMsgPipeline = new ActionBlock<(PlayHubDataSet, HubMsgBase)>
                (
                    action: async tuple => await ProcessAsync(tuple)
                )
            });
        }

        /// <summary>
        /// Send packet.
        /// </summary>
        public bool SendPacket<T>(string pid, int packetCode, T packet)
            where T : HubPacketBase_SC
        {
            var packetBytes = _dataSerializer.Serialize(packet);

            // Send.
            return _hubServer.Send(pid, packetCode, packetBytes);
        }

        /// <summary>
        /// Tell.
        /// </summary>
        public void Tell<T>(T msg)
        {
            _hubProvider.Tell(msg);
        }
    }
}
