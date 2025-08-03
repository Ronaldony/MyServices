using System.Threading.Tasks.Dataflow;

namespace Server_Realm.TaskDispatcher.WorldSection
{
    using Server_Realm.TaskDispatcher.Node;

    /// <summary>
    /// TaskNodeObserver_WorldSection.
    /// </summary>
    public class TaskNodeObserver_WorldSection
    {
        // 완료 작업 노드 그룹.
        private List<TaskNodeGroup_WorldSection> _completedTaskNodeGroups;

        // 관찰 대상
        private List<TaskNodeGroup_WorldSection> _observations;

        // 관찰 프로세스.
        // input: 작업 노드 그룹 Group id.
        private ActionBlock<TaskNodeGroup_WorldSection> _observePipeline;

        private ExecutionDataflowBlockOptions _options;

        public TaskNodeObserver_WorldSection(CancellationToken ct)
        {
            _completedTaskNodeGroups = new List<TaskNodeGroup_WorldSection>();
            _observations = new List<TaskNodeGroup_WorldSection>();
            _options = new ExecutionDataflowBlockOptions
            {
                MaxDegreeOfParallelism = 1,
                CancellationToken = ct
            };
        }

        /// <summary>
        /// 파이프라인 갱신.
        /// </summary>
        public void RefreshPipeline()
        {
            _observePipeline = new ActionBlock<TaskNodeGroup_WorldSection>(group =>
            {
                _completedTaskNodeGroups.Add(group);

                // 모든 작업이 완료되었다면, 관찰 종료.
                if (_observations.Count() == _completedTaskNodeGroups.Count())
                {
                    _completedTaskNodeGroups.Clear();
                    _observePipeline.Complete();
                }
            }, _options);
        }

        /// <summary>
        /// 완료된 작업 그룹 추가.
        /// </summary>
        public void AlertCompletedNodeGroup(TaskNodeGroup_WorldSection group)
        {
            _observePipeline.Post(group);
        }

        /// <summary>
        /// Wait observer pipeline completed.
        /// </summary>
        public async Task WaitCompletedAsync()
        {
            await _observePipeline.Completion;
        }

        /// <summary>
        /// Add observation.
        /// </summary>
        public void AddObservation(TaskNodeGroup_WorldSection taskNodeGroup)
        {
            _observations.Add(taskNodeGroup);
        }
    }
}
