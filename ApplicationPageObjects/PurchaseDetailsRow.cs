using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PageObjects;

namespace SonePageObjects
{
    public enum COLUMN
    {
        ROW_NUM,
        RECEIPT_NUM,
        RECEIPT_DATE,
        GOODS_DESC,
        QUANTITY,
        PRICE,
        PURCHASE_AMOUNT,
        PURCHASE_AMOUNT1,
        PURCHASE_AMOUNT2,
        GROSS_AMOUNT,
        VAT,
        AMOUNT1,
        AMOUNT2,
        AMOUNT3,
        AMOUNT4,
        AMOUNT5,
        SERIAL_NUMBER
    }

    public class PurchaseDetailsRow : TableRow
    {
        private IDictionary<COLUMN, int> _columnMap = null;

        public PageElement this[COLUMN column]
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
                catch(KeyNotFoundException)
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
            _columnMap = new Dictionary<COLUMN, int>();
            for(int idx = 0; idx < TableHeader.NumColumns; idx++)
            {
                string headerId = TableHeader[idx].WebElement.GetAttribute("id");
                string locId = TableHeader[idx].WebElement.GetAttribute("data-i18n");
                string HeaderText = TableHeader[idx].WebElement.Text;

                if ("app.ChequeNumberHeader".Equals(locId))
                {
                    _columnMap.Add(COLUMN.ROW_NUM, idx);
                }
                else if ("app.Descr".Equals(locId))
                {
                    _columnMap.Add(COLUMN.GOODS_DESC, idx);
                }
                else if ("app.Qty".Equals(locId))
                {
                    _columnMap.Add(COLUMN.QUANTITY, idx);
                }
                else if ("app.Price".Equals(locId))
                {
                    _columnMap.Add(COLUMN.PRICE, idx);
                }
                else if ("app.VAT".Equals(locId))
                {
                    _columnMap.Add(COLUMN.VAT, idx);
                }
                else if ("app.RecieptNo".Equals(locId))
                {
                    _columnMap.Add(COLUMN.RECEIPT_NUM, idx);
                }
                else if ("app.ReceiptDate".Equals(locId))
                {
                    _columnMap.Add(COLUMN.RECEIPT_DATE, idx);
                }
                else if ("app.GROSSAmount".Equals(locId))
                {
                    _columnMap.Add(COLUMN.GROSS_AMOUNT, idx);
                }
                else if ("app.PurchaseAmount".Equals(locId) || HeaderText.Contains("Purchase Amount"))
                {
                    if (_columnMap.ContainsKey(COLUMN.PURCHASE_AMOUNT))
                    {
                        if (_columnMap.ContainsKey(COLUMN.PURCHASE_AMOUNT1))
                        {
                            _columnMap.Add(COLUMN.PURCHASE_AMOUNT2, idx);
                        }
                        else
                        {
                            _columnMap.Add(COLUMN.PURCHASE_AMOUNT1, idx);
                        }
                    }
                    else { _columnMap.Add(COLUMN.PURCHASE_AMOUNT, idx); }
                }
                else if ("amount1".Equals(headerId))
                {
                    _columnMap.Add(COLUMN.AMOUNT1, idx);
                }
                else if ("amount2".Equals(headerId))
                {
                    _columnMap.Add(COLUMN.AMOUNT2, idx);
                }
                else if ("amount3".Equals(headerId))
                {
                    _columnMap.Add(COLUMN.AMOUNT3, idx);
                }
                else if ("amount4".Equals(headerId))
                {
                    _columnMap.Add(COLUMN.AMOUNT4, idx);
                }
                else if ("amount5".Equals(headerId))
                {
                    _columnMap.Add(COLUMN.AMOUNT5, idx);
                }
                else if(HeaderText.Contains("Serial Number"))
                {
                    _columnMap.Add(COLUMN.SERIAL_NUMBER, idx);
                }
                else
                {
                    throw new NotImplementedException("Unmapped purchase details column: " + locId);
                }
            }
        }

        // Convenience Properties
        public string RowNum
        {
            get
            {
                return this[COLUMN.ROW_NUM].Value;
            }
        }

        public string ReceiptNumber
        {
            get
            {
                return this[COLUMN.RECEIPT_NUM].Value;
            }
            set
            {
                this[COLUMN.RECEIPT_NUM].Value = value;
            }
        }

        public string ReceiptDate
        {
            get
            {
                return this[COLUMN.RECEIPT_DATE].Value;
            }
            set
            {
                this[COLUMN.RECEIPT_DATE].Value = value;
            }
        }

        public string GoodsDescription
        {
            get
            {
                return this[COLUMN.GOODS_DESC].Value;
            }
            set
            {
                this[COLUMN.GOODS_DESC].Value = value;
            }
        }

        public string Quantity
        {
            get
            {
                return this[COLUMN.QUANTITY].Value;
            }
            set
            {
                this[COLUMN.QUANTITY].Value = value;
            }
        }

        public string Price
        {
            get
            {
                return this[COLUMN.PRICE].Value;
            }
            set
            {
                this[COLUMN.PRICE].Value = value;
            }
        }

        public string PurchaseAmount
        {
            get
            {
                return this[COLUMN.PURCHASE_AMOUNT].Value;
            }
            set
            {
                this[COLUMN.PURCHASE_AMOUNT].Value = value;
            }
        }

        public string GrossAmount
        {
            get
            {
                return this[COLUMN.GROSS_AMOUNT].Value;
            }
            set
            {
                this[COLUMN.GROSS_AMOUNT].Value = value;
            }
        }

        public string Amount1
        {
            get
            {
                return this[COLUMN.AMOUNT1].Value;
            }
            set
            {
                this[COLUMN.AMOUNT1].Value = value;
            }
        }

        public string Amount2
        {
            get
            {
                return this[COLUMN.AMOUNT2].Value;
            }
            set
            {
                this[COLUMN.AMOUNT2].Value = value;
            }
        }

        public string Amount3
        {
            get
            {
                return this[COLUMN.AMOUNT3].Value;
            }
            set
            {
                this[COLUMN.AMOUNT3].Value = value;
            }
        }

        public string Amount4
        {
            get
            {
                return this[COLUMN.AMOUNT4].Value;
            }
            set
            {
                this[COLUMN.AMOUNT4].Value = value;
            }
        }

        public string Amount5
        {
            get
            {
                return this[COLUMN.AMOUNT5].Value;
            }
            set
            {
                this[COLUMN.AMOUNT5].Value = value;
            }
        }

        public string VAT
        {
            get
            {
                return this[COLUMN.VAT].Value;
            }
            set
            {
                this[COLUMN.VAT].Value = value;
            }
        }

        public string SERIAL_NUM
        {
            get
            {
                return this[COLUMN.SERIAL_NUMBER].Value;
            }
            set
            {
                this[COLUMN.SERIAL_NUMBER].Value = value;
            }
        }
    }
}
