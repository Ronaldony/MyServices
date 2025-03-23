namespace Framework.Core.Database.Attributes
{
    /// <summary>
    /// DataInfoObjectAttribute.
    /// </summary>
    public class DataInfoObjectAttribute : Attribute
    {
        public DataInfoObjectAttribute(Type infoObject)
        {
            InfoObject = infoObject;
        }

        /// <summary>
        /// Info object.
        /// </summary>
        public Type InfoObject { get; init; }
    }
}
