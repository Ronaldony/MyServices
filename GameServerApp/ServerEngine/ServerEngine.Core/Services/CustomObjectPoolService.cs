using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.ObjectPool;

namespace ServerEngine.Core.Services
{
    using ServerEngine.Core.DataObject;
    using ServerEngine.Core.Services.Interfaces;
    using ServerEngine.Test;

    /// <summary>
    /// CustomObjectPoolService.
    /// </summary>
    public sealed class CustomObjectPoolService : IObjectPoolService
    {
        private readonly ObjectPoolProvider _obejctPoolProvider;

        private Dictionary<Type, ObjectPool<object>> _objectPool;

        public CustomObjectPoolService(IServiceProvider serviceProvider)
        {
            _obejctPoolProvider = serviceProvider.GetRequiredService<ObjectPoolProvider>();
        }

        /// <summary>
        /// Initialize.
        /// </summary>
        public void Initialize()
        {
            // Get DataObjectbase type.
            var baseType = typeof(DataObjectBase);
            var types = AppDomain.CurrentDomain.GetAssemblies().SelectMany(t => t.GetTypes()).Where(t => t.IsSubclassOf(baseType));

            _objectPool = new Dictionary<Type, ObjectPool<object>>();

            // DataObjectBase 상속 클래스 오브젝트 풀 생성
            foreach (var type in types)
            {
                _objectPool.Add(type, _obejctPoolProvider.Create(new DataObjectPolicy(type)));
            }
        }

        /// <summary>
        /// Acquire object pool.
        /// </summary>
        public T Acquire<T>() where T : class, new()
        {
            var obj = _objectPool[typeof(T)].Get();

            return obj as T;
        }

        /// <summary>
        /// Release object pool.
        /// </summary>
        public bool Release<T>(T obj) where T : class, new()
        {
            try
            {
                _objectPool[typeof(T)].Return(obj);

                return true;
            }
            catch
            {
                return false;
            }
        }
    }

    /// <summary>
    /// DataObjectPolicy.
    /// </summary>
    public class DataObjectPolicy : PooledObjectPolicy<object>
    {
        private Type _type;

        public DataObjectPolicy(Type type)
        {
            _type = type;
        }

        public override object Create()
        {
            TestCounter.ObjectPool_Acquire++;

            return Activator.CreateInstance(_type);
        }

        public override bool Return(object obj)
        {
            TestCounter.ObjectPool_Release++;

            if (obj is IResettable resettable)
            {
                return resettable.TryReset();
            }

            return true;
        }
    }
}
