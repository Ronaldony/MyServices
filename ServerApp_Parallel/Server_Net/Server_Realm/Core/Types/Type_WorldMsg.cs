namespace Server_Realm.Core.Types
{
    /// <summary>
    /// Type_WorldMsg.
    /// </summary>
    public enum Type_WorldMsg
    {
        None = 0,

        UpdateFrame = 10,           // Update frame.
        SpawnEnemy = 11,            // Spanw enemy.
        UpdateMovePattern = 12,     // Update move pattern.
        UpdateDummy = 13,           // Update dummy.

        SpawnPlayer = 100,          // Spanw player.
        LeavePlayer = 101,            // Out player.
    }
}