using Microsoft.AspNetCore.Mvc;

namespace Framework.WebAPI.Controllers
{
    using ServerEngine.Database.Interfaces;

    /// <summary>
    /// GameController
    /// </summary>
    public abstract partial class GameController : Controller, IGameController
    {
        private readonly IServiceProvider _serviceProvider;

        /// <summary>
        /// IPAddress
        /// </summary>
        public string Address { get; set; }

		/// <summary>
		/// Data obejct dictionary.
		/// key: Type,
		/// value: DataObject interface,
		/// </summary>
		private Dictionary<Type, IDataObject> _dataObjectDict;

		public GameController(IServiceProvider serviceProvider)
		{
			_serviceProvider = serviceProvider;

			_dataObjectDict = new Dictionary<Type, IDataObject>();
        }

		/// <summary>
		/// Get object.
		/// </summary>
		protected T GetObject<T>() where T: class, IDataObject
		{
			var type = typeof(T);

			if (_dataObjectDict.ContainsKey(type))
			{
				return _dataObjectDict[type] as T;
            }

			var dataObject = Activator.CreateInstance(type, _serviceProvider) as T;
			_dataObjectDict.Add(type, dataObject);

            return dataObject;
		}
    }
}
