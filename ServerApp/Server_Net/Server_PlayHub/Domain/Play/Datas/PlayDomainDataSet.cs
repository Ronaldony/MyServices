using System.Threading.Tasks.Dataflow;

namespace Server_PlayHub.Domain.Play.Datas
{
    using GameCore.Datas.Player;
    using Server_PlayHub.Domain.Play.Msg;

    /// <summary>
    /// PlayDomainDataSet.
    /// </summary>
    public class PlayDomainDataSet
    {
        public PlayDomainDataSet()
        {
        }

        /// <summary>
        /// Pid.
        /// </summary>
        public string Pid { get; set; }

        /// <summary>
        /// Domain 접속 여부.
        /// </summary>
        public bool IsEnter { get; set; }

        /// <summary>
        /// PostInfo.
        /// </summary>
        public DTO_PostInfo PostInfo { get; set; }

        /// <summary>
        /// 메시지 파이프라인.
        /// </summary>
        public ActionBlock<(PlayDomainDataSet, PlayDomainMsg)> DomainMsgPipeline { get; set; }
    }
}
