namespace ServerEngine.Core.Services
{
    using ServerEngine.Core.DataObject;
    using ServerEngine.Core.ObejctPool;
    using ServerEngine.Core.Services.Interfaces;

    /// <summary>
    /// DataObjectPoolService.
    /// </summary>
    public sealed class DataObjectPoolService : IObjectPoolService
    {
        private readonly Dictionary<Type, DataObjectPool> _objectPoolDic;

        public DataObjectPoolService(IServiceProvider serviceProvider)
        {
            _objectPoolDic = new Dictionary<Type, DataObjectPool>();
        }

        /// <summary>
        /// Initialize.
        /// </summary>
        public void Initialize()
        {
            // Get DataObjectbase type.
            var baseType = typeof(DataObjectBase);
            var types = AppDomain.CurrentDomain.GetAssemblies().SelectMany(t => t.GetTypes()).Where(t => t.IsSubclassOf(baseType));

            // DataObjectBase 상속 클래스 오브젝트 풀 생성
            foreach (var type in types)
            {
                var objectPool = new DataObjectPool(type);
                _objectPoolDic.Add(type, objectPool);
            }
        }

        /// <summary>
        /// Acquire object pool.
        /// </summary>
        public T Acquire<T>() where T : DataObjectBase, new()
        {
            return _objectPoolDic[typeof(T)].Get<T>();
        }

        /// <summary>
        /// Release object pool.
        /// </summary>
        public bool Release<T>(T obj) where T : DataObjectBase, new()
        {
            try
            {
                _objectPoolDic[typeof(T)].Return(obj);

                return true;
            }
            catch
            {
                return false;
            }
        }
    }
}
