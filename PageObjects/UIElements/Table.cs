using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenQA.Selenium;

namespace PageObjects
{
    public class Table<T> : PageElement, IEnumerable<T> where T : TableRow, new()
    {
        public int BodyIndex = 1;
        public override string Value
        {
            get
            {
                return this.WebElement.Text; // does this make sense? do we ever need the text value of a table element? is there such a thing?
            }
            set
            {
                throw new NotSupportedException("Cannot set value to a table.");
            }
        }

        public T this[int number]
        {
            get
            {
                number = number + 1;
                if(_tableRowCache.ContainsKey(number))
                {
                    return _tableRowCache[number];
                }

                if(number >= 1 && number <= NumRows)
                {
                    string xpath = "./tbody[" + this.BodyIndex + "]/tr[" + number + "]";
                    IWebElement webelement = WebElement.FindElement(By.XPath(xpath));

                    T tableRow = PageElementFactory.InitElement<T>(WebDriver, webelement);
                    tableRow.TableHeader = TableHeader;

                    _tableRowCache.Add(number, tableRow);

                    return tableRow;
                }
                else
                {
                    throw new IndexOutOfRangeException("Tried to access invalid row number: " + number);
                }
            }
        }

        private TableHeader _tableHeader = null;
        private IDictionary<int, T> _tableRowCache = new Dictionary<int, T>();
        public TableHeader TableHeader 
        { 
            get
            {
                if(_tableHeader == null)
                {
                        string xpath = "./thead";
                        IWebElement webelement = WebElement.FindElement(By.XPath(xpath));
                        _tableHeader = PageElementFactory.InitElement<TableHeader>(WebDriver, webelement);
                }

                return _tableHeader;
            }
        }

        public int NumRows
        {
            get
            {
                return WebElement.FindElements(By.XPath("./tbody/tr")).Count;
            }
        }

        public int OnlySearchResultsRows
        {
            get
            {
                return WebElement.FindElements(By.XPath("./tbody[2]/tr")).Count;
            }
        }

        public int NumColumns
        {
            get
            {
                return WebElement.FindElements(By.XPath("./thead/tr/th")).Count;
            }
        }

        public IEnumerator<T> GetEnumerator()
        {
            throw new NotImplementedException();
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }

    public class TableEnumerator<T> : IEnumerator<T> where T : TableRow, new()
    {
        private Table<T> _table;
        private int _index = 0;

        public TableEnumerator(Table<T> table)
        {
            _table = table;
        }

        public T Current
        {
            get 
            {
                return _table[_index];
            }
        }

        public void Dispose()
        {
            _table = null;
        }

        object System.Collections.IEnumerator.Current
        {
            get { return Current; }
        }

        public bool MoveNext()
        {
            _index++;

            return _index <= _table.NumRows;
        }

        public void Reset()
        {
            _index = 0;
        }
    }
}
