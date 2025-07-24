namespace Server_Realm.Entities
{
    using GameCore.Datas.Enemy;
    using Server_Realm.Core.Types;

    /// <summary>
    /// EnemyUnit.
    /// </summary>
    public class EnemyUnit : Unit
    {
        public EnemyUnit()
        {
            UnitType = Type_Unit.Enemy;
        }

        /// <summary>
        /// DTO_Enemy.
        /// </summary>
        public DTO_Enemy Enemy { get; set; }

        /// <summary>
        /// Enemy uid.
        /// </summary>
        public ulong EnemyUid { get; set; }

        /// <summary>
        /// Update.
        /// </summary>
        public override void Update(float deltaTime)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Dispose.
        /// </summary>
        public override void Dispose()
        {
            throw new NotImplementedException();
        }
    }
}
