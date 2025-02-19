namespace ServerEngine.Core.Services
{
    using ServerEngine.Core.DataObject;
    using ServerEngine.Core.ObejctPool;
    using ServerEngine.Core.Services.Interfaces;

	/// <summary>
	/// CustomMemoryPoolService.
	/// </summary>
	public sealed class CustomMemoryPoolService : IMemoryPoolService
    {
        private readonly Dictionary<Type, CustomMemoryPool> _memoryPoolDict;

        public CustomMemoryPoolService(IServiceProvider serviceProvider)
        {
            _memoryPoolDict = new Dictionary<Type, CustomMemoryPool>();
        }

        /// <summary>
        /// Initialize.
        /// </summary>
        public void Initialize()
        {
            // Get DataObjectbase type.
            var baseType = typeof(MemoryPoolBase);
            var types = AppDomain.CurrentDomain.GetAssemblies().SelectMany(t => t.GetTypes()).Where(t => t.IsSubclassOf(baseType));

            // DataObjectBase 상속 클래스 오브젝트 풀 생성
            foreach (var type in types)
            {
                var objectPool = new CustomMemoryPool();
                _memoryPoolDict.Add(type, objectPool);
            }
        }

        /// <summary>
        /// Acquire object pool.
        /// </summary>
        public T Acquire<T>() where T : MemoryPoolBase, new()
        {
            return _memoryPoolDict[typeof(T)].Get<T>();
        }

        /// <summary>
        /// Release object pool.
        /// </summary>
        public bool Release<T>(T obj) where T : MemoryPoolBase, new()
        {
            try
            {
                _memoryPoolDict[typeof(T)].Return(obj);

                return true;
            }
            catch
            {
                return false;
            }
        }
    }
}
