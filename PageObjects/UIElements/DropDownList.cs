using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;

namespace PageObjects
{
    public class DropDownList : PageElement
    {
        public override string Value
        {
            get
            {
                return new SelectElement(WebElement).SelectedOption.Text;
            }
            set 
            { 
                this.Select(value, false);
            }
        }

        public string KeyValue
        {
            get
            {
                return new SelectElement(WebElement).SelectedOption.GetAttribute("value");
            }
            set
            {
                SelectElement selector = new SelectElement(WebElement);
                selector.SelectByValue(value);
            }
        }

        public void Select(int index)
        {
            SelectElement selector = new SelectElement(WebElement);
            selector.SelectByIndex(index);
        }

        public void Select(string text)
        {
            Select(text, true);
        }

        public void Select(string text, bool exactMatch)
        {
            if(exactMatch)
            {
                SelectElement selector = new SelectElement(WebElement);
                selector.SelectByText(text);
            }
            else
            {
                try
                {
                    WebElement.SendKeys(text);
                }
                catch(Exception)
                {

                }
            }
        }

        public int NumOptions()
        {
            SelectElement selector = new SelectElement(WebElement);
            return selector.Options.Count();
        }
    }
}
