using System;
using FMCore.Models.UI.Pages;
using FMCore.Models.CatalogTree;

namespace FMRun
{
    internal class Program
    {
        static void Main(string[] args)
        {
            Page page = new Page("D:\\0");
            page.Print(0);
        }
    }
}
