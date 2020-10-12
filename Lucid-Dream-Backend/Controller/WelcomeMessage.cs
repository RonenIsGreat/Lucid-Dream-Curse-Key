using System;

namespace Controller
{
    public class WelcomeMessage
    {
        public static void WriteMessage()
        {
            Console.WriteLine("Enter channels to listen to and then enter 'end'");
            Console.WriteLine("[0] CasStave\n" +
                              "[1] FasTasStave\n" +
                              "[2] PRSStave\n" +
                              "[3] CasBeam\n" +
                              "[4] FasTasBeam\n" +
                              "[5] IDRSBus\n");
        }
    }
}