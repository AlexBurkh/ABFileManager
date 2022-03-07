using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

using FMCore.Engine;
using FMCore.Models.UI.Borders;

namespace FMCore.Models.UI.PAGES
{
    public class Page
    {
        static readonly int textHeight = 30;
        static readonly int pageHeight = 40;
        static readonly int pageWidth = 180;
        CommonBorder commonBorder { get; set; }
        PropertiesBorder propertiesBorder { get; set; }
        List<string> pageContent { get; set; }

        public void Print(int currIndex)
        {
            // Ограничиваем размеры окна
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                Console.SetWindowSize(pageWidth, pageHeight);
                Console.SetBufferSize(pageWidth, pageHeight);       
            }
            
            Console.WriteLine(commonBorder);
            Console.WriteLine(propertiesBorder);

            string[] cuttedPageContent;

            // Высчитываем строки, которые нужно показать
            if ((currIndex + textHeight - 1) < pageContent.Count)
            {
                cuttedPageContent = pageContent.GetRange(currIndex, currIndex + textHeight).ToArray();
            }
            else
            {
                cuttedPageContent = pageContent.GetRange(currIndex, currIndex + (pageContent.Count - textHeight)).ToArray();
            }

            // Пишем строки в консоль
            for (int i = 0; i < cuttedPageContent.Length; i++)
            {
                ConsoleUtils.WriteAt(cuttedPageContent[i], 2, 2);
            }
        }

        public Page(List<string> content)
        {
            this.commonBorder = new CommonBorder(pageHeight, pageWidth);
            this.propertiesBorder = new PropertiesBorder(pageWidth);
            pageContent = content;
        }
    }
}
