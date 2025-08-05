using Microsoft.Extensions.DependencyInjection;

namespace Server_Realm.Entities
{
    using GameCore.Datas.Play;
    using Server_Realm.Core.Factories;
    using Server_Realm.Core.Types;
    using Server_Realm.Core.Utilities;
    using Server_Realm.UnitHandler;

    /// <summary>
    /// PlayerUnit.
    /// </summary>
    public partial class PlayerUnit : Unit
    {
        public PlayerUnit()
        {
            var serviceProvider = ServiceUtility.GetServiceProvider();

            // Unit.
            UnitType = Type_Unit.Player;

            // handler.
            CombatHandler = new UnitCombatHandler(serviceProvider);
            MovementHandler = new UnitMovementHandler(serviceProvider);
            StatHandler = new UnitStatHandler(serviceProvider);
            EquipmentHandler = new UnitEquipmentHandler(serviceProvider);
        }

        #region setup
        public void Setup(long unitId, DTO_PlayInfo playInfo)
        {
            // Unit.
            IsDead = false;
            UnitId = unitId;
            UnitType = Type_Unit.Player;

            // Setup handler.
            CombatHandler.Setup(this);
            MovementHandler.Setup(this, playInfo.Location, playInfo.Stat);
            StatHandler.Setup(this, playInfo.Stat);
            EquipmentHandler.Setup(this, playInfo.Equipment);

            // Player.
            Pid = playInfo.Pid;
            PlayInfo = playInfo;
        }
        #endregion

        /// <summary>
        /// Pid.
        /// </summary>
        public string Pid { get; private set; }

        /// <summary>
        /// PlayInfo.
        /// </summary>
        public DTO_PlayInfo PlayInfo { get; private set; }

        /// <summary>
        /// UnitEquipmentHandler.
        /// </summary>
        public UnitEquipmentHandler EquipmentHandler { get; private set; }

        /// <summary>
        /// Dispose.
        /// </summary>
        public override void Dispose()
        {
            // Dispose.
            CombatHandler.Dispose();
        }
    }
}
