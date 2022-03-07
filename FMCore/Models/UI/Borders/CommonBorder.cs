using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FMCore.Models.UI.Borders
{
    public class CommonBorder : Border
    {
        public int xStartPos { get; } = 0;
        public int yStartPos { get; } = 0;


        public override string Draw()
        {
            StringBuilder sb = new StringBuilder();
            sb.Clear();
            for (int i = 0; i < (this.borderHeight - 2); i++)
            {
                if (i == 0)
                {
                    sb.Append(LEFTTOP);
                    for (int j = 0; j < (this.borderWidth - 2); j++)
                    {
                        sb.Append(HORIZONTAL);
                    }
                    sb.Append(RIGHTTOP);
                    sb.Append('\n');
                }

                if ((i > 0) && (i < (this.borderHeight - 1)))
                {
                    sb.Append(VERTICAL);
                    for (int j = 0; j < (this.borderWidth - 2); j++)
                    {
                        sb.Append(' ');
                    }
                    sb.Append(VERTICAL);
                    sb.Append('\n');
                }

                if (i == (this.borderHeight - 3))
                {
                    sb.Append(LEFTBOTTOM);
                    for (int j = 0; j < (this.borderWidth - 2); j++)
                    {
                        sb.Append(HORIZONTAL);
                    }
                    sb.Append(RIGHTBOTTOM);
                    sb.Append('\n');
                }
            }
            return sb.ToString();
        }

        public CommonBorder(int height, int width)
            : base(height, width)
        { }
    }
}
