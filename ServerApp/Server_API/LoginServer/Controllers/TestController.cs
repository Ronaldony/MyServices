using Framework.Core.Database.DAO;
using Framework.WebAPI;
using Microsoft.AspNetCore.Mvc;

namespace LoginServer.Controllers
{
    public class TestController : Controller
    {
        private readonly DBUser _dbUser;

        public TestController(DBUser dbUser)
        {
            _dbUser = dbUser;
        }

        [HttpGet]
        [Route("test")]
        public async Task Process()
        {
            // database userlocal data get
            var dboIdentityUserLocal = await _dbUser.SelectUserOrDefualtAsync("KIMJAESEUNG");
            if (null == dboIdentityUserLocal)
            {
                throw new ServerException(1, $"local user not exist. user_id: KIMJAESEUNG");
            }

        }
    }
}
