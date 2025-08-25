using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using ServerEngine.Core.Services.Interfaces;
using System.Reactive.Disposables;
using System.Reactive.Linq;

namespace Server_Realm.Worlds.MsgTrigger
{
    using GameCore;
    using GameCore.Interfaces;
    using Server_Realm.Core.Factories;
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
        private readonly MobCreateService _mobCreateService;

        private readonly UnitFactory _unitFactory;

        // Actor provider.
        private readonly IZoneProcessor _zoneProcessor;

        // Mobs.
        private readonly List<TB_Mob> _tbMobs;
        private readonly Random _rand;

        // Disposables.
        private readonly CompositeDisposable _subscriptions;

        public WorldMsgTrigger_SpawnMob(IServiceProvider serviceProvider, IZoneProcessor zoneProcessor)
        {
            _logger = serviceProvider.GetRequiredService<ILogger<WorldMsgTrigger_SpawnMob>>();

            _serviceProvider = serviceProvider;
            _uniqueIdService = serviceProvider.GetRequiredService<IUniqueIdService>();
            _mobCreateService = serviceProvider.GetRequiredService<MobCreateService>();

            _unitFactory = serviceProvider.GetRequiredService<UnitFactory>();

            _zoneProcessor = zoneProcessor;

            var tdbService = serviceProvider.GetRequiredService<ITDBService>();

            _tbMobs = tdbService.SelectAll<TB_Mob>().ToList();

            _subscriptions = new CompositeDisposable();
        }

        /// <summary>
        /// 스폰 타이머 시작.
        /// </summary>
        public void Start()
        {
            // 구독.
            _subscriptions.Add(
                Observable.Interval(TimeSpan.FromSeconds(1))
                .Subscribe(count =>
                {
                    SpawnMob();
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
        /// 주기적으로 적 스폰.
        /// </summary>
        private void SpawnMob()
        {
            try
            {
                // Create spawn enemy message.
                var spawnMsg = new WorldMsg_SpawnMob();

                //var mob = _tbMobs.Last();
                foreach (var mob in _tbMobs)
                {
                    // 유닛 생성.
                    var dtoMob = _mobCreateService.CreateMob(mob.Id, Type_Area.HuntZone);

                    if (null == dtoMob)
                    {
                        _logger.LogError($"fail to create mob. mobId: {dtoMob.MobId}");
                        continue;
                    }

                    // Get section.
                    var section = SectionId.ToSection(dtoMob.Location.Pos);

                    if (false == spawnMsg.MobUnits.ContainsKey(section))
                    {
                        spawnMsg.MobUnits.Add(section, new List<MobUnit>());
                    }

                    // Create mob unit.
                    var mu = _unitFactory.CreateUnit<MobUnit>(Type_Unit.Mob);
                    mu.Setup(_serviceProvider, _uniqueIdService.GenerateIdAsLong(), dtoMob);

                    spawnMsg.MobUnits[section].Add(mu);
                }

                // 메시지 전달.
                _zoneProcessor.Tell(spawnMsg);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
            }
        }
    }
}
