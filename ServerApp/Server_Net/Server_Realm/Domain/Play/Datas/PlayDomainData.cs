using System.Threading.Tasks.Dataflow;

namespace Server_Realm.Domain.Play.Datas
{
    using GameCore.Datas.Player;

    /// <summary>
    /// PlayDomainData.
    /// </summary>
    public class PlayDomainData
    {
        public PlayDomainData()
        {
        }

        /// <summary>
        /// Pid.
        /// </summary>
        public string Pid { get; set; }

        /// <summary>
        /// PostInfo.
        /// </summary>
        public DTO_PostInfo PostInfo { get; set; }

        /// <summary>
        /// 메시지 파이프라인.
        /// </summary>
        public ActionBlock<(PlayDomainData, PlayDomainMsg)> DomainMsgPipeline { get; set; }
    }
}
