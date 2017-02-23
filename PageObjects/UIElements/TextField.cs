using OpenQA.Selenium;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PageObjects
{
    public class TextField : PageElement
    {
        public override string Value
        {
            get { return this.WebElement.GetAttribute("value"); }
            set 
            {
                this.WebElement.Clear();
                SelectAll();
                this.WebElement.SendKeys(value);
            }
        }

        public void SetValue(string input, bool clearFirst)
        {
            SelectAll();
            Value = input;
        }

        public void SelectAll()
        {
            WebElement.SendKeys(Keys.Control + "a");
        }
    }
}
