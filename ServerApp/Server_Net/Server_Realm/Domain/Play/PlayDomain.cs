using Akka.Actor;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using ServerEngine.Core.Services.Interfaces;
using System.Threading.Tasks.Dataflow;

namespace Server_Realm.Domain.Play
{
    using Framework.Core.Database.DAO;
    using Server_Realm.Core.Utilities;
    using Server_Realm.Domain.Play.Datas;
    using Server_Realm.Server.HuntZone;

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
        private readonly HuntZoneServer _huntZoneServer;

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
                case PlayDomainMsg_PostPush:
                    var postPush = msg as PlayDomainMsg_PostPush;

                    foreach (var pair in postPush.Posts)
                    {
                        // Domain 그룹에 Pid 정보 추가.
                        if (false == _domainGroup.DomainDatas.ContainsKey(pair.Key))
                        {
                            AddPlayerToDomainGroup(pair.Key);
                        }

                        // Domain 메시지 전달.
                        var domainData = _domainGroup.DomainDatas[pair.Key];

                        domainData.DomainMsgPipeline.Post((domainData, msg));
                    }
                    break;
            }
        }

        /// <summary>
        /// Process domain.
        /// </summary>
        private async Task ProcessDomainAsync((PlayDomainData domainData, PlayDomainMsg msg) tuple)
        {
            switch (tuple.msg)
            {
                case PlayDomainMsg_PostPush:
                    await PostPushAsync(tuple.domainData, tuple.msg as PlayDomainMsg_PostPush);
                    break;
            }
        }

        /// <summary>
        /// Add player to domain group.
        /// </summary>
        private void AddPlayerToDomainGroup(string pid)
        {
            _domainGroup.DomainDatas.Add(pid, new PlayDomainData
            {
                Pid = pid,
                DomainMsgPipeline = new ActionBlock<(PlayDomainData, PlayDomainMsg)>
                (
                    action: async tuple => await ProcessDomainAsync(tuple)
                )
            });
        }

        public void Tell<T>(T msg)
        {

        }
    }
}
