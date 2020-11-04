using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Cors;

namespace AudioCreate.RestAPIController
{
    [EnableCors(origins: "http://localhost:5555", headers: "*", methods: "*")]
    public class ChannelController : ApiController
    {

        [HttpGet]
    
        public IHttpActionResult GetFileNames()
        {
            return Ok(AudioCreation.fileNames);
        }
    }
}
     
   