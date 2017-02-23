using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;

namespace PageObjects
{
    public class Label : PageElement
    {
        public bool IsVisible
        {
            get { return this.WebElement.Displayed; }
        }

        public void Click()
        {
            int i = 0;
            while (i < 10)
            {
                i++;
                try
                {
                    this.WebElement.Click();
                    break;
                }
                catch (Exception e)
                {

                }
                Thread.Sleep(1000);
            }
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
