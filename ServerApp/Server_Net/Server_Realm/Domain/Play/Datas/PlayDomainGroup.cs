
namespace Server_Realm.Domain.Play.Datas
{
    using Server_Realm.Domain.Play.Datas;

    public class PlayDomainGroup
    {
        public PlayDomainGroup()
        {
            DomainDatas = new Dictionary<string, PlayDomainData>();
        }

        /// <summary>
        /// DomainDatas.
        /// key: pid.
        /// value: PlayDomainData.
        /// </summary>
        public Dictionary<string, PlayDomainData> DomainDatas { get; set; }
    }
}
