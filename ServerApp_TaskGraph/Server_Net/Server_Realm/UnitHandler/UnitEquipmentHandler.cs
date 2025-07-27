using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Server_Realm.UnitHandler
{
    using GameCore;
    using GameCore.Datas.Plays;
    using GameCore.Interfaces;
    using Server_Realm.Entities;

    /// <summary>
    /// UnitEquipmentHandler.
    /// </summary>
    public class UnitEquipmentHandler   
    {
        private readonly ILogger<UnitEquipmentHandler> _logger;

        // Services.
        private readonly ITDBService _tdbService;

        // Unit.
        private readonly Unit _owner;

        // DTO.
        private readonly DTO_Equipment _equipment;

        private TB_Weapon _nowTBWeapon;

        #region common
        public Type_Collision GetWeaponCollisionType() => _nowTBWeapon.CollisionType;
        public int GetDamage() => _nowTBWeapon.Damage;
        public Type_Projectile GetProjectileType() => _nowTBWeapon.ProjectileType;
        #endregion

        public UnitEquipmentHandler(IServiceProvider serviceProvider, Unit owner, DTO_Equipment equipment)
        {
            _logger = serviceProvider.GetRequiredService<ILogger<UnitEquipmentHandler>>();

            _tdbService = serviceProvider.GetRequiredService<ITDBService>();

            _owner = owner;
            _equipment = equipment;
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
    }
}
