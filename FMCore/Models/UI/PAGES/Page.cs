using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

using FMCore.Engine;
using FMCore.Models.CatalogTree;
using FMCore.Models.UI.Borders;

namespace FMCore.Models.UI.Pages
{
    public class Page
    {
        static readonly int textHeight = 30;
        static readonly int pageHeight = 40;
        static readonly int pageWidth = 80;
        CommonBorder commonBorder { get; set; }
        PropertiesBorder propertiesBorder { get; set; }
        FileSystemTree tree { get; set; }
        List<string> pageContent { get; set; }

        string currentDir { get; set; }

        public void Print(int currIndex)
        {
            int xTop = 0;
            int yTop = 0;

            int xProp = 0;
            int yProp = 31;

            int xBot = 0;
            int yBot = 39;

            // Ограничиваем размеры окна
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                Console.SetWindowSize(pageWidth, pageHeight);
                Console.SetBufferSize(pageWidth, pageHeight);       
            }

            string commonBorderString = commonBorder.Draw();
            string propBorderString = propertiesBorder.Draw();

            ConsoleUtils.WriteAt(commonBorderString, xTop, yTop);
            ConsoleUtils.WriteAt(propBorderString, xProp, yProp);
            Console.SetCursorPosition(xTop, yTop);

            string[] cuttedPageContent;

            // Высчитываем строки, которые нужно показать
            if ((currIndex + textHeight - 1) < pageContent.Count)
            {
                cuttedPageContent = pageContent.GetRange(currIndex, textHeight - 1).ToArray();
            }
            else
            {
                cuttedPageContent = pageContent.GetRange(currIndex, pageContent.Count - textHeight - 1).ToArray();
            }

            Console.SetCursorPosition(2, 2);
            // Пишем строки в консоль
            for (int i = 0; i < cuttedPageContent.Length; i++)
            {
                ConsoleUtils.WriteAt(cuttedPageContent[i], 2, 1 + i);
            }

            Console.SetCursorPosition(xBot, yBot);
        }

        public Page(string dir)
        {
            currentDir = dir;
            tree = new FileSystemTree(this.currentDir);
            this.pageContent = new List<string>(tree.Tree.Split(new char[] { '\n' }, StringSplitOptions.RemoveEmptyEntries));
            this.commonBorder = new CommonBorder(pageHeight, pageWidth);
            this.propertiesBorder = new PropertiesBorder(pageWidth);
        }
    }
}
