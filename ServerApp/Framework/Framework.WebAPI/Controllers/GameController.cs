using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Mvc;

namespace Framework.WebAPI.Controllers
{
	using Framwork.Core.Session;

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
		public GameSession GameSession { get; set; }

		/// DI
		private readonly ILogger<GameController> _logger;
		private readonly IServiceProvider _serviceProvider;

		public GameController(ILogger<GameController> logger)
		{
			_logger = logger;
		}
	}
}
