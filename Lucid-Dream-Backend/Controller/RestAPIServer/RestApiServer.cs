using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.SelfHost;

namespace Controller.RestAPIController
{
    public class RestApiServer
    {
        private HttpSelfHostServer _server;
        public RestApiServer(int port)
        {
            string restApiAdress = $"http://localhost:{port}";

            var config = new HttpSelfHostConfiguration(restApiAdress);
            config.Routes.MapHttpRoute("Default", "api/{controller}/{action}");
            config.Formatters.Add(new BrowserJsonFormatter());

            _server = new HttpSelfHostServer(config);
        }
    }
}
