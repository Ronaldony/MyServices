using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Server_Realm.Context.UnitHandler
{
    using GameCore;
    using GameCore.Datas.Plays;
    using GameCore.Interfaces;
    using Server_Realm.Context.Units;

    /// <summary>
    /// UnitEquipmentHandler.
    /// </summary>
    public class UnitEquipmentHandler : UnitHandlerBase
    {
        private readonly ILogger<UnitEquipmentHandler> _logger;

        // Services.
        private readonly ITDBService _tdbService;

        // Unit.
        private Unit _owner;

        // Datas.
        private DTO_Equipment _equipment;
        private TB_Weapon _nowTBWeapon;

        public int GetDamage() => _nowTBWeapon.Damage;
        public Type_Collision GetWeaponCollisionType() => _nowTBWeapon.CollisionType;
        public Type_Projectile GetProjectileType() => _nowTBWeapon.ProjectileType;

        public UnitEquipmentHandler(IServiceProvider serviceProvider)
        {
            _logger = serviceProvider.GetRequiredService<ILogger<UnitEquipmentHandler>>();

            _tdbService = serviceProvider.GetRequiredService<ITDBService>();
        }

        /// <summary>
        /// 기본 상태 초기화.
        /// </summary>
        public void Setup(Unit owner, DTO_Equipment equipment)
        {
            _owner = owner;
            _equipment = equipment;

            // Switch weapon.
            SwitchWeapon(_equipment.Weapon);
        }

        /// <summary>
        /// Siwtch weapon.
        /// </summary>
        public bool SwitchWeapon(int weaponId)
        {
            var tbWeapon = _tdbService.GetById<TB_Weapon>(weaponId);

            if (null == tbWeapon)
            {
                // 교체 무기 없음.
                return false;
            }

            // 무기 갱신.
            _equipment.Weapon = tbWeapon.Id;
            _nowTBWeapon = tbWeapon;

            return true;
        }

        /// <summary>
        /// Dispose.
        /// </summary>
        public override void Dispose()
        {
        }
    }
}
