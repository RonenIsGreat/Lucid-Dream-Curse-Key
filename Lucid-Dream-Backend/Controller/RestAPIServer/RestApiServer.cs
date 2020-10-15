using System;
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
            config.EnableCors();
            config.Routes.MapHttpRoute("Default", "api/{controller}/{action}");
            config.Formatters.Add(new BrowserJsonFormatter());

            _server = new HttpSelfHostServer(config);
        }

        public async Task<bool> StartAsync()
        {
            try
            {
                await _server.OpenAsync();

            }
            catch(Exception e)
            {
                Console.WriteLine("FAILED to start server with error: "+e.Message);
                return false;
            }

            return true;
        }
    }
}
