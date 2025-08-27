
namespace Server_PlayHub.Domain.Play.Datas
{
    using GameCore.Datas.Post;
    using Server_PlayHub.Domain.Play.Msg;

    /// <summary>
    /// PlayDomainMsg_RetrievePost.
    /// </summary>
    public class PlayDomainMsg_RetrievePost : PlayDomainMsg
    {
        /// <summary>
        /// 수령 우편물.
        /// </summary>
        public DTO_Post Post { get; set; }
    }
}
