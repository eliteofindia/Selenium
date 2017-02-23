using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenQA.Selenium;

namespace PageObjects
{
    public class TableHeader : PageElement, IEnumerable<PageElement>
    {
        public override string Value
        {
            get
            {
                return this.WebElement.Text;
            }
            set
            {
                throw new NotSupportedException("Cannot set value to a table header.");
            }
        }

        public PageElement this[int number]
        {
            get
            {
                number = number + 1;
                if (number >= 1 && number <= NumColumns)
                {
                    string xpath = "./tr/th[" + number + "]";
                    IWebElement cellWebElement = WebElement.FindElement(By.XPath(xpath));
                    PageElement pageElement = PageElementFactory.InitElement<PageElement>(WebDriver, cellWebElement);

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
                    _numColumns = WebElement.FindElements(By.XPath("./tr/th")).Count;
                }

                return _numColumns;
            }
        }

        public IEnumerator<PageElement> GetEnumerator()
        {
            return new TableHeaderEnumerator(this);
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }

    public class TableHeaderEnumerator : IEnumerator<PageElement>
    {
        private TableHeader _tableHeader;
        private int _index = 0;

        public TableHeaderEnumerator(TableHeader tableHeader)
        {
            _tableHeader = tableHeader;
        }

        public PageElement Current
        {
            get 
            {
                return _tableHeader[_index];
            }
        }

        public void Dispose()
        {
            _tableHeader = null;
        }

        object System.Collections.IEnumerator.Current
        {
            get { return Current; }
        }

        public bool MoveNext()
        {
            _index++;
            return _index < _tableHeader.NumColumns;
        }

        public void Reset()
        {
            _index = 0;
        }
    }
}
