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

        private string          _status = string.Empty;

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
        public string   Status
        {
            get { return _status; }
            set { _status = value; }
        }


        public void MakePage(int selectedItemIndex, string workDir)
        {
            _selectedItemIndex = selectedItemIndex;
            _currentWorkDir = workDir;

            _treeContent = new List<string>(_tree.LoadTree(workDir).Split(new char[] { '\n' }, StringSplitOptions.RemoveEmptyEntries));

            if (_currentPageContentStartIndex > (_treeContent.Count - Page.TextHeight))
            {
                _currentPageContentStartIndex = 0;
                _currentPage.PageContent = _treeContent.GetRange(_currentPageContentStartIndex, (_treeContent.Count < Page.TextHeight) ? _treeContent.Count : Page.TextHeight);
            }
            else
            {
                _currentPage.PageContent = (_treeContent.Count > Page.TextHeight) ? _treeContent.GetRange(_currentPageContentStartIndex, Page.TextHeight) : _treeContent.GetRange(0, _treeContent.Count);
            }

            _maxIndex = _treeContent.Count - 1;

            if (_selectedItemIndex >= _treeContent.Count)
            {
                _currentPageContentStartIndex = 0;
                this.MakePage(_currentPageContentStartIndex, new DirectoryInfo(workDir).Parent.FullName);
            }

            (bool isOnPage, int itemIndex) = _currentPage.IsOnPage(_treeContent[_selectedItemIndex]);
            if (isOnPage)
            {
                _currentPage.Print(itemIndex, _status);
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
                _currentPage.Print(index, _status);
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
