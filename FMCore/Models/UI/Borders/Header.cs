using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FMCore.Models.UI.Borders
{
    internal class Header : Border
    {
        /* КОНСТРУКТОРЫ */
        public Header(int height, int width) : base(height, width)
        { }

        /// <summary>
        /// Формирование строки, содержащей границу окна свойств выбранного элемента
        /// </summary>
        /// <returns></returns>
        public override string Draw()
        {
            StringBuilder sb = new StringBuilder();
            sb.Clear();
            for (int i = 0; i < this.borderHeight; i++)
            {
                if (i == 0)
                {
                    sb.Append(LEFTTOP);
                    for (int j = 1; j < (this.borderWidth - 1); j++)
                    {
                        if (j == 18 || j == 36 || j == 54)
                        {
                            sb.Append(UPCENTER);
                            continue;
                        }
                        sb.Append(HORIZONTAL);
                    }
                    sb.Append(RIGHTTOP);
                    sb.Append('\n');
                }

                if (i == 1)
                {
                    sb.Append(VERTICAL);
                    for (int j = 1; j < (this.borderWidth - 1); j++)
                    {
                        if (j == 18 || j == 36 || j == 54)
                        {
                            sb.Append(VERTICAL);
                            continue;
                        }
                        sb.Append(' ');
                    }
                    sb.Append(VERTICAL);
                    sb.Append('\n');
                }

                if (i == 2)
                {
                    sb.Append(LEFTCENTER);
                    for (int j = 1; j < (this.borderWidth - 1); j++)
                    {
                        if (j == 18 || j == 36 || j == 54)
                        {
                            sb.Append(DOWNCENTER);
                            continue;
                        }
                        sb.Append(HORIZONTAL);
                    }
                    sb.Append(RIGHTCENTER);
                    sb.Append('\n');
                }
            }
            return sb.ToString();
        }
    }
}
