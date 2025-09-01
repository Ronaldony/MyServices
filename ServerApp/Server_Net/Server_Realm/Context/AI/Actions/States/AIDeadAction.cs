using Microsoft.Extensions.DependencyInjection;
using ServerEngine.Core.Services.Interfaces;

namespace Server_Realm.Context.AI.Actions.States
{
    using GameCore;
    using GameCore.GamePacket;
    using Microsoft.Extensions.Logging;
    using Server_Realm.Context.AI;
    using Server_Realm.Context.AI.Actions;
    using Server_Realm.Core.Types;
    using Server_Realm.Entities;
    using Server_Realm.Units.Mob;

    /// <summary>
    /// AIDeadAction.
    /// </summary>
    public class AIDeadAction : AIAction
    {
        private readonly ILogger<AIDeadAction> _logger;
        private readonly IMemoryPoolService _memoryPoolService;

        public AIDeadAction(IServiceProvider serviceProvider, MobUnit owner, AIBrain brain)
            : base(owner, brain)
        {
            _logger = serviceProvider.GetRequiredService<ILogger<AIDeadAction>>();
            _memoryPoolService = serviceProvider.GetRequiredService<IMemoryPoolService>();
        }

        public override void Start()
        {
            Owner.IsDead = true;

            _logger.LogDebug($"AI Dead.");

            // 드랍 아이템 처리.
            var aggroPoints = Owner.CombatHandler.GetAggroPoints();
            var takeMostAggroPoints = aggroPoints.OrderBy(d => d.Value).Take(GameData.MAX_DROP_UNITS);
            int aggroOrder = 0;

            foreach (var pair in takeMostAggroPoints)
            {
                var unit = pair.Key;

                // Unit이 플레이어인 경우 드랍 아이템 처리.
                if (unit.UnitType == Type_Unit.Player)
                {
                    var pu = unit as PlayerUnit;
                    var packet = _memoryPoolService.Rent<GamePacketSC_DropItems>();

                    // 드랍 위치.
                    packet.UnitId = Owner.UnitId;

                    // 드랍 아이템.
                    foreach (var dropItem in Owner.DropItems[aggroOrder])
                    {
                        pu.AddDropItemList(dropItem.DropId, dropItem.Item);
                        packet.DropItems.Add(dropItem.DropId, dropItem.Item);
                    }

                    // 패킷 전송 - 드랍 아이템.
                    pu.ZoneProcessor.SendPacket(pu.Pid, GamePacketCode.SC_DropItems, packet);
                    aggroOrder++;
                }
            }
        }

        public override void Update(double deltaTime)
        {
        }

        public override void Stop()
        {
        }
    }
}
