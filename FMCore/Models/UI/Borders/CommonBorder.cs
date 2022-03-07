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


        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < (this.borderHeight - 1); i++)
            {
                if (i == 0)
                {
                    sb.Append(Border.LEFTTOP);
                    for (int j = 0; j < (this.borderWidth - 2); j++)
                    {
                        sb.Append(Border.HORIZONTAL);
                    }
                    sb.Append(Border.RIGHTTOP);
                }

                if ((i > 0) && (i < (this.borderHeight - 1)))
                {
                    sb.Append(Border.VERTICAL);
                    for (int j = 0; j < (this.borderWidth - 2); j++)
                    {
                        sb.Append(Border.HORIZONTAL);
                    }
                    sb.Append(Border.VERTICAL);
                }

                if (i == (this.borderHeight - 1))
                {
                    sb.Append(Border.LEFTBOTTOM);
                    for (int j = 0; j < (this.borderWidth - 2); j++)
                    {
                        Console.Write(Border.HORIZONTAL);
                    }
                    sb.Append(Border.RIGHTBOTTOM);
                }
            }
            return sb.ToString();
        }

        public CommonBorder(int height, int width)
            : base(height, width)
        { }
    }
}
