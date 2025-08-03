using System.Threading.Tasks.Dataflow;

namespace Server_Realm.TaskDispatcher.Node
{
    using Server_Realm.Core.Types;
    using Server_Realm.TaskDispatcher.WorldSection;

    /// <summary>
    /// TaskNodeGroup_WorldSection.
    /// </summary>
    public class TaskNodeGroup_WorldSection
    {
        /// <summary>
        /// Group Id.
        /// </summary>
        public int GroupId;

        /// <summary>
        /// Task pipe line.
        /// </summary>
        public TransformBlock<(TaskNode_WorldSection, Type_TaskNode), (TaskNode_WorldSection, Type_TaskNode)> TaskPipeline;

        /// <summary>
        /// Post task pipe line.
        /// </summary>
        public ActionBlock<(TaskNode_WorldSection, Type_TaskNode)> PostTaskPipeline;

        /// <summary>
        /// TaskNode.
        /// </summary>
        public List<TaskNode_WorldSection> TaskNodes;

        /// <summary>
        /// CancelToken.
        /// </summary>
        public CancellationToken CancelToken;

        /// <summary>
        /// 옵저버.
        /// </summary>
        private TaskNodeObserver_WorldSection _observer;

        public TaskNodeGroup_WorldSection(int groupId, TaskNodeObserver_WorldSection observer, CancellationToken ct)
        {
            GroupId = groupId;
            TaskNodes = new List<TaskNode_WorldSection>();
            CancelToken = ct;
            _observer = observer;
        }

        /// <summary>
        /// Add task node.
        /// </summary>
        public void ExecuteNextTaskNode(TaskNode_WorldSection taskNode, Type_TaskNode taskNodeType)
        {
            TaskPipeline.Post((taskNode, taskNodeType));
        }

        /// <summary>
        /// Start task.
        /// </summary>
        public void StartTask(Type_TaskNode taskNodeType)
        {
            TaskPipeline.Post((TaskNodes.First(), taskNodeType));
        }

        /// <summary>
        /// Alert completed.
        /// </summary>
        public void AlertCompletedToObserver()
        {
            _observer.AlertCompletedNodeGroup(this);
        }
    }
}
