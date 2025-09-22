namespace ServerEngine.Test.Database.DataObject
{
    using Couchbase.Client.Transactions;
    using GameCore.Datas.Player;
    using ServerEngine.Database.DataObject;
    using ServerEngine.Test.ConfigManager;

    public class DBPlayerInfo : DBObject_Couchbase
    {
        private readonly ILogger<DBPlayerInfo> _logger;

        public DBPlayerInfo(IServiceProvider serviceProvider)
            : base(
                  serviceProvider,
                  GameConfigManager.GetCouchbaseConnection("PlayerInfo")
                  )
        {
            _logger = serviceProvider.GetRequiredService<ILogger<DBPlayerInfo>>();
        }

        /// <summary>
        /// Select async.
        /// </summary>
        public async Task<DTOPlayerInfo> SelectAsync(string pid)
        {
            return await base.SelectAsync<DTOPlayerInfo>(pid);
        }

        /// <summary>
        /// Upsert async.
        /// </summary>
        public async Task<bool> UpsertAsync(string pid, DTOPlayerInfo dataObject)
        {
            return await base.UpsertAsync(pid, dataObject);
        }
    }
}