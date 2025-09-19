namespace Server_Control.Hubs.Play
{
    using Framework.Core.Database.Actors.Msgs;

    /// <summary>
    /// PlayHub.
    /// </summary>
    public partial class PlayHub
    {
        /// <summary>
        /// Save datas.
        /// </summary>
        public void SaveDatas()
        {
            var saveInfoMsg = _memoryPoolService.Rent<DBActorMsg_SaveInfo>();

            foreach (var pair in _hubGroup.DataObjectDict)
            {
                saveInfoMsg.SaveDatas.Add(pair.Key, pair.Value);
            }

            _dataInfoSaveProvider.Tell(saveInfoMsg);
        }
    }
}
