namespace Server_World.Core.Types
{
    /// <summary>
    /// Type_WorldMsg.
    /// </summary>
    public enum Type_WorldMsg
    {
        None = 0,

       SpawnPlayer = 1,             // Spanw player.
       SpawnEnemy = 2,              // Spanw enemy.

       UpdateFrame = 3,             // Update frame.
       UpdateMovePattern = 4,       // Update move pattern.
       
       OutPlayer = 5,               // Out player.

       UpdateDummy = 6,             // Update dummy.

       NetPacket = 7,               // Network packet.
    }
}
