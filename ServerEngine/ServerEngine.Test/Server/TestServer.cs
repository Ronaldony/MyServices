using System.Net.Sockets;

namespace ServerEngine.Test.Server
{
    using ServerEngine.Network.UDP;
    using ServerEngine.Test.Network;

    /// <summary>
    /// TestServer.
    /// </summary>
    public class TestServer : UdpServer
	{
		private readonly ILogger<UdpServer> _logger;

		/// <summary>
		/// Game session dictionary.
		/// key: session uid.
		/// value: game session.
		/// </summary>
		private readonly Dictionary<ulong, GameSession> _gameSessionDict;

		public TestServer(IServiceProvider serviceProvider)
			: base(serviceProvider)
		{
			_logger = serviceProvider.GetRequiredService<ILogger<UdpServer>>();

			_gameSessionDict = new Dictionary<ulong, GameSession>();
		}

		/// <summary>
		/// OnRequestConnection.
		/// </summary>
		protected override void OnConnected(int sessionId, ulong sessionUid)
		{
			try
			{
				// 게임 세션 추가.
				_gameSessionDict.Add(
					sessionUid,
					new GameSession
					{
						SessionId = sessionId,
						SessionUid = sessionUid,
						Uid = string.Empty,
						Pid = string.Empty,
					});
			}
			catch (Exception ex)
			{
				// 실패 시 Disconnection.
				_logger.LogError(ex.Message);
				Disconnect(sessionId, sessionUid);
				return;
			}

			// TODO: Monitoring user indicator.
		}

		/// <summary>
		/// OnDisconnected.
		/// </summary>
		protected override void OnDisconnected(int sessionId, ulong sessionUid)
		{
			// 게임 세션 제거.
			if (false == _gameSessionDict.Remove(sessionUid))
			{
				_logger.LogError($"[TestServer] Disconnection failed. sid: {sessionId} , suid:  {sessionUid}");
			}
			else
			{
				_logger.LogDebug($"[TestServer] Disconneted. sid: {sessionId}, suid: {sessionUid}");
			}

			// TODO: Monitoring user indicator.
		}

		/// <summary>
		/// OnNetError.
		/// </summary>
		protected override void OnNetError(SocketError sockError)
		{
			_logger.LogError($"[TestServer] Network error. error: {sockError}");
		}
	}
}