
namespace Server_World.Core.Datas.GameEvents
{
    using Server_World.Core.Types;
    using Server_World.Core.Worlds;

    /// <summary>
    /// MoveSectionGroup.
    /// </summary>
    public class MoveSectionGroup : GameEvent
    {
        public MoveSectionGroup()
            : base (Type_GameEvent.MoveSectionGroup)
        {

        }

        /// <summary>
        /// World_Player.
        /// </summary>
        public World_Player WorldPlayer { get; set; }
    }
}
