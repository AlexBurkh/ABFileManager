using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using FMCore.Models.UI.Borders;

namespace FMCore.Models.UI.PAGES
{
    public class Page
    {
        CommonBorder commonBorder { get; set; }
        PropertiesBorder propertiesBorder { get; set; }


        public void Print()
        {
            Console.WriteLine(commonBorder);
            Console.WriteLine(propertiesBorder);
        }

        public Page(CommonBorder cb, PropertiesBorder pb)
        {
            this.commonBorder = cb;
            this.propertiesBorder = pb;
        }
    }
}
