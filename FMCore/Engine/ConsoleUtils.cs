using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FMCore.Engine
{
    public static class ConsoleUtils
    {
        public static void WriteColoredAt(string text, (int x, int y) position, ConsoleColor background , ConsoleColor foreground = ConsoleColor.White)
        {
            Console.SetCursorPosition(position.x, position.y);
            Console.ForegroundColor = foreground;
            Console.BackgroundColor = background;
            Console.Write(text);
            Console.ResetColor();
        }

        public static void WriteAt(string text, (int x, int y) position)
        {
            Console.SetCursorPosition(position.x, position.y);
            int lineLength = Models.UI.Pages.Page.PageWidth - 4;
            if (text.Length < lineLength)
            {
                Console.Write(text);
            }
            else
            {
                Console.Write(text.Substring(0, lineLength));
            }
            
        }
    }
}
