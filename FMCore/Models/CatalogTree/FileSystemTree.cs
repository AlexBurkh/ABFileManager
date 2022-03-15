using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace FMCore.Models.CatalogTree
{
    /// <summary>
    /// Обеспечивает построение дерева файлов и каталогов
    /// </summary>
    internal class FileSystemTree
    {
        /* КОНСТРУКТОРЫ */
        public FileSystemTree()
        {
            _sb = new StringBuilder();
        }

        /* ПОЛЯ */
        private StringBuilder _sb;              // StringBuilder для промежуточного хранения дерева каталогов (очищается перед возвратом контента)
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


        /* МЕТОДЫ */
        /* Public */
        /// <summary>
        /// Обеспечивает создание дерева каталогов из указанного корневого каталога
        /// </summary>
        /// <param name="workDir">Корневой каталог для старта построения дерева</param>
        /// <returns>Текущий контент StringBuilder-а (дерево файлов и каталогов)</returns>
        public string LoadTree(string workDir)
        {
            CurrentDir = new DirectoryInfo(workDir);
            BuildTree();
            string result = _sb.ToString();
            _sb.Clear();
            return result;
        }
        /* Private */
        /// <summary>
        /// Составляет дерево относительно свойства CurrentDir - поля _currentDir. Заполняет контент StringBuilder-а
        /// </summary>
        /// <param name="prefix">Элемент для оформления отступов между компонентами дерева</param>
        private void BuildTree(string prefix = "\t")
        {
            List<DirectoryInfo> rootDirs = new List<DirectoryInfo>(CurrentDir.GetDirectories());
            List<FileInfo> rootFiles = new List<FileInfo>(CurrentDir.GetFiles());

            _sb.AppendLine($"{prefix}{CurrentDir.FullName}");

            for (int i = 0; i < rootDirs.Count; i++)
            {
                try
                {
                    if (rootDirs[i] == rootDirs.Last())
                    {
                        _sb.AppendLine($"{prefix}└── {rootDirs[i].FullName}");
                    }
                    else
                    {
                        _sb.AppendLine($"{prefix}\u251c── {rootDirs[i].FullName}");
                    }
                    //_sb.AppendLine($"{prefix}└── {rootDirs[i].FullName}");

                    List<DirectoryInfo> childDirs = new List<DirectoryInfo>(rootDirs[i].GetDirectories());
                    List<FileInfo> childFiles = new List<FileInfo>(rootDirs[i].GetFiles());

                    for (int j = 0; j < childDirs.Count; j++)
                    {
                        if (childDirs[j] == childDirs.Last())
                        {
                            _sb.AppendLine($"{prefix}\u2502{prefix}└── {childDirs[j].FullName}");
                        }
                        else
                        {
                            _sb.AppendLine($"{prefix}\u2502{prefix}\u251c── {childDirs[j].FullName}");
                        }
                        //_sb.AppendLine($"{prefix}\u2502{prefix}└── {childDirs[j].FullName}");
                    }

                    for (int k = 0; k < childFiles.Count; k++)
                    {
                        if (childFiles[k] == childFiles.Last())
                        {
                            _sb.AppendLine($"{prefix}\u2502{prefix}└── {childFiles[k].FullName}");
                        }
                        else
                        {
                            _sb.AppendLine($"{prefix}\u2502{prefix}\u251c── {childFiles[k].FullName}");
                        }
                        //_sb.AppendLine($"{prefix}\u2502{prefix}└── {childFiles[k].FullName}");
                    }
                }
                catch (Exception ex)
                {
                    //Console.WriteLine(ex.Message);
                    Console.WriteLine();
                }
            }
            for (int i = 0; i < rootFiles.Count; i++)
            {
                _sb.AppendLine($"{prefix}└── {rootFiles[i].FullName}");
            }
         
        }
    }
}
