using System.Numerics;
using TestProject.Entity;
using TestProject.Types;

namespace TestProject.Objects
{
    /// <summary>
    /// IWorldObject.
    /// </summary>
    public interface IWorldObject
    {
        EntityId Id { get; }
        Vector3 Position { get; set; }
        ObjectType ObjectType { get; }
        bool IsInWorld { get; set; }
        bool IsDead { get; set; }
    }
}
