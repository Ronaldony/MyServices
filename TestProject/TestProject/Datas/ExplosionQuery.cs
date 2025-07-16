using System.Numerics;
using TestProject.Architectures;

namespace TestProject.Datas
{
    /// <summary>
    /// ExplosionQuery.
    /// </summary>
    public class ExplosionQuery : ICollidable
    {
        public ExplosionQuery(BoundingBox boundingBox)
        {

        }

        public int Id => throw new NotImplementedException();

        public Vector3 Position => throw new NotImplementedException();

        public float Radius => throw new NotImplementedException();

        public BoundingBox GetBounds()
        {
            throw new NotImplementedException();
        }
    }
}
