
namespace Server_World.Core.Datas.GameEvents
{
    using Server_World.Core.Types;
    using Server_World.Core.Worlds;

    /// <summary>
    /// MoveDistrictEvent.
    /// </summary>
    public class MoveDistrictEvent : GameEvent
    {
        public MoveDistrictEvent()
            : base (Type_GameEvent.MoveDistrict)
        {
            WorldPlayers = new List<World_Player>();
        }

        /// <summary>
        /// Section.
        /// </summary>
        public SectionId Section { get; set; }

        /// <summary>
        /// World_Player.
        /// </summary>
        public List<World_Player> WorldPlayers { get; set; }
    }
}
