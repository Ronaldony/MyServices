using Akka.Actor;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using ServerEngine.Core.Services.Interfaces;
using System.Reactive.Linq;

namespace Server_Realm.Actors.Post
{
    using Framework.Core.Database.DAO;
    using GameCore.Datas.Items;
    using GameCore.Datas.Post;
    using Server_Realm.Core.Utilities;
    using Server_Realm.Hubs;
    using Server_Realm.Hubs.Msg;

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

        private readonly IDisposable _disposable;

        public PushPostActor()
        {
            var serviceProvider = ServiceUtility.GetServiceProvider();

            _logger = serviceProvider.GetRequiredService<ILogger<PushPostActor>>();

            _dataSerializer = serviceProvider.GetRequiredService<IDataSerializer>();
            _memoryPoolService = serviceProvider.GetRequiredService<IMemoryPoolService>();

            _pushPostActorProvider = serviceProvider.GetRequiredService<PushPostActorProvider>();
            _playHubProvider = serviceProvider.GetRequiredService<PlayHubProvider>();

            _dbPost = serviceProvider.GetRequiredService<DBPost>();

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

            // Schedules.
            Observable
            .Interval(TimeSpan.FromSeconds(1))      // 1초 마다
            .Subscribe(d =>                         // 메시지 전달.
            {
                _pushPostActorProvider.Tell("flush");
            });
        }

        /// <summary>
        /// Flush post.
        /// </summary>
        public async Task FlushAsync()
        {
            var dboPosts = await _dbPost.SelectLimitAsync();
            var pushPost = new HubMsg_PushPost();

            // Read posts.
            foreach (var post in dboPosts)
            {
                // Post 메시지 생성
                var dtoItems = _dataSerializer.Deserialize<List<DTO_Item>>(post.Items);

                // Pid 확인.
                if (false == pushPost.Posts.ContainsKey(post.Pid))
                {
                    pushPost.Posts.Add(post.Pid, new List<DTO_Post>());
                }

                // Add Post.
                pushPost.Posts[post.Pid].Add(new DTO_Post
                {
                    PostUid = post.Uid,
                    Items = dtoItems,
                });
            }

            // Domain에 메시지 전달.
            _playHubProvider.Tell<HubMsg_PushPost>(pushPost);
        }
    }
}
