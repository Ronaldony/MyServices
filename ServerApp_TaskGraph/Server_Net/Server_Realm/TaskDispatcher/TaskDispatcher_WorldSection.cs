using System.Threading.Tasks.Dataflow;

namespace Server_Realm.TaskSchedulers
{
    using Server_Realm.TaskDispatcher.Node;
    using Server_Realm.TaskDispatcher.WorldSection;
    using Server_Realm.Worlds;

    /// <summary>
    /// TaskDispatcher_WorldSection.
    /// </summary>
    public class TaskDispatcher_WorldSection
    {
        private const int MAX_TASKNODE_NUM = 3;

        // Group.
        public List<TaskNodeGroup_WorldSection> GetTaskNodeGroups() => _taskNodeGroups;
        private List<TaskNodeGroup_WorldSection> _taskNodeGroups;

        // Obsever.
        private TaskNodeObserver_WorldSection _taskNodeObserver;

        private CancellationToken _ct;

        public TaskDispatcher_WorldSection(CancellationToken ct)
        {
            _taskNodeObserver = new TaskNodeObserver_WorldSection(ct);
            _ct = ct;
        }

        #region initialize
        /// <summary>
        /// Initialize task node.
        /// </summary>
        public void Initialize(IEnumerable<SectionId> allSections, Action<TaskNode_WorldSection> action)
        {
            // Task nodes.
            Initialize_TaskNodes(allSections, _ct);

            // Register task.
            Initialize_RegisterTask(action);
        }

        /// <summary>
        /// Initialize task nodes.
        /// </summary>
        private void Initialize_TaskNodes(IEnumerable<SectionId> allSections, CancellationToken ct)
        {
            // X, Y 정렬.
            var filteredSections = allSections.OrderBy(d => d.X).ThenBy(d => d.Y).DistinctBy(d => d.X);

            // Group id.
            int groupId = 0;

            // TaskNodes.
            var taskNodeGroup = new TaskNodeGroup_WorldSection(groupId, _taskNodeObserver, ct); ;
            TaskNode_WorldSection nowStartNode = null;
            TaskNode_WorldSection prevStartNode = null;
            TaskNode_WorldSection prevNode = null;

            foreach (var section in filteredSections)
            {
                // X 섹션이 동일한 영역만 가져옴.
                var taskSections = allSections.Where(d => d.X == section.X).ToList();
                var taskNode = new TaskNode_WorldSection
                {
                    Section = section,
                    TaskSections = taskSections,
                };

                // Group 개수 검사.
                if (taskNodeGroup.TaskNodes.Count() == 0)
                {
                    _taskNodeGroups.Add(taskNodeGroup);

                    // save start node.
                    prevStartNode = nowStartNode;
                    nowStartNode = taskNode;
                }

                taskNodeGroup.TaskNodes.Add(taskNode);

                // 노드 최대 개수인지 검사.
                if (taskNodeGroup.TaskNodes.Count() == MAX_TASKNODE_NUM)
                {
                    groupId++;
                    taskNodeGroup = new TaskNodeGroup_WorldSection(groupId, _taskNodeObserver, ct); ;
                    prevNode = null;
                }
                else
                {
                    if (null != prevNode)
                    {
                        // 다음 작업 노드 추가.
                        prevNode.NextNode = taskNode;
                    }

                    if (prevStartNode != null)
                    {
                        prevStartNode = prevStartNode.NextNode;
                        prevStartNode.DependancyNode = taskNode;
                        taskNode.DependentNode = prevStartNode;
                    }

                    prevNode = taskNode;
                }
            }
        }

        /// <summary>
        /// Initialize for register task.
        /// </summary>
        private void Initialize_RegisterTask(Action<TaskNode_WorldSection> action)
        {
            var options = new ExecutionDataflowBlockOptions
            {
                MaxDegreeOfParallelism = 1,
                CancellationToken = _ct
            };

            foreach (var group in _taskNodeGroups)
            {
                group.TaskPipeline = new ActionBlock<TaskNode_WorldSection>(action, options);
            }
        }
#endregion

        /// <summary>
        /// Refresh observer.
        /// </summary>
        public async Task DispatchAndExecuteTasks()
        {
            // Refresh pipeline.
            _taskNodeObserver.RefreshPipeline();

            // Start tasks.
            foreach (var nodeGroup in _taskNodeGroups)
            {
                nodeGroup.StartTask();
            }

            await _taskNodeObserver.WaitCompleted();
        }
    }
}
