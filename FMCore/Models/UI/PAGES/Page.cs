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
        static readonly ConsoleColor background = ConsoleColor.Blue;    // Фоновый цвет консоли
        static readonly int textHeight = 30;                            // Количество строк, выделенных под отрисовку контента страницы
        static readonly int porpertiesHeight = 9;                       // количество строк (вместе с границей) для окна свойств
        static readonly int pageHeight = 40;                            // Количество строк, выделенных под отрисовку окна в целом
        static readonly int pageWidth = 120;                             // Количество столбцов, выделенных под отрисовку окна в целом


        /* СВОЙСТВА */
        // Устанавливаются в конструкторе
        CommonBorder CommonBorder { get; set; }
        PropertiesBorder PropertiesBorder { get; set; }
        public static int TextHeight
        {
            get { return textHeight; }
        }
        /* Постоянные на время вызова функции */
        FileSystemTree FsTree { get; set; }                             // Дерево ФС
        List<string> PageContent { get; set; }                          // Контент страницы (строки дерева, которые подлежат размещению на странице)
        public int maxIndex { get; private set; }                       // Максимально возможный индекс на странице
        /* Оперативные (могут изменяться втечении выполенния функции) */
        private int CurrentIndex { get; set; }                          // Текущий индекс (выбираемый стрелками элемент)
        public string CurrentDir
        {
            get { return _currentDir; }
            set
            {
                if (Directory.Exists(value))
                {
                    this._currentDir = value;
                    this.FsTree = new FileSystemTree(this.CurrentDir);
                    this.PageContent = new List<string>(FsTree.LoadTree(value).Split(new char[] { '\n' }, StringSplitOptions.RemoveEmptyEntries));
                    this.maxIndex = PageContent.Count - 1;
                }
            }
        }                                      // Если используется сеттер , то еще и заново выстариваются дерево и контент страницы
        private string[] CurrentCuttedPageContent;
        /* Координаты точек окна консоли */
        // Точки для рисования границ
        (int x, int y) topBorderCoordinates = (0, 0);
        (int x, int y) propBorderCoordinates = (0, Page.pageHeight - Page.porpertiesHeight); // 31
        // Точки для отрисовки контента страниц
        (int x, int y) topContentCoordinates = (2, 1);
        (int x, int y) propContentCoordinates = (36, Page.pageHeight - Page.porpertiesHeight + 2);


        /* МЕТОДЫ */
        /* Public */
        public void Print(int currIndex, string currentDirPath = "")
        {
            this.CurrentIndex = currIndex;
            Console.BackgroundColor = Page.background;

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
            ConsoleUtils.WriteColoredAt(CommonBorder.Draw(), (topBorderCoordinates.x, topBorderCoordinates.y), Page.background);       // Отрисовка внешней границы
            ConsoleUtils.WriteColoredAt(PropertiesBorder.Draw(), (propBorderCoordinates.x, propBorderCoordinates.y), Page.background); // Отрисовка границы окна свойств

            /* Отрисовка контента страницы */
            PrintPageContent(topContentCoordinates, propContentCoordinates, background);
        }
        public string GetSelectedItem()
        {
            string[] cuttedPageContent = (PageContent.Count > Page.textHeight) ? new string[Page.textHeight] : new string[PageContent.Count];   // Массив для текущих отображаемых элементов string[30] индексы от 0 до 29

            // Высчитываем строки, которые нужно показать
            if (this.CurrentIndex < textHeight)                                                                              // && (PageContent.Count >= (textHeight - 1)))
            {
                if (PageContent.Count < Page.textHeight)
                {
                    cuttedPageContent = PageContent.GetRange(0, PageContent.Count).ToArray();
                }
                else
                {
                    cuttedPageContent = PageContent.GetRange(0, textHeight).ToArray();
                }

                for (int i = 0; i < cuttedPageContent.Length; i++)
                {
                    if (i == this.CurrentIndex)
                    {
                        return GetFullNameFromContentString(cuttedPageContent[i]);
                    }
                }
            }
            else
            {
                cuttedPageContent = PageContent.GetRange(((this.CurrentIndex + 1) - textHeight), textHeight).ToArray();
                return GetFullNameFromContentString(cuttedPageContent.Last());
                /*for (int i = 0; i < cuttedPageContent.Length; i++)
                {
                    if (i == (this.currentIndex -1))
                    {
                        return GetFullNameFromContentString(cuttedPageContent[i]);
                    }
                }*/
            }
            return string.Empty;
        }
        public bool IsDirectory(string fileName)
        {
            FileSystemInfo fsItem = (Directory.Exists(fileName)) ? new DirectoryInfo(fileName) : new FileInfo(fileName);
            return (fsItem.Attributes & FileAttributes.Directory) == FileAttributes.Directory;
        }

        /* Private */
        private void PrintPageContent((int xTop, int yTop) topCoordinates, (int xProp, int yProp) propCoordinates, ConsoleColor background)
        {
            // Массив текущих строк контента страницы
            this.CurrentCuttedPageContent = (PageContent.Count > Page.textHeight) ? new string[Page.textHeight] : new string[PageContent.Count];

            // Высчитываем строки, которые нужно показать
            if (this.CurrentIndex < textHeight)                                                                      
            {
                if (PageContent.Count < Page.textHeight)
                {
                    this.CurrentCuttedPageContent = PageContent.GetRange(0, PageContent.Count).ToArray();
                }
                else
                {
                    this.CurrentCuttedPageContent = PageContent.GetRange(0, textHeight).ToArray();
                }

                for (int i = 0; i < this.CurrentCuttedPageContent.Length; i++)
                {
                    ConsoleColor foreground = (i == this.CurrentIndex) ? ConsoleColor.DarkRed : ColorFilesAndDirs(GetFullNameFromContentString(this.CurrentCuttedPageContent[i]));
                    ConsoleUtils.WriteColoredAt(this.CurrentCuttedPageContent[i], (topCoordinates.xTop, topCoordinates.yTop + i), background, foreground);
                }
                if (this.CurrentCuttedPageContent.Length > 0)
                {
                    PrintFSItemProperty(GetFullNameFromContentString(this.CurrentCuttedPageContent[this.CurrentIndex]), propCoordinates, background);
                }
            }
            // Если нужно изменять выборку строк (при выхода за пределы окна щелчками "вниз")
            else
            {
                this.CurrentCuttedPageContent = PageContent.GetRange(((this.CurrentIndex + 1) - textHeight), textHeight).ToArray();
                for (int i = 0; i < this.CurrentCuttedPageContent.Length; i++)
                {
                    ConsoleColor foreground = (i == (this.CurrentCuttedPageContent.Length - 1)) ? ConsoleColor.DarkRed : ColorFilesAndDirs(GetFullNameFromContentString(this.CurrentCuttedPageContent[i]));
                    ConsoleUtils.WriteColoredAt(this.CurrentCuttedPageContent[i], (topCoordinates.xTop, topCoordinates.yTop + i), background, foreground);
                }
                if (this.CurrentCuttedPageContent.Length > 0)
                {
                    PrintFSItemProperty(GetFullNameFromContentString(this.CurrentCuttedPageContent[this.CurrentCuttedPageContent.Length - 1]), propCoordinates, background);
                }
            }
        }
        private void PrintFSItemProperty(string fullFSItemName, (int xPos, int yPos) propCoordinates, ConsoleColor backgound)
        {
            FileSystemInfo fsInfo = (Directory.Exists(fullFSItemName)) ? new DirectoryInfo(fullFSItemName) : new FileInfo(fullFSItemName);
            string[] propertyStrings = new string[] { $"{fsInfo.Name}\n",
                                                      $"{fsInfo.CreationTime}\n",
                                                      $"{fsInfo.LastWriteTime}\n",
                                                      $"{fsInfo.LastAccessTime}\n" };
            for (int i = 0; i < propertyStrings.Length; i++)
            {
                ConsoleUtils.WriteColoredAt(propertyStrings[i], (propCoordinates.xPos, propCoordinates.yPos + i), backgound);
            }
        }
        private string GetFullNameFromContentString(string contentString)
        {
            return contentString.Split(new char[] { '─' }, StringSplitOptions.RemoveEmptyEntries).Last().Trim();
        }
        private ConsoleColor ColorFilesAndDirs(string path)
        {
            if (Directory.Exists(path))
            {
                return ConsoleColor.Yellow;
            }
            else
            {
                return ConsoleColor.Black;
            }
        }


        /* КОНСТРУКТОРЫ */
        public Page()
        {
            CurrentDir = dir;
            this.CommonBorder = new CommonBorder(Page.pageHeight, Page.pageWidth);
            this.PropertiesBorder = new PropertiesBorder(Page.porpertiesHeight , Page.pageWidth);
        }
    }
}
