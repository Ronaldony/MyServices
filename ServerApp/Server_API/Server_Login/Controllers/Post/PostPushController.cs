
using Microsoft.AspNetCore.Mvc;
using ServerEngine.Core.Services.Interfaces;

namespace Server_Login.Controllers.Post
{
    using Framework.Core.Database.DAO;
    using GameCore.Datas.Items;

    public class PostPushController : Controller
    {
        private readonly DBPost _dbPost;
        private readonly IUniqueIdService _uniqueIdService;
        private readonly IDataSerializer _dataSerializer;

        public PostPushController(
            DBPost dbPost, 
            IUniqueIdService uniqueIdService,
            IDataSerializer dataSerializer)
        {
            _dbPost = dbPost;
            _uniqueIdService = uniqueIdService;
            _dataSerializer = dataSerializer;
        }

        [HttpGet]
        [Route("post/push")]
        public async Task<string> Process(string pid, string text, int itemId, int count)
        {
            var postUid = _uniqueIdService.GenerateId();
            var dtoItems = new List<DTO_Item>();

            dtoItems.Add(new DTO_Item
            {
                RefId = itemId,
                Count = count,
            });

            // create post.
            var isCreated = await _dbPost.Create(postUid, pid, _dataSerializer.Serialize(dtoItems), text);

            if (isCreated) return "create post";
            else return "fail create post";
        }
    }
}
;