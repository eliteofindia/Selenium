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
    public static class WebDriverExtensions
    {
        public static int DefaultWaitInSec = 30;
        public static TimeSpan DefaultWait = TimeSpan.FromSeconds(DefaultWaitInSec);
        private const int MAX_RETRY = 3;

        public static void EnableWait(this IWebDriver webDriver, bool enable)
        {
            EnableWait(webDriver, enable, 1);
        }

        private static void EnableWait(this IWebDriver webDriver, bool enable, int retryCount)
        {
            try
            {
                if (enable)
                {
                    webDriver.Manage().Timeouts().ImplicitlyWait(DefaultWait);
                }
                else
                {
                    webDriver.Manage().Timeouts().ImplicitlyWait(TimeSpan.FromSeconds(0));
                }
            }
            catch(Exception e)
            {
                if(retryCount >= MAX_RETRY)
                {
                    throw e;
                }
                else
                {
                    EnableWait(webDriver, enable, ++retryCount);
                }
            }
        }

        public static IWebElement FindElement(this IWebDriver webDriver, By by, int timeoutInSeconds)
        {
            IWebElement retval = null;

            if(timeoutInSeconds > 0)
            {
                var wait = new WebDriverWait(webDriver, TimeSpan.FromSeconds(timeoutInSeconds));
                retval = wait.Until(drv => drv.FindElement(by));
            }
            else
            {
                try
                {
                    webDriver.EnableWait(false);
                    retval = webDriver.FindElement(by);
                }
                catch(Exception)
                {

                }
                finally
                {
                    webDriver.EnableWait(true);
                }
            }

            return retval;
        }

        public static IWebElement WaitForElement(this IWebDriver webDriver, By by, int timeoutInSeconds)
        {
            IWebElement retval = null;

            var wait = new WebDriverWait(webDriver, TimeSpan.FromSeconds(timeoutInSeconds));
            retval = wait.Until(ExpectedConditions.ElementIsVisible(by));

            return retval;
        }


        public static T WaitForElement<T>(this IWebDriver webDriver, T element, int timeoutInSeconds) where T : PageElement, new()
        {
            T retval = null;
            int i = 0;
            while (i < timeoutInSeconds)
            {
                i++;
                if(element.Visible)
                {
                    retval = element;
                    break;
                }
                System.Threading.Thread.Sleep(1000);
            }

            return retval;
        }

        public static void TakeScreenshot(this IWebDriver webDriver, string fullFileName)
        {
            Screenshot screenShot = ((ITakesScreenshot)webDriver).GetScreenshot();
            screenShot.SaveAsFile(fullFileName, System.Drawing.Imaging.ImageFormat.Png);
        }

        public static Bitmap TakeScreenshot(this IWebDriver webDriver)
        {
            Screenshot screenShot = ((ITakesScreenshot)webDriver).GetScreenshot();
            Bitmap wholeBmp;

            using (var memstream = new MemoryStream(screenShot.AsByteArray))
            {
                wholeBmp = new Bitmap(memstream);
            }

            return wholeBmp;
        }

        public static Tuple<Bitmap, Bitmap> TakeScreenshot(this IWebDriver webDriver, IWebElement webElement, string fullFileName)
        {
            int x = webElement.Location.X;
            int y = webElement.Location.Y;
            int width = webElement.Size.Width;
            int height = webElement.Size.Height;

            return TakeScreenshot(webDriver, x, y, width, height);
        }

        public static Tuple<Bitmap,Bitmap> TakeScreenshot(this IWebDriver webDriver, int x, int y, int width, int height)
        {
            Screenshot screenShot = ((ITakesScreenshot)webDriver).GetScreenshot();
            Rectangle cropArea = new Rectangle(x, y, width, height);

            Bitmap wholeBmp;
            Bitmap croppedBmp;

            using (var memstream = new MemoryStream(screenShot.AsByteArray))
            {
                wholeBmp = new Bitmap(memstream);
                if((cropArea.Height + cropArea.Y) > wholeBmp.Height)
                {
                    if(cropArea.Height > wholeBmp.Height)
                    {
                        cropArea.Height = wholeBmp.Height;
                    }

                    cropArea.Y = (wholeBmp.Height - cropArea.Height) / 2;
                }

                try
                {
                    croppedBmp = wholeBmp.Clone(cropArea, wholeBmp.PixelFormat);
                }
                catch(Exception)
                {
                    croppedBmp = wholeBmp;
                }
            }

            return new Tuple<Bitmap, Bitmap>(wholeBmp, croppedBmp);
        }

        private static void LOG(string msg)
        {
            Console.Out.WriteLine("WebDriverExtensions: " + msg);
        }

        public static void WaitTillPageLoads(this IWebDriver WebDriver)
        {
            new WebDriverWait(WebDriver, TimeSpan.FromSeconds(10)).Until(wd => ((IJavaScriptExecutor)wd).ExecuteScript("return document.readyState").Equals("complete"));
        }

        public static void ScrollToElement(this IWebDriver angWebDriver, IWebElement WebElement)
        {
            ((IJavaScriptExecutor)(angWebDriver)).ExecuteScript("arguments[0].scrollIntoView(true);", WebElement);
        }

        public static IWebElement TabAndReturnFocusedElement(this IWebDriver WebDriver)
        {
            OpenQA.Selenium.Interactions.Actions press = new OpenQA.Selenium.Interactions.Actions(WebDriver);
            press.SendKeys(Keys.Tab).Perform();
            return WebDriver.SwitchTo().ActiveElement();
        }
    }
}
