namespace Server_Realm.Entities
{
    using Framework.Core.Utilities;
    using GameCore.Datas.Inventory;
    using GameCore.Datas.Items;
    using GameCore.Datas.Play;
    using Server_Realm.Context.UnitHandler;
    using Server_Realm.Context.Units;
    using Server_Realm.Core.Types;

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

            // Drop items.
            MobDropItems = new Dictionary<long, DTO_Item>();
        }

        #region setup
        public void Setup(long unitId, DTO_PlayInfo playInfo, DTO_InventoryInfo inventoryInfo)
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

            // Drop items.
            MobDropItems.Clear();

            // Player.
            Pid = playInfo.Pid;
            PlayInfo = playInfo;
            InventoryInfo = inventoryInfo;
        }
        #endregion

        /// <summary>
        /// Pid.
        /// </summary>
        public string Pid { get; private set; }

        /// <summary>
        /// DTO_PlayInfo.
        /// </summary>
        public DTO_PlayInfo PlayInfo { get; private set; }

        /// <summary>
        /// DTO_InventoryInfo.
        /// </summary>
        public DTO_InventoryInfo InventoryInfo { get; private set; }

        /// <summary>
        /// UnitEquipmentHandler.
        /// </summary>
        public UnitEquipmentHandler EquipmentHandler { get; private set; }

        /// <summary>
        /// Mob drop items.
        /// key: Drop id.
        /// value: DTO_Item.
        /// </summary>
        public Dictionary<long, DTO_Item> MobDropItems { get; private set; }

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
