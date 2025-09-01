using Akka.Actor;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using ServerEngine.Core.Services.Interfaces;
using System.Threading.Tasks.Dataflow;

namespace Server_Realm.Hubs.Play
{
    using Framework.Core.Database.DAO;
    using GameCore.HubPacket;
    using Server_Realm.Core.Utilities;
    using Server_Realm.Hubs;
    using Server_Realm.Hubs.Msg;
    using Server_Realm.Hubs.Play.Datas;
    using Server_Realm.Server.Hub;
    using Server_Realm.Zones;

    /// <summary>
    /// PlayHub.
    /// </summary>
    public partial class PlayHub : ReceiveActor
    {
        private readonly ILogger<PlayHub> _logger;

        // Services.
        private readonly IDataSerializer _dataSerializer;
        private readonly IMemoryPoolService _memoryPoolService;

        // DB object.
        private readonly DBPostInfo _dbPostInfo;

        // Server.
        private readonly HubServer _hubServer;
        private readonly PlayHubProvider _hubProvider;

        // 도메인 그룹.
        public PlayHubGroup _hubGroup;

        // Zones.
        public ZoneRouter _zoneRouter;

        // Cancel token.
        private CancellationTokenSource _cts;

        public PlayHub()
        {
            var serviceProvider = ServiceUtility.GetServiceProvider();

            _dataSerializer = serviceProvider.GetRequiredService<IDataSerializer>();
            _memoryPoolService = serviceProvider.GetRequiredService<IMemoryPoolService>();

            _dbPostInfo = serviceProvider.GetRequiredService<DBPostInfo>();

            _hubServer = serviceProvider.GetRequiredService<HubServer>();
            _hubProvider = serviceProvider.GetRequiredService<PlayHubProvider>();

            _hubGroup = Initialize_HubGroup();

            _zoneRouter = serviceProvider.GetRequiredService<ZoneRouter>();

            _cts = new CancellationTokenSource();

            RegisterReceive();
        }

        /// <summary>
        /// Initialize domain group.
        /// </summary>
        private PlayHubGroup Initialize_HubGroup()
        {
            var domainGroup = new PlayHubGroup();

            return domainGroup;
        }

        /// <summary>
        /// Receive 등록.
        /// </summary>
        private void RegisterReceive()
        {
            Receive<HubMsg_Enter>(msg => DispatchDomainMsg(msg));
            Receive<HubMsg_Leave>(msg => DispatchDomainMsg(msg));
            Receive<HubMsg_PushPost>(msg => DispatchDomainMsg(msg));
            Receive<HubMsg_RetrievePost>(msg => DispatchDomainMsg(msg));
            Receive<HubMsg_DeletePost>(msg => DispatchDomainMsg(msg));
            Receive<HubMsg_FailRetrievePost>(msg => DispatchDomainMsg(msg));
        }

        /// <summary>
        /// Dispatch domain message.
        /// </summary>
        private void DispatchDomainMsg(HubMsgBase msg)
        {
            switch (msg)
            {
                case HubMsg_PushPost:    // 우편물 푸시.
                    var postPush = msg as HubMsg_PushPost;

                    foreach (var pair in postPush.Posts)
                    {
                        var pid = pair.Key;

                        // Domain 그룹에 Pid 정보 추가.
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
                    // Domain 그룹에 Pid 정보 추가.
                    if (false == _hubGroup.DataSets.ContainsKey(msg.TargetPid))
                    {
                        // DomainSet에 로드가 되어있지 않은 상황.
                        AddToHubGroup(msg.TargetPid);
                    }

                    // 메시지 전달.
                    var targetDomainData = _hubGroup.DataSets[msg.TargetPid];
                    targetDomainData.HubMsgPipeline.Post((targetDomainData, msg));
                    break;
            }
        }

        /// <summary>
        /// Process domain.
        /// </summary>
        private async Task ProcessDomainAsync((PlayHubDataSet domainData, HubMsgBase msg) tuple)
        {
            switch (tuple.msg)
            {
                case HubMsg_Enter:
                    await EnterAsync(tuple.domainData, tuple.msg as HubMsg_Enter);
                    break;
                case HubMsg_Leave:
                    Leave(tuple.domainData, tuple.msg as HubMsg_Leave);
                    break;
                case HubMsg_PushPost:
                    await PushPostAsync(tuple.domainData, tuple.msg as HubMsg_PushPost);
                    break;
                case HubMsg_RetrievePost:
                    RetrievePost(tuple.domainData, tuple.msg as HubMsg_RetrievePost);
                    break;
                case HubMsg_DeletePost:
                    DeletePost(tuple.domainData, tuple.msg as HubMsg_DeletePost);
                    break;
                case HubMsg_FailRetrievePost:
                    FailRetrievePost(tuple.domainData, tuple.msg as HubMsg_FailRetrievePost);
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
                    action: async tuple => await ProcessDomainAsync(tuple)
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
