
namespace Server_Realm.Hubs.Play
{
    using GameCore;
    using GameCore.HubPacket;
    using Server_Realm.Hubs.Msg;
    using Server_Realm.Hubs.Play.Datas;

    /// <summary>
    /// PlayHub.
    /// </summary>
    public partial class PlayHub
    {
        /// <summary>
        /// Enter
        /// </summary>
        private async Task EnterAsync(PlayHubDataSet dataSet, HubMsg_Enter msg)
        {
            var pid = dataSet.Pid;

            dataSet.IsEnter = true;
            dataSet.PostInfo = await _dbPostInfo.SelectAsync(pid);

            // 우편물 정보 전송.
            SendPacket<HubPacketSC_Enter>(pid, HubPacketCode.SC_Enter, new HubPacketSC_Enter
            {
                PostInfo = dataSet.PostInfo,
            });
        }

        /// <summary>
        /// Leave
        /// </summary>
        private void Leave(PlayHubDataSet dataSet, HubMsg_Leave msg)
        {
            _hubGroup.DataSets.Remove(dataSet.Pid);
        }
    }
}
