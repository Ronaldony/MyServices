
namespace Server_Realm.TaskSchedulers
{
    using Server_Realm.TaskDispatcher.Node;
    using Server_Realm.Worlds;

    /// <summary>
    /// TaskDispatcher_WorldSection.
    /// </summary>
    public class TaskDispatcher_WorldSection
    {
        private const int MAX_TASKNODE_NUM = 3;

        // Task node list.
        public List<TaskNode_WorldSection> GetTaskNodes() => _taskNodes;
        private List<TaskNode_WorldSection> _taskNodes;

        public TaskDispatcher_WorldSection(IEnumerable<SectionId> allSections)
        {
            // Initialize.
            _taskNodes = Initialize_TaskNode(allSections);
        }

        /// <summary>
        /// Initialize task node.
        /// </summary>
        private List<TaskNode_WorldSection> Initialize_TaskNode(IEnumerable<SectionId> allSections)
        {
            var taskNodes = new List<TaskNode_WorldSection>();

            // X, Y 정렬.
            var filteredSections = allSections.OrderBy(d => d.X).ThenBy(d => d.Y).DistinctBy(d => d.X);

            int taskNodeNum = 0;
            TaskNode_WorldSection nowStartNode = null;
            TaskNode_WorldSection prevStartNode = null;
            TaskNode_WorldSection prevNode = null;

            foreach (var section in filteredSections)
            {
                var sameYSections = allSections.Where(d => d.Y == section.Y).ToList();
                var taskNode = new TaskNode_WorldSection
                {
                    Section = section,
                    TaskSections = sameYSections,
                };

                // Task 시작 노드인지 검사.
                if (taskNodeNum == 0)
                {
                    taskNodes.Add(taskNode);
                    prevStartNode = nowStartNode;
                    nowStartNode = taskNode;
                }

                ++taskNodeNum;

                // 노드 최대 개수인지 검사.
                if (taskNodeNum == MAX_TASKNODE_NUM)
                {
                    taskNodeNum = 0;
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

            return taskNodes;
        }
    }
}
