
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
        private const int SPAWN_RANGE = 10;

        private readonly ILogger<MobCreateService> _logger;

        // Services.
        private readonly IServiceProvider _serviceProvider;
        private readonly ITDBService _tdbService;

        // Datas.
        private readonly List<Type_Item> _itemTypes;
        private Dictionary<Type_Area, TB_Area> _confAreas;
        private Dictionary<Type_Area, List<TB_AreaSpawn>> _tbAreaSpawns;

        // Random.
        private readonly Random _rand;

        public MobCreateService(IServiceProvider serviceProvider)
        {
            _logger = serviceProvider.GetRequiredService<ILogger<MobCreateService>>();

            _serviceProvider = serviceProvider;
            _tdbService = serviceProvider.GetService<ITDBService>();

            _itemTypes = Enum.GetValues<Type_Item>().Where(d => d != Type_Item.None).ToList();
            
            _rand = new Random();
        }

        /// <summary>
        /// Initialize.
        /// </summary>
        public void Initialize()
        {
            // initlaize datas.
            _confAreas = _tdbService.SelectAll<TB_Area>().ToDictionary(d => d.Area, d => d);
            _tbAreaSpawns = new Dictionary<Type_Area, List<TB_AreaSpawn>>();

            // TB_AreaSpawn.
            var tbAreaSpawns = _tdbService.SelectAll<TB_AreaSpawn>();

            foreach (var tb in tbAreaSpawns)
            {
                if (false == _tbAreaSpawns.ContainsKey(tb.Area))
                {
                    _tbAreaSpawns.Add(tb.Area, new List<TB_AreaSpawn>());
                }

                _tbAreaSpawns[tb.Area].Add(tb);
            }

            _logger.LogInformation($"MobCreateService initialized.");
        }

        /// <summary>
        /// Get mob as template.
        /// </summary>
        public DTO_Mob CreateMob(int refId, Type_Area areaType)
        {
            var tbMob = _tdbService.GetById<TB_Mob>(refId);
            var confArea = _confAreas[areaType];

            // 데이터 검사.
            if (tbMob == null || confArea == null)
            {
                return null;
            }

            // 유닛 생성.
            var dtoMob = new DTO_Mob
            {
                MobId = refId,
            };

            // 스폰 위치 생성.
            var spawnPos = GetSpawnPosition(areaType);

            // Stat.
            dtoMob.Stat.HealthMax = tbMob.Health;
            dtoMob.Stat.Health = tbMob.Health;
            dtoMob.Stat.Power = tbMob.Damage;
            dtoMob.Stat.Defense = tbMob.Defense;
            dtoMob.Stat.Velocity = tbMob.Speed;

            // Location.
            dtoMob.Location.Area = areaType;
            dtoMob.Location.Pos = spawnPos;
            dtoMob.Location.PosEnd = Vector2.Zero;
            dtoMob.Location.MoveDirection = Vector2.Zero;

            // AttackType.
            dtoMob.AttackType = tbMob.AttackType;

            // DropItems.
            for (int repeat = 0; repeat < _rand.Next(0, 4); repeat++)
            {
                dtoMob.DropItems.Add(_itemTypes.OrderBy(d => new Guid()).First());
            }

            _logger.LogDebug($"Spawn mob. pos: ({dtoMob.Location.Pos.X}, {dtoMob.Location.Pos.Y})");

            return dtoMob;
        }

        /// <summary>
        /// Get spawn position.
        /// </summary>
        private Vector2 GetSpawnPosition(Type_Area area)
        {
            var tbAreas = _tbAreaSpawns[area];
            var tbArea = tbAreas.OrderBy(d => Guid.NewGuid()).First();

            return new Vector2(30, 10);
            //return new Vector2(
            //    x: _rand.Next(tbArea.X - SPAWN_RANGE, tbArea.X + SPAWN_RANGE),
            //    y: _rand.Next(tbArea.Y - SPAWN_RANGE, tbArea.Y + SPAWN_RANGE));
        }
    }
}
