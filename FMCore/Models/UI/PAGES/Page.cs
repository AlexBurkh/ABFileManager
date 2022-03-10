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
    internal class Page
    {
        /* ПОЛЯ */
        private string _currentDir;
        /* Константы  */
        private static readonly ConsoleColor    _background                 =    ConsoleColor.Blue;         // Фоновый цвет консоли
        private static readonly int             _textHeight                 =    30;                        // Количество строк, выделенных под отрисовку контента страницы
        private static readonly int             _porpertiesHeight           =    9;                         // количество строк (вместе с границей) для окна свойств
        private static readonly int             _pageHeight                 =    40;                        // Количество строк, выделенных под отрисовку окна в целом
        private static readonly int             _pageWidth                  =    120;                       // Количество столбцов, выделенных под отрисовку окна в целом

        // Важные используемые на странице сущности
        private CommonBorder                    _commonBorder;                                              // Внешняя граница окна
        private PropertiesBorder                _propertiesBorder;                                          // Граница окна свойств
        private List<string>                    _pageContent;                                               // Отображаемая на этой странице часть дерева

        private int                             _currentIndex;                                              // Индекс текущего выбранного элемента
        private string                          _selectedItem               =    string.Empty;              // Элемент, на котором на данный момент находится указатель пользователя

        // Точки для отрисовки границ страницы
        (int x, int y)                          _topBorderCoor              =    (0, 0);
        (int x, int y)                          _propBorderCoord            =    (0, _pageHeight - _porpertiesHeight); // 31

        // Точки для отрисовки контента страницы
        (int x, int y)                          _topContentCoord            =    (2, 1);
        (int x, int y)                          _propContentCoord           =    (36, _pageHeight - _porpertiesHeight + 2); // 33

        // Точки для отрисовки статус-бара
        (int x, int y)                          _statusBarCoord             =    (2, 39);


        /* СВОЙСТВА */
        internal List<string> PageContent
        {
            get { return _pageContent; }
            set { _pageContent = value; }
        }
        public static int TextHeight
        {
            get { return _textHeight; }
        }


        /* МЕТОДЫ */
        /* Static */
        public static bool IsDirectory(string fileName)
        {
            FileSystemInfo fsItem = (Directory.Exists(fileName)) ? new DirectoryInfo(fileName) : new FileInfo(fileName);
            return (fsItem.Attributes & FileAttributes.Directory) == FileAttributes.Directory;
        }
        /* Public */
        public void          Print(int currIndex)
        {
            _currentIndex = currIndex;
            _selectedItem = _pageContent[_currentIndex];
            Console.BackgroundColor = Page._background;

            // Ограничиваем размеры окна
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                Console.SetWindowSize(_pageWidth, _pageHeight + 5); // +5 - на время работы в студии
                Console.SetBufferSize(_pageWidth, _pageHeight + 5); // +5 - на время работы в студии      
            }

            /* Отрисовка границ */
            ConsoleUtils.WriteColoredAt(_commonBorder.Draw(), _topBorderCoor, Page._background);       // Отрисовка внешней границы
            ConsoleUtils.WriteColoredAt(_propertiesBorder.Draw(), _propBorderCoord, Page._background); // Отрисовка границы окна свойств

            /* Отрисовка контента страницы */
            PrintPageContent();
        }
        public string        GetSelectedItem()
        {
            return _selectedItem;
        }
        public (bool, int)   IsOnPage(string fSItem)
        {
            for (int i = 0; i < _pageContent.Count(); i++)
            {
                if (_pageContent[i] == fSItem)
                {
                    return (true, i);
                }
            }
            return (false, -1);
        }

        /* Private */
        private void            PrintPageContent()
        {
            for (int i = 0; i < _pageContent.Count; i++)
            {
                ConsoleColor foreground = (i == _currentIndex) ? ConsoleColor.DarkRed : ColorFilesAndDirs(GetFullNameFromContentString(_pageContent[i]));
                ConsoleUtils.WriteColoredAt(_pageContent[i], (_topContentCoord.x, _topContentCoord.y + i), _background, foreground);
            }
            if (_pageContent.Count() > 0)
            {
                PrintFSItemProperty(GetFullNameFromContentString(_pageContent[_currentIndex]));
            }
        }
        private void            PrintFSItemProperty(string fullFSItemName)
        {
            FileSystemInfo fsInfo = (Directory.Exists(fullFSItemName)) ? new DirectoryInfo(fullFSItemName) : new FileInfo(fullFSItemName);
            string[] propertyStrings = new string[] { $"{fsInfo.Name}\n",
                                                      $"{fsInfo.CreationTime}\n",
                                                      $"{fsInfo.LastWriteTime}\n",
                                                      $"{fsInfo.LastAccessTime}\n" };
            for (int i = 0; i < propertyStrings.Length; i++)
            {
                ConsoleUtils.WriteColoredAt(propertyStrings[i], (_propContentCoord.x, _propContentCoord.y + i), _background);
            }
            Console.SetCursorPosition(_statusBarCoord.x, _statusBarCoord.y);
        }
        internal string         GetFullNameFromContentString(string contentString)
        {
            return contentString.Split(new char[] { '─' }, StringSplitOptions.RemoveEmptyEntries).Last().Trim();
        }
        private ConsoleColor    ColorFilesAndDirs(string path)
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
            this._commonBorder = new CommonBorder(_pageHeight, _pageWidth);
            this._propertiesBorder = new PropertiesBorder(_porpertiesHeight , _pageWidth);
        }
    }
}
