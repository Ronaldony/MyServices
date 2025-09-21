using UnityEngine;

public class OpponentDummy : MonoBehaviour
{
    /// <summary>
    /// Update position.
    /// </summary>
    public void FixPosition(Vector2 updatePos)
    {
        transform.position = updatePos;
    }

    /// <summary>
    /// Update position.
    /// </summary>
    public void MoveTo(Vector2 move)
    {
        transform.position = (Vector2)transform.position + move;
    }

    /// <summary>
    /// Get position.
    /// </summary>
    public Vector2 GetPosition() => transform.position;
}
