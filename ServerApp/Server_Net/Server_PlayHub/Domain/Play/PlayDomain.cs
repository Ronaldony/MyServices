using Akka.Actor;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using ServerEngine.Core.Services.Interfaces;
using System.Threading.Tasks.Dataflow;

namespace Server_PlayHub.Domain.Play
{
    using Framework.Core.Database.DAO;
    using GameCore.GamePacket;
    using Server_PlayHub.Core.Utilities;
    using Server_PlayHub.Domain.Play.Datas;
    using Server_PlayHub.Domain.Play.Msg;
    using Server_PlayHub.Server.HuntZone;

    /// <summary>
    /// PlayDomain.
    /// </summary>
    public partial class PlayDomain : ReceiveActor, IGameDomain
    {
        private readonly ILogger<PlayDomain> _logger;

        // Services.
        private readonly IDataSerializer _dataSerializer;
        private readonly IMemoryPoolService _memoryPoolService;

        // DB object.
        private readonly DBPostInfo _dbPostInfo;

        // Server.
        private readonly PlayDomainServer _domainServer;

        // 도메인 그룹.
        public PlayDomainGroup _domainGroup;

        // Cancel token.
        private CancellationTokenSource _cts;

        public PlayDomain()
        {
            var serviceProvider = ServiceUtility.GetServiceProvider();

            _memoryPoolService = serviceProvider.GetRequiredService<IMemoryPoolService>();

            _cts = new CancellationTokenSource();
        }

        /// <summary>
        /// Initialize domain group.
        /// </summary>
        private PlayDomainGroup Initialize_DomainGroup()
        {
            var domainGroup = new PlayDomainGroup();

            return domainGroup;
        }

        /// <summary>
        /// Receive 등록.
        /// </summary>
        private void RegisterReceive()
        {
            Receive<PlayDomainMsg>(msg =>
            {
                DispatchDomainMsg(msg);
            });
        }

        /// <summary>
        /// Dispatch domain message.
        /// </summary>
        private void DispatchDomainMsg(PlayDomainMsg msg)
        {
            switch (msg)
            {
                case PlayDomainMsg_PushPost:    // 우편물 푸시.
                    var postPush = msg as PlayDomainMsg_PushPost;

                    foreach (var pair in postPush.Posts)
                    {
                        var pid = pair.Key;

                        // Domain 그룹에 Pid 정보 추가.
                        if (false == _domainGroup.DomainDataSets.ContainsKey(pid))
                        {
                            AddPlayerToDomainGroup(pid);
                        }

                        // 메시지 전달.
                        var domainData = _domainGroup.DomainDataSets[pid];
                        domainData.DomainMsgPipeline.Post((domainData, msg));
                    }
                    break;
                default:
                    // Domain 그룹에 Pid 정보 추가.
                    if (false == _domainGroup.DomainDataSets.ContainsKey(msg.TargetPid))
                    {
                        // DomainSet에 로드가 되어있지 않은 상황.
                        AddPlayerToDomainGroup(msg.TargetPid);
                    }

                    // 메시지 전달.
                    var targetDomainData = _domainGroup.DomainDataSets[msg.TargetPid];
                    targetDomainData.DomainMsgPipeline.Post((targetDomainData, msg));
                    break;
            }
        }

        /// <summary>
        /// Process domain.
        /// </summary>
        private async Task ProcessDomainAsync((PlayDomainDataSet domainData, PlayDomainMsg msg) tuple)
        {
            switch (tuple.msg)
            {
                case PlayDomainMsg_PushPost:
                    await PushPostAsync(tuple.domainData, tuple.msg as PlayDomainMsg_PushPost);
                    break;
                case PlayDomainMsg_RetrievePost:
                    RetrievePost(tuple.domainData, tuple.msg as PlayDomainMsg_RetrievePost);
                    break;
            }
        }

        /// <summary>
        /// Add player to domain group.
        /// </summary>
        private void AddPlayerToDomainGroup(string pid)
        {
            _domainGroup.DomainDataSets.Add(pid, new PlayDomainDataSet
            {
                Pid = pid,
                DomainMsgPipeline = new ActionBlock<(PlayDomainDataSet, PlayDomainMsg)>
                (
                    action: async tuple => await ProcessDomainAsync(tuple)
                )
            });
        }

        /// <summary>
        /// Send packet.
        /// </summary>
        public bool SendPacket<T>(string pid, int packetCode, T packet)
            where T : DomainPacketBase_SC
        {
            var packetBytes = _dataSerializer.Serialize(packet);

            // Send.
            return _domainServer.Send(pid, packetCode, packetBytes);
        }

        /// <summary>
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="msg"></param>
        public void Tell<T>(T msg)
        {

        }
    }
}
