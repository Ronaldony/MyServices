using System.Numerics;

namespace Server_World.Core.Datas
{
    using GameCore;

    /// <summary>
    /// SectionId.
    /// </summary>
    public record struct SectionId
    {
        public SectionId(int x, int y)
        {
            X = x;
            Y = y;
        }

        /// <summary>
        /// X section.
        /// </summary>
        public int X { get; set; }

        /// <summary>
        /// Y section.
        /// </summary>
        public int Y { get; set; }

        /// <summary>
        /// Get section.
        /// </summary>
        public static SectionId GetSection(Vector2 position)
        {
            return new SectionId((int)position.X / GameData.SECTION_WIDTH, (int)position.Y / GameData.SECTION_HEIGHT);
        }
    }
}
