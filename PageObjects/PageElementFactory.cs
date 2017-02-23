using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenQA.Selenium;

namespace PageObjects
{
    public class PageElementFactory
    {
        public static T InitElement<T>(IWebDriver webDriver, By by)  where T : PageElement, new()
        {
            if (webDriver == null)
            {
                throw new InvalidOperationException("Set WebDriver instance before initializing elements.");
            }

            IWebElement webElement = null;
            try
            {
                // webElement can be null if FindElement throws an exception
                webElement = webDriver.FindElement(by);
            }
            catch(Exception)
            {

            }

            return InitElement<T>(webDriver, webElement);
        }

        // If the specific PageElement type is provided, we use that to initialize
        public static T InitElement<T>(IWebDriver webDriver, IWebElement webElement) where T : PageElement, new()
        {
            if(webDriver == null)
            {
                throw new InvalidOperationException("Set WebDriver instance before initializing elements.");
            }

            T pageElement = new T() { WebElement = webElement };

            pageElement.WebDriver = webDriver;
            // do other initialization here, perhaps cross check the specified T with the element tag name?
            return pageElement;
        }
    }
}
