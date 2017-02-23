using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Remote;
using OpenQA.Selenium.Support.PageObjects;
using SonePageObjects;
using PageObjects;
using NUnit.Framework.Interfaces;

namespace SoneAutomatedTests
{
    [Parallelizable(ParallelScope.Fixtures)]
    //[TestFixture(BROWSER_TARGETS.IE10)]
    //[TestFixture(BROWSER_TARGETS.IE11)]
    //[TestFixture(BROWSER_TARGETS.FIREFOX)]
    [TestFixture(BROWSER_TARGETS.CHROME)]    
    public abstract class BaseTestAllBrowsers
    {
        protected const string SONE_LOGIN_ADDRESS = "http://10.65.2.230:8609/";//"http://s1issuingprod.grh.lab:8605/";
        protected SoneModel S1 { get; set; }

        public DesiredCapabilities BrowserCapabilities { get; private set; }
        public BROWSER_TARGETS BrowserTarget { get; private set; }
        public IWebDriver WebDriver { get; set; }
        public bool takeScreenshot = true; 

        public BaseTestAllBrowsers(BROWSER_TARGETS target)
        {
            BrowserTarget = target;
            switch (target)
            {
                case BROWSER_TARGETS.FIREFOX:
                    BrowserCapabilities = DesiredCapabilities.Firefox();
                    break;
                case BROWSER_TARGETS.FIREFOX36:
                    BrowserCapabilities = DesiredCapabilities.Firefox();
                    BrowserCapabilities.SetCapability(CapabilityType.Version, "3.6");
                    break;
                case BROWSER_TARGETS.IE8:
                    BrowserCapabilities = DesiredCapabilities.InternetExplorer();
                    BrowserCapabilities.SetCapability(CapabilityType.Version, "8");
                    break;
                case BROWSER_TARGETS.IE9:
                    BrowserCapabilities = DesiredCapabilities.InternetExplorer();
                    BrowserCapabilities.SetCapability(CapabilityType.Version, "9");
                    break;
                case BROWSER_TARGETS.IE10:
                    BrowserCapabilities = DesiredCapabilities.InternetExplorer();
                    BrowserCapabilities.SetCapability(CapabilityType.Version, "10");
                    break;
                case BROWSER_TARGETS.IE11:
                    BrowserCapabilities = DesiredCapabilities.InternetExplorer();
                    BrowserCapabilities.SetCapability(CapabilityType.Version, "11");
                    break;
                case BROWSER_TARGETS.CHROME:
                    {
                        ChromeOptions options = new ChromeOptions();
                        options.AddExcludedArguments((new string[] { "test-type", "ignore-certificate-errors" }).ToList<string>());
                        BrowserCapabilities = (DesiredCapabilities)options.ToCapabilities();
                    }
                    break;
                case BROWSER_TARGETS.CHROMEINCOGNITO:
                    {
                        ChromeOptions options = new ChromeOptions();
                        options.AddExcludedArguments((new string[] { "test-type", "ignore-certificate-errors" }).ToList<string>());
                        options.AddArgument("-incognito");
                        BrowserCapabilities = (DesiredCapabilities)options.ToCapabilities();
                    }
                    break;
                case BROWSER_TARGETS.OPERA:
                    BrowserCapabilities = DesiredCapabilities.Opera();
                    break;
                case BROWSER_TARGETS.ANDROID:
                    BrowserCapabilities = DesiredCapabilities.Android();
                    break;
                case BROWSER_TARGETS.IPAD:
                    BrowserCapabilities = DesiredCapabilities.IPad();
                    break;
                default:
                    BrowserCapabilities = new DesiredCapabilities();
                    break;
            }

            BrowserCapabilities.IsJavaScriptEnabled = true;
        }


        [SetUp]
        public void BaseInit()
        {
            // WebDriver = new RemoteWebDriver(new Uri(SeleniumGridHubAddress), BrowserCapabilities);
            WebDriver = new RemoteWebDriver(new Uri(SeleniumGridHubAddress), BrowserCapabilities, TimeSpan.FromMinutes(5));
            //WebDriver.EnableWait(true);

            WebDriver.Manage().Window.Maximize();
            WebDriver.Url = SONE_LOGIN_ADDRESS;

            S1 = new SoneModel(WebDriver, BrowserTarget);
        }

        [TearDown]
        public void BaseDispose()
        {
            // Explicit logout if we are in issuing page
            try
            {
                ResultState result = TestContext.CurrentContext.Result.Outcome;
                if ("PASSED" != result.Status.ToString().ToUpper() && takeScreenshot)
                {
                    string filePrefix = TestContext.CurrentContext.Test.Name.Split(',')[0].Replace("\"", "").Replace("(", "").Replace(")", "").Replace("Test", "_");
                    string filepath = TestResultsDir + "/" + filePrefix + "_" + CurrentBrowser + DateTime.Now.ToString().Replace("/", "").Replace(":", "") + ".png";
                    TakeScreenshot(filepath);
                }
                else
                {
                    takeScreenshot = true;
                }
            }
            catch (Exception)
            {

            }
            try
            {
                S1.Logout();
            }
            catch(Exception)
            {

            }
            
            WebDriver.Quit();
        }

        protected string CurrentBrowser { get { return BrowserTarget.ToString(); } }
        protected string TestResultsDir 
        { 
            get 
            {
                string strTestResultsDir = WorkDir + "/TestResults/";
                if(!Directory.Exists(strTestResultsDir))
                {
                    Directory.CreateDirectory(strTestResultsDir);
                }

                return strTestResultsDir; 
            } 
        }

        protected string WorkDir { get { return TestContext.CurrentContext.WorkDirectory; } }

        private const string HUB_ADDRESS_TESTENV = "http://10.65.2.230:4444/wd/hub";//"http://s1issuingprod.grh.lab:4444/wd/hub";
        private const string HUB_ADDRESS_LOCAL = "http://localhost:4444/wd/hub";
        private string SeleniumGridHubAddress
        {
            get
            {
                return (IsDebugging ? HUB_ADDRESS_LOCAL : HUB_ADDRESS_TESTENV);
                //return (IsDebugging ? HUB_ADDRESS_LOCAL : HUB_ADDRESS_LOCAL);
                //return (IsDebugging ? HUB_ADDRESS_TESTENV : HUB_ADDRESS_TESTENV);
            }
        }

        private bool IsDebugging
        {
            get
            {
                return System.Diagnostics.Debugger.IsAttached;
            }
        }

        public void TakeScreenshot(string screenshotname)
        {
            WebDriver.TakeScreenshot(screenshotname.ToString());
        }
    }
}
