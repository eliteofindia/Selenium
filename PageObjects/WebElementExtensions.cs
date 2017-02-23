using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using OpenQA.Selenium;
using OpenQA.Selenium.Support;
using OpenQA.Selenium.Remote;
using OpenQA.Selenium.Support.UI;

namespace PageObjects
{
   public static class WebElementExtensions
    {
        public static bool WaitToHide(this IWebElement WebElement, int WaitTime)
        {
            bool hide = true;
            try
            {
                hide = !(WebElement.Displayed);
            }
            catch (Exception)
            {
                return false;
            }
            while (!hide)
            {
                try
                {
                    hide = !(WebElement.Displayed);
                }
                catch (Exception)
                {
                    hide = false;
                }
                System.Threading.Thread.Sleep(1000);
                WaitTime--;
                if (WaitTime <= 0) { break; }
            }

            return hide;
        }
    }
}
