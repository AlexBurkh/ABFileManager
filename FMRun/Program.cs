using System;
using FMCore.Models.UI.Pages;
using FMCore.Models.CatalogTree;

namespace FMRun
{
    internal class Program
    {
        static int currentIndex = 0;
        static string startCatalog = "D:\\0";

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
                        }
                        break;
                    case ConsoleKey.DownArrow:
                        if (currentIndex < page.maxIndex)
                        {
                            currentIndex += 1;
                        }
                        break;
                }
                page.Print(currentIndex);
            }
        }
    }
}
