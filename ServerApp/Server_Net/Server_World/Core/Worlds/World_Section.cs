
namespace Server_World.Core.Worlds
{
    using GameCore.Datas.Enemy;
    using GameCore.Datas.Play;

    /// <summary>
    /// World_Section.
    /// </summary>
    public class World_Section
    {
        public World_Section()
        {
            PlayInfos = new List<DTO_PlayInfo>();
            Enemies = new List<DTO_Enemy>();
        }

        /// <summary>
        /// Player.
        /// </summary>
        public List<DTO_PlayInfo> PlayInfos { get; set; }

        /// <summary>
        /// Enemy. 
        /// </summary>
        public List<DTO_Enemy> Enemies { get; set; }
    }
}
