namespace Server_World.Core.Datas.GameEvents
{
    using GameCore.NetPacket;
    using Server_World.Core.Collision;
    using Server_World.Core.Types;

    /// <summary>
    /// AttackEvent.
    /// 이벤트 처리.
    /// 1) 현재 구역 적 유닛과의 충돌 처리.
    /// </summary>
    public class AttackEvent : GameEvent
    {
        public AttackEvent()
            : base(Type_GameEvent.Attack)
        {
        }

        /// <summary>
        /// NetPacket.
        /// </summary>
        public NetPacketSC_Attack NetPacket { get; set; }

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
