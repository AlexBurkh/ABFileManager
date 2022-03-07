using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FMCore.Engine
{
    public static class ConsoleUtils
    {
        public static void WriteColored(string text, ConsoleColor color)
        {
            var prevColor = Console.ForegroundColor;
            Console.ForegroundColor = color;
            Console.Write(text);
            Console.ForegroundColor = prevColor;
        }

        public static void WriteAt(string text, int lOffset, int rOffset)
        {
            (int currLPos, int currRPos) = Console.GetCursorPosition();
            Console.SetCursorPosition(currLPos + lOffset, currRPos + rOffset);
            Console.WriteLine(text);
        }
    }
}
