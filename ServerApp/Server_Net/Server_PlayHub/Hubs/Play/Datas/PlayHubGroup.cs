namespace Server_PlayHub.Hubs.Play.Datas
{
    /// <summary>
    /// PlayHubGroup.
    /// </summary>
    public class PlayHubGroup
    {
        public PlayHubGroup()
        {
            DataSets = new Dictionary<string, PlayHubDataSet>();
        }

        /// <summary>
        /// HubDataSet.
        /// key: pid.
        /// value: PlayDomainData.
        /// </summary>
        public Dictionary<string, PlayHubDataSet> DataSets { get; set; }
    }
}
