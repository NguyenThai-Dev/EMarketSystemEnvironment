using System.Web.Http;

namespace EMarket.ApiControllers.Admin
{
    [Authorize]
    [AllowAnonymous]
    [RoutePrefix("api/test")]
    public class TestController : ApiController
    {
        [HttpGet]
        [Route("hello")]
        public IHttpActionResult Hello() => Ok("Hệ thống ổn!");
    }
}
