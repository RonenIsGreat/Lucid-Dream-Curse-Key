using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http;

namespace AudioCreate.RestAPIController
{
    public class ChannelController : ApiController
    {

        [HttpGet]
    
        public IHttpActionResult GetFileNames()
        {
            if (AudioCreation.fileNames.Count == 0)
                return NotFound();
            return Ok(AudioCreation.fileNames);
        }
    }
}
     
   