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
        public string LoadTree (string rootDir)
        {
            CurrentDir = new DirectoryInfo(rootDir);
            BuildTree();
            string result = _sb.ToString();
            _sb.Clear();
            return result;
        }       // Особое свойства, возвращает текущий контент StringBuilder-а и очищает его
        //private int depth { get; set; } = 2;

        /* МЕТОДЫ */
        /* Public */
        private void BuildTree(string prefix = "\t")
        {
            List<DirectoryInfo> rootDirs = new List<DirectoryInfo>(CurrentDir.GetDirectories());
            List<FileInfo> rootFiles = new List<FileInfo>(CurrentDir.GetFiles());

            _sb.AppendLine($"{prefix}{CurrentDir.FullName}");

            for (int i = 0; i < rootDirs.Count; i++)
            {
                try
                {
                    _sb.AppendLine($"{prefix}└── {rootDirs[i].FullName}");

                    List<DirectoryInfo> childDirs = new List<DirectoryInfo>(rootDirs[i].GetDirectories());
                    List<FileInfo> childFiles = new List<FileInfo>(rootDirs[i].GetFiles());

                    for (int j = 0; j < childDirs.Count; j++)
                    {
                        _sb.AppendLine($"{prefix}{prefix}└── {childDirs[j].FullName}");
                    }

                    for (int k = 0; k < childFiles.Count; k++)
                    {
                        _sb.AppendLine($"{prefix}{prefix}└── {childFiles[k].FullName}");
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }
            }
            for (int i = 0; i < rootFiles.Count; i++)
            {
                _sb.AppendLine($"{prefix}└── {rootFiles[i].FullName}");
            }
/*            int depth = 2;
            if (depth <= 0)
            {
                return;
            }
            depth -= 1;
            try
            {
                DirectoryInfo di = CurrentDir;
                List<FileSystemInfo> fsItems = di.GetFileSystemInfos()
                .OrderBy(f => f.Name)
                .ToList();

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
            depth++;*/
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
