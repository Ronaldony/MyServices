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
        public ActionBlock<WorldMsg> initialize_WorldMsgPipeline(CancellationToken ct)
        {
            var pipeline = new ActionBlock<WorldMsg>(
                dataflowBlockOptions: new ExecutionDataflowBlockOptions
                {
                    MaxDegreeOfParallelism = 1,
                    CancellationToken = ct,
                },
                action: wm =>
                {
                    switch (wm.MsgType)
                    {
                        case Type_WorldMsg.SpawnPlayer:
                            {
                                // Spawn player.
                                var msg = wm as WorldMsg_SpawnPlayer;
                                var sectionGroup = GetSectionGroup(msg.WorldPlayer.Section);

                                _playerSectionGroupDict.AddOrUpdate(msg.WorldPlayer.PlayInfo.Pid, sectionGroup, (key, old) => sectionGroup);
                                _gameProccessors[sectionGroup].WriteWorldMsg(msg);
                            }
                            break;
                        case Type_WorldMsg.OutPlayer:
                            {
                                // Out player.
                                var msg = wm as WorldMsg_OutPlayer;

                                _playerSectionGroupDict.TryRemove(msg.Pid, out var sectionGroup);
                                _gameProccessors[sectionGroup].WriteWorldMsg(msg);
                            }
                            break;
                        case Type_WorldMsg.SpawnEnemy:
                        case Type_WorldMsg.UpdateFrame:
                        case Type_WorldMsg.UpdateMovePattern:
                        case Type_WorldMsg.UpdateDummy:
                            foreach (var processor in _gameProccessors.Values)
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
