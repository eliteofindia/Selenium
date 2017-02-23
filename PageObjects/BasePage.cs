using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenQA.Selenium;
using OpenQA.Selenium.Support.PageObjects;

namespace PageObjects
{
    public enum BROWSER_TARGETS
    {
        FIREFOX,
        FIREFOX36,
        IE8,
        IE9,
        IE10,
        IE11,
        CHROME,
        CHROMEINCOGNITO,
        OPERA,
        ANDROID,
        IPAD
    }

    public abstract class BasePage
    {
        public IWebDriver WebDriver { get; private set; }
        public BROWSER_TARGETS BrowserTarget { get; set; }

       
        protected BasePage(IWebDriver webDriver)
        {
            WebDriver = webDriver;
        }

        protected void LOG(string msg)
        {
            System.Console.WriteLine("DBG: " + msg);
        }

        protected T InitElement<T>(By by) where T : PageElement, new()
        {
            return PageElementFactory.InitElement<T>(WebDriver, by);
        }

        protected T InitElement<T>(IWebElement webElement) where T : PageElement, new()
        {
            return PageElementFactory.InitElement<T>(WebDriver, webElement);
        }

        public abstract void InitPageElements(BROWSER_TARGETS browserTarget);

        // popup dialog handling
        public IAlert AlertDialog
        {
            get 
            {
                try
                {
                    IAlert alert = WebDriver.SwitchTo().Alert();
                    return alert;
                }
                catch(NoAlertPresentException)
                {
                    return null;
                }
            }
        }
    }
}
