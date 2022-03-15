using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FMCore.Engine
{
    public static class ConsoleUtils
    {
        /// <summary>
        /// Вывод окрашенной строки из параметра text в консоль по координатам из параметра position
        /// </summary>
        /// <param name="text">Текст для вывода на консоль</param>
        /// <param name="position">Координаты старта вывода</param>
        /// <param name="background">Цвет фона в строке</param>
        /// <param name="foreground">Цвет текста в строке</param>
        public static void WriteColoredAt(string text, (int x, int y) position, ConsoleColor background , ConsoleColor foreground = ConsoleColor.White)
        {
            Console.SetCursorPosition(position.x, position.y);
            Console.ForegroundColor = foreground;
            Console.BackgroundColor = background;
            Console.Write(text);
            Console.ResetColor();
        }
        
        /// <summary>
        /// Вывод строки из парметра text в консоль по координатам, указанным в параметре position
        /// </summary>
        /// <param name="text"></param>
        /// <param name="position"></param>
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
