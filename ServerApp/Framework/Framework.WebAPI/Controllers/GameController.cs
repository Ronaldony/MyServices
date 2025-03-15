using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Framework.WebAPI.Controllers
{
    using Framwork.Core.Session;
    using ServerEngine.Database.Interfaces;

    /// <summary>
    /// GameController
    /// </summary>
    public abstract partial class GameController : Controller, IGameController
	{
		/// <summary>
		/// UserId
		/// </summary>
		public string UserId { get; private set; }

		/// <summary>
		/// unique id
		/// </summary>
		public string Uid { get; private set; }

		/// <summary>
		/// unique id
		/// </summary>
		public string Pid { get; private set; }

		/// <summary>
		/// IPAddress
		/// </summary>
		public string Address { get; set; }

		/// <summary>
		/// Game session.
		/// </summary>
		public SessionInfo GameSession { get; set; }

		private Dictionary<Type, IDataObject> _dataObjectDict;

		/// DI
		private readonly ILogger<GameController> _logger;
		private readonly IServiceProvider _serviceProvider;

		public GameController(IServiceProvider serviceProvider)
		{
			_serviceProvider = serviceProvider;
            _logger = serviceProvider.GetRequiredService<ILogger<GameController>>();

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
