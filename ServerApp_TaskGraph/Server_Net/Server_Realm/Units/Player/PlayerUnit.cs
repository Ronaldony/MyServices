using Microsoft.Extensions.DependencyInjection;

namespace Server_Realm.Entities
{
    using GameCore.Datas.Play;
    using Server_Realm.Collisions;
    using Server_Realm.Core.Factory;
    using Server_Realm.Core.Types;
    using Server_Realm.UnitHandler;

    /// <summary>
    /// PlayerUnit.
    /// </summary>
    public partial class PlayerUnit : Unit
    {
        #region constructor
        public PlayerUnit(IServiceProvider serviceProvider, long unitId, DTO_PlayInfo playInfo, Collision collision)
        {
            // Unit.
            UnitId = unitId;
            UnitType = Type_Unit.Player;
            UnitCollision = collision;

            // handler.
            CombatHandler = new UnitCombatHandler(serviceProvider, this);
            MovementHandler = new UnitMovementHandler(serviceProvider, this, playInfo.Location, playInfo.Stat);
            StatHandler = new UnitStatHandler(serviceProvider, this, playInfo.Stat);
            EquipmentHandler = new UnitEquipmentHandler(serviceProvider, this, playInfo.Equipment);

            // Player.
            Pid = playInfo.Pid;
            PlayInfo = playInfo;

            // Factories.
            CollisionFactory = serviceProvider.GetRequiredService<CollisionFactory>();

            // Switch weapon.
            EquipmentHandler.SwitchWeapon(playInfo.Equipment.Weapon);
        }
        #endregion

        /// <summary>
        /// Pid.
        /// </summary>
        public string Pid { get; }

        /// <summary>
        /// PlayInfo.
        /// </summary>
        public DTO_PlayInfo PlayInfo { get; }

        /// <summary>
        /// UnitEquipmentHandler.
        /// </summary>
        public UnitEquipmentHandler EquipmentHandler { get; }

        /// <summary>
        /// Dispose.
        /// </summary>
        public override void Dispose()
        {
            // Release memory.
            CollisionFactory.Return(UnitCollision);
            
            // Dispose.
            CombatHandler.Dispose();
        }
    }
}
