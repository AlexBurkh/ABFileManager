using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using FMCore.Models.CatalogTree;
using FMCore.Models.UI.Pages;

namespace FMCore.Models.UI.Pages
{
    public class PageManager
    {
        /* ПОЛЯ */
        private Page            _currentPage;
        private FileSystemTree  _tree;
        private List<string>    _treeContent;
        private string          _currentWorkDir = string.Empty;
        private int             _maxIndex;
        private int             _selectedItemIndex;
        private int             _previousSelectedItemIndex = 0;
        private int             _currentPageContentStartIndex = 0;

        /* СВОЙСТВА */
        public int      MaxIndex        { get { return _maxIndex; } }
        public string   CurrentWorkDir  { get { return _currentWorkDir; } } 
        public string   SelectedItem    
        { 
            get 
            { 
                return _currentPage.GetFullNameFromContentString(_currentPage.GetSelectedItem()); 
            } 
        }


        public void MakePage(int selectedItemIndex, string workDir)
        {
            _selectedItemIndex = selectedItemIndex;
            _currentWorkDir = workDir;

            _treeContent = new List<string>(
                                    _tree.LoadTree(workDir)
                                    .Split(new char[] { '\n' }, StringSplitOptions.RemoveEmptyEntries)
                                    );
/*            _currentPage.PageContent = (_treeContent.Count > Page.TextHeight) ?
                                            _treeContent.GetRange(_currentPageContentStartIndex, Page.TextHeight) :
                                            _treeContent.GetRange(_currentPageContentStartIndex, _treeContent.Count);*/
            _currentPage.PageContent = (_treeContent.Count > Page.TextHeight) ? 
                                            _treeContent.GetRange(_currentPageContentStartIndex, Page.TextHeight) : 
                                            _treeContent.GetRange(0, _treeContent.Count);

            _maxIndex = _treeContent.Count - 1;

            if (_selectedItemIndex >= _treeContent.Count)
            {
                this.MakePage(0, new DirectoryInfo(workDir).Parent.FullName);
            }
            (bool isOnPage, int itemIndex) = _currentPage.IsOnPage(_treeContent[_selectedItemIndex]);
            if (isOnPage)
            {
                _currentPage.Print(itemIndex);
            }
            else
            {
                int index;
                if (_selectedItemIndex > _previousSelectedItemIndex)
                {
                    index = Page.TextHeight - 1;
                    _currentPageContentStartIndex += 1;
                }
                else
                {
                    _currentPageContentStartIndex -= 1;
                    index = 0;
                }
                _currentPage.PageContent = _treeContent.GetRange(_currentPageContentStartIndex, Page.TextHeight);
                _currentPage.Print(index);
            }
            _previousSelectedItemIndex = _selectedItemIndex;
        }

        public PageManager()
        {
            _tree = new FileSystemTree();
            _currentPageContentStartIndex = 0;
            _currentPage = new Page();
        }
    }
}
