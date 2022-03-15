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
    /// <summary>
    /// Обеспечивает отрисовку контента страницы
    /// </summary>
    internal class Page
    {
        /* КОНСТРУКТОРЫ */
        public Page(int text_height)
        {
            _textHeight = text_height;
            this._header = new Header(_headerHeight, _pageWidth);
            this._commonBorder = new CommonBorder(_pageHeight, _pageWidth);
            this._propertiesBorder = new PropertiesBorder(_porpertiesHeight, _pageWidth);
        }

        /* ПОЛЯ */
        /* Константы  */
        private static readonly ConsoleColor    _background                 =    ConsoleColor.Blue;         // Фоновый цвет консоли
        private static int                      _textHeight                 =    30;                        // Количество строк, выделенных под отрисовку контента страницы
        private static readonly int             _porpertiesHeight           =    9;                         // количество строк (вместе с границей) для окна свойств
        private static readonly int             _headerHeight               =    3;                         // Количество строк, выделенных под заголовок
        private static readonly int             _pageHeight                 =    _textHeight + 10;          // Количество строк, выделенных под отрисовку окна в целом
        private static readonly int             _pageWidth                  =    200;                       // Количество столбцов, выделенных под отрисовку окна в целом

        // Важные используемые на странице сущности
        private Header                          _header;                                                    // Заголовок
        private CommonBorder                    _commonBorder;                                              // Внешняя граница окна
        private PropertiesBorder                _propertiesBorder;                                          // Граница окна свойств
        private List<string>                    _pageContent;                                               // Отображаемая на этой странице часть дерева

        private int                             _currentIndex;                                              // Индекс текущего выбранного элемента
        private string                          _selectedItem               =    string.Empty;              // Элемент, на котором на данный момент находится указатель пользователя

        // Точки для отрисовки Заголовка
        (int x, int y)                          _headerBorderCoord          =    (0, 0);
        (int x, int y)                          _headerContentCoord         =    (2, 1);

        // Точки для отрисовки границ страницы
        (int x, int y)                          _topBorderCoor              =    (0, 2);
        (int x, int y)                          _propBorderCoord            =    (0, _pageHeight - _porpertiesHeight + _headerHeight); // 31

        // Точки для отрисовки контента страницы
        (int x, int y)                          _topContentCoord            =    (2, 3);
        (int x, int y)                          _propContentCoord           =    (17, _pageHeight - _porpertiesHeight + _headerHeight + 1); // 33

        // Точки для отрисовки статус-бара
        (int x, int y)                          _statusBarCoord             =    (2, _textHeight + _porpertiesHeight + _headerHeight - 1);


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
        public static int PageWidth
        {
            get { return _pageWidth; }
        }

        /* МЕТОДЫ */
        /* Static */
        /// <summary>
        /// Обеспечивает проверку, является ли выбранный элемент папкой или файлом
        /// </summary>
        /// <param name="fileName">путь к элементу файловой системы для проверки</param>
        /// <returns>Возвращает true, если в аргументе - каталог</returns>
        public static bool IsDirectory(string fileName)
        {
            FileSystemInfo fsItem = (Directory.Exists(fileName)) ? new DirectoryInfo(fileName) : new FileInfo(fileName);
            return (fsItem.Attributes & FileAttributes.Directory) == FileAttributes.Directory;
        }
        /* Public */
        /// <summary>
        /// Отрисовывает контент страницы
        /// </summary>
        /// <param name="currIndex">Индекс для отрисовки</param>
        /// <param name="status">Текст для статус-бара</param>
        public void Print(int currIndex, string status)
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
            ConsoleUtils.WriteColoredAt(_header.Draw(), _headerBorderCoord, Page._background);         // Отрисовка границ заголовка
            ConsoleUtils.WriteColoredAt(_propertiesBorder.Draw(), _propBorderCoord, Page._background); // Отрисовка границы окна свойств

            if ( ! string.IsNullOrWhiteSpace(status))
            {

                ConsoleUtils.WriteColoredAt("                                                                                ", _statusBarCoord, _background);
                ConsoleUtils.WriteColoredAt(status + '\r', _statusBarCoord, _background);
            }

            /* Отрисовка контента страницы */
            PrintPageContent();
        }
        /// <summary>
        /// Находит выбранный в данный момент элемент
        /// </summary>
        /// <returns>Возвращает путь в системе к выбранному элементу</returns>
        public string GetSelectedItem()
        {
            return _selectedItem;
        }
        /// <summary>
        /// Проверяет, нахоодится ли на странице переданный в параметрах элемент
        /// </summary>
        /// <param name="fSItem"></param>
        /// <returns>Возвращает кортеж, содержащий булево значение и индекс элемента, если первый элемент кортежа - true</returns>
        public (bool, int) IsOnPage(string fSItem)
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

        /* Internal */
        /// <summary>
        /// Преобразовывает строку контента странцы (со всеми символами рисования) в путь файловой системы
        /// </summary>
        /// <param name="contentString"></param>
        /// <returns>Возвращает путь к выбранному элементу</returns>
        internal string GetFullNameFromContentString(string contentString)
        {
            return contentString.Split(new char[] { '─' }, StringSplitOptions.RemoveEmptyEntries).Last().Trim();
        }

        /* Private */
        /// <summary>
        /// Отрисовка контента страницы, который устанавливается pageManager -ом
        /// </summary>
        private void PrintPageContent()
        {
            ConsoleUtils.WriteColoredAt("F1 - копировать", _headerContentCoord, _background);
            ConsoleUtils.WriteColoredAt("F2 - вставить", (_headerContentCoord.x + 18, _headerContentCoord.y), _background);
            ConsoleUtils.WriteColoredAt("F3 - удалить", (_headerContentCoord.x + 36, _headerContentCoord.y), _background);
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
        /// <summary>
        /// Отрисовка свойств активного элемента контента страницы
        /// </summary>
        /// <param name="fullFSItemName"></param>
        private void PrintFSItemProperty(string fullFSItemName)
        {
            string[] propertyStrings;
            FileSystemInfo fsInfo = (Directory.Exists(fullFSItemName)) ? new DirectoryInfo(fullFSItemName) : new FileInfo(fullFSItemName);
            if (Directory.Exists(fullFSItemName))
            {
                DirectoryInfo di = new DirectoryInfo(fullFSItemName);
                propertyStrings = new string[]
                {
                    $"{di.Name}",
                    $"{di.CreationTime}",
                    $"{di.LastWriteTime}",
                    $"{di.LastAccessTime}",
                    $"{DirectorySize(di)} Б",
                };
            }
            else
            {
                FileInfo fi = new FileInfo(fullFSItemName);
                propertyStrings = new string[]
                {
                    $"{fi.Name}",
                    $"{fi.CreationTime}",
                    $"{fi.LastWriteTime}",
                    $"{fi.LastAccessTime}",
                    $"{fi.Length} Б",
                };
            }

            for (int i = 0; i < propertyStrings.Length; i++)
            {
                ConsoleUtils.WriteColoredAt(propertyStrings[i], (_propContentCoord.x, _propContentCoord.y + i), _background);
            }
            Console.SetCursorPosition(_statusBarCoord.x, _statusBarCoord.y);
        }
        /// <summary>
        /// Определение размера каталога (если это не запрещено правами)
        /// </summary>
        /// <param name="di">Каталог для определения размера</param>
        /// <returns>Возвращает размер в байтах</returns>
        private long DirectorySize(DirectoryInfo di)
        {
            try
            {
                long folderSize = 0;
                string[] files = Directory.GetFiles(di.FullName, "*", SearchOption.AllDirectories);
                for (int i = 0; i < files.Length; i++)
                {
                    folderSize += new FileInfo(files[i]).Length;
                }
                return folderSize;
            }
            catch (Exception ex)
            {
                return -1;
            }

        }
        /// <summary>
        /// Возвращает цвет для отрисовки выбранного элемента контента
        /// </summary>
        /// <param name="path">Путь к файлу строки контента</param>
        /// <returns>Цвет, на основе разделения: каталог - файл</returns>
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
    }
}
