using GameCore.Datas.Plays;
using UnityEngine;

public enum Type_MobMsg
{
    None = 0,

    Spawn = 1,
    UpdateMovement = 2,
    FixMovement = 3
}

public abstract class MobMsg
{
    public MobMsg(Type_MobMsg msgType)
    {
        MsgType = msgType;
    }

    public Type_MobMsg MsgType { get; set; }
}

/// <summary>
/// MobMsg_Spawn.
/// </summary>
public class MobMsg_Spawn : MobMsg
{
    public MobMsg_Spawn()
        : base(Type_MobMsg.Spawn)
    {

    }

    public DTO_Location Location { get; set; }

    public UnitDummy Dummy { get; set; }
}

/// <summary>
/// MobMsg_UpdateMovement.
/// </summary>
public class MobMsg_UpdateMovement : MobMsg
{
    public MobMsg_UpdateMovement()
        : base(Type_MobMsg.UpdateMovement)
    {

    }

    public DTO_Location Location { get; set; }
}

/// <summary>
/// MobMsg_FixMovement.
/// </summary>
public class MobMsg_FixMovement : MobMsg
{
    public MobMsg_FixMovement()
        : base(Type_MobMsg.FixMovement)
    {

    }

    public Vector2 FixPos { get; set; }
}
