using System.Numerics;

namespace Server_Realm.GameEvents
{
    using GameCore;
    using GameCore.GamePacket;
    using Server_Realm.Core.Types;

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
        /// GamePacket.
        /// </summary>
        public GamePacketSC_Attack GamePacket { get; set; }

        /// <summary>
        /// Damage.
        /// </summary>
        public int Damage { get; set; }

        /// <summary>
        /// 공격 시간.
        /// </summary>
        public DateTime AttackTime { get; set; }

        /// <summary>
        /// 공격 각도.
        /// </summary>
        public Vector2 Angle { get; set; }

        /// <summary>
        /// 공격자 위치.
        /// </summary>
        public Vector2 AttackerPos { get; set; }

        /// <summary>
        /// 무기 충돌 유형.
        /// </summary>
        public Type_Collision WeaponCollisionType { get; set; }
    }
}
