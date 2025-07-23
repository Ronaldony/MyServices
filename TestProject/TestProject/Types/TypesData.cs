namespace TestProject.Types
{
    public enum ObjectType : byte
    {
        Player = 1,
        Creature = 2,
        GameObject = 3,
        Corpse = 4
    }

    // ====================================================================
    // AI STATE MACHINE SYSTEM
    // ====================================================================

    public enum AIState : byte
    {
        Idle = 0,
        Patrol = 1,
        Alert = 2,
        Combat = 3,
        Chase = 4,
        Evade = 5,
        Return = 6,
        Dead = 7,
        Casting = 8
    }

    public enum AIType : byte
    {
        Passive = 0,
        Aggressive = 1,
        Defensive = 2,
        Neutral = 3,
        Guard = 4
    }
}
