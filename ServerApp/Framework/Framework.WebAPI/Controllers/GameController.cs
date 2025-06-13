using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;

namespace Framework.WebAPI.Controllers
{
    using Framework.Core.Database.Interfaces;

    /// <summary>
    /// GameController
    /// </summary>
    public abstract partial class GameController : Controller, IGameController
    {
        private readonly IServiceProvider _serviceProvider;

        public string Uid { get; set; }

        public string Pid { get; set; }

        /// <summary>
        /// IPAddress
        /// </summary>
        public string Address { get; set; }

		/// <summary>
		/// Data obejct dictionary.
		/// key: Type,
		/// value: DataObject interface,
		/// </summary>
        public Dictionary<Type, IDataObject> DataObjectDict => _dataObjectDict;
		private Dictionary<Type, IDataObject> _dataObjectDict;


        public GameController(IServiceProvider serviceProvider)
		{
			_serviceProvider = serviceProvider;

			_dataObjectDict = new Dictionary<Type, IDataObject>();
        }

		/// <summary>
		/// Get object.
		/// </summary>
		protected async Task<T> GetObjectAsync<T>() where T: class, IDataObject
		{
			var type = typeof(T);

			if (false == _dataObjectDict.ContainsKey(type))
			{
				var dataObject = _serviceProvider.GetRequiredService<T>();
				_dataObjectDict.Add(type, dataObject);
            }

			await _dataObjectDict[type].LoadDataAsync(Pid);

            return _dataObjectDict[type] as T;
		}
    }
}
