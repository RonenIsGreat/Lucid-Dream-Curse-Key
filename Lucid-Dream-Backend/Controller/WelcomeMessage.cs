﻿using System;

namespace Controller
{
    public class WelcomeMessage
    {
        public static void WriteMessage()
        {
            Console.WriteLine("Enter channels to listen to and then enter 'end'");
            Console.WriteLine("[0] CasBeam\n" +
                              "[1] CasStave\n" +
                              "[2] FasTasBeam\n" +
                              "[3] FasTasStave\n" +
                              "[4] PRSStave\n" +
                              "[5] IDRSBus\n");
        }
    }
}