using Microsoft.Extensions.DependencyInjection;

namespace Server_Realm.Entities
{
    using GameCore.Datas.Play;
    using GameCore.Interfaces;
    using Server_Realm.Collisions;
    using Server_Realm.Core.Services;
    using Server_Realm.Core.Types;
    using Server_Realm.UnitHandler;
    using Server_Realm.Worlds;

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
            IsActive = true;
            UnitType = Type_Unit.Player;
            Position = playInfo.Location.Pos;
            Section = SectionId.GetSection(this.Position);
            UnitCollision = collision;

            // handler.
            CombatHandler = new UnitCombatHandler(serviceProvider, this, playInfo.Stat);
            MovementHandler = new UnitMovementHandler(serviceProvider, this, playInfo.Location, playInfo.Stat);
            StatHandler = new UnitStatHandler(serviceProvider, this, playInfo.Stat);
            EquipmentHandler = new UnitEquipmentHandler(serviceProvider, this, playInfo.Equipment);

            // Player.
            Pid = playInfo.Pid;
            PlayInfo = playInfo;

            // Services.
            _tdbService = serviceProvider.GetRequiredService<ITDBService>();
            _collisionService = serviceProvider.GetRequiredService<CollisionService>();
            _projectileService = serviceProvider.GetRequiredService<ProjectileService>();
        }
        #endregion

        #region props
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
        #endregion

        #region field
        // Services.
        private readonly ITDBService _tdbService;
        private readonly CollisionService _collisionService;
        private readonly ProjectileService _projectileService;
        #endregion

        /// <summary>
        /// Update.
        /// </summary>
        public override void Update(double deltaTime)
        {
            StatHandler.Update(deltaTime);
            CombatHandler.Update(deltaTime);
            MovementHandler.Update(deltaTime);
        }

        /// <summary>
        /// Dispose.
        /// </summary>
        public override void Dispose()
        {
            throw new NotImplementedException();
        }
    }
}
