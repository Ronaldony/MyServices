namespace Server_Realm.AI.Actions.Movement
{
    using Server_Realm.Core.Types;
    using Server_Realm.Entities;

    /// <summary>
    /// RoamAction.
    /// </summary>
    public class RoamAction : AIAction
    {
        public RoamAction(Unit owner)
            : base(owner, Type_AIAction.Roam)
        {
        }
    }
}
