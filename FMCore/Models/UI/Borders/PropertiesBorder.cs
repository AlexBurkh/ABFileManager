using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FMCore.Models.UI.Borders
{
    public class PropertiesBorder : Border
    {
        static private readonly int propBorderHeight = 6;

        int xStartPos = 0;
        int yStartPos = Console.WindowHeight - propBorderHeight;

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();

            for (int spaceCounter = 0; spaceCounter < propBorderHeight; spaceCounter++)
            {
                sb.AppendLine();
            }

            for (int i = 0; i < (this.borderHeight - 1); i++)
            {
                if (i == 0)
                {
                    sb.Append(Border.LEFTCENTER);
                    for (int j = 0; j < (this.borderWidth - 2); j++)
                    {
                        sb.Append(Border.HORIZONTAL);
                    }
                    sb.Append(Border.RIGHTCENTER);
                }

                if ((i > 0) && (i < (this.borderHeight - 1)))
                {
                    sb.Append(Border.VERTICAL);
                    for (int j = 0; j < (this.borderWidth - 2); j++)
                    {
                        sb.Append(' ');
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

        public PropertiesBorder(int width)
            : base(PropertiesBorder.propBorderHeight, width)
        { }
    }
}
