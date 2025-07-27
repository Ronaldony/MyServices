
namespace Server_Realm.TaskDispatcher.Node
{
    using Server_Realm.Worlds;

    /// <summary>
    /// TaskNode_WorldSection.
    /// </summary>
    public class TaskNode_WorldSection
    {
        /// <summary>
        /// 섹션.
        /// </summary>
        public SectionId Section;

        /// <summary>
        /// 작업 섹션.
        /// </summary>
        public List<SectionId> TaskSections;

        /// <summary>
        /// 다음 노드.
        /// </summary>
        public TaskNode_WorldSection NextNode;

        /// <summary>
        /// 의존 노드.
        /// </summary>
        public TaskNode_WorldSection DependancyNode;

        /// <summary>
        /// 의지하는 노드.
        /// </summary>
        public TaskNode_WorldSection DependentNode;
    }
}
