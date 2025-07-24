namespace Server_Realm.AI.Actions.Movement
{
    using GameCore;
    using Server_Realm.Entities;

    /// <summary>
    /// ReturnAction.
    /// </summary>
    public class ReturnAction : AIAction
    {
        public ReturnAction(Unit owner)
            : base(owner, Type_AIAction.Return)
        {
        }
    }
}
