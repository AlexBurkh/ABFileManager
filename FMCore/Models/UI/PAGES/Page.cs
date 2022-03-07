using System;
using System.Collections.Generic;
using System.IO;
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
        /* ПОЛЯ */
        private string _currentDir;
        /* Статические неизменяемые */
        static readonly int textHeight = 30;    // Количество строк, выделенных под отрисовку контента страницы
        static readonly int pageHeight = 40;    // Количество строк, выделенных под отрисовку окна в целом
        static readonly int pageWidth = 80;     // Количество столбцов, выделенных под отрисовку окна в целом


        /* СВОЙСТВА */
        // Устанавливаются в конструкторе
        CommonBorder CommonBorder { get; set; }     
        PropertiesBorder PropertiesBorder { get; set; }
        // Устанавливаются в методе ScanTreeAndSetupProperties() внутри метода Print()
        // - для возможности динамического иззменения выбранного корневого каталога
        string CurrentDir
        {
            get { return _currentDir; }
            set
            {
                if (Directory.Exists(value))
                {
                    _currentDir = value;
                    FsTree = new FileSystemTree(this.CurrentDir);
                    this.PageContent = new List<string>(FsTree.Tree.Split(new char[] { '\n' }, StringSplitOptions.RemoveEmptyEntries));
                    maxIndex = PageContent.Count - textHeight;
                }
            }
        }       // Если используется сеттер свойства, то кроме того, что изменяется поле _currentDir - еще и заново выстариваются дерево и контент страницы
        FileSystemTree FsTree { get; set; }     // Дерево ФС
        List<string> PageContent { get; set; }      // Контент страницы (строки дерева, которые подлежат размещению на странице)
        public int maxIndex { get; private set; }
        

        /* МЕТОДЫ */
        /* Public */
        public void Print(int currIndex, string currentDirPath = "")
        {
            // Координаты точек окна консоли

            int xTop = 0;
            int yTop = 0;

            int xProp = 0;
            int yProp = 31;

            int xBot = 0;
            int yBot = 39;

            // Определение текущего каталога и установка значения свойств: currentDir и pageContent
            if (!string.IsNullOrWhiteSpace(currentDirPath))
            {
                this.CurrentDir = currentDirPath;
            }

            // Ограничиваем размеры окна
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                Console.SetWindowSize(pageWidth, pageHeight + 5); // +5 - на время работы в студии
                Console.SetBufferSize(pageWidth, pageHeight + 5); // +5 - на время работы в студии      
            }

            /* Отрисовка границ */
            ConsoleUtils.WriteAt(CommonBorder.Draw(), xTop, yTop);       // Отрисовка внешней границы
            ConsoleUtils.WriteAt(PropertiesBorder.Draw(), xProp, yProp);       // Отрисовка границы окна свойств

            Console.SetCursorPosition(xTop, yTop);

            /* Отрисовка контента страницы */
            PrintPageContext(currIndex);

            /* Возвращение картеки */
            Console.SetCursorPosition(xBot, yBot);
        }
        /* Private */
        private void PrintPageContext(int currIndex)
        {
            string[] cuttedPageContent;


            // Высчитываем строки, которые нужно показать
            if ((currIndex + textHeight - 1) < PageContent.Count)
            {
                cuttedPageContent = PageContent.GetRange(currIndex, textHeight - 1).ToArray();
            }
            else
            {
                cuttedPageContent = PageContent.GetRange(currIndex, PageContent.Count - textHeight - 1).ToArray();
            }

            Console.SetCursorPosition(2, 1);
            // Пишем строки в консоль
            for (int i = 0; i < cuttedPageContent.Length; i++)
            {
                if (i == currIndex)
                {
                    ConsoleUtils.WriteColoredAt(cuttedPageContent[i], ConsoleColor.DarkGreen, 2, 1 + i);
                }
                else
                {
                    ConsoleUtils.WriteAt(cuttedPageContent[i], 2, 1 + i);
                }
            }
        }
        

        /* КОНСТРУКТОРЫ */
        public Page(string dir)
        {
            CurrentDir = dir;
            this.CommonBorder = new CommonBorder(pageHeight, pageWidth);
            this.PropertiesBorder = new PropertiesBorder(pageWidth);
        }
    }
}
