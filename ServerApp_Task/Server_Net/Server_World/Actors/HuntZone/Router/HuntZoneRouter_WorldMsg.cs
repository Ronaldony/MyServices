using Microsoft.Extensions.Logging;
using System.Threading.Tasks.Dataflow;

namespace Server_World.Actors.HuntZone.Router
{
    using Server_World.Core.Types;
    using Server_World.Core.Worlds.Msg;

    /// <summary>
    /// SectionUpstreamer.
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
                                var worldPlayer = msg.WorldPlayer;

                                // Add player.
                                _playerSectionDict.Add(worldPlayer.PlayInfo.Pid, worldPlayer.Section);
                                
                                // Write to processor.
                                _districtProcessors[GetSectionGroup(worldPlayer.Section)].WriteWorldMsg(msg);
                            }
                            break;
                        case Type_WorldMsg.OutPlayer:
                            {
                                // Out player.
                                var msg = wm as WorldMsg_OutPlayer;

                                // Remove player.
                                _playerSectionDict.Remove(msg.Pid, out var section);

                                // Write to processor.
                                _districtProcessors[GetSectionGroup(section)].WriteWorldMsg(msg);
                            }
                            break;
                        case Type_WorldMsg.SpawnEnemy:
                        case Type_WorldMsg.UpdateFrame:
                        case Type_WorldMsg.UpdateMovePattern:
                        case Type_WorldMsg.UpdateDummy:
                            foreach (var processor in _districtProcessors.Values)
                            {
                                processor.WriteWorldMsg(wm);
                            }
                            break;
                        default:
                            _logger.LogError($"world message type error. type: {wm.MsgType}");
                            break;
                    }
                })
            {

            };
            return pipeline;
        }
    }
}
