using ServerEngine.Database.Types;

namespace ServerEngine.Database.Attributes
{
    public class DataObjectAttribute : Attribute
    {
        public DataObjectAttribute(Type_DataObject blobDataType, string database, string table) { }
    }
}
