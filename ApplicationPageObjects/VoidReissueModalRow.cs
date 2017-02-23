using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PageObjects;

namespace SonePageObjects
{
   
        public enum ModalCols
        {
            FIRST,
            SECOND
        }
        public class VoidReissueModalRow : TableRow
        {
            private IDictionary<ModalCols, int> _columnMap = null;

            public PageElement this[ModalCols column]
            {
                get
                {
                    if (_columnMap == null)
                    {
                        InitColumnMap();
                    }

                    int columnIndex = 0;
                    PageElement pageElement = null;
                    try
                    {
                        columnIndex = _columnMap[column];
                    }
                    catch (KeyNotFoundException)
                    {
                        throw new InvalidOperationException("Index was not found for: " + column);
                    }

                    try
                    {
                        pageElement = this[columnIndex];
                    }
                    catch (KeyNotFoundException)
                    {
                        throw new InvalidOperationException("Page Element was not found for: " + column);
                    }

                    return pageElement;
                }
            }

            private void InitColumnMap()
            {
                _columnMap = new Dictionary<ModalCols, int>();
                for (int idx = 0; idx < TableHeader.NumColumns; idx++)
                {
                 _columnMap.Add(ModalCols.FIRST, 0);
                 _columnMap.Add(ModalCols.SECOND, 1);
                }
            }

            // Convenience Properties
            public string FirstValue
            {
                get
                {
                    return this[ModalCols.FIRST].Value;
                }
            }

            public string SecondValue
            {
                get
                {
                    return this[ModalCols.SECOND].Value;
                }
            
            }

    }
}
