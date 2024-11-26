
namespace ServerEngine.Core.Services.Interfaces
{
    using ServerEngine.Core.DataObject;

    /// <summary>
    /// Object pool interface.
    /// </summary>
    public interface IObjectPoolService
    {
        /// <summary>
        /// Initialize.
        /// </summary>
        void Initialize();

        /// <summary>
        /// Acquire.
        /// </summary>
        T Acquire<T>() where T: DataObjectBase, new ();

        /// <summary>
        /// Release.
        /// </summary>
        bool Release<T>(T obj) where T: DataObjectBase, new ();
    }
}
