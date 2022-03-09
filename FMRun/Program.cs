using System;
using FMCore.Models.UI.Pages;
using FMCore.Models.CatalogTree;
using System.IO;

namespace FMRun
{
    internal class Program
    {
        static int prevIndex = 0;
        static int currentIndex = 0;
        static string startCatalog = "D:\\";

        static void Main(string[] args)
        {
            Page page = new Page(startCatalog);
            page.Print(31);

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
                        break;
                    case ConsoleKey.DownArrow:
                        if (currentIndex < page.MaxIndex)
                        {
                            currentIndex += 1;
                            page.Print(currentIndex);
                        }
                        break;
                    case ConsoleKey.LeftArrow:
                        var parentDir = new DirectoryInfo(page.CurrentDir).Parent;
                        if (parentDir != null)
                        {
                            //currentIndex = prevIndex;
                            currentIndex = 0;
                            page.Print(currentIndex, parentDir.FullName);
                        }
                        continue;
                    case ConsoleKey.RightArrow:
                        var selectedItem = page.GetSelectedElement();
                        if (Directory.Exists(selectedItem))
                        {
                            var currentDir = new DirectoryInfo(selectedItem);
                            //prevIndex = currentIndex;
                            currentIndex = 0;
                            page.Print(currentIndex, currentDir.FullName);
                        }
                        continue;
                }
            }
        }
    }
}
