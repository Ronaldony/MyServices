namespace Server_Realm.AI.Actions.Movement
{
    using GameCore;
    using Server_Realm.Core.Types;
    using Server_Realm.Entities;

    /// <summary>
    /// IdleAction.
    /// </summary>
    public class IdleAction : AIAction
    {
        public IdleAction(Unit owner)
            : base(owner, Type_AIAction.Idle)
        {
        }
    }
}
