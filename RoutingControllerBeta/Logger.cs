using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RoutingControllerBeta
{
    class Logger
    {
        public static void timestamp()
        {
            Console.ForegroundColor = ConsoleColor.Magenta;
            Console.Write(" <" + DateTime.Now.ToString("hh:mm:ss:fff") + "> ");
        }
    }
}
