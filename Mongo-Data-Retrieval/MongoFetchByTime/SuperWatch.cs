using System;
using System.Diagnostics;

namespace MongoFetchByTime
{
    public class SuperWatch : IDisposable
    {
        static Stopwatch Watch = new Stopwatch();
        static SuperWatch()
        {
            Watch.Start();
        }

        TimeSpan Start;
        public SuperWatch()
        {
            Start = Watch.Elapsed;
        }

        public void Dispose()
        {
            TimeSpan elapsed = Watch.Elapsed - Start;
            Console.WriteLine($"Time elapsed: {elapsed}");
        }
    }
}
