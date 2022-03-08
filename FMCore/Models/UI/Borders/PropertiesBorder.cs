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
            sb.AppendLine($"\tКороткое имя:              ");
            sb.AppendLine($"\tДата создания:             ");
            sb.AppendLine($"\tДата последнего изменения: ");
            sb.AppendLine($"\tДата последнего доступа:   ");

            return sb.ToString();
        }

        public PropertiesBorder(int height, int width)
            : base(height, width)
        { }
    }
}
