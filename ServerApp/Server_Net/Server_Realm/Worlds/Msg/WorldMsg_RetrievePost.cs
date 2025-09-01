
namespace Server_Realm.Worlds.Msg
{
    using GameCore.Datas.Post;
    using Server_Realm.Core.Types;

    /// <summary>
    /// WorldMsg_RetrievePost.
    /// </summary>
    public class WorldMsg_RetrievePost : WorldMsgBase
    {
        public WorldMsg_RetrievePost()
            : base(Type_WorldMsg.RetrievePost)
        {

        }

        /// <summary>
        /// Pid.
        /// </summary>
        public string Pid { get; set; }

        /// <summary>
        /// Post.
        /// </summary>
        public DTO_Post Post { get; set; }

        /// <summary>
        /// 수령 시도 횟수.
        /// Zone에 유저가 존재하지 않은 경우, 최대 3번까지 재시도.
        /// 수령 실패 시, 우편물 미수령으로 전환.
        /// </summary>
        public int TryCount { get; set; }
    }
}
