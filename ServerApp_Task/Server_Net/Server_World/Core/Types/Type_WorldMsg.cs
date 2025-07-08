namespace Server_World.Core.Types
{
    /// <summary>
    /// Type_WorldMsg.
    /// </summary>
    public enum Type_WorldMsg
    {
        None = 0,

        SpawnPlayer = 10,           // Spanw player.
        OutPlayer = 11,             // Out player.

        SpawnEnemy = 30,            // Spanw enemy.
        UpdateFrame = 31,           // Update frame.
        UpdateMovePattern = 32,     // Update move pattern.
        UpdateDummy = 33,           // Update dummy.
    }
}
