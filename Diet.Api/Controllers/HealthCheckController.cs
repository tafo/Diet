using Diet.Api.Domain;
using Diet.Api.Infrastructure.Providers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Diet.Api.Controllers
{
    [Route("[controller]")]
    public class HealthCheckController
    {
        [HttpGet]
        public IActionResult Index()
        {
            return new ObjectResult("OK");
        }

        /// <summary>
        /// This is used to check authorization rules
        /// It can be also used to get Id
        /// </summary>
        /// <param name="currentAccount"></param>
        /// <returns></returns>
        [HttpGet("GetAccountId")]
        [Authorize(Roles = Role.Admin)]
        public IActionResult GetAccountId([FromServices]ICurrentAccountProvider currentAccount)
        {
            return new ObjectResult(new
            {
                AccountId = currentAccount.Id
            });
        }
    }
}