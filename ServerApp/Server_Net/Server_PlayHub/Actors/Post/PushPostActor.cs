using Akka.Actor;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using ServerEngine.Core.Services.Interfaces;
using System.Reactive.Linq;

namespace Server_PlayHub.Actors.Post
{
    using Framework.Core.Database.DAO;
    using GameCore.Datas.Items;
    using GameCore.Datas.Post;
    using Server_PlayHub.Core.Utilities;
    using Server_PlayHub.Hubs;
    using Server_PlayHub.Hubs.Msg;

    /// <summary>
    /// PushPostActor.
    /// </summary>
    public class PushPostActor : ReceiveActor
    {
        private readonly ILogger<PushPostActor> _logger;

        private readonly IDataSerializer _dataSerializer;
        private readonly IMemoryPoolService _memoryPoolService;

        // Provider.
        private readonly PushPostActorProvider _pushPostActorProvider;
        private readonly PlayHubProvider _playHubProvider;

        private readonly DBPost _dbPost;

        public PushPostActor()
        {
            var serviceProvider = ServiceUtility.GetServiceProvider();

            _logger = serviceProvider.GetRequiredService<ILogger<PushPostActor>>();

            _dataSerializer = serviceProvider.GetRequiredService<IDataSerializer>();
            _memoryPoolService = serviceProvider.GetRequiredService<IMemoryPoolService>();

            _pushPostActorProvider = serviceProvider.GetRequiredService<PushPostActorProvider>();
            _playHubProvider = serviceProvider.GetRequiredService<PlayHubProvider>();

            _dbPost = serviceProvider.GetRequiredService<DBPost>();

            // Alarm.
            Observable
                .Interval(TimeSpan.FromSeconds(1))      // 1초 마다
                .Subscribe(d =>                         // 메시지 전달.
                {
                    _pushPostActorProvider.Tell("flush");
                });

            // Setup message.
            ReceiveAsync<string>(async msg =>
            {
                switch (msg)
                {
                    case "flush":
                        await FlushAsync();
                        break;
                }
            });
        }

        /// <summary>
        /// Flush post.
        /// </summary>
        public async Task FlushAsync()
        {
            var dboPosts = await _dbPost.SelectLimitAsync();
            var domainMsg = new HubMsg_PushPost();

            // Read posts.
            foreach (var post in dboPosts)
            {
                // Post 메시지 생성
                var dtoItems = _dataSerializer.Deserialize<List<DTO_Item>>(post.Items);

                // Pid 확인.
                if (false == domainMsg.Posts.ContainsKey(post.Pid))
                {
                    domainMsg.Posts.Add(post.Pid, new List<DTO_Post>());
                }

                // Add Post.
                domainMsg.Posts[post.Pid].Add(new DTO_Post
                {
                    PostUid = post.Uid,
                    Items = dtoItems,
                });
            }

            // Domain에 메시지 전달.
            _playHubProvider.Tell<HubMsg_PushPost>(domainMsg);
        }
    }
}
