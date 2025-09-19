using Framework.Core.Database.Interfaces;
using System.Collections.Concurrent;

namespace Server_Control.Hubs.Play.Datas
{
    /// <summary>
    /// PlayHubGroup.
    /// </summary>
    public class PlayHubGroup
    {
        public PlayHubGroup()
        {
            DataSets = new Dictionary<string, PlayHubDataSet>();
            DataObjectDict = new ConcurrentDictionary<string, List<IDataObject>>();
        }

        /// <summary>
        /// HubDataSet.
        /// key: pid.
        /// value: PlayDomainData.
        /// </summary>
        public Dictionary<string, PlayHubDataSet> DataSets { get; set; }

        /// <summary>
        /// DataObject dictionary.
        /// key: Pid.
        /// value: IDataObject.
        /// </summary>
        public ConcurrentDictionary<string, List<IDataObject>> DataObjectDict;
    }
}
