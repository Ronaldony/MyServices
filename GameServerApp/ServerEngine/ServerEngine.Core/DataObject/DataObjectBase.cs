using Microsoft.Extensions.ObjectPool;

namespace ServerEngine.Core.DataObject
{
    public class DataObjectBase : IResettable
    {
        /// <summary>
        /// Reset by ObjectPoolService.
        /// </summary>
        public virtual bool TryReset()
        {
            return true;
        }
    }
}
