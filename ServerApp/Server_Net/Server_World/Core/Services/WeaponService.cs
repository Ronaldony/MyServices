
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Server_World.Core.Services
{
    using DnsClient.Internal;
    using GameCore;
    using GameCore.Interfaces;

    /// <summary>
    /// WeaponService.
    /// </summary>
    public class WeaponService
    {
        private readonly ILogger<WeaponService> _logger;

        private readonly ITDBService _tdbService;

        private IEnumerable<TB_Weapon> _tbWeapons;

        public WeaponService(IServiceProvider serviceProvider)
        {
            _logger = serviceProvider.GetRequiredService< ILogger<WeaponService>>();
            _tdbService = serviceProvider.GetRequiredService<ITDBService>();
        }

        /// <summary>
        /// Initialize.
        /// </summary>
        public void Initialize()
        {
            _tbWeapons = _tdbService.SelectAll<TB_Weapon>();

            _logger.LogInformation("CollisionService initialized.");
        }

        /// <summary>
        /// Get weapon damage.
        /// </summary>
        public int GetWeaponDamage(int weaponId)
        {
            var tbWeapon = _tbWeapons.FirstOrDefault(d => d.Id == weaponId);

            if (tbWeapon == null)
            {
                return 0;
            }

            return tbWeapon.Damage;
        }
    }
}
