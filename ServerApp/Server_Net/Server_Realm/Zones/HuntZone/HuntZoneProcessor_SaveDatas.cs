
namespace Server_Realm.Zones.HuntZone
{
    using Framework.Core.Database.Actors.Msgs;

    /// <summary>
    /// HuntZoneProcessor.
    /// </summary>
    public partial class HuntZoneProcessor
    {
        /// <summary>
        /// Save datas.
        /// </summary>
        public void SaveDatas()
        {
            var saveInfoMsg = _memoryPoolService.Rent<DBActorMsg_SaveInfo>();

            foreach (var pair in _worldGroup.DataObjectDict)
            {
                saveInfoMsg.SaveDatas.Add(pair.Key, pair.Value);
            }

            _dataInfoSaveProvider.Tell(saveInfoMsg);
        }
    }
}
