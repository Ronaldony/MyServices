using System.Numerics;

namespace Server_Realm.Worlds
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
        public int X;

        /// <summary>
        /// Y section.
        /// </summary>
        public int Y;

        /// <summary>
        /// Convert position to section.
        /// </summary>
        public static SectionId ToSection(Vector2 position)
        {
            return new SectionId((int)position.X / GameData.SECTION_WIDTH, (int)position.Y / GameData.SECTION_HEIGHT);
        }
    }
}
