
namespace Server_Realm.Worlds.Msg
{
    using GameCore.Datas.Post;
    using Server_Realm.Core.Types;
    using System.Security.Cryptography.X509Certificates;

    /// <summary>
    /// WorldMsg_Post.
    /// </summary>
    public class WorldMsg_Post : WorldMsgBase
    {
        public WorldMsg_Post()
            : base(Type_WorldMsg.Post)
        {

        }

        public List<DTO_Post> Posts { get; set; }
    }
}
