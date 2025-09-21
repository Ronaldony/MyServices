using GameCore.Datas.Plays;
using UnityEngine;

public enum Type_OpponentMsg
{
    None = 0,

    // Controller.
    ControllerSpawn = 1,
    MoveStart = 2,
    MoveEnd = 3,
    MoveFix = 4,
    MoveShift = 5,
    Dash = 6,
    Knockback = 7,
}

public abstract class OpponentMsg
{
    public OpponentMsg(Type_OpponentMsg msgType)
    {
        MsgType = msgType;
    }

    public Type_OpponentMsg MsgType { get; set; }
}

/// <summary>
/// OpponentMsg_ControllerSpawn.
/// </summary>
public class OpponentMsg_ControllerSpawn : OpponentMsg
{
    public OpponentMsg_ControllerSpawn()
        :base (Type_OpponentMsg.ControllerSpawn)
    {

    }

    public DTO_Location Location { get; set; }

    public OpponentDummy Dummy { get; set; }
}

/// <summary>
/// OpponentMsg_MoveStart.
/// </summary>
public class OpponentMsg_MoveStart : OpponentMsg
{
    public OpponentMsg_MoveStart()
        : base(Type_OpponentMsg.MoveStart)
    {

    }

    public System.Numerics.Vector2 MoveDir { get; set; }
}

/// <summary>
/// OpponentMsg_MoveEnd.
/// </summary>
public class OpponentMsg_MoveEnd: OpponentMsg
{
    public OpponentMsg_MoveEnd()
        : base(Type_OpponentMsg.MoveEnd)
    {

    }

    public System.Numerics.Vector2 EndPos { get; set; }
}

/// <summary>
/// OpponentMsg_MoveFix.
/// </summary>
public class OpponentMsg_MoveFix : OpponentMsg
{
    public OpponentMsg_MoveFix()
        : base(Type_OpponentMsg.MoveFix)
    {

    }

    public Vector2 FixPos { get; set; }
}

/// <summary>
/// OpponentMsg_MoveShift.
/// </summary>
public class OpponentMsg_MoveShift : OpponentMsg
{
    public OpponentMsg_MoveShift()
        : base(Type_OpponentMsg.MoveShift)
    {

    }

    public System.Numerics.Vector2 MoveDir { get; set; }
    public System.Numerics.Vector2 Position { get; set; }

}

/// <summary>
/// OpponentMsg_Dash.
/// </summary>
public class OpponentMsg_Dash : OpponentMsg
{
    public OpponentMsg_Dash()
        : base(Type_OpponentMsg.Dash)
    {

    }

    public Vector2 DashedPos { get; set; }
}

/// <summary>
/// OpponentMsg_Knockback.
/// </summary>
public class OpponentMsg_Knockback : OpponentMsg
{
    public OpponentMsg_Knockback()
        : base(Type_OpponentMsg.Knockback)
    {

    }

    public System.Numerics.Vector2 Position { get; set; }
    public System.Numerics.Vector2 SrcPosition { get; set; }
}
