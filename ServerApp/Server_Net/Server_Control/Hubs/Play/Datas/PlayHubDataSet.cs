using System.Threading.Tasks.Dataflow;

namespace Server_Control.Hubs.Play.Datas
{
    using GameCore.Datas.Player;
    using Server_Control.Hubs.Msg;

    /// <summary>
    /// PlayHubDataSet.
    /// </summary>
    public class PlayHubDataSet
    {
        public PlayHubDataSet()
        {
        }

        /// <summary>
        /// Pid.
        /// </summary>
        public string Pid { get; set; }

        /// <summary>
        /// Hub 접속 여부.
        /// </summary>
        public bool IsEnter { get; set; }

        /// <summary>
        /// Zone id.
        /// </summary>
        public string ZoneId { get; set; }

        /// <summary>
        /// PostInfo.
        /// </summary>
        public DTO_PostInfo PostInfo { get; set; }

        /// <summary>
        /// 메시지 파이프라인.
        /// </summary>
        public ActionBlock<(PlayHubDataSet, HubMsgBase)> HubMsgPipeline { get; set; }
    }
}
