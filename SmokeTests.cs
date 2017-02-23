using System;
using System.Runtime.CompilerServices;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text.RegularExpressions;
using NUnit.Framework;
using PageObjects;
using SonePageObjects;
using SoneAutomatedTests.DataModel;
using Newtonsoft.Json;

namespace SoneAutomatedTests
{
    /*
     *  Smoke Tests Scope
     *   1. Issue TFF
     *   2. Void TFF
     *   3. Amend TFF
     *   4. Status TFF
     * 
     *  Parameters
     *   1. Autofill vs Anonymous travellers
     *   2. Cash vs CC Refund targets
     */
    [Parallelizable(ParallelScope.Fixtures)]
    [Category("SmokeTests")]
    public class SmokeTests : BaseTestAllBrowsers
    {
        string doc_ID = null;
        public SmokeTests(BROWSER_TARGETS target)
            : base(target)
        {
        }

        [CsvTestCaseSource]
        public void LoginTest(string username, string password, bool isValid)
        {
            bool loggedIn = S1.Login(username, password);

            if (loggedIn != isValid)
            {
                Bitmap screenshot = WebDriver.TakeScreenshot();
                SaveBitmap(WebDriver.TakeScreenshot(), "LoginTest_" + username);

                throw new AssertionException("Login failed for user: " + username);
            }
        }


        [CsvTestCaseSource]
        public void TestBasicSearch(string username, string password,
        string identifier, string travellerID,
        string firstname, string lastname,
        string passport, string passportCountry,
        string email, string dateOfBirth, string gender,
        string address, string city, string zipCode, string residenceCountry,
        string receiptNum, string receiptDate, string goodsDescription,
        string quantity, string price,
        string purchaseAmount, string grossAmount,
        string amount1, string amount2, string amount3, string amount4, string amount5,
        string vat, string refundService, string refundMethod,
        string creditCardNum, string cardScheme, string expiryDate,
        string IBANNo, string BICNo,
        string bankName, string bankAddress, string bankAccountNo,
        string merchandiseDesc, string shopInvoiceNumber,
        string purchaseDate, string arrivalDate, string departureDate,
        string finalDestination, string tourGuideNumber,
        bool expectError)
        {
            if (creditCardNum != string.Empty)
                Regex.Replace(creditCardNum, @"\s+", "");

            IssuingTestCase tc = new IssuingTestCase
            {
                ExpectError = expectError,
                Login = new LoginDetails
                {
                    Username = username,
                    Password = password
                },
                Traveller = new TravellerDetails
                {
                    Autofill = new AutofillDetails
                    {
                        Identifier = identifier,
                        TravellerID = travellerID
                    },
                    FirstName = firstname,
                    LastName = lastname,
                    Passport = new PassportDetails
                    {
                        Number = passport,
                        Country = passportCountry
                    },
                    Address = new AddressDetails
                    {
                        Street = address,
                        City = city,
                        ZipCode = zipCode,
                        Country = residenceCountry
                    },
                    Email = email,
                    BirthDate = dateOfBirth,
                    Gender = gender,
                    Flight = new FlightDetails
                    {
                        ArrivalDate = arrivalDate,
                        DepartureDate = departureDate,
                        FinalDestination = finalDestination,
                        TourGuideNumber = tourGuideNumber
                    }
                },
                Purchase = new PurchaseDetails
                {
                    Items = new List<ItemDetails>(),
                    MerchandiseDescription = merchandiseDesc,
                    ShopInvoiceNumber = shopInvoiceNumber,
                    PurchaseDate = purchaseDate
                },
                Refund = new RefundDetails
                {
                    Service = refundService,
                    Method = refundMethod,
                    CreditCard = new CreditCardDetails
                    {
                        CreditCardNumber = creditCardNum,
                        CardScheme = cardScheme,
                        ExpiryDate = expiryDate
                    },
                    Bank = new BankDetails
                    {
                        IBAN = IBANNo,
                        BIC = BICNo,
                        Name = bankName,
                        Address = bankAddress,
                        AccountNumber = bankAccountNo,
                        AccountName = firstname + " " + lastname
                    }
                }
            };

            tc.Purchase.Items.Add(new ItemDetails
            {
                Receipt = new ReceiptDetails
                {
                    Number = receiptNum,
                    Date = receiptDate
                },
                Description = goodsDescription,
                Quantity = quantity,
                Price = price,
                PurchaseAmount = purchaseAmount,
                GrossAmount = grossAmount,
                Amount1 = amount1,
                Amount2 = amount2,
                Amount3 = amount3,
                Amount4 = amount4,
                Amount5 = amount5,
                Vat = vat,

            });

            TestBasicSearchTxnByDocID("TestBasicSearch", tc);
        }


        [JsonTestCaseSource]
        public void TestBasicSearchTxnByDocID(string tcName, IssuingTestCase tc)
        {
            RunIssuingTest(tc, out doc_ID);
            //SearchPage searchPage = S1.IssuingPage.NavigateToSearchPage();
            S1.NavigateToSearchPage();
            bool searched = S1.SearchPage.SearchTxnByDocID(doc_ID);
            if (searched)
            {
                string displayedDocID = S1.SearchPage.GetColumnValue(0, COLUMNS.DOC_ID); //(searchPage.S_SearchedResults[0].Doc_ID).Replace(" ", "");
                Assert.AreEqual(doc_ID, displayedDocID);
            }
            else
            {
                Assert.Fail("Search Transaction by Doc ID is not performed");
            }

        }


        [CsvTestCaseSource]
        public void TestBasicVoid(string username, string password,
        string identifier, string travellerID,
        string firstname, string lastname,
        string passport, string passportCountry,
        string email, string dateOfBirth, string gender,
        string address, string city, string zipCode, string residenceCountry,
        string receiptNum, string receiptDate, string goodsDescription,
        string quantity, string price,
        string purchaseAmount, string grossAmount,
        string amount1, string amount2, string amount3, string amount4, string amount5,
        string vat, string refundService, string refundMethod,
        string creditCardNum, string cardScheme, string expiryDate,
        string IBANNo, string BICNo,
        string bankName, string bankAddress, string bankAccountNo,
        string merchandiseDesc, string shopInvoiceNumber,
        string purchaseDate, string arrivalDate, string departureDate,
        string finalDestination, string tourGuideNumber,
        bool expectError)
        {
            if (creditCardNum != string.Empty)
                Regex.Replace(creditCardNum, @"\s+", "");

            IssuingTestCase tc = new IssuingTestCase
            {
                ExpectError = expectError,
                Login = new LoginDetails
                {
                    Username = username,
                    Password = password
                },
                Traveller = new TravellerDetails
                {
                    Autofill = new AutofillDetails
                    {
                        Identifier = identifier,
                        TravellerID = travellerID
                    },
                    FirstName = firstname,
                    LastName = lastname,
                    Passport = new PassportDetails
                    {
                        Number = passport,
                        Country = passportCountry
                    },
                    Address = new AddressDetails
                    {
                        Street = address,
                        City = city,
                        ZipCode = zipCode,
                        Country = residenceCountry
                    },
                    Email = email,
                    BirthDate = dateOfBirth,
                    Gender = gender,
                    Flight = new FlightDetails
                    {
                        ArrivalDate = arrivalDate,
                        DepartureDate = departureDate,
                        FinalDestination = finalDestination,
                        TourGuideNumber = tourGuideNumber
                    }
                },
                Purchase = new PurchaseDetails
                {
                    Items = new List<ItemDetails>(),
                    MerchandiseDescription = merchandiseDesc,
                    ShopInvoiceNumber = shopInvoiceNumber,
                    PurchaseDate = purchaseDate
                },
                Refund = new RefundDetails
                {
                    Service = refundService,
                    Method = refundMethod,
                    CreditCard = new CreditCardDetails
                    {
                        CreditCardNumber = creditCardNum,
                        CardScheme = cardScheme,
                        ExpiryDate = expiryDate
                    },
                    Bank = new BankDetails
                    {
                        IBAN = IBANNo,
                        BIC = BICNo,
                        Name = bankName,
                        Address = bankAddress,
                        AccountNumber = bankAccountNo,
                        AccountName = firstname + " " + lastname
                    }
                }
            };

            tc.Purchase.Items.Add(new ItemDetails
            {
                Receipt = new ReceiptDetails
                {
                    Number = receiptNum,
                    Date = receiptDate
                },
                Description = goodsDescription,
                Quantity = quantity,
                Price = price,
                PurchaseAmount = purchaseAmount,
                GrossAmount = grossAmount,
                Amount1 = amount1,
                Amount2 = amount2,
                Amount3 = amount3,
                Amount4 = amount4,
                Amount5 = amount5,
                Vat = vat,

            });

            TestBasicVoid("TestBasicVoid", tc);
        }

        [JsonTestCaseSource]
        public void TestBasicVoid(string tcName, IssuingTestCase tc)
        {
            RunIssuingTest(tc, out doc_ID);
            //VoidPage voidPage = S1.IssuingPage.NavigateToVoidPage();
            S1.NavigateToVoidPage();
            if (!S1.VoidPage.searchPage.S_DocID.Visible) { S1.NavigateToVoidPage(); }
            bool searched = S1.VoidPage.SearchTxnByDocID(doc_ID);
            if (searched)
            {
                //voidPage.searchPage.S_SearchedResults.BodyIndex = 2;
                string displayedDocID = S1.VoidPage.searchPage.GetColumnValue(0, COLUMNS.DOC_ID); //(searchPage.S_SearchedResults[0].Doc_ID).Replace(" ", "");
                Assert.AreEqual(doc_ID, displayedDocID);
                bool voidPerformed = S1.VoidPage.VoidTransaction(0);
                if (voidPerformed)
                {
                    S1.NavigateToVoidPage();
                    if (!S1.VoidPage.searchPage.S_DocID.Visible) { S1.NavigateToVoidPage(); }

                    if (!S1.VoidPage.VerifyTransactionState(doc_ID))
                    {
                        Assert.Fail("After performing void operation; Transaction state is not displayed as Voided");
                    }

                }
                else
                {
                    Assert.Fail("Unable to perform void operation");
                }

            }
            else
            {
                Assert.Fail("Search Transaction by Doc ID to void transaction is not performed");
            }

        }


        [CsvTestCaseSource]
        public void TestBasicReissue(string username, string password,
        string identifier, string travellerID,
        string firstname, string lastname,
        string passport, string passportCountry,
        string email, string dateOfBirth, string gender,
        string address, string city, string zipCode, string residenceCountry,
        string receiptNum, string receiptDate, string goodsDescription,
        string quantity, string price,
        string purchaseAmount, string grossAmount,
        string amount1, string amount2, string amount3, string amount4, string amount5,
        string vat, string refundService, string refundMethod,
        string creditCardNum, string cardScheme, string expiryDate,
        string IBANNo, string BICNo,
        string bankName, string bankAddress, string bankAccountNo,
        string merchandiseDesc, string shopInvoiceNumber,
        string purchaseDate, string arrivalDate, string departureDate,
        string finalDestination, string tourGuideNumber,
        bool expectError)
        {
            if (creditCardNum != string.Empty)
                Regex.Replace(creditCardNum, @"\s+", "");

            IssuingTestCase tc = new IssuingTestCase
            {
                ExpectError = expectError,
                Login = new LoginDetails
                {
                    Username = username,
                    Password = password
                },
                Traveller = new TravellerDetails
                {
                    Autofill = new AutofillDetails
                    {
                        Identifier = identifier,
                        TravellerID = travellerID
                    },
                    FirstName = firstname,
                    LastName = lastname,
                    Passport = new PassportDetails
                    {
                        Number = passport,
                        Country = passportCountry
                    },
                    Address = new AddressDetails
                    {
                        Street = address,
                        City = city,
                        ZipCode = zipCode,
                        Country = residenceCountry
                    },
                    Email = email,
                    BirthDate = dateOfBirth,
                    Gender = gender,
                    Flight = new FlightDetails
                    {
                        ArrivalDate = arrivalDate,
                        DepartureDate = departureDate,
                        FinalDestination = finalDestination,
                        TourGuideNumber = tourGuideNumber
                    }
                },
                Purchase = new PurchaseDetails
                {
                    Items = new List<ItemDetails>(),
                    MerchandiseDescription = merchandiseDesc,
                    ShopInvoiceNumber = shopInvoiceNumber,
                    PurchaseDate = purchaseDate
                },
                Refund = new RefundDetails
                {
                    Service = refundService,
                    Method = refundMethod,
                    CreditCard = new CreditCardDetails
                    {
                        CreditCardNumber = creditCardNum,
                        CardScheme = cardScheme,
                        ExpiryDate = expiryDate
                    },
                    Bank = new BankDetails
                    {
                        IBAN = IBANNo,
                        BIC = BICNo,
                        Name = bankName,
                        Address = bankAddress,
                        AccountNumber = bankAccountNo,
                        AccountName = firstname + " " + lastname
                    }
                }
            };

            tc.Purchase.Items.Add(new ItemDetails
            {
                Receipt = new ReceiptDetails
                {
                    Number = receiptNum,
                    Date = receiptDate
                },
                Description = goodsDescription,
                Quantity = quantity,
                Price = price,
                PurchaseAmount = purchaseAmount,
                GrossAmount = grossAmount,
                Amount1 = amount1,
                Amount2 = amount2,
                Amount3 = amount3,
                Amount4 = amount4,
                Amount5 = amount5,
                Vat = vat,

            });

            TestBasicReissue("TestBasicReissue", tc);
        }

        [JsonTestCaseSource]
        public void TestBasicReissue(string tcName, IssuingTestCase tc)
        {
            //Dictionary<string, string> details = new Dictionary<string, string>();
            string new_Doc_ID = null;
            RunIssuingTest(tc, out doc_ID);
            S1.NavigateToReIssuePage();
            bool reIssued = S1.ReIssuePage.ReissueTrxn(doc_ID, out new_Doc_ID);
            if (reIssued)
            {
                S1.NavigateToReIssuePage();
                try
                {
                    Assert.AreEqual("Voided", S1.ReIssuePage.GetStateOfTrxn(doc_ID));
                }
                catch (Exception e)
                {
                    S1.NavigateToReIssuePage();
                    Assert.AreEqual("Voided", S1.ReIssuePage.GetStateOfTrxn(doc_ID));
                }

                S1.NavigateToReIssuePage();
                try
                {
                    Assert.AreEqual("Issued", S1.ReIssuePage.GetStateOfTrxn(new_Doc_ID));
                }
                catch (Exception e)
                {
                    S1.NavigateToReIssuePage();
                    Assert.AreEqual("Issued", S1.ReIssuePage.GetStateOfTrxn(new_Doc_ID));
                }


            }
            else
            {
                Assert.Fail("Not able to re issue the transaction");
            }
        }


        [CsvTestCaseSource]
        public void TestBasicAmend(string username, string password,
        string identifier, string travellerID,
        string firstname, string lastname,
        string passport, string passportCountry,
        string email, string dateOfBirth, string gender,
        string address, string city, string zipCode, string residenceCountry,
        string receiptNum, string receiptDate, string goodsDescription,
        string quantity, string price,
        string purchaseAmount, string grossAmount,
        string amount1, string amount2, string amount3, string amount4, string amount5,
        string vat, string refundService, string refundMethod,
        string creditCardNum, string cardScheme, string expiryDate,
        string IBANNo, string BICNo,
        string bankName, string bankAddress, string bankAccountNo,
        string merchandiseDesc, string shopInvoiceNumber,
        string purchaseDate, string arrivalDate, string departureDate,
        string finalDestination, string tourGuideNumber,
        bool expectError)
        {
            if (creditCardNum != string.Empty)
                Regex.Replace(creditCardNum, @"\s+", "");

            IssuingTestCase tc = new IssuingTestCase
            {
                ExpectError = expectError,
                Login = new LoginDetails
                {
                    Username = username,
                    Password = password
                },
                Traveller = new TravellerDetails
                {
                    Autofill = new AutofillDetails
                    {
                        Identifier = identifier,
                        TravellerID = travellerID
                    },
                    FirstName = firstname,
                    LastName = lastname,
                    Passport = new PassportDetails
                    {
                        Number = passport,
                        Country = passportCountry
                    },
                    Address = new AddressDetails
                    {
                        Street = address,
                        City = city,
                        ZipCode = zipCode,
                        Country = residenceCountry
                    },
                    Email = email,
                    BirthDate = dateOfBirth,
                    Gender = gender,
                    Flight = new FlightDetails
                    {
                        ArrivalDate = arrivalDate,
                        DepartureDate = departureDate,
                        FinalDestination = finalDestination,
                        TourGuideNumber = tourGuideNumber
                    }
                },
                Purchase = new PurchaseDetails
                {
                    Items = new List<ItemDetails>(),
                    MerchandiseDescription = merchandiseDesc,
                    ShopInvoiceNumber = shopInvoiceNumber,
                    PurchaseDate = purchaseDate
                },
                Refund = new RefundDetails
                {
                    Service = refundService,
                    Method = refundMethod,
                    CreditCard = new CreditCardDetails
                    {
                        CreditCardNumber = creditCardNum,
                        CardScheme = cardScheme,
                        ExpiryDate = expiryDate
                    },
                    Bank = new BankDetails
                    {
                        IBAN = IBANNo,
                        BIC = BICNo,
                        Name = bankName,
                        Address = bankAddress,
                        AccountNumber = bankAccountNo,
                        AccountName = firstname + " " + lastname
                    }
                }
            };

            tc.Purchase.Items.Add(new ItemDetails
            {
                Receipt = new ReceiptDetails
                {
                    Number = receiptNum,
                    Date = receiptDate
                },
                Description = goodsDescription,
                Quantity = quantity,
                Price = price,
                PurchaseAmount = purchaseAmount,
                GrossAmount = grossAmount,
                Amount1 = amount1,
                Amount2 = amount2,
                Amount3 = amount3,
                Amount4 = amount4,
                Amount5 = amount5,
                Vat = vat,

            });

            TestBasicAmend("TestBasicAmend", tc);
        }

        [JsonTestCaseSource]
        public void TestBasicAmend(string tcName, IssuingTestCase tc)
        {
            string new_Doc_ID = null;
            RunIssuingTest(tc, out doc_ID);
            //IssuingTestCase amendedTC = (IssuingTestCase)tc.Clone();
            S1.NavigateToAmendPage();
            ModifyDetails.ModifyTravellerDetails(tc.Traveller);
            ModifyDetails.ModifyPurchaseDetails(tc.Purchase);
            S1.AmendPage.searchPage.SearchTxnByDocID(doc_ID);
            ((Label)S1.AmendPage.searchPage.GetAmendButton(0)).Click();
            if (string.IsNullOrEmpty(S1.Firstname)) { System.Threading.Thread.Sleep(1000); }
            SetValue(tc.Traveller.FirstName, S1, s1 => s1.Firstname);
            SetValue(tc.Traveller.LastName, S1, s1 => s1.Lastname);
            bool Amended = S1.AmendPage.AmendTxn(out new_Doc_ID);
            if (Amended)
            {
                S1.NavigateToAmendPage();
                try
                {
                    Assert.AreEqual("Voided", S1.AmendPage.GetStateOfTrxn(doc_ID));
                    Assert.Pass("Working till here");
                }
                catch (Exception e)
                {
                    S1.NavigateToAmendPage();
                    Assert.AreEqual("Voided", S1.AmendPage.GetStateOfTrxn(doc_ID));
                }

                S1.NavigateToAmendPage();
                try
                {
                    Assert.AreEqual("Issued", S1.AmendPage.GetStateOfTrxn(new_Doc_ID));
                }
                catch (Exception e)
                {
                    S1.NavigateToAmendPage();
                    Assert.AreEqual("Issued", S1.AmendPage.GetStateOfTrxn(new_Doc_ID));
                }

                S1.NavigateToAmendPage();
                S1.AmendPage.searchPage.SearchTxnByDocID(new_Doc_ID);//.GetStateOfTrxn(new_Doc_ID);
                ((Label)S1.AmendPage.searchPage.GetAmendButton(0)).Click();
                S1.AmendPage.WebDriver.WaitForElement(S1.AmendPage.ModifyButton, 5);
                if (string.IsNullOrEmpty(S1.Firstname)) { System.Threading.Thread.Sleep(1000); }
                if (!ModifyDetails.verifyModifieldsDetials(S1.AmendPage, tc))
                {
                    Assert.Fail("Modified values are not displayed after Amend operation "+tc.Traveller.FirstName+" = "+S1.AmendPage.issuingPage.Firstname.Value+" : "+ S1.AmendPage.issuingPage.Lastname.Value);
                }

            }
            else
            {
                Assert.Fail("Not able to Amend the transaction");
            }
        }

        [CsvTestCaseSource]
        public void BasicIssuingTest(string username, string password,
            string identifier, string travellerID,
            string firstname, string lastname,
            string passport, string passportCountry,
            string email, string dateOfBirth, string gender,
            string address, string city, string zipCode, string residenceCountry,
            string receiptNum, string receiptDate, string goodsDescription,
            string quantity, string price,
            string purchaseAmount, string grossAmount,
            string amount1, string amount2, string amount3, string amount4, string amount5,
            string vat, string refundService, string refundMethod,
            string creditCardNum, string cardScheme, string expiryDate,
            string IBANNo, string BICNo,
            string bankName, string bankAddress, string bankAccountNo,
            string merchandiseDesc, string shopInvoiceNumber,
            string purchaseDate, string arrivalDate, string departureDate,
            string finalDestination, string tourGuideNumber,
            bool expectError)
        {
            if (creditCardNum != string.Empty)
                Regex.Replace(creditCardNum, @"\s+", "");

            IssuingTestCase tc = new IssuingTestCase
            {
                ExpectError = expectError,
                Login = new LoginDetails
                {
                    Username = username,
                    Password = password
                },
                Traveller = new TravellerDetails
                {
                    Autofill = new AutofillDetails
                    {
                        Identifier = identifier,
                        TravellerID = travellerID
                    },
                    FirstName = firstname,
                    LastName = lastname,
                    Passport = new PassportDetails
                    {
                        Number = passport,
                        Country = passportCountry
                    },
                    Address = new AddressDetails
                    {
                        Street = address,
                        City = city,
                        ZipCode = zipCode,
                        Country = residenceCountry
                    },
                    Email = email,
                    BirthDate = dateOfBirth,
                    Gender = gender,
                    Flight = new FlightDetails
                    {
                        ArrivalDate = arrivalDate,
                        DepartureDate = departureDate,
                        FinalDestination = finalDestination,
                        TourGuideNumber = tourGuideNumber
                    }
                },
                Purchase = new PurchaseDetails
                {
                    Items = new List<ItemDetails>(),
                    MerchandiseDescription = merchandiseDesc,
                    ShopInvoiceNumber = shopInvoiceNumber,
                    PurchaseDate = purchaseDate
                },
                Refund = new RefundDetails
                {
                    Service = refundService,
                    Method = refundMethod,
                    CreditCard = new CreditCardDetails
                    {
                        CreditCardNumber = creditCardNum,
                        CardScheme = cardScheme,
                        ExpiryDate = expiryDate
                    },
                    Bank = new BankDetails
                    {
                        IBAN = IBANNo,
                        BIC = BICNo,
                        Name = bankName,
                        Address = bankAddress,
                        AccountNumber = bankAccountNo,
                        AccountName = firstname + " " + lastname
                    }
                }
            };

            tc.Purchase.Items.Add(new ItemDetails
            {
                Receipt = new ReceiptDetails
                {
                    Number = receiptNum,
                    Date = receiptDate
                },
                Description = goodsDescription,
                Quantity = quantity,
                Price = price,
                PurchaseAmount = purchaseAmount,
                GrossAmount = grossAmount,
                Amount1 = amount1,
                Amount2 = amount2,
                Amount3 = amount3,
                Amount4 = amount4,
                Amount5 = amount5,
                Vat = vat,

            });

            RunIssuingTest(tc, out doc_ID);
        }


        [JsonTestCaseSource]
        public void MultiItemIssuingTest(string tcName, IssuingTestCase tc)
        {

            RunIssuingTest(tc, out doc_ID);
        }


        private void RunIssuingTest(IssuingTestCase tc, out string doc_ID, [CallerMemberName] string callerName = "")
        {
            LoginDetails login = tc.Login;
            TravellerDetails traveller = tc.Traveller;
            PurchaseDetails purchase = tc.Purchase;
            RefundDetails refund = tc.Refund;

            bool loggedIn = S1.Login(login.Username, login.Password);
            Assert.IsTrue(loggedIn);
            WebDriver.WaitForElement(S1.IssuingPage.AutofillSearchButton, 10);
            //System.Threading.Thread.Sleep(3000);

            if (traveller.Autofill !=null && !string.IsNullOrEmpty(traveller.Autofill.Identifier))
            {
                if (S1.IssuingPage.AddTokenButton.Visible)
                {
                    S1.IssuingPage.AddTokenButton.Click();
                    WebDriver.SwitchTo().Frame("travellerIFrame");
                    SetValue(traveller.Autofill.Identifier, S1, s1 => s1.SGPIdentifier);
                    S1.IssuingPage.SG_IDSaveButton.Click();
                    WebDriver.SwitchTo().DefaultContent();
                }
                else
                {
                    SetValue(traveller.Autofill.Identifier, S1, s1 => s1.Identifier);
                }
                if (traveller.Autofill.Identifier == traveller.Passport.Number)
                {
                    WebDriver.TabAndReturnFocusedElement();
                    if (S1.IssuingPage.AutofillSearchButton.Visible) { S1.IssuingPage.AutofillSearchButton.Click(); }
                    if (!string.IsNullOrEmpty(traveller.Passport.Country))
                    {
                        SetValue(traveller.Passport.Country.ToUpper(), S1, s1 => s1.PassportAutofillCountry);
                    }
                }

                bool autofill = S1.SearchAutofillTraveller();
                System.Threading.Thread.Sleep(3000);
                if (!autofill)
                {
                    Bitmap screenshot = WebDriver.TakeScreenshot();
                    SaveBitmap(WebDriver.TakeScreenshot(), "BasicIssuingwithAutofillTest_" + login.Username);

                    throw new AssertionException("Basic Issuing with Autofill failed for user: " + login.Username);
                }
            }
            else
            {
                SetValue(traveller.FirstName, S1, s1 => s1.Firstname);
                SetValue(traveller.LastName, S1, s1 => s1.Lastname);

                if (traveller.Passport != null)
                {
                    SetValue(traveller.Passport.Number, S1, s1 => s1.Passport);
                    if (!string.IsNullOrEmpty(traveller.Passport.Country)) {
                        SetValue(traveller.Passport.Country.ToUpper(), S1, s1 => s1.PassportCountry);
                    }
                    SetValue(traveller.Passport.CountryKey, S1, s1 => s1.PassportCountryKey);
                }

                SetValue(traveller.BirthDate, S1, s1 => s1.DateOfBirth);
                SetValue(traveller.Email, S1, s1 => s1.Email);

                if (traveller.Address != null)
                {
                    SetValue(traveller.Address.Street, S1, s1 => s1.Address);
                    SetValue(traveller.Address.City, S1, s1 => s1.City);
                    SetValue(traveller.Address.ZipCode, S1, s1 => s1.PostalCode);
                    if (!string.IsNullOrEmpty(traveller.Address.Country))
                    {
                        SetValue(traveller.Address.Country.ToUpper(), S1, s1 => s1.ResidenceCountry);
                    }
                    SetValue(traveller.Address.CountryKey, S1, s1 => s1.ResidenceCountryKey);
                }
            }

            SetValue(refund.Service, S1, s1 => s1.RefundService);
            SetValue(refund.ServiceKey, S1, s1 => s1.RefundServiceKey);

            if (traveller.Autofill == null || string.IsNullOrEmpty(traveller.Autofill.Identifier))
            {
                SetValue(refund.Method, S1, s1 => s1.RefundMethod);
                string refundTarget = string.Empty;
                if (refund.CreditCard != null)
                {
                    SetValue(refund.CreditCard.CreditCardNumber, S1, s1 => s1.CreditCardNumber);
                    SetValue(refund.CreditCard.ExpiryDate, S1, s1 => s1.CreditCardExpiryDate);
                }
                else if (refund.Bank != null)
                {
                    SetValue(refund.Bank.Name, S1, s1 => s1.BankName);
                    SetValue(refund.Bank.Address, S1, s1 => s1.BankAddress);
                    SetValue(refund.Bank.AccountNumber, S1, s1 => s1.BankAccountNumber);
                    SetValue(refund.Bank.AccountName, S1, s1 => s1.BankAccountName);
                    SetValue(refund.Bank.IBAN, S1, s1 => s1.BankSwiftBic);
                    SetValue(refund.Bank.BIC, S1, s1 => s1.BankSwiftBic);
                }
            }

            if (traveller.Flight != null)
            {
                SetValue(traveller.Flight.ArrivalDate, S1, s1 => s1.ArrivalDate);
                SetValue(traveller.Flight.DepartureDate, S1, s1 => s1.DepartureDate);
                SetValue(traveller.Flight.FinalDestination, S1, s1 => s1.FinalDesitnation);
                SetValue(traveller.Flight.FinalDestinationKey, S1, s1 => s1.FinalDesitnationKey);
                SetValue(traveller.Flight.TourGuideNumber, S1, s1 => s1.TourGuideNumber);
            }

            for (int i = 0; i < purchase.Items.Count; i++)
            {
                if (i >= S1.IssuingPage.PurchaseDetails.NumRows)
                {
                    S1.IssuingPage.LoadMoreRowsButton.Click();
                }

                ItemDetails item = purchase.Items[i];
                PurchaseDetailsRow currentRow = S1.PurchaseDetails[i];

                if (item.Receipt != null)
                {
                    SetValue(item.Receipt.Number, currentRow, row => row.ReceiptNumber);
                    //GMX validate a receipt date with in 60 days older than current date
                    try
                    {
                        var receiptDate = DateTime.Today.Date.ToString("dd/MM/yyyy");
                        //if (item.Receipt.Date == "" || DateTime.Today.Date.Subtract(Convert.ToDateTime(Convert.ToDateTime(item.Receipt.Date)
                        //    .ToString("dd/MM/yyyy"))).Days > 60)
                        //{
                            item.Receipt.Date = receiptDate;
                        //}
                    }
                    catch (Exception) { }
                    SetValue(item.Receipt.Date, currentRow, row => row.ReceiptDate);
                }

                SetValue(item.Description, currentRow, row => row.GoodsDescription);
                SetValue(item.Quantity, currentRow, row => row.Quantity);
                SetValue(item.Price, currentRow, row => row.Price);
                SetValue(item.PurchaseAmount, currentRow, row => row.PurchaseAmount);
                SetValue(item.GrossAmount, currentRow, row => row.GrossAmount);
                SetValue(item.Amount1, currentRow, row => row.Amount1);
                SetValue(item.Amount2, currentRow, row => row.Amount2);
                SetValue(item.Amount3, currentRow, row => row.Amount3);
                SetValue(item.Amount4, currentRow, row => row.Amount4);
                SetValue(item.Amount5, currentRow, row => row.Amount5);
                SetValue(item.Vat, currentRow, row => row.VAT);
            }

            SetValue(purchase.PurchaseDate, S1, s1 => s1.PurchaseDate);
            SetValue(purchase.MerchandiseDescription, S1, s1 => s1.MerchandiseDescription);
            SetValue(purchase.ShopInvoiceNumber, S1, s1 => s1.ShopInvoiceNumber);

            var result = S1.IssueTff();
            string docid = result.DocID;
            doc_ID = docid;

            if (string.IsNullOrEmpty(docid) && !tc.ExpectError)
            {
                SaveBitmap(result.Bitmap, callerName + "_" + tc.Login.Username);
                throw new AssertionException("Issuing TFF failed with error: " + result.ToastMsg);
            }
            else if (string.IsNullOrEmpty(docid) && tc.ExpectError)
            {
                Assert.Pass("TFF was not issued (expected behavior), with error: " + result.ToastMsg);
            }
            else
            {
                //Assert.Pass("TFF was issued successfully. DocID: " + docid);
                S1.IssuingPage.CloseButton.Click();
            }

        }

        private void SetValue<T>(string input, T outObj, Expression<Func<T, string>> outExpr)
        {
            if (!string.IsNullOrEmpty(input))
            {
                var expr = (MemberExpression)outExpr.Body;
                var prop = (PropertyInfo)expr.Member;
                try { prop.SetValue(outObj, input, null); } catch (Exception) { }
                
            }
        }

        private void SaveBitmap(Bitmap bmp, string filePrefix)
        {
            string filepath = TestResultsDir + "/" + filePrefix + "_" + CurrentBrowser + DateTime.Now.ToString().Replace("/", "").Replace(":", "") + ".png";
            bmp.Save(filepath);
        }

        [CsvTestCaseSource]
        public void BasicIssuingTestwithAutofill(string username, string password,
            string identifier, string travellerID,
            string firstname, string lastname,
            string passport, string passportCountry,
            string email, string dateOfBirth, string gender,
            string address, string city, string zipCode, string residenceCountry,
            string receiptNum, string receiptDate, string goodsDescription,
            string quantity, string price,
            string purchaseAmount, string grossAmount,
            string amount1, string amount2, string amount3, string amount4, string amount5,
            string vat, string refundService, string refundMethod,
            string creditCardNum, string cardScheme, string expiryDate,
            string IBANNo, string BICNo,
            string bankName, string bankAddress, string bankAccountNo,
            string merchandiseDesc, string shopInvoiceNumber,
            string purchaseDate, string arrivalDate, string departureDate,
            string finalDestination, string tourGuideNumber,
            bool expectError)
        {
            BasicIssuingTest(username, password,
            identifier, travellerID,
            firstname, lastname,
            passport, passportCountry,
            email, dateOfBirth, gender,
            address, city, zipCode, residenceCountry,
            receiptNum, receiptDate, goodsDescription,
            quantity, price,
            purchaseAmount, grossAmount,
            amount1, amount2, amount3, amount4, amount5,
            vat, refundService, refundMethod,
            creditCardNum, cardScheme, expiryDate,
            IBANNo, BICNo,
            bankName, bankAddress, bankAccountNo,
            merchandiseDesc, shopInvoiceNumber,
            purchaseDate, arrivalDate, departureDate,
            finalDestination, tourGuideNumber,
            expectError);
        }
    }
}
