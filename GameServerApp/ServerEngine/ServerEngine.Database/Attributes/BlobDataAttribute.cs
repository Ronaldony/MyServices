using ServerEngine.Database.Types;

namespace ServerEngine.Database.Attributes
{
    public class BlobDataAttribute : Attribute
    {
        public BlobDataAttribute(Type_BlobData blobDataType, string database, string table) { }
    }
}
