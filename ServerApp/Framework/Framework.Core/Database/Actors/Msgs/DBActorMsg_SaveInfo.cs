
namespace Framework.Core.Database.Actors.Msgs
{
    using Framework.Core.Database.Interfaces;

    /// <summary>
    /// DBActorMsg_SaveInfo.
    /// </summary>
    public class DBActorMsg_SaveInfo : IDisposable
    {
        public DBActorMsg_SaveInfo()
        {
            SaveDatas = new Dictionary<string, List<IDataObject>>();
        }

        /// <summary>
        /// Save datas.
        /// key: Pid.
        /// value: IDataObject.
        /// </summary>
        public Dictionary<string, List<IDataObject>> SaveDatas { get; set; }

        /// <summary>
        /// Dispose.
        /// </summary>
        public void Dispose()
        {
            SaveDatas.Clear();
        }
    }
}
