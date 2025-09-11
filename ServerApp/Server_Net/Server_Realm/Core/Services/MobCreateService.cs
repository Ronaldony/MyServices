
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

            _tdbService = serviceProvider.GetRequiredService<ITDBService>();

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
        public DTO_Mob CreateMob(int refId, Type_Area areaType, int areaSpawnId)
        {
            if (false == _confAreas.ContainsKey(areaType))
            {
                return null;
            }

            var tbMob = _tdbService.GetById<TB_Mob>(refId);
            var confArea = _confAreas[areaType];

            // 유닛 생성.
            var dtoMob = new DTO_Mob
            {
                MobId = refId,
            };

            // 스폰 위치 생성.
            var spawnPos = GetSpawnPosition(areaType, areaSpawnId);

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

            return dtoMob;
        }

        /// <summary>
        /// Get spawn position.
        /// </summary>
        private Vector2 GetSpawnPosition(Type_Area area, int areaSpawnId)
        {
            var tbAreaSpawns = _tbAreaSpawns[area];
            var tbAreaSpawn = tbAreaSpawns.First(d => d.Id == areaSpawnId);

            //return new Vector2(-105, -68);
            var spawnPos = new Vector2(
                x: _rand.Next(tbAreaSpawn.X - SPAWN_RANGE, tbAreaSpawn.X + SPAWN_RANGE),
                y: _rand.Next(tbAreaSpawn.Y - SPAWN_RANGE, tbAreaSpawn.Y + SPAWN_RANGE));

            return spawnPos;
        }
    }
}
