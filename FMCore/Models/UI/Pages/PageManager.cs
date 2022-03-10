using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using FMCore.Models.CatalogTree;
using FMCore.Models.UI.Pages;

namespace FMCore.Models.UI.Pages
{
    class PageManager
    {
        /* ПОЛЯ */
        private Page            _currentPage;
        private FileSystemTree  _tree;
        private List<string>    _treeContent;
        private int             _currentIndex;


        public void MakePage(int selectedItemIndex, string workDir)
        {
            _treeContent = new List<string>(
                                    _tree.LoadTree(workDir)
                                    .Split(new char[] { '\n' }, StringSplitOptions.RemoveEmptyEntries)
                                    );

            if (selectedItemIndex < Page.TextHeight)
            {
                _currentPage = new Page(_treeContent.GetRange(0, Page.TextHeight));
            }
            else
            {
                _currentPage = new Page(_treeContent.GetRange(_currentIndex, selectedItemIndex));
            }
        }

        public PageManager()
        {
            _tree = new FileSystemTree();
        }
    }
}
