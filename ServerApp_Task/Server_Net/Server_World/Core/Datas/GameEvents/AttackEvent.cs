namespace Server_World.Core.Datas.GameEvents
{
    using GameCore.NetPacket;
    using Server_World.Core.Collision;
    using Server_World.Core.Types;

    /// <summary>
    /// AttackEvent.
    /// </summary>
    public class AttackEvent : GameEvent
    {
        public AttackEvent()
            : base(Type_GameEvent.Attack)
        {

        }

        /// <summary>
        /// Damage.
        /// </summary>
        public int Damage { get; set; }

        /// <summary>
        /// Attack time.
        /// </summary>
        public DateTime AttackTime { get; set; }

        /// <summary>
        /// Bounding.
        /// </summary>
        public BoudingBox BoudingBox { get; set; }
    }
}
