using Akka.Actor;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using ServerEngine.Core.Services.Interfaces;

namespace Server_Realm.Actors.Post
{
    using Framework.Core.Database.DAO;
    using GameCore.Datas.Items;
    using GameCore.Datas.Post;
    using Server_Realm.Core.Utilities;
    using Server_Realm.Domain;
    using Server_Realm.Domain.Play.Datas;

    /// <summary>
    /// PostPushActor.
    /// </summary>
    public class PostPushActor : ReceiveActor
    {
        private readonly ILogger<PostPushActor> _logger;

        private readonly IDataSerializer _dataSerializer;
        private readonly IMemoryPoolService _memoryPoolService;
        private readonly PlayDomainProvider _playDomainProvider;

        private readonly DBPost _dbPost;

        public PostPushActor()
        {
            var serviceProvider = ServiceUtility.GetServiceProvider();

            _logger = serviceProvider.GetRequiredService<ILogger<PostPushActor>>();

            _dataSerializer = serviceProvider.GetRequiredService<IDataSerializer>();
            _memoryPoolService = serviceProvider.GetRequiredService<IMemoryPoolService>();
            _playDomainProvider = serviceProvider.GetRequiredService<PlayDomainProvider>();

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
        }

        /// <summary>
        /// Flush post.
        /// </summary>
        public async Task FlushAsync()
        {
            var dboPosts = await _dbPost.SelectLimitAsync();
            var domainMsg = new PlayDomainMsg_PostPush();

            // Read posts.
            foreach (var post in dboPosts)
            {
                // Post 메시지 생성
                var dtoItems = _dataSerializer.Deserialize<List<DTO_Item>>(post.Items);

                // Pid 확인.
                if (domainMsg.Posts.ContainsKey(post.Pid))
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

            // Tell.
            _playDomainProvider.Tell<PlayDomainMsg_PostPush>(domainMsg);
        }
    }
}
