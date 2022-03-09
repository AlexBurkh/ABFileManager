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
        private int _currentIndex;
        private string _currentDir;
        private List<string> _currentCuttedPageContent;
        /* Статические неизменяемые */
        static readonly ConsoleColor background = ConsoleColor.Blue;    // Фоновый цвет консоли
        static readonly int TextHeight = 30;                            // Количество строк, выделенных под отрисовку контента страницы
        static readonly int porpertiesHeight = 9;                       // количество строк (вместе с границей) для окна свойств
        static readonly int pageHeight = 40;                            // Количество строк, выделенных под отрисовку окна в целом
        static readonly int pageWidth = 120;                             // Количество столбцов, выделенных под отрисовку окна в целом


        /* СВОЙСТВА */
        CommonBorder CommonBorder { get; set; }
        PropertiesBorder PropertiesBorder { get; set; }
        FileSystemTree FsTree { get; set; }                             // Дерево ФС
        List<string> PageContent { get; set; }                          // Контент страницы (строки дерева, которые подлежат размещению на странице)
        public int MaxIndex { get; private set; }                       // Максимально возможный индекс на странице
        private int CurrentIndex
        {
            get
            {
                return _currentIndex;
            }
            set
            {
                _currentIndex = (value >= 0) ? value : 0;
            }
        }                                     // Текущий индекс элемента (выбирается стрелками и управляется вне сборки)
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
                    this.MaxIndex = PageContent.Count - 1;
                }
            }
        }                                     // Если используется сеттер , то еще и заново выстариваются дерево и контент страницы
        private List<string> CurrentCuttedPageContent
        {
            get
            {
                return _currentCuttedPageContent;
            }
            set
            {
                _currentCuttedPageContent = value;
            }
        }                // Текущий обрезанный по рамкам окна контент
        
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
            PrintPageContent();
        }
        public string GetSelectedElement()
        {
            string[] cuttedPageContent = (PageContent.Count > Page.TextHeight) ? new string[Page.TextHeight] : new string[PageContent.Count];   // Массив для текущих отображаемых элементов string[30] индексы от 0 до 29

            // Высчитываем строки, которые нужно показать
            if (this.CurrentIndex < TextHeight)                                                                              // && (PageContent.Count >= (textHeight - 1)))
            {
                if (PageContent.Count < Page.TextHeight)
                {
                    cuttedPageContent = PageContent.GetRange(0, PageContent.Count).ToArray();
                }
                else
                {
                    cuttedPageContent = PageContent.GetRange(0, TextHeight).ToArray();
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
                cuttedPageContent = PageContent.GetRange(((this.CurrentIndex + 1) - TextHeight), TextHeight).ToArray();
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

        /* Private */
        private void PrintPageContent()
        {
            int selectedItemIndex = -1;

            if (CurrentCuttedPageContent == null)
            {
                if (PageContent.Count > TextHeight)
                {
                    CurrentCuttedPageContent = PageContent.GetRange(0, TextHeight);
                    PageContent.RemoveRange(0, TextHeight);
                }
                else
                {
                    PageContent.GetRange(0, PageContent.Count);
                    PageContent.RemoveRange(0, PageContent.Count);
                }
            }
            
            for (int i = 0; i < CurrentCuttedPageContent.Count; i++)
            {
                if (CurrentCuttedPageContent[i] == PageContent[CurrentIndex])
                {
                    selectedItemIndex = i;
                    break;
                }
            }

            if (selectedItemIndex == -1)
            {
                CurrentCuttedPageContent.RemoveAt(0);
                CurrentCuttedPageContent.Add(PageContent[0]);
                PageContent.RemoveAt(0);
                PrintPageContent();
            }
            else
            {
                for (int i = 0; i < this.CurrentCuttedPageContent.Count; i++)
                {
                    ConsoleColor foreground = (i == selectedItemIndex) ? ConsoleColor.DarkRed : ColorFilesAndDirs(GetFullNameFromContentString(this.CurrentCuttedPageContent[i]));
                    ConsoleUtils.WriteColoredAt(this.CurrentCuttedPageContent[i], (topContentCoordinates.x, topContentCoordinates.y + i), background, foreground);
                }
                if (this.CurrentCuttedPageContent.Count > 0)
                {
                    PrintFSItemProperty(GetFullNameFromContentString(this.CurrentCuttedPageContent[selectedItemIndex]), propContentCoordinates, background);
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
        private int ComputeLocalIndex()
        {
            return (CurrentIndex < TextHeight) ? CurrentIndex : CurrentIndex - TextHeight * (int)(Math.Floor(Convert.ToDouble(CurrentIndex)) / Convert.ToDouble(TextHeight));
        }


        /* КОНСТРУКТОРЫ */
        public Page(string dir)
        {
            CurrentDir = dir;
            this.CommonBorder = new CommonBorder(Page.pageHeight, Page.pageWidth);
            this.PropertiesBorder = new PropertiesBorder(Page.porpertiesHeight , Page.pageWidth);
        }
    }
}
