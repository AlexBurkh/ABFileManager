using System;
using FMCore.Models.UI.Pages;
using FMCore.Models.CatalogTree;
using System.Linq;
using System.IO;
using System.Diagnostics;
using System.Collections.Generic;

namespace FMRun
{
    internal class Program
    {
        static int prevIndex = 0;
        static int currentIndex = 0;
        static string startCatalog = "D:\\";
        static string currCatalog = string.Empty;
        static string sourceFileToCopy;
        static List<string> directoryCopyBuffer = new List<string>();

        static void Main(string[] args)
        {
            currCatalog = startCatalog;
            PageManager pageManager = new PageManager();
            pageManager.MakePage(currentIndex, currCatalog);

            while (true)
            {
                ConsoleKeyInfo keyInfo = Console.ReadKey();

                switch (keyInfo.Key)
                {
                    case ConsoleKey.UpArrow:
                        if (currentIndex > 0)
                        {
                            currentIndex -= 1;
                            pageManager.MakePage(currentIndex, currCatalog);
                        }
                        continue;
                    case ConsoleKey.DownArrow:
                        if (currentIndex < pageManager.MaxIndex)
                        {
                            currentIndex += 1;
                            pageManager.MakePage(currentIndex, currCatalog);
                        }
                        continue;
                    case ConsoleKey.LeftArrow:
                        DirectoryInfo parentDir = new DirectoryInfo(pageManager.CurrentWorkDir).Parent;
                        if (parentDir != null)
                        {
                            currCatalog = parentDir.FullName;
                            currentIndex = 0;
                            pageManager.MakePage(currentIndex, currCatalog);
                        }
                        continue;
                    case ConsoleKey.RightArrow:
                        {
                            string selectedItem = pageManager.SelectedItem;
                            if (Directory.Exists(selectedItem))
                            {
                                currCatalog = selectedItem;
                                currentIndex = 0;
                                pageManager.MakePage(currentIndex, currCatalog);
                            }
                        }
                        continue;
                    case ConsoleKey.Enter:
                        {
                            string selectedItem = pageManager.SelectedItem;
                            if (File.Exists(selectedItem))
                            {
                                Process.Start(new ProcessStartInfo() { FileName = selectedItem, UseShellExecute = true} );
                                pageManager.MakePage(currentIndex, currCatalog);
                            } 
                        }
                        continue;
                    case ConsoleKey.F1:
                        {
                            var selectedItem = pageManager.SelectedItem;
                            if (Directory.Exists(selectedItem))
                            {
                                CopyDirectory(selectedItem);
                            }
                            else
                            {
                                sourceFileToCopy = selectedItem;
                                pageManager.Status = $"Файл {new FileInfo(selectedItem).Name} выбран для копирования";
                                pageManager.MakePage(currentIndex, currCatalog);
                            }
                        }
                        continue;


                    // В копировании папки есть ошибка. Скопированное содержимое каталога вставляетсся не в него, а рядом с ним.
                    case ConsoleKey.F2:
                        {
                            var selectedItem = pageManager.SelectedItem;
                            if (Directory.Exists(selectedItem))
                            {
                                if (! string.IsNullOrWhiteSpace(sourceFileToCopy))
                                {
                                    try
                                    {
                                        File.Copy(sourceFileToCopy, selectedItem);
                                        sourceFileToCopy = string.Empty;
                                        pageManager.Status = $"Файл {new FileInfo(selectedItem).Name} скопирован в каталог {currCatalog}\\";
                                    }
                                    catch
                                    {

                                    }
                                    
                                }
                                else
                                {
                                    if (directoryCopyBuffer != null)
                                    {
                                        for (int i = 0; i < directoryCopyBuffer.Count; i++)
                                        {
                                            try
                                            {
                                                if (Directory.Exists(directoryCopyBuffer[i]))
                                                {
                                                    Directory.CreateDirectory($"{selectedItem}\\{new DirectoryInfo(directoryCopyBuffer[i]).Name}");
                                                }
                                                File.Copy(directoryCopyBuffer[i], $"{selectedItem}\\{new FileInfo(directoryCopyBuffer[i]).Name}");
                                            }
                                            catch
                                            {
                                                continue;
                                            }
                                        }
                                    }
                                }
                            }
                            pageManager.MakePage(currentIndex, currCatalog);
                        }
                        continue;
                }
                break;
            }
        }

        static void CopyDirectory(string dirPath)
        {
            try
            {
                if (Directory.Exists(dirPath))
                {
                    DirectoryInfo root = new DirectoryInfo(dirPath);
                    directoryCopyBuffer.Add(root.FullName);
                    FileInfo[] files = root.GetFiles();
                    for (int i = 0; i < files.Length; i++)
                    {
                        directoryCopyBuffer.Add(files[i].FullName);
                    }
                    DirectoryInfo[] dirs = root.GetDirectories();
                    for(int i = 0; i < dirs.Length; i++)
                    {
                        CopyDirectory(dirs[i].FullName);
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }
    }
}
