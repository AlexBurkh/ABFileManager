using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FMCore.Models.UI.Borders
{
    /// <summary>
    /// Класс задает абстракцию границ окна
    /// </summary>
    public abstract class Border
    {
        /* КОНСТРУКТОРЫ */
        public Border(int height, int width)
        {
            borderHeight = height;
            borderWidth = width;
        }

        /* Символы UNICODE, отображающие границы */
        protected static readonly char LEFTTOP     = '\u2554';
        protected static readonly char RIGHTTOP    = '\u2557';
        protected static readonly char LEFTBOTTOM  = '\u255A';
        protected static readonly char RIGHTBOTTOM = '\u255D';
        protected static readonly char HORIZONTAL  = '\u2550';
        protected static readonly char VERTICAL    = '\u2551';
        protected static readonly char LEFTCENTER  = '\u2560';
        protected static readonly char RIGHTCENTER = '\u2563';
        protected static readonly char UPCENTER    = '\u2566';
        protected static readonly char DOWNCENTER  = '\u2569';

        public int borderHeight { get; private set; }
        public int borderWidth { get; private set; }

        /// <summary>
        /// Виртуальный метод для перегрузки в наследниках - создает строки, представляющие собой границы окна
        /// </summary>
        /// <returns></returns>
        public virtual string Draw()
        {
            return string.Empty;
        }
    }
}
