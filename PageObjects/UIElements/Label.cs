using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PageObjects
{
    public class Label : PageElement
    {
        public bool IsVisible
        {
            get { return this.WebElement.Displayed; }
        }

        public override string Value
        {
            get
            {
                if (this.IsVisible)
                {
                    return this.WebElement.Text;
                }
                else
                {
                    return string.Empty;
                }

            }
        }
    }
}
