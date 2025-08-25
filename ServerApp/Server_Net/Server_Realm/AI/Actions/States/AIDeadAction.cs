using Microsoft.Extensions.DependencyInjection;

namespace Server_Realm.AI.Actions.States
{
    using GameCore;
    using GameCore.GamePacket;
    using Server_Realm.Core.Types;
    using Server_Realm.Entities;
    using Server_Realm.Units.Mob;
    using ServerEngine.Core.Services.Interfaces;

    /// <summary>
    /// AIDeadAction.
    /// </summary>
    public class AIDeadAction : AIAction
    {
        private readonly IMemoryPoolService _memoryPoolService;

        public AIDeadAction(IServiceProvider serviceProvider, MobUnit owner, AIBrain brain)
            :base(owner, brain)
        {
            _memoryPoolService = serviceProvider.GetRequiredService<IMemoryPoolService>();
        }

        public override void Start()
        {
            Owner.IsDead = true;

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
                    packet.Position = Owner.MovementHandler.NowPos();

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
