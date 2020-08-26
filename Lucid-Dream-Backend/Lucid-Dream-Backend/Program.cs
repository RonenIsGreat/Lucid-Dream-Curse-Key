

using System.Configuration;

namespace Lucid_Dream_Backend
{
    class Program
    {
        static void Main(string[] args)
        {
           var test = ConfigurationManager.AppSettings["save-path"];

        }
    }
}
