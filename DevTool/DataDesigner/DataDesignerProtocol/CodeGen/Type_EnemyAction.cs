namespace GameCore
{
    public enum Type_EnemyAction
    {
        /// <summary>
        /// none
        /// </summary>
        None = 1,
        /// <summary>
        /// moving
        /// </summary>
        Move = 2,
        /// <summary>
        /// tracking player
        /// </summary>
        Track = 3,
        /// <summary>
        /// return to spawn position
        /// </summary>
        Return = 4
    }
}