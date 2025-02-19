using Microsoft.Extensions.ObjectPool;

namespace ServerEngine.Core.DataObject
{
	/// <summary>
	/// MemoryPoolBase.
    /// Description: The class for the memory pool class.
	/// </summary>
	public abstract class MemoryPoolBase
    {
        /// <summary>
        /// Reset by ObjectPoolService.
        /// </summary>
        public abstract void TryReset();
    }
}
