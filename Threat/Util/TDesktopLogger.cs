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
        private static bool debug = true;
        public static bool SetDebug { get=>debug; set=>debug=value; }
        public static TDesktopLogger Default = new TDesktopLogger();
        public void Debug(string message)
        {
            if (!debug) return;
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine(message + '\r');         
            Console.ResetColor();
        }
        public void Error(string message)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine(message + '\r');
            Console.Error.WriteLine(message);
            Console.ResetColor();
        }

        public void Info(string message)
        {
            Console.ForegroundColor = ConsoleColor.White;
            Console.WriteLine(message + '\r');
        }

        public void Warn(string message)
        {
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine(message + '\r');
            Console.Error.WriteLine(message);
        }
    }
}
