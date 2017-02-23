using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;

namespace PageObjects
{
    public class TableRow : PageElement, IEnumerable<PageElement>
    {
        public TableHeader TableHeader { get; set; }

        public override string Value
        {
            get
            {
                return this.WebElement.Text;
            }
            set
            {
                throw new NotSupportedException("Cannot set value to a table row.");
            }
        }

        public PageElement this[int number]
        {
            get
            {
                number = number + 1; // HTML tables are 1-index
                if (number >= 1 && number <= NumColumns)
                {
                    string xpath = "./td[" + number + "]";
                    IWebElement cellWebElement = WebElement.FindElement(By.XPath(xpath));
                    PageElement pageElement = null;

                    // we will try to initialize the contents of the table cell, depending on the tag name of the contents
                    try
                    {
                        WebDriver.EnableWait(false);
                        IWebElement childElement = null;
                        if ((childElement = FindDescendantByType(cellWebElement, "select")) != null)
                        {
                            pageElement = PageElementFactory.InitElement<DropDownList>(WebDriver, childElement);
                        }
                        else if ((childElement = FindDescendantByType(cellWebElement, "input")) != null)
                        {
                            pageElement = PageElementFactory.InitElement<TextField>(WebDriver, childElement);
                        }
                        else
                        {
                            pageElement = PageElementFactory.InitElement<Label>(WebDriver, cellWebElement);
                        }
                    }
                    catch(Exception)
                    {
                    }
                    finally
                    {
                        WebDriver.EnableWait(true);
                    }

                    return pageElement;
                }
                else
                {
                    throw new IndexOutOfRangeException("Tried to access invalid index number: " + number);
                }
            }
        }

        private int _numColumns = -1;
        public int NumColumns
        {
            get
            {
                if(_numColumns < 0)
                {
                    _numColumns = WebElement.FindElements(By.XPath("./td")).Count;
                }

                return _numColumns;
            }
        }

        public IEnumerator<PageElement> GetEnumerator()
        {
            throw new NotImplementedException();
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            throw new NotImplementedException();
        }

        public class TableRowEnumerator : IEnumerator<PageElement>
        {
            private TableRow _tableRow;
            private int _index = 0;

            public TableRowEnumerator(TableRow tableRow)
            {
                _tableRow = tableRow;
            }

            public PageElement Current
            {
                get
                {
                    return _tableRow[_index];
                }
            }

            public void Dispose()
            {
                _tableRow = null;
            }

            object System.Collections.IEnumerator.Current
            {
                get { return Current; }
            }

            public bool MoveNext()
            {
                _index++;
                return _index < _tableRow.NumColumns;
            }

            public void Reset()
            {
                _index = 0;
            }
        }
    }
}
