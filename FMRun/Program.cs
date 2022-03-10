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
        static string fileCopyBuffer = string.Empty;
        static List<string> directoryCopyBuffer = new List<string>();

        static void Main(string[] args)
        {
            Page page = new Page(startCatalog);
            page.Print(0);

            while (true)
            {
                ConsoleKeyInfo keyInfo = Console.ReadKey();

                switch (keyInfo.Key)
                {
                    case ConsoleKey.UpArrow:
                        if (currentIndex > 0)
                        {
                            currentIndex -= 1;
                            page.Print(currentIndex);
                        }
                        continue;
                    case ConsoleKey.DownArrow:
                        if (currentIndex < page.maxIndex)
                        {
                            currentIndex += 1;
                            page.Print(currentIndex);
                        }
                        continue;
                    case ConsoleKey.LeftArrow:
                        var parentDir = new DirectoryInfo(page.CurrentDir).Parent;
                        if (parentDir != null)
                        {
                            currentIndex = 0;
                            page.Print(currentIndex, parentDir.FullName);
                        }
                        continue;
                    case ConsoleKey.RightArrow:
                        {
                            var selectedItem = page.GetSelectedItem();
                            if (Directory.Exists(selectedItem))
                            {
                                var currentDir = new DirectoryInfo(selectedItem);
                                currentIndex = 0;
                                page.Print(currentIndex, currentDir.FullName);
                            }
                        }
                        continue;
                    case ConsoleKey.Enter:
                        {
                            var selectedItem = page.GetSelectedItem();
                            if (File.Exists(selectedItem))
                            {
                                Process.Start(new ProcessStartInfo() { FileName = selectedItem, UseShellExecute = true} );
                                page.Print(currentIndex);
                            } 
                        }
                        continue;
                    case ConsoleKey.F1:
                        {
                            var selectedItem = page.GetSelectedItem();
                            if (page.IsDirectory(selectedItem))
                            {
                                
                            }
                            else
                            {
                                fileCopyBuffer = selectedItem;
                            }
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
