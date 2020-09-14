using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http;
using System.Net.Http;

namespace Controller.RestAPIController
{
    public class RestApiController : ApiController
    {
        [HttpPost]

        public IHttpActionResult sendActiveList(string[] activeList)
        {
            return Ok();
        }
    }
}
