
namespace ServerEngine.Database.Interfaces
{
    using ServerEngine.Database.Data;
    using ServerEngine.Database.Types;

    public interface IDataObjectService
    {
        /// <summary>
        /// Initialize.
        /// </summary>
        void Initialize();

        /// <summary>
        /// Get table from Type_DataObject.
        /// </summary>
        DataObjectInfo GetDataObjectInfo(Type_DataObject dataObjectType);
    }
}
