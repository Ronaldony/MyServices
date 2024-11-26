using Microsoft.Extensions.ObjectPool;

namespace ServerEngine.Core.DataObject
{
    public abstract class DataObjectBase
    {
        /// <summary>
        /// Reset by ObjectPoolService.
        /// </summary>
        public abstract void TryReset();
    }
}
