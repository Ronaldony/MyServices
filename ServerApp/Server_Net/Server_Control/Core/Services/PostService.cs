using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using ServerEngine.Core.Services.Interfaces;

using System.Reactive.Linq;

namespace Server_Control.Core.Services
{
    using Framework.Core.Database.DAO;
    using Framework.Core.Types;
    using Framework.Core.Utilities;
    using GameCore.Datas.Items;
    using GameCore.Datas.Post;
    using Server_Control.Hubs;
    using Server_Control.Hubs.Msg;

    /// <summary>
    /// PostService.
    /// </summary>
    public class PostService : BackgroundService
    {
        private const int TASK_DELAY_MS = 1;

        private readonly ILogger<PostService> _logger;

        private readonly IDataSerializer _dataSerializer;

        // Provider.
        private readonly PlayHubProvider _playHubProvider;

        private readonly DBPost _dbPost;

        public PostService()
        {
            var serviceProvider = ServiceUtility.GetServiceProvider();

            _logger = serviceProvider.GetRequiredService<ILogger<PostService>>();

            _dataSerializer = serviceProvider.GetRequiredService<IDataSerializer>();

            _playHubProvider = serviceProvider.GetRequiredService<PlayHubProvider>();

            _dbPost = serviceProvider.GetRequiredService<DBPost>();
        }

        /// <summary>
        /// Initialize.
        /// </summary>
        public void Initialize()
        {
            _logger.LogDebug($"PostService initialize.");
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            stoppingToken.Register(() => _logger.LogError($"PostService is stopped."));

            while (!stoppingToken.IsCancellationRequested)
            {
                // 주기적으로 우편물 정보를 가져와 PlayHub에 전달.
                var dboPosts = await _dbPost.SelectPostsAsync(Type_PostStep.None);

                if (false == dboPosts.Any())
                {
                    await Task.Delay(TASK_DELAY_MS);
                    continue;
                }

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

                // Post 상태 갱신.
                await _dbPost.UpdatePostStatusAsync(dboPosts.Select(d => d.Uid), Type_PostStep.Progress);

                // PlayHub에 새로운 우편물 전달.
                _playHubProvider.Tell(pushPost);

                await Task.Delay(TASK_DELAY_MS);
            }
        }
    }
}
