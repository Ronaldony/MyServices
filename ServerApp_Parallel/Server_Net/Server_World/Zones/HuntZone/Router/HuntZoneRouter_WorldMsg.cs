using System.Threading.Tasks.Dataflow;

namespace Server_World.Zones.HuntZone.Router
{
    using Microsoft.Extensions.Logging;
    using Server_World.Core.Types;
    using Server_World.Core.Worlds.Msg;

    /// <summary>
    /// HuntZoneRouter.
    /// </summary>
    public partial class HuntZoneRouter
    {
        public ActionBlock<WorldMsg> Initialize_WorldMsgPipeline(CancellationToken ct)
        {
            var pipeline = new ActionBlock<WorldMsg>(
                dataflowBlockOptions: new ExecutionDataflowBlockOptions
                {
                    MaxDegreeOfParallelism = 1,     // single thread.
                    CancellationToken = ct,         // cancel token.
                },
                action: wm =>
                {
                    switch (wm.MsgType)
                    {
                        case Type_WorldMsg.SpawnPlayer:
                            {
                                // Spawn player.
                                var msg = wm as WorldMsg_SpawnPlayer;
                                var worldPlayer = msg.PlayerEntity;

                                if (false == IsValidSection(worldPlayer.Section))
                                {
                                    _logger.LogError($"invalid section. pid: {worldPlayer.Pid}");
                                }

                                var district = GetDistrict(worldPlayer.Section);

                                // Add player.
                                _playerDistrictDict.AddOrUpdate(worldPlayer.Pid, district, (_, _) => district);

                                // Write to processor.
                                _districtProcessors[district].WriteWorldMsg(msg);

                                _logger.LogDebug($"Login player. pid: {worldPlayer.Pid}");
                            }
                            break;
                        case Type_WorldMsg.OutPlayer:
                            {
                                // Out player.
                                var msg = wm as WorldMsg_OutPlayer;

                                // Remove player.
                                _playerDistrictDict.Remove(msg.Pid, out var district);

                                // Write to processor.
                                _districtProcessors[district].WriteWorldMsg(msg);
                                _logger.LogDebug($"Remove player. pid: {msg.Pid}");
                            }
                            break;
                        default:
                            foreach (var processor in _districtProcessors.Values)
                            {
                                processor.WriteWorldMsg(wm);
                            }
                            break;
                    }
                });
            return pipeline;
        }
    }
}
