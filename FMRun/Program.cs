using System;
using FMCore.Models.UI.Pages;
using FMCore.Models.CatalogTree;
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
        static string prevCatalog = string.Empty;
        static string currCatalog = string.Empty;
        static (string name, byte[] buffer) fileToCopy;
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
                                prevCatalog = currCatalog;
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

                            }
                            else
                            {
                                fileToCopy = (new FileInfo(selectedItem).Name, File.ReadAllBytes(selectedItem));
                            }
                        }
                        continue;
                    case ConsoleKey.F2:
                        {
                            var selectedItem = pageManager.SelectedItem;
                            if (Directory.Exists(selectedItem))
                            {
                                File.WriteAllBytes($"{currCatalog}\\{fileToCopy.name}", fileToCopy.buffer);
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

                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }
    }
}
