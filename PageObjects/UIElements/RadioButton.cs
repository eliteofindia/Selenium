using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PageObjects
{
    public class RadioButton : PageElement
    {
        public bool BoolValue
        {
            get
            {
                return this.WebElement.Selected;
            }
            set
            {
                if (this.WebElement.Selected != value)
                {
                    Click();
                }
            }
        }

        public override string Value
        {
            get
            {
                return this.WebElement.GetAttribute("value");
            }
            set
            {
                if ("true".Equals(value, StringComparison.OrdinalIgnoreCase))
                {
                    this.BoolValue = true;
                }
                else
                {
                    this.BoolValue = false;
                }
            }
        }

        public void Click()
        {
            this.WebElement.Click();
        }
    }
}
