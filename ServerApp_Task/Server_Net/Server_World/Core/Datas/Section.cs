namespace Server_World.Core.Datas
{
    /// <summary>
    /// Section.
    /// </summary>
    public class Section
    {
        public Section(int x, int y)
        {
            X = x;
            Y = y;
        }

        /// <summary>
        /// Section X.
        /// </summary>
        public int X { get; private set; }

        /// <summary>
        /// Section Y.
        /// </summary>
        public int Y { get; private set; }
    }
    
}
