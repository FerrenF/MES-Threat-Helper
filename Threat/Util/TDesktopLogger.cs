using Microsoft.VisualBasic.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MESHelper.Threat.Util
{
    public class TDesktopLogger : TLogInterface
    {
        private static bool debug = false;
        public static bool SetDebug { get=>debug; set=>debug=value; }
        public void Debug(string message)
        {
            if (!debug) return;
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine(message + "\n");
            Console.ResetColor();
        }
        public void Error(string message)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine(message + "\n");
            Console.Error.WriteLine(message + "\n");
            Console.ResetColor();
        }

        public void Info(string message)
        {
            Console.ForegroundColor = ConsoleColor.White;
            Console.WriteLine(message + "\n");
        }

        public void Warn(string message)
        {
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine(message + "\n");
            Console.Error.WriteLine(message + "\n");
        }
    }
}
