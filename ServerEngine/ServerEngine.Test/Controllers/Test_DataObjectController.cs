using Microsoft.AspNetCore.Mvc;
using System.Transactions;

namespace ServerEngine.Test.Controllers
{
    using GameCore.Datas.Player;
    using ServerEngine.Core.Util;
    using ServerEngine.Database.DBObject.Connections;
    using ServerEngine.Test.Database.DataObject;

    /// <summary>
    /// Test_Controller.
    /// </summary>
    public class Test_DataObjectController : Controller
    {
        private readonly ILogger<Test_DataObjectController> _logger;
        private readonly DBPlayerInfo _dbPlayerInfo;
        private readonly CouchbaseConnection _conn;

        public Test_DataObjectController(
            ILogger<Test_DataObjectController> logger,
            DBPlayerInfo dbPlayerInfo,
            CouchbaseConnection conn)
        {
            _logger = logger;
            _dbPlayerInfo = dbPlayerInfo;
            _conn = conn;
        }

        [HttpGet]
        [Route("test-dataobject")]
        public async Task<string> Process()
        {
            using (var ts = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
            {
                try
                {
                    var ss = await _conn.TransactionAsync(async (ctx) =>
                    {
                        await ctx.UpsertAsync(_dbPlayerInfo, "TestPid4", new DTOPlayerInfo
                        {
                            Pid = "TestPid",
                            PlayerName = "Test name",
                            RegTime = TimeUtil.Now,
                            Uid = "aa"
                        });

                        //var result = await ctx.QueryAsync<dynamic>(
                        //    query,
                        //    options => options
                        //        .AdHoc(false)
                        //        .Parameter("key", "TestPid")
                        //        .Parameter("value", new DTOPlayerInfo
                        //        {
                        //            Pid = "TestPid",
                        //            PlayerName = "Test name",
                        //            RegTime = TimeUtil.Now,
                        //            Uid = "aa"
                        //        }));

                    });

                }
                catch (Exception ex)
                {
                    _logger.LogError(ex.Message);
                }

                return "Ok";
            }
        }
    }
}
