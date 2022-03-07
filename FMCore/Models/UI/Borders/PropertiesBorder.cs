using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FMCore.Models.UI.Borders
{
    public class PropertiesBorder : Border
    {
        static private readonly int propBorderHeight = 9;

        int xStartPos = 0;
        int yStartPos = Console.WindowHeight - propBorderHeight;

        public override string Draw()
        {
            StringBuilder sb = new StringBuilder();
            sb.Clear();

            sb.Append(Border.LEFTCENTER);
            //Console.Write(LEFTCENTER);
            for (int j = 0; j < (this.borderWidth - 2); j++)
            {
                sb.Append(Border.HORIZONTAL);
                //Console.Write(HORIZONTAL);
            }
            sb.Append(Border.RIGHTCENTER);
            sb.Append('\n');
            //Console.WriteLine(RIGHTCENTER);

            return sb.ToString();
        }

        public PropertiesBorder(int width)
            : base(PropertiesBorder.propBorderHeight, width)
        { }
    }
}
