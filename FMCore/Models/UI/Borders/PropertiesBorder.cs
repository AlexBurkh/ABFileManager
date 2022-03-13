using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FMCore.Models.UI.Borders
{
    public class PropertiesBorder : Border
    {
        public override string Draw()
        {
            StringBuilder sb = new StringBuilder();
            sb.Clear();

            sb.Append(LEFTCENTER);
            for (int j = 1; j < (this.borderWidth - 1); j++)
            {
                sb.Append(HORIZONTAL);
            }
            sb.Append(RIGHTCENTER);
            sb.Append('\n');
            sb.Append('\n');
            sb.AppendLine($"\tИмя:     ");
            sb.AppendLine($"\tСоздан:  ");
            sb.AppendLine($"\tИзменен: ");
            sb.AppendLine($"\tОткрыт:  ");
            sb.AppendLine($"\tРазмер:  ");
            return sb.ToString();
        }

        public PropertiesBorder(int height, int width)
            : base(height, width)
        { }
    }
}
