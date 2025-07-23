using System.Numerics;
using TestProject.Objects;

namespace TestProject.Threats
{
    /// <summary>
    /// ThreatEntry.
    /// </summary>
    public class ThreatEntry
    {
        public IWorldObject Target { get; set; }
        public float ThreatValue { get; set; }
        public float LastThreatTime { get; set; }
        public Vector3 LastKnownPosition { get; set; }
    }
}
