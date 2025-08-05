
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System.Numerics;

namespace Server_Realm.Core.Services
{
    using GameCore;
    using GameCore.Datas.Mobs;
    using GameCore.Interfaces;

    /// <summary>
    /// MobCreateService.
    /// </summary>
    public class MobCreateService
    {
        private readonly ILogger<MobCreateService> _logger;

        // Services.
        private readonly IServiceProvider _serviceProvider;
        private readonly ITDBService _tdbService;

        // Datas.
        private readonly List<Type_Item> _itemTypes;
        private readonly List<CONF_Area> _confAreas;

        // Random.
        private readonly Random _rand;

        public MobCreateService(IServiceProvider serviceProvider)
        {
            _logger = serviceProvider.GetRequiredService<ILogger<MobCreateService>>();

            _serviceProvider = serviceProvider;
            _tdbService = serviceProvider.GetService<ITDBService>();

            _itemTypes = Enum.GetValues<Type_Item>().Where(d => d != Type_Item.None).ToList();
            _confAreas = _tdbService.SelectAll<CONF_Area>().ToList();

            _rand = new Random();
        }

        /// <summary>
        /// Initialize.
        /// </summary>
        public void Initialize()
        {
            _logger.LogInformation($"MobCreateService initialized.");
        }

        /// <summary>
        /// Get mob as template.
        /// </summary>
        public DTO_Mob CreateMob(int refId, Type_Area areaType)
        {
            var tbMob = _tdbService.GetById<TB_Mob>(refId);
            var confArea = _confAreas.FirstOrDefault(d => d.Area == areaType);

            // 데이터 검사.
            if (tbMob == null || confArea == null)
            {
                return null;
            }

            // 유닛 생성.
            var dtoEnemy = new DTO_Mob
            {
                MobId = refId,
            };

            // Generate random section.
            var x = _rand.Next(confArea.MinX, confArea.MaxX);
            var y = _rand.Next(confArea.MinY, confArea.MaxY);

            // Stat.
            dtoEnemy.Stat.HealthMax = tbMob.Health;
            dtoEnemy.Stat.Health = tbMob.Health;
            dtoEnemy.Stat.Power = tbMob.Damage;
            dtoEnemy.Stat.Defense = tbMob.Defense;
            dtoEnemy.Stat.Velocity = tbMob.Speed;

            // Location.
            dtoEnemy.Location.Area = areaType;
            dtoEnemy.Location.Pos = new Vector2(x, y);
            dtoEnemy.Location.PosEnd = Vector2.Zero;
            dtoEnemy.Location.MoveDirection = Vector2.Zero;

            // AttackType.
            dtoEnemy.AttackType = tbMob.AttackType;

            // DropItems.
            for (int repeat = 0; repeat < _rand.Next(0, 4); repeat++)
            {
                dtoEnemy.DropItems.Add(_itemTypes.OrderBy(d => new Guid()).First());
            }

            _logger.LogDebug($"Spawn enemy. pos: ({x}, {y})");

            return dtoEnemy;
        }
    }
}
