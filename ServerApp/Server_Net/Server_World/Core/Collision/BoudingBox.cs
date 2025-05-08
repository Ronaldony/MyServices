using System.Numerics;

namespace Server_World.Core.Collision
{
    /// <summary>
    /// BoudingBox.
    /// </summary>
    public class BoudingBox
    {
        public BoudingBox()
        {
            Min = new Vector2();
            Max = new Vector2();
        }

        public Vector2 Min { get; set; }

        public Vector2 Max { get; set; }
    }
}
