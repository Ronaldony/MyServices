
namespace Server_PlayHub.Domain.Play.Datas
{
    /// <summary>
    /// PlayDomainGroup.
    /// </summary>
    public class PlayDomainGroup
    {
        public PlayDomainGroup()
        {
            DomainDataSets = new Dictionary<string, PlayDomainDataSet>();
        }

        /// <summary>
        /// DomainDatas.
        /// key: pid.
        /// value: PlayDomainData.
        /// </summary>
        public Dictionary<string, PlayDomainDataSet> DomainDataSets { get; set; }
    }
}
