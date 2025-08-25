using Akka.Actor;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Server_Realm.Actors.Post
{
    using Framework.Core.Database.DAO;
    using Framework.Core.Database.DBO;
    using GameCore.Datas.Items;
    using GameCore.Datas.Post;
    using Microsoft.AspNetCore.Authentication;
    using Server_Realm.Core.ActorProviders;
    using Server_Realm.Core.Utilities;
    using ServerEngine.Core.Services.Interfaces;

    /// <summary>
    /// PostPushActor.
    /// </summary>
    public class PostPushActor : ReceiveActor
    {
        private readonly ILogger<PostPushActor> _logger;

        private readonly IDataSerializer _dataSerializer;

        private readonly HuntZoneProcessorProvider _huntZoneProcessorProvider;

        private readonly DBPost _dbPost;

        public PostPushActor()
        {
            var serviceProvider = ServiceUtility.GetServiceProvider();

            _logger = serviceProvider.GetRequiredService<ILogger<PostPushActor>>();

            _dataSerializer = serviceProvider.GetRequiredService<IDataSerializer>();
            _huntZoneProcessorProvider = serviceProvider.GetRequiredService<HuntZoneProcessorProvider>();

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
            var dtoPostDict = new Dictionary<string, List<DTO_Post>>();

            // Read posts.
            foreach (var post in dboPosts)
            {
                // Post 메시지 생성
                var dtoItems = _dataSerializer.Deserialize<List<DTO_Item>>(post.Items);

                // Pid 확인.
                if (dtoPostDict.ContainsKey(post.Pid))
                {
                    dtoPostDict.Add(post.Pid, new List<DTO_Post>());
                }

                // Add Post.
                dtoPostDict[post.Pid].Add(new DTO_Post
                {
                    PostUid = post.Uid,
                    Items = dtoItems,
                });
            }

        }
    }
}
