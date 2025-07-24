namespace Server_Realm.AI.Actions.Movement
{
    using Server_Realm.Core.Types;
    using Server_Realm.Entities;

    /// <summary>
    /// TrackAction.
    /// </summary>
    public class TrackAction : AIAction
    {
        public TrackAction(Unit owner)
            : base(owner, Type_AIAction.Track)
        {
        }
    }
}
