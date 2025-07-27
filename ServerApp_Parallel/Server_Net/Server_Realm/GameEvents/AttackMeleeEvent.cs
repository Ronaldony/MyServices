using System.Numerics;

namespace Server_Realm.GameEvents
{
    using Server_Realm.Core.Types;
    using Server_Realm.Entities;

    /// <summary>
    /// AttackMeleeEvent.
    /// 이벤트 처리.
    /// 1) 현재 구역 적 유닛과의 충돌 처리.
    /// </summary>
    public class AttackMeleeEvent : GameEvent
    {
        public AttackMeleeEvent()
            : base(Type_GameEvent.AttackMelee)
        {
        }

        /// <summary>
        /// PlayerUnit.
        /// </summary>
        public PlayerUnit PlayerUnit { get; set; }

        /// <summary>
        /// Angle.
        /// </summary>
        public Vector2 Angle { get; set; }
    }
}
