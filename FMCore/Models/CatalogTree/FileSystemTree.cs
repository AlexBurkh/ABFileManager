using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace FMCore.Models.CatalogTree
{
    internal class FileSystemTree
    {
        /* ПОЛЯ */
        private StringBuilder _sb;     // StringBuilder для промежуточного хранения дерева каталогов (очищается перед возвратом контента)
        private DirectoryInfo _currentDir;      // Текущий корневой каталог для постройки дерева


        /* СВОЙСТВА */
        private DirectoryInfo CurrentDir
        {
            get
            {
                return _currentDir;
            }
            set
            {
                if (Directory.Exists(value.FullName))
                {
                    _currentDir = value;
                }
            }
        }
        public string Tree
        {
            get
            {
                BuildTree();
                string result = _sb.ToString();
                _sb.Clear();
                return result;
            }
        }       // Особое свойства, возвращает текущий контент StringBuilder-а и очищает его


        /* МЕТОДЫ */
        /* Public */
        private void BuildTree(string prefix = "\t")
        {
            DirectoryInfo di = CurrentDir;
            List<FileSystemInfo> fsItems = di.GetFileSystemInfos()
                .OrderBy(f => f.Name)
                .ToList();

            try
            {
                for (int i = 0; i < fsItems.Count; i++)
                {
                    FileSystemInfo fsItem = fsItems[i];

                    if (i == fsItems.Count - 1)
                    {
                        _sb.AppendLine($"{prefix}└── {fsItem}");
                        if (IsDirectory(fsItem))
                        {
                            CurrentDir = (DirectoryInfo)fsItem;
                            BuildTree(prefix + '\t');
                        }
                    }
                    else
                    {
                        _sb.AppendLine($"{prefix}├── {fsItem}");
                        if (IsDirectory(fsItem))
                        {
                            CurrentDir = (DirectoryInfo) fsItem;
                            BuildTree(prefix + "│   ");
                        }
                    }
                }
            }
            catch(Exception ex)
            {
                Console.ForegroundColor = ConsoleColor.DarkRed;
                Console.Error.WriteLine(ex.Message);
                Console.ResetColor();
            }
        }           // Составляет дерево относительно свойства CurrentDir - поля _currentDir
        /* Pricate */
        private bool IsDirectory(FileSystemInfo fsItem)
        {
            return (fsItem.Attributes & FileAttributes.Directory) == FileAttributes.Directory;
        }


        /* КОНСТРУКТОРЫ */
        public FileSystemTree(string rootCatalog)
        {
            _sb = new StringBuilder();
            CurrentDir = new DirectoryInfo(rootCatalog);
        }
    }
}
