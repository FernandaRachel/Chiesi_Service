using Chiesi;
using Chiesi.Log;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ConsoleTest
{
    class Program
    {
        static void Main(string[] args)
        {
            Logic.DontStopMeNOW = true;
            Logic.MainLogic(new string[0]);

        }


    }
}
