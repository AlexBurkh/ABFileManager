using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FMCore.Models.UI.Borders
{
    public abstract class Border
    {
        protected static readonly char LEFTTOP = '\u2554';
        protected static readonly char RIGHTTOP = '\u2557';
        protected static readonly char LEFTBOTTOM = '\u255A';
        protected static readonly char RIGHTBOTTOM = '\u255D';
        protected static readonly char HORIZONTAL = '\u2550';
        protected static readonly char VERTICAL = '\u2551';
        protected static readonly char LEFTCENTER = '\u2560';
        protected static readonly char RIGHTCENTER = '\u2563';

        public int borderHeight { get; private set; }
        public int borderWidth { get; private set; }

        public virtual string Draw()
        {
            return string.Empty;
        }

        public Border(int height, int width) 
        {
            borderHeight = height;
            borderWidth = width;
        }
    }
}
