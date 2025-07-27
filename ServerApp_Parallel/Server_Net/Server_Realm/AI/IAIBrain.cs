
namespace Server_Realm.AI
{
    using GameCore;

    /// <summary>
    /// IAIBrain.
    /// </summary>
    public interface IAIBrain
    {
        /// <summary>
        /// Update action.
        /// </summary>
        void Update(double deltaTime);

        /// <summary>
        /// Change state.
        /// </summary>
        void ChangeState(Type_AIState state);
    }
}
