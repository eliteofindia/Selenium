using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenQA.Selenium;

namespace PageObjects
{
    public class PageElement
    {
        // Bad design, ideally PageElement would not have a WebDriver property. However, it seems to be a necessary
        //  evil as we need direct access to the WebDriver in some cases (ie setting explicit waits)
        public IWebDriver WebDriver { protected get; set; }
        public virtual IWebElement WebElement { get; set; }

        protected static IWebElement FindDescendantByType(IWebElement parent, string type)
        {
            IWebElement child = null;
            try
            {
                child = parent.FindElement(By.XPath(".//" + type));
                if (child.GetAttribute("type") == "date")
                {
                    child = parent.FindElements(By.XPath(".//" + type))[1];
                }
            }
            catch(Exception)
            {

            }

            return child;
        }

        public bool Visible
        {
            get 
            {
                bool retval = false;

                if(WebElement != null)
                {
                    try
                    {
                        WebDriver.EnableWait(false);
                        retval = WebElement.Displayed;

                        // now check if the element has the 'display' attribute
                        if (WebElement.GetAttribute("display").Equals("none", StringComparison.InvariantCultureIgnoreCase))
                        {
                            retval = false;
                        }
                    }
                    catch (Exception)
                    {

                    }
                    finally
                    {
                        WebDriver.EnableWait(true);
                    }
                }

                return retval;
            }
        }

        public bool Enabled
        {
            get 
            {
                bool retval = false;

                if(WebElement != null)
                {
                    retval = WebElement.Enabled;
                }

                return retval; 
            }
        }

        public bool Selected
        {
            get
            {
                bool retval = false;

                if (WebElement != null)
                {
                    retval = WebElement.Selected;
                }

                return retval;
            }
        }

        public virtual string Value
        {
            get
            {
                return this.WebElement.Text;
            }
            set
            {
                this.WebElement.SendKeys(value);
            }

        }

        public static void LOG(string msg)
        {
            Console.WriteLine("PageElement: ");
        }
    }
}
