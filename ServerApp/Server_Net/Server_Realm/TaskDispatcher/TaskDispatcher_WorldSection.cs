using System.Threading.Tasks.Dataflow;

namespace Server_Realm.TaskSchedulers
{
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.Logging;
    using Server_Realm.Core.Types;
    using Server_Realm.TaskDispatcher.Node;
    using Server_Realm.TaskDispatcher.WorldSection;
    using Server_Realm.Worlds;

    /// <summary>
    /// TaskDispatcher_WorldSection.
    /// </summary>
    public class TaskDispatcher_WorldSection
    {
        private const int MAX_TASKNODE_NUM = 3;

        private ILogger<TaskDispatcher_WorldSection> _logger;

        // Group.
        private List<TaskNodeGroup_WorldSection> _taskNodeGroups;

        // Obsever.
        private TaskNodeObserver_WorldSection _taskNodeObserver;

        private CancellationToken _ct;

        public TaskDispatcher_WorldSection(IServiceProvider serviceProvider, CancellationToken ct)
        {
            _logger = serviceProvider.GetRequiredService<ILogger<TaskDispatcher_WorldSection>>();

            _taskNodeObserver = new TaskNodeObserver_WorldSection(ct);
            _ct = ct;
            _taskNodeGroups = new List<TaskNodeGroup_WorldSection>();
        }

        #region initialize
        /// <summary>
        /// Initialize task node.
        /// </summary>
        public void Initialize(IEnumerable<SectionId> allSections, Func<(TaskNode_WorldSection, Type_TaskNode), (TaskNode_WorldSection, Type_TaskNode)> action)
        {
            // Task nodes.
            Initialize_TaskNodes(allSections, _ct);

            // Register task.
            Initialize_RegisterTask(action);

            _logger.LogInformation("TaskDispatcher_WorldSection initialize.");
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
                    NodeGroup = taskNodeGroup
                };

                // Group 개수 검사.
                if (taskNodeGroup.TaskNodes.Count() == 0)
                {
                    _taskNodeObserver.AddObservation(taskNodeGroup);
                    _taskNodeGroups.Add(taskNodeGroup);

                    // save start node.
                    prevStartNode = nowStartNode;
                    nowStartNode = taskNode;
                }

                taskNodeGroup.TaskNodes.Add(taskNode);

                // 노드 최대 개수인지 검사.
                if (taskNodeGroup.TaskNodes.Count() == MAX_TASKNODE_NUM)
                {
                    // 다음 작업 노드 추가.
                    prevNode.NextNode = taskNode;

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
        private void Initialize_RegisterTask
            (Func<(TaskNode_WorldSection, Type_TaskNode), (TaskNode_WorldSection, Type_TaskNode)> action)
        {
            var options = new ExecutionDataflowBlockOptions
            {
                MaxDegreeOfParallelism = 1,
                CancellationToken = _ct
            };

            foreach (var group in _taskNodeGroups)
            {
                // Task pipeline.
                group.TaskPipeline = new TransformBlock<(TaskNode_WorldSection, Type_TaskNode), (TaskNode_WorldSection, Type_TaskNode)>(action, options);

                // Post task pipeline.
                group.PostTaskPipeline = new ActionBlock<(TaskNode_WorldSection TaskNode, Type_TaskNode TaskNodeType)>(
                    action: tuple =>
                    {
                        var taskNode = tuple.TaskNode;

                        // 작업이 완료된 후 본 노드에 의지하는 노드 작업을 전달.
                        taskNode.DependentNode?.NodeGroup.ExecuteNextTaskNode(taskNode.DependentNode, tuple.TaskNodeType);

                        if (taskNode.NextNode != null)
                        {
                            // 다음 노드의 의존 노드가 없는 경우, 해당 노드 작업 스케줄링.
                            if (taskNode.NextNode.DependancyNode == null)
                            {
                                taskNode.NodeGroup.ExecuteNextTaskNode(taskNode.NextNode, tuple.TaskNodeType);
                            }
                        }
                        else
                        {
                            // 노드 그룹 작업 완료.
                            taskNode.NodeGroup.AlertCompletedToObserver();
                        }
                    },
                    dataflowBlockOptions: options);

                group.TaskPipeline.LinkTo(group.PostTaskPipeline);
            }
        }
#endregion

        /// <summary>
        /// Refresh observer.
        /// </summary>
        public async Task DispatchTasksAsync(Type_TaskNode taskNodeType)
        {
            // Refresh pipeline.
            _taskNodeObserver.RefreshPipeline();

            // Start tasks.
            foreach (var nodeGroup in _taskNodeGroups)
            {
                nodeGroup.StartTask(taskNodeType);
            }

            await _taskNodeObserver.WaitCompletedAsync();
        }
    }
}
