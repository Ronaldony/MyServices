using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using ServerEngine.Core.Services.Interfaces;
using System.Collections.Concurrent;
using System.Reactive.Disposables;
using System.Reactive.Linq;

namespace Server_Realm.Worlds.MsgTrigger
{
    using GameCore;
    using GameCore.Interfaces;
    using Server_Realm.Core.Factories;
    using Server_Realm.Core.Items;
    using Server_Realm.Core.Services;
    using Server_Realm.Core.Types;
    using Server_Realm.Units.Mob;
    using Server_Realm.Worlds;
    using Server_Realm.Worlds.Msg;
    using Server_Realm.Zones;

    /// <summary>
    /// WorldMsgTrigger_SpawnMob.
    /// </summary>
    public sealed class WorldMsgTrigger_SpawnMob
    {
        private readonly ILogger<WorldMsgTrigger_SpawnMob> _logger;

        private readonly IServiceProvider _serviceProvider;
        private readonly IUniqueIdService _uniqueIdService;
        private readonly ITDBService _tdbService;

        private readonly MobCreateService _mobCreateService;
        private readonly ItemCreateService _itemCreateService;

        private readonly UnitFactory _unitFactory;

        // Actor provider.
        private readonly IZoneProcessor _zoneProcessor;

        private readonly IEnumerable<TB_Mob> _tbMobs;

        /// <summary>
        /// 리스폰 지역 id.
        /// </summary>
        private readonly ConcurrentQueue<int> _respawnAreaIds;

        // Disposables.
        private readonly CompositeDisposable _subscriptions;

        public WorldMsgTrigger_SpawnMob(IServiceProvider serviceProvider, IZoneProcessor zoneProcessor)
        {
            _logger = serviceProvider.GetRequiredService<ILogger<WorldMsgTrigger_SpawnMob>>();

            _serviceProvider = serviceProvider;
            _uniqueIdService = serviceProvider.GetRequiredService<IUniqueIdService>();

            _mobCreateService = serviceProvider.GetRequiredService<MobCreateService>();
            _itemCreateService = serviceProvider.GetRequiredService<ItemCreateService>();
            _tdbService = serviceProvider.GetRequiredService<ITDBService>();

            _unitFactory = serviceProvider.GetRequiredService<UnitFactory>();

            _zoneProcessor = zoneProcessor;

            _tbMobs = _tdbService.SelectAll<TB_Mob>();

            _respawnAreaIds = new ConcurrentQueue<int>();

            _subscriptions = new CompositeDisposable();

        }

        /// <summary>
        /// 스폰 타이머 시작.
        /// </summary>
        public void Start(Type_Area area)
        {
            // 최초 스폰
            FirstSpawnMobs(area);

            // 스폰 스케줄링.
            _subscriptions.Add(
                Observable.Interval(TimeSpan.FromSeconds(5))
                .Subscribe(count =>
                {
                    CheckRespawn();
                }));

            _logger.LogInformation("start mob spawn.");
        }

        /// <summary>
        /// 스폰 타이머 종료.
        /// </summary>
        public void Stop()
        {
            foreach (var disposable in _subscriptions)
            {
                disposable.Dispose();
            }

            _subscriptions.Clear();

            _logger.LogInformation("stop mob spawn.");
        }

        /// <summary>
        /// Request respawn.
        /// </summary>
        public void RequestRespawn(int tbAreaSpawnId)
        {
            _respawnAreaIds.Enqueue(tbAreaSpawnId);
        }

        /// <summary>
        /// First spawn mobs.
        /// </summary>
        private void FirstSpawnMobs(Type_Area area)
        {
            var tbAreaSpawns = _tdbService.SelectAll<TB_AreaSpawn>().Where(d => d.Area == area);
            var spawnMob = new WorldMsg_SpawnMob();

            foreach (var tbAreaSpawn in tbAreaSpawns)
            {
                // TB_AreaSpawn의 최대 몹 수량만큼 스폰.
                for (var spawnCnt = 0; spawnCnt < tbAreaSpawn.MaxCount; spawnCnt++)
                {
                    var mu = SpawnMobUnit(tbAreaSpawn.Id);

                    if (mu == null)
                    {
                        continue;
                    }

                    // Get section.
                    var spawnPos = SectionId.ToSection(mu.Mob.Location.Pos);

                    if (false == spawnMob.MobUnits.ContainsKey(spawnPos))
                    {
                        spawnMob.MobUnits.Add(spawnPos, new List<MobUnit>());
                    }

                    spawnMob.MobUnits[spawnPos].Add(mu);
                }
            }

            // Zone에 전송
            _zoneProcessor.Tell(spawnMob);
        }

        /// <summary>
        /// 리스폰 검사.
        /// </summary>
        private void CheckRespawn()
        {
            var spawnMob = new WorldMsg_SpawnMob();

            while (_respawnAreaIds.TryDequeue(out var areaSpawnId))
            {
                // 몹 스폰.
                var mu = SpawnMobUnit(areaSpawnId);

                if (mu == null)
                {
                    continue;
                }

                // Get section.
                var spawnPos = SectionId.ToSection(mu.Mob.Location.Pos);

                if (false == spawnMob.MobUnits.ContainsKey(spawnPos))
                {
                    spawnMob.MobUnits.Add(spawnPos, new List<MobUnit>());
                }

                spawnMob.MobUnits[spawnPos].Add(mu);
            }

            // Zone에 전달.
            if (spawnMob.MobUnits.Any())
            {
                _zoneProcessor.Tell(spawnMob);
            }
        }

        /// <summary>
        /// Spawn mobs.
        /// </summary>
        private MobUnit SpawnMobUnit(int areaSpawnId)
        {
            // Pick random TB_Mob.
            var tbMob = _tbMobs.OrderBy(d => Guid.NewGuid()).First();

            // 유닛 생성.
            var dtoMob = _mobCreateService.CreateMob(tbMob.Id, Type_Area.HuntZone, areaSpawnId);

            if (null == dtoMob)
            {
                _logger.LogError($"fail to create mob. mobId: {dtoMob.MobId}");
                return null;
            }

            _logger.LogDebug($"Spawn mob. pos: ({dtoMob.Location.Pos.X}, {dtoMob.Location.Pos.Y})");

            // 드랍 아이템 처리.
            var dropItems = new Dictionary<int, List<DropItem>>();

            for (int aggroOrder = 0; aggroOrder < GameData.MAX_MOB_SPAWN; aggroOrder++)
            {
                dropItems.Add(aggroOrder, new List<DropItem>());

                // Create item groups.
                // Aggro 순서가 증가할 때마다 드랍 확률 떨어짐.
                var dtoItems = _itemCreateService.CreateItemGroups(tbMob.ItemGroupId, aggroOrder);

                foreach (var item in dtoItems)
                {
                    var dropId = _uniqueIdService.GenerateIdAsLong();

                    dropItems[aggroOrder].Add(new DropItem
                    {
                        DropId = dropId,
                        Item = item
                    });
                }
            }

            // Create mob unit.
            var mu = _unitFactory.CreateUnit<MobUnit>(Type_Unit.Mob);
            mu.Setup(_serviceProvider, _uniqueIdService.GenerateIdAsLong(), dtoMob, dropItems, areaSpawnId);

            return mu;
        }
    }
}
