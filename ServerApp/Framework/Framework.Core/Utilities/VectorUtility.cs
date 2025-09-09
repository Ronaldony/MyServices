using System.Numerics;

namespace Framework.Core.Utilities
{
    /// <summary>
    /// VectorUtility.
    /// </summary>
    public class VectorUtility
    {
        /// <summary>
        /// Normalize (v1 - v2) safetely
        /// </summary>
        public static Vector2 SafeNormalize(Vector2 v1, Vector2 v2)
        {
            if (v1 == v2)
            {
                return Vector2.Zero;
            }

            return Vector2.Normalize(v1 - v2);
        }
    }
}
