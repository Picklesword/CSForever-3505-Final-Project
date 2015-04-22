using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SS;
using SpreadsheetUtilities;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading;
using System.IO;

namespace SpreadsheetTests
{
    [TestClass]
    public class UnitTest1
    {
        /// <summary>
        /// Normalizes all the variables in the
        /// formula class
        /// </summary>
        /// <param name="st"></param>
        /// <returns></returns>
        public String normalizer(String st)
        {
            //String inputChanger;
            st = st.ToUpper();
            //inputChanger = "_" + st;

            return st;
        }

        /// <summary>
        /// Makes sure the variable follows the newly
        /// enforced rules on variables
        /// </summary>
        /// <param name="st"></param>
        /// <returns></returns>
        public bool isValid(String st)
        {
            //Regex check = new Regex(@"(^[A-Z]");
            Regex check = new Regex(@"(^[A-Z]+[0-9]+)");
            if (check.IsMatch(st))
                return true;
            else
                return false;
        }
        private TestContext testContextInstance;

        /// <summary>
        ///Gets or sets the test context which provides
        ///information about and functionality for the current test run.
        ///</summary>
        public TestContext TestContext
        {
            get
            {
                return testContextInstance;
            }
            set
            {
                testContextInstance = value;
            }
        }

        #region Additional test attributes
        // 
        //You can use the following additional attributes as you write your tests:
        //
        //Use ClassInitialize to run code before running the first test in the class
        //[ClassInitialize()]
        //public static void MyClassInitialize(TestContext testContext)
        //{
        //}
        //
        //Use ClassCleanup to run code after all tests in a class have run
        //[ClassCleanup()]
        //public static void MyClassCleanup()
        //{
        //}
        //
        //Use TestInitialize to run code before running each test
        //[TestInitialize()]
        //public void MyTestInitialize()
        //{
        //}
        //
        //Use TestCleanup to run code after each test has run
        //[TestCleanup()]
        //public void MyTestCleanup()
        //{
        //}
        //
        #endregion

        // Verifies cells and their values, which must alternate.
        public void VV(AbstractSpreadsheet sheet, params object[] constraints)
        {
            for (int i = 0; i < constraints.Length; i += 2)
            {
                if (constraints[i + 1] is double)
                {
                    Assert.AreEqual((double)constraints[i + 1], (double)sheet.GetCellValue((string)constraints[i]), 1e-9);
                }
                else
                {
                    Assert.AreEqual(constraints[i + 1], sheet.GetCellValue((string)constraints[i]));
                }
            }
        }


        // For setting a spreadsheet cell.
        public IEnumerable<string> Set(AbstractSpreadsheet sheet, string name, string contents)
        {
            List<string> result = new List<string>(sheet.SetContentsOfCell(name, contents));
            return result;
        }

        // Tests IsValid
        [TestMethod()]
        public void IsValidTest1()
        {
            AbstractSpreadsheet s = new Spreadsheet();
            s.SetContentsOfCell("A1", "x");
        }

        [TestMethod()]
        [ExpectedException(typeof(InvalidNameException))]
        public void IsValidTest2()
        {
            AbstractSpreadsheet ss = new Spreadsheet(s => s[0] != 'A', s => s, "");
            ss.SetContentsOfCell("A1", "x");
        }

        [TestMethod()]
        public void IsValidTest3()
        {
            AbstractSpreadsheet s = new Spreadsheet();
            s.SetContentsOfCell("B1", "= A1 + C1");
        }

        [TestMethod()]
        [ExpectedException(typeof(FormulaFormatException))]
        public void IsValidTest4()
        {
            AbstractSpreadsheet ss = new Spreadsheet(s => s[0] != 'A', s => s, "");
            ss.SetContentsOfCell("B1", "= A1 + C1");
        }

        // Tests Normalize
        [TestMethod()]
        public void NormalizeTest1()
        {
            AbstractSpreadsheet s = new Spreadsheet();
            s.SetContentsOfCell("B1", "hello");
            Assert.AreEqual("", s.GetCellContents("b1"));
        }

        [TestMethod()]
        public void NormalizeTest2()
        {
            AbstractSpreadsheet ss = new Spreadsheet(s => true, s => s.ToUpper(), "");
            ss.SetContentsOfCell("B1", "hello");
            Assert.AreEqual("hello", ss.GetCellContents("b1"));
        }

        [TestMethod()]
        public void NormalizeTest3()
        {
            AbstractSpreadsheet s = new Spreadsheet();
            s.SetContentsOfCell("a1", "5");
            s.SetContentsOfCell("A1", "6");
            s.SetContentsOfCell("B1", "= a1");
            Assert.AreEqual(5.0, (double)s.GetCellValue("B1"), 1e-9);
        }

        [TestMethod()]
        public void NormalizeTest4()
        {
            AbstractSpreadsheet ss = new Spreadsheet(s => true, s => s.ToUpper(), "");
            ss.SetContentsOfCell("a1", "5");
            ss.SetContentsOfCell("A1", "6");
            ss.SetContentsOfCell("B1", "= a1");
            Assert.AreEqual(6.0, (double)ss.GetCellValue("B1"), 1e-9);
        }

        // Simple tests
        [TestMethod()]
        public void EmptySheet()
        {
            AbstractSpreadsheet ss = new Spreadsheet();
            VV(ss, "A1", "");
        }


        [TestMethod()]
        public void OneString()
        {
            AbstractSpreadsheet ss = new Spreadsheet();
            OneString(ss);
        }

        public void OneString(AbstractSpreadsheet ss)
        {
            Set(ss, "B1", "hello");
            VV(ss, "B1", "hello");
        }


        [TestMethod()]
        public void OneNumber()
        {
            AbstractSpreadsheet ss = new Spreadsheet();
            OneNumber(ss);
        }

        public void OneNumber(AbstractSpreadsheet ss)
        {
            Set(ss, "C1", "17.5");
            VV(ss, "C1", 17.5);
        }


        [TestMethod()]
        public void OneFormula()
        {
            AbstractSpreadsheet ss = new Spreadsheet();
            OneFormula(ss);
        }

        public void OneFormula(AbstractSpreadsheet ss)
        {
            Set(ss, "A1", "4.1");
            Set(ss, "B1", "5.2");
            Set(ss, "C1", "= A1+B1");
            VV(ss, "A1", 4.1, "B1", 5.2, "C1", 9.3);
        }


        [TestMethod()]
        public void Changed()
        {
            AbstractSpreadsheet ss = new Spreadsheet();
            Assert.IsFalse(ss.Changed);
            Set(ss, "C1", "17.5");
            Assert.IsTrue(ss.Changed);
        }


        [TestMethod()]
        public void DivisionByZero1()
        {
            AbstractSpreadsheet ss = new Spreadsheet();
            DivisionByZero1(ss);
        }

        public void DivisionByZero1(AbstractSpreadsheet ss)
        {
            Set(ss, "A1", "4.1");
            Set(ss, "B1", "0.0");
            Set(ss, "C1", "= A1 / B1");
            Assert.IsInstanceOfType(ss.GetCellValue("C1"), typeof(FormulaError));
        }

        [TestMethod()]
        public void DivisionByZero2()
        {
            AbstractSpreadsheet ss = new Spreadsheet();
            DivisionByZero2(ss);
        }

        public void DivisionByZero2(AbstractSpreadsheet ss)
        {
            Set(ss, "A1", "5.0");
            Set(ss, "A3", "= A1 / 0.0");
            Assert.IsInstanceOfType(ss.GetCellValue("A3"), typeof(FormulaError));
        }



        [TestMethod()]
        public void EmptyArgument()
        {
            AbstractSpreadsheet ss = new Spreadsheet();
            EmptyArgument(ss);
        }

        public void EmptyArgument(AbstractSpreadsheet ss)
        {
            Set(ss, "A1", "4.1");
            Set(ss, "C1", "= A1 + B1");
            Assert.IsInstanceOfType(ss.GetCellValue("C1"), typeof(FormulaError));
        }


        [TestMethod()]
        public void StringArgument()
        {
            AbstractSpreadsheet ss = new Spreadsheet();
            StringArgument(ss);
        }

        public void StringArgument(AbstractSpreadsheet ss)
        {
            Set(ss, "A1", "4.1");
            Set(ss, "B1", "hello");
            Set(ss, "C1", "= A1 + B1");
            Assert.IsInstanceOfType(ss.GetCellValue("C1"), typeof(FormulaError));
        }


        [TestMethod()]
        public void ErrorArgument()
        {
            AbstractSpreadsheet ss = new Spreadsheet();
            ErrorArgument(ss);
        }

        public void ErrorArgument(AbstractSpreadsheet ss)
        {
            Set(ss, "A1", "4.1");
            Set(ss, "B1", "");
            Set(ss, "C1", "= A1 + B1");
            Set(ss, "D1", "= C1");
            Assert.IsInstanceOfType(ss.GetCellValue("D1"), typeof(FormulaError));
        }


        [TestMethod()]
        public void NumberFormula1()
        {
            AbstractSpreadsheet ss = new Spreadsheet();
            NumberFormula1(ss);
        }

        public void NumberFormula1(AbstractSpreadsheet ss)
        {
            Set(ss, "A1", "4.1");
            Set(ss, "C1", "= A1 + 4.2");
            VV(ss, "C1", 8.3);
        }


        [TestMethod()]
        public void NumberFormula2()
        {
            AbstractSpreadsheet ss = new Spreadsheet();
            NumberFormula2(ss);
        }

        public void NumberFormula2(AbstractSpreadsheet ss)
        {
            Set(ss, "A1", "= 4.6");
            VV(ss, "A1", 4.6);
        }


        // Repeats the simple tests all together
        [TestMethod()]
        public void RepeatSimpleTests()
        {
            AbstractSpreadsheet ss = new Spreadsheet();
            Set(ss, "A1", "17.32");
            Set(ss, "B1", "This is a test");
            Set(ss, "C1", "= A1+B1");
            OneString(ss);
            OneNumber(ss);
            OneFormula(ss);
            DivisionByZero1(ss);
            DivisionByZero2(ss);
            StringArgument(ss);
            ErrorArgument(ss);
            NumberFormula1(ss);
            NumberFormula2(ss);
        }

        // Four kinds of formulas
        [TestMethod()]
        public void Formulas()
        {
            AbstractSpreadsheet ss = new Spreadsheet();
            Formulas(ss);
        }

        public void Formulas(AbstractSpreadsheet ss)
        {
            Set(ss, "A1", "4.4");
            Set(ss, "B1", "2.2");
            Set(ss, "C1", "= A1 + B1");
            Set(ss, "D1", "= A1 - B1");
            Set(ss, "E1", "= A1 * B1");
            Set(ss, "F1", "= A1 / B1");
            VV(ss, "C1", 6.6, "D1", 2.2, "E1", 4.4 * 2.2, "F1", 2.0);
        }

        [TestMethod()]
        public void Formulasa()
        {
            Formulas();
        }

        [TestMethod()]
        public void Formulasb()
        {
            Formulas();
        }


        // Are multiple spreadsheets supported?
        [TestMethod()]
        public void Multiple()
        {
            AbstractSpreadsheet s1 = new Spreadsheet();
            AbstractSpreadsheet s2 = new Spreadsheet();
            Set(s1, "X1", "hello");
            Set(s2, "X1", "goodbye");
            VV(s1, "X1", "hello");
            VV(s2, "X1", "goodbye");
        }

        [TestMethod()]
        public void Multiplea()
        {
            Multiple();
        }

        [TestMethod()]
        public void Multipleb()
        {
            Multiple();
        }

        [TestMethod()]
        public void Multiplec()
        {
            Multiple();
        }

        // Reading/writing spreadsheets
        [TestMethod()]
        [ExpectedException(typeof(SpreadsheetReadWriteException))]
        public void SaveTest1()
        {
            AbstractSpreadsheet ss = new Spreadsheet();
            ss.Save("q:\\missing\\save.txt");
        }

        [TestMethod()]
        [ExpectedException(typeof(SpreadsheetReadWriteException))]
        public void SaveTest2()
        {
            AbstractSpreadsheet ss = new Spreadsheet("q:\\missing\\save.txt", s => true, s => s, "");
        }

        [TestMethod()]
        public void SaveTest3()
        {
            AbstractSpreadsheet s1 = new Spreadsheet();
            Set(s1, "A1", "hello");
            s1.Save("save1.txt");
            s1 = new Spreadsheet("save1.txt", s => true, s => s, "default");
            Assert.AreEqual("hello", s1.GetCellContents("A1"));
        }

        [TestMethod()]
        [ExpectedException(typeof(SpreadsheetReadWriteException))]
        public void SaveTest4()
        {
            using (StreamWriter writer = new StreamWriter("save2.txt"))
            {
                writer.WriteLine("This");
                writer.WriteLine("is");
                writer.WriteLine("a");
                writer.WriteLine("test!");
            }
            AbstractSpreadsheet ss = new Spreadsheet("save2.txt", s => true, s => s, "");
        }

        [TestMethod()]
        [ExpectedException(typeof(SpreadsheetReadWriteException))]
        public void SaveTest5()
        {
            AbstractSpreadsheet ss = new Spreadsheet();
            ss.Save("save3.txt");
            ss = new Spreadsheet("save3.txt", s => true, s => s, "version");
        }

        [TestMethod()]
        public void SaveTest6()
        {
            AbstractSpreadsheet ss = new Spreadsheet(s => true, s => s, "hello");
            ss.Save("save4.txt");
            Assert.AreEqual("hello", new Spreadsheet().GetSavedVersion("save4.txt"));
        }

        //[TestMethod()]
        //public void SaveTest7()
        //{
        //    using (XmlWriter writer = XmlWriter.Create("save5.txt"))
        //    {
        //        writer.WriteStartDocument();
        //        writer.WriteStartElement("spreadsheet");
        //        writer.WriteAttributeString("version", "");

        //        writer.WriteStartElement("cell");
        //        writer.WriteElementString("name", "A1");
        //        writer.WriteElementString("contents", "hello");
        //        writer.WriteEndElement();

        //        writer.WriteStartElement("cell");
        //        writer.WriteElementString("name", "A2");
        //        writer.WriteElementString("contents", "5.0");
        //        writer.WriteEndElement();

        //        writer.WriteStartElement("cell");
        //        writer.WriteElementString("name", "A3");
        //        writer.WriteElementString("contents", "4.0");
        //        writer.WriteEndElement();

        //        writer.WriteStartElement("cell");
        //        writer.WriteElementString("name", "A4");
        //        writer.WriteElementString("contents", "= A2 + A3");
        //        writer.WriteEndElement();

        //        writer.WriteEndElement();
        //        writer.WriteEndDocument();
        //    }
        //    AbstractSpreadsheet ss = new Spreadsheet("save5.txt", s => true, s => s, "");
        //    VV(ss, "A1", "hello", "A2", 5.0, "A3", 4.0, "A4", 9.0);
        //}

        //[TestMethod()]
        //public void SaveTest8()
        //{
        //    AbstractSpreadsheet ss = new Spreadsheet();
        //    Set(ss, "A1", "hello");
        //    Set(ss, "A2", "5.0");
        //    Set(ss, "A3", "4.0");
        //    Set(ss, "A4", "= A2 + A3");
        //    ss.Save("save6.txt");
        //    using (XmlReader reader = XmlReader.Create("save6.txt"))
        //    {
        //        int spreadsheetCount = 0;
        //        int cellCount = 0;
        //        bool A1 = false;
        //        bool A2 = false;
        //        bool A3 = false;
        //        bool A4 = false;
        //        string name = null;
        //        string contents = null;

        //        while (reader.Read())
        //        {
        //            if (reader.IsStartElement())
        //            {
        //                switch (reader.Name)
        //                {
        //                    case "spreadsheet":
        //                        Assert.AreEqual("default", reader["version"]);
        //                        spreadsheetCount++;
        //                        break;

        //                    case "cell":
        //                        cellCount++;
        //                        break;

        //                    case "name":
        //                        reader.Read();
        //                        name = reader.Value;
        //                        break;

        //                    case "contents":
        //                        reader.Read();
        //                        contents = reader.Value;
        //                        break;
        //                }
        //            }
        //            else
        //            {
        //                switch (reader.Name)
        //                {
        //                    case "cell":
        //                        if (name.Equals("A1")) { Assert.AreEqual("hello", contents); A1 = true; }
        //                        else if (name.Equals("A2")) { Assert.AreEqual(5.0, Double.Parse(contents), 1e-9); A2 = true; }
        //                        else if (name.Equals("A3")) { Assert.AreEqual(4.0, Double.Parse(contents), 1e-9); A3 = true; }
        //                        else if (name.Equals("A4")) { contents = contents.Replace(" ", ""); Assert.AreEqual("=A2+A3", contents); A4 = true; }
        //                        else Assert.Fail();
        //                        break;
        //                }
        //            }
        //        }
        //        Assert.AreEqual(1, spreadsheetCount);
        //        Assert.AreEqual(4, cellCount);
        //        Assert.IsTrue(A1);
        //        Assert.IsTrue(A2);
        //        Assert.IsTrue(A3);
        //        Assert.IsTrue(A4);
        //    }
        //}


        // Fun with formulas
        [TestMethod()]
        public void Formula1()
        {
            Formula1(new Spreadsheet());
        }
        public void Formula1(AbstractSpreadsheet ss)
        {
            Set(ss, "a1", "= a2 + a3");
            Set(ss, "a2", "= b1 + b2");
            Assert.IsInstanceOfType(ss.GetCellValue("a1"), typeof(FormulaError));
            Assert.IsInstanceOfType(ss.GetCellValue("a2"), typeof(FormulaError));
            Set(ss, "a3", "5.0");
            Set(ss, "b1", "2.0");
            Set(ss, "b2", "3.0");
            VV(ss, "a1", 10.0, "a2", 5.0);
            Set(ss, "b2", "4.0");
            VV(ss, "a1", 11.0, "a2", 6.0);
        }

        [TestMethod()]
        public void Formula2()
        {
            Formula2(new Spreadsheet());
        }
        public void Formula2(AbstractSpreadsheet ss)
        {
            Set(ss, "a1", "= a2 + a3");
            Set(ss, "a2", "= a3");
            Set(ss, "a3", "6.0");
            VV(ss, "a1", 12.0, "a2", 6.0, "a3", 6.0);
            Set(ss, "a3", "5.0");
            VV(ss, "a1", 10.0, "a2", 5.0, "a3", 5.0);
        }

        [TestMethod()]
        public void Formula3()
        {
            Formula3(new Spreadsheet());
        }
        public void Formula3(AbstractSpreadsheet ss)
        {
            Set(ss, "a1", "= a3 + a5");
            Set(ss, "a2", "= a5 + a4");
            Set(ss, "a3", "= a5");
            Set(ss, "a4", "= a5");
            Set(ss, "a5", "9.0");
            VV(ss, "a1", 18.0);
            VV(ss, "a2", 18.0);
            Set(ss, "a5", "8.0");
            VV(ss, "a1", 16.0);
            VV(ss, "a2", 16.0);
        }

        [TestMethod()]
        public void Formula4()
        {
            AbstractSpreadsheet ss = new Spreadsheet();
            Formula1(ss);
            Formula2(ss);
            Formula3(ss);
        }

        [TestMethod()]
        public void Formula4a()
        {
            Formula4();
        }


        [TestMethod()]
        public void MediumSheet()
        {
            AbstractSpreadsheet ss = new Spreadsheet();
            MediumSheet(ss);
        }

        public void MediumSheet(AbstractSpreadsheet ss)
        {
            Set(ss, "A1", "1.0");
            Set(ss, "A2", "2.0");
            Set(ss, "A3", "3.0");
            Set(ss, "A4", "4.0");
            Set(ss, "B1", "= A1 + A2");
            Set(ss, "B2", "= A3 * A4");
            Set(ss, "C1", "= B1 + B2");
            VV(ss, "A1", 1.0, "A2", 2.0, "A3", 3.0, "A4", 4.0, "B1", 3.0, "B2", 12.0, "C1", 15.0);
            Set(ss, "A1", "2.0");
            VV(ss, "A1", 2.0, "A2", 2.0, "A3", 3.0, "A4", 4.0, "B1", 4.0, "B2", 12.0, "C1", 16.0);
            Set(ss, "B1", "= A1 / A2");
            VV(ss, "A1", 2.0, "A2", 2.0, "A3", 3.0, "A4", 4.0, "B1", 1.0, "B2", 12.0, "C1", 13.0);
        }

        [TestMethod()]
        public void MediumSheeta()
        {
            MediumSheet();
        }


        [TestMethod()]
        public void MediumSave()
        {
            AbstractSpreadsheet ss = new Spreadsheet();
            MediumSheet(ss);
            ss.Save("save7.txt");
            ss = new Spreadsheet("save7.txt", s => true, s => s, "default");
            VV(ss, "A1", 2.0, "A2", 2.0, "A3", 3.0, "A4", 4.0, "B1", 1.0, "B2", 12.0, "C1", 13.0);
        }

        [TestMethod()]
        public void MediumSavea()
        {
            MediumSave();
        }


        // A long chained formula.  If this doesn't finish within 60 seconds, it fails.
        [TestMethod()]
        public void LongFormulaTest()
        {
            object result = "";
            Thread t = new Thread(() => LongFormulaHelper(out result));
            t.Start();
            t.Join();
            if (t.IsAlive)
            {
                //t.Abort();
               // Assert.Fail("Computation took longer than 60 seconds");
            }
            Assert.AreEqual("ok", result);
        }

        public void LongFormulaHelper(out object result)
        {
            try
            {
                AbstractSpreadsheet s = new Spreadsheet();
                s.SetContentsOfCell("sum1", "= a1 + a2");
                int i;
                int depth = 100;
                for (i = 1; i <= depth * 2; i += 2)
                {
                    s.SetContentsOfCell("a" + i, "= a" + (i + 2) + " + a" + (i + 3));
                    s.SetContentsOfCell("a" + (i + 1), "= a" + (i + 2) + "+ a" + (i + 3));
                }
                s.SetContentsOfCell("a" + i, "1");
                s.SetContentsOfCell("a" + (i + 1), "1");
                Assert.AreEqual(Math.Pow(2, depth + 1), (double)s.GetCellValue("sum1"), 1.0);
                s.SetContentsOfCell("a" + i, "0");
                Assert.AreEqual(Math.Pow(2, depth), (double)s.GetCellValue("sum1"), 1.0);
                s.SetContentsOfCell("a" + (i + 1), "0");
                Assert.AreEqual(0.0, (double)s.GetCellValue("sum1"), 0.1);
                result = "ok";
            }
            catch (Exception e)
            {
                result = e;
            }
        }

        /// <summary>
        /// Tests value of cell
        /// </summary>
        [TestMethod]
        public void Test1EvaluateCellWithMultipleFormulas()
        {
            AbstractSpreadsheet sheet = new Spreadsheet();
            sheet.SetContentsOfCell("A1", "5");
            sheet.SetContentsOfCell("B1", "10");
            sheet.SetContentsOfCell("C1", "=A1 + B1");

            Assert.AreEqual(15.0, sheet.GetCellValue("C1"));
        }

        /// <summary>
        /// Tests value of cell
        /// </summary>
        [TestMethod]
        public void Test2EvaluateCellWithMultipleFormulas()
        {
            AbstractSpreadsheet sheet = new Spreadsheet();
            sheet.SetContentsOfCell("A1", "=B1 + C1");
            sheet.SetContentsOfCell("B1", "=D1 + C1");
            sheet.SetContentsOfCell("C1", "5");
            sheet.SetContentsOfCell("D1", "10");

            Assert.AreEqual(20.0, sheet.GetCellValue("A1"));
        }

        /// <summary>
        /// Tests value of cell
        /// </summary>
        [TestMethod]
        public void Test3EvaluateCellWithMultipleFormulas()
        {
            AbstractSpreadsheet sheet = new Spreadsheet();
            sheet.SetContentsOfCell("A1", "=(B1 + C1) * 10");
            sheet.SetContentsOfCell("B1", "=C1 + D1 + 5");
            sheet.SetContentsOfCell("C1", "=D1 + 10");
            sheet.SetContentsOfCell("D1", "25");

            Assert.AreEqual(1000.0, sheet.GetCellValue("A1"));
        }

        /// <summary>
        /// Tests value of cell
        /// </summary>
        [TestMethod]
        public void Test4EvaluateCellWithMultipleFormulas()
        {
            AbstractSpreadsheet sheet = new Spreadsheet();
            sheet.SetContentsOfCell("A1", "=B1 + C1 + 10");
            sheet.SetContentsOfCell("B1", "=C1 + D1 + 5");
            sheet.SetContentsOfCell("C1", "=D1 + 10");
            sheet.SetContentsOfCell("D1", "25");

            Assert.AreEqual(110.0, sheet.GetCellValue("A1"));

            sheet.SetContentsOfCell("D1", "20");

            Assert.AreEqual(95.0, sheet.GetCellValue("A1"));
            sheet.SetContentsOfCell("D1", "String");
            Assert.AreEqual(new FormulaError("Error: cell value not valid or empty"), sheet.GetCellValue("A1"));
            Assert.AreEqual(new FormulaError("Error: cell value not valid or empty"), sheet.GetCellValue("B1"));
            Assert.AreEqual(new FormulaError("Error: cell value not valid or empty"), sheet.GetCellValue("C1"));

        }

        /// <summary>
        /// Tests value of cell
        /// </summary>
        [TestMethod]
        public void Test5EvaluateCellWithString()
        {
            AbstractSpreadsheet sheet = new Spreadsheet();
            sheet.SetContentsOfCell("A1", "String");

            Assert.AreEqual("String", sheet.GetCellValue("A1"));
        }

        /// <summary>
        /// Tests value of cell
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(InvalidNameException))]
        public void Test5GetInvalidName()
        {
            AbstractSpreadsheet sheet = new Spreadsheet();
            sheet.SetContentsOfCell("A1", "String");

            sheet.GetCellValue("1B");
        }

        /// <summary>
        /// Tests value of cell
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void Test5GetInvalidContent()
        {
            AbstractSpreadsheet sheet = new Spreadsheet();
            sheet.SetContentsOfCell("A1", null);


        }

        /// <summary>
        /// Tests value of cell
        /// </summary>
        [TestMethod]

        public void Test5GetValidNameEmptyValue()
        {
            AbstractSpreadsheet sheet = new Spreadsheet();
            sheet.SetContentsOfCell("A1", "String");

            Assert.AreEqual("", sheet.GetCellValue("B1"));
        }

        /// <summary>
        /// Tests value of cell
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(InvalidNameException))]
        public void Test5GetNullName()
        {
            AbstractSpreadsheet sheet = new Spreadsheet();
            sheet.SetContentsOfCell("A1", "String");

            sheet.GetCellValue(null);
        }
        /// <summary>
        /// Tests value of cell
        /// </summary>
        [TestMethod]
        public void Test6EvaluateCellWithDoubleThenString()
        {
            AbstractSpreadsheet sheet = new Spreadsheet();
            sheet.SetContentsOfCell("A1", "15");
            sheet.SetContentsOfCell("A1", "String");

            Assert.AreEqual("String", sheet.GetCellValue("A1"));
        }

        /// <summary>
        /// Tests value of cell
        /// </summary>
        [TestMethod]
        public void TestFormulaEqualString()
        {
            AbstractSpreadsheet sheet = new Spreadsheet();
            sheet.SetContentsOfCell("A1", "=B1");
            sheet.SetContentsOfCell("B1", "String");

            Assert.AreEqual(new FormulaError("Error: cell value not valid or empty"), sheet.GetCellValue("A1"));
            Assert.AreEqual("String", sheet.GetCellValue("B1"));

        }
        /// <summary>
        /// Tests value of cell
        /// </summary>
        [TestMethod]
        public void Test7EvaluateCellWithDelegateConst()
        {
            AbstractSpreadsheet sheet = new Spreadsheet(isValid, normalizer, "V2.1");

            sheet.SetContentsOfCell("a1", "String");

            Assert.AreEqual("A1", sheet.GetNamesOfAllNonemptyCells().ElementAt(0));
            Assert.AreEqual("String", sheet.GetCellValue("A1"));
        }

        /// <summary>
        /// Tests to see if name is null fails, should
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(InvalidNameException))]
        public void TestGetCellContentsNullName()
        {
            AbstractSpreadsheet sheet = new Spreadsheet();

            Assert.IsTrue(sheet.GetCellContents(null).GetType().Equals((typeof(Formula))));
        }


        /// <summary>
        /// Tests to see if name is null fails, should
        /// </summary>
        [TestMethod]

        public void TestGetCellContents()
        {
            AbstractSpreadsheet sheet = new Spreadsheet();
            sheet.SetContentsOfCell("A1", "string");
            sheet.SetContentsOfCell("B1", "=C1");
            sheet.SetContentsOfCell("C1", "5.5");
            Assert.AreEqual("string", sheet.GetCellContents("A1"));
            Assert.AreEqual("=C1", sheet.GetCellContents("B1"));
            Assert.AreEqual(5.5, sheet.GetCellContents("C1"));
            Assert.AreEqual("", sheet.GetCellContents("D1"));
        }

        /// <summary>
        /// Tests to see if formula tests for circular dep, should fail
        /// creates the cell first as a double then trys to change it to
        /// a formula that should throw a circular exception
        /// 
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(CircularException))]
        public void TestForCircularDepCreatFormulaThenConvertToNewFormulaCell()
        {
            AbstractSpreadsheet sheet = new Spreadsheet();
            sheet.SetContentsOfCell("A1", ("=Z1 + X1"));

            sheet.SetContentsOfCell("B1", ("=A1 + C1"));
            sheet.SetContentsOfCell("C1", ("=A1"));
            sheet.SetContentsOfCell("A1", ("=B1"));

        }

        /// <summary>
        /// Tests to see if name is invalid fails, should
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(InvalidNameException))]
        public void TestGetCellInvalidCellName()
        {
            AbstractSpreadsheet sheet = new Spreadsheet();

            Assert.IsTrue(sheet.GetCellContents("1B").GetType().Equals((typeof(Formula))));
        }

        /// <summary>
        /// Tests to see if formula tests for circular dep, should fail
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(CircularException))]
        public void TestForCircularDepFormulaCell()
        {
            AbstractSpreadsheet sheet = new Spreadsheet();
            sheet.SetContentsOfCell("B1", "=A1 + C1");
            sheet.SetContentsOfCell("C1", "=A1");
            sheet.SetContentsOfCell("A1", "=B1");
        }

        /// <summary>
        /// Tests to see if formula tests for circular dep, should fail
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(CircularException))]
        public void TestForCircularDepFormulaCell2()
        {
            AbstractSpreadsheet sheet = new Spreadsheet();
            sheet.SetContentsOfCell("B1", "=A1 + C1");
            sheet.SetContentsOfCell("C1", "=A1");
            sheet.SetContentsOfCell("A1", "=D1");
            sheet.SetContentsOfCell("A1", "=B1");
        }

        /// <summary>
        /// Tests to see if formula tests for circular dep, should fail
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(CircularException))]
        public void TestForCircularDepFormulaCell3()
        {
            AbstractSpreadsheet sheet = new Spreadsheet();
            sheet.SetContentsOfCell("B1", "=A1 + C1");
            sheet.SetContentsOfCell("C1", "=A1");
            sheet.SetContentsOfCell("A1", "random text");
            sheet.SetContentsOfCell("A1", "=B1");

        }

        /// <summary>
        /// Tests to see if formula tests for circular dep, should fail
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(CircularException))]
        public void TestForCircularDepFormulaCell4()
        {
            AbstractSpreadsheet sheet = new Spreadsheet();
            sheet.SetContentsOfCell("B1", "=A1 + C1");
            sheet.SetContentsOfCell("C1", "=A1");
            sheet.SetContentsOfCell("A1", "5.0");
            sheet.SetContentsOfCell("A1", "=B1");

        }

        /// <summary>
        /// Tests to see if invalid name is caught in text cell
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(InvalidNameException))]
        public void TestForInvalidNameForTextCell()
        {
            AbstractSpreadsheet sheet = new Spreadsheet();
            sheet.SetContentsOfCell("1B", "Invalid name?");
        }


        /// <summary>
        /// Tests value of cell
        /// </summary>
        [TestMethod]
        public void Test8TestSaveMethod()
        {
            AbstractSpreadsheet sheet = new Spreadsheet();

            sheet.SetContentsOfCell("A1", ("=B1 + C1"));
            sheet.SetContentsOfCell("B1", "10");
            sheet.SetContentsOfCell("C1", "15");

            sheet.Save("firstSave");

            sheet.SetContentsOfCell("D1", "52");

            sheet.Save("firstSave");
        }

        /// <summary>
        /// Tests create spreedsheat 1 add data, save data. Creat spreadsheet 2, load
        /// data from saved file and compare that it was done
        /// </summary>
        [TestMethod]
        public void Test9TestSaveThenLoadWithPathToFileConst()
        {
            String fileName = "firstSave";

            AbstractSpreadsheet sheet1 = new Spreadsheet();

            sheet1.SetContentsOfCell("A1", ("=B1 + C1"));
            sheet1.SetContentsOfCell("B1", "50");
            sheet1.SetContentsOfCell("C1", "15");

            List<String> cellsOfSheet1 = sheet1.GetNamesOfAllNonemptyCells().ToList();

            sheet1.Save(fileName);

            AbstractSpreadsheet sheet2 = new Spreadsheet(fileName, s => true, s => s, "default");

            List<String> cellsOfSheet2 = sheet2.GetNamesOfAllNonemptyCells().ToList();

            for (int i = 0; i < cellsOfSheet1.Count; i++)
                Assert.AreEqual(cellsOfSheet1.ElementAt(i), cellsOfSheet2.ElementAt(i));
        }

        /// <summary>
        /// Tests to see if saved version is same as version passed in const, should fail
        /// </summary>
        [TestMethod]
        public void TestTestValidVersionFromSavedVersionToVersonPassedIn()
        {
            String fileName = "firstSave";

            AbstractSpreadsheet sheet1 = new Spreadsheet(s => true, s => s, "Version 1.0");

            sheet1.SetContentsOfCell("A1", ("=B1 + C1"));
            sheet1.SetContentsOfCell("B1", "20");
            sheet1.SetContentsOfCell("C1", "15");

            List<String> cellsOfSheet1 = sheet1.GetNamesOfAllNonemptyCells().ToList();

            sheet1.Save(fileName);

            AbstractSpreadsheet sheet2 = new Spreadsheet(fileName, s => true, s => s, "Version 1.0");
            Assert.AreEqual("Version 1.0", sheet2.GetSavedVersion(fileName));
            List<String> cellsOfSheet2 = sheet2.GetNamesOfAllNonemptyCells().ToList();
        }

        /// <summary>
        /// Tests to see if saved version is same as version passed in const, should fail
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(SpreadsheetReadWriteException))]
        public void Test10TestInvalidVersionFromSavedVersionToVersonPassedIn()
        {
            String fileName = "firstSave";

            AbstractSpreadsheet sheet1 = new Spreadsheet(s => true, s => s, "Version 1.0");

            sheet1.SetContentsOfCell("A1", ("=B1 + C1"));
            sheet1.SetContentsOfCell("B1", "20");
            sheet1.SetContentsOfCell("C1", "15");

            List<String> cellsOfSheet1 = sheet1.GetNamesOfAllNonemptyCells().ToList();

            sheet1.Save(fileName);

            AbstractSpreadsheet sheet2 = new Spreadsheet(fileName, s => true, s => s, "Version 1.1");

            List<String> cellsOfSheet2 = sheet2.GetNamesOfAllNonemptyCells().ToList();
        }

        /// <summary>
        /// Tests to see if saved version is same as version passed in const, should fail
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(SpreadsheetReadWriteException))]
        public void Test13TestInvalidVersionFromSavedVersionToVersonPassedIn()
        {
            String fileName = "firstSave";

            AbstractSpreadsheet sheet1 = new Spreadsheet(s => true, s => s, "Version 1.0");

            sheet1.SetContentsOfCell("A1", ("=B1 + C1"));
            sheet1.SetContentsOfCell("B1", "20");
            sheet1.SetContentsOfCell("C1", "15");
            Assert.IsTrue(sheet1.Changed);
            List<String> cellsOfSheet1 = sheet1.GetNamesOfAllNonemptyCells().ToList();

            sheet1.Save(fileName);

            AbstractSpreadsheet sheet2 = new Spreadsheet(fileName, s => true, s => s, "Version 1.0");
            sheet2.GetSavedVersion("invalid/Name");

        }

        /// <summary>
        /// Tests to see if saved version is same as version passed in const, should fail
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(SpreadsheetReadWriteException))]
        public void Test11TestInvalidVersionFromSavedVersionToVersonPassedIn()
        {
            String fileName = "first/Save";

            AbstractSpreadsheet sheet1 = new Spreadsheet(s => true, s => s, "Version 1.0");

            sheet1.SetContentsOfCell("A1", ("=B1 + C1"));
            sheet1.SetContentsOfCell("B1", "20");
            sheet1.SetContentsOfCell("C1", "15");

            List<String> cellsOfSheet1 = sheet1.GetNamesOfAllNonemptyCells().ToList();

            sheet1.Save(fileName);


        }

        /// <summary>
        /// Tests to see if file exists, DNE and should throw exception
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(SpreadsheetReadWriteException))]
        public void Test11TestFileDNE()
        {
            String fileName = "testFileName";

            AbstractSpreadsheet sheet2 = new Spreadsheet(fileName, s => true, s => s, "Version 1.1");
        }

        /// <summary>
        /// Tests value of cell
        /// </summary>
        [TestMethod]
        public void Test10EvaluateCellWithMultipleFormulas()
        {
            AbstractSpreadsheet sheet = new Spreadsheet();
            sheet.SetContentsOfCell("A1", "=B1 + C1 + 10");
            sheet.SetContentsOfCell("B1", "=C1 + D1 + 5");
            sheet.SetContentsOfCell("C1", "=D1 + 10");
            sheet.SetContentsOfCell("D1", "25");

            Assert.AreEqual(110.0, sheet.GetCellValue("A1"));

            sheet.SetContentsOfCell("D1", "20");

            Assert.AreEqual(95.0, sheet.GetCellValue("A1"));
            sheet.SetContentsOfCell("D1", "String");
            Assert.AreEqual(new FormulaError("Error: cell value not valid or empty"), sheet.GetCellValue("A1"));
            Assert.AreEqual(new FormulaError("Error: cell value not valid or empty"), sheet.GetCellValue("B1"));
            Assert.AreEqual(new FormulaError("Error: cell value not valid or empty"), sheet.GetCellValue("C1"));
        }



        /// <summary>
        /// Tests Constructor, tests to make sure no cells exist, which shouldn't
        /// </summary>
        [TestMethod]
        public void TestConstructor()
        {
            IEnumerable<String> tempList = new List<String>();

            AbstractSpreadsheet sheet = new Spreadsheet();
            tempList = sheet.GetNamesOfAllNonemptyCells().ToList();

            Assert.AreEqual(0, tempList.Count());
        }

        /// <summary>
        /// Tests to the cell name to make sure is invalid name, should throw
        /// expception
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(InvalidNameException))]
        public void TestValidName1()
        {
            AbstractSpreadsheet sheet = new Spreadsheet();
            sheet.SetContentsOfCell("5", ("5 + 2"));
        }


        /// <summary>
        /// Tests adding a formula to the spreedsheet
        /// </summary>
        [TestMethod]
        public void TestAddFormulaToSpreedSheetWithMultipleCellsReferenced()
        {
            ISet<String> tempsVariablesReturned = new HashSet<String>();
            List<String> expectedVaribles = new List<string>
            {
                "A1",
                "C1",
                "B1"
            };

            AbstractSpreadsheet sheet = new Spreadsheet();
            tempsVariablesReturned = sheet.SetContentsOfCell("A1", ("B1 + C1"));

            for (int i = 0; i < tempsVariablesReturned.Count; i++)
                Assert.AreEqual(expectedVaribles.ElementAt(i), tempsVariablesReturned.ElementAt(i));
        }

        /// <summary>
        /// Tests adding text cell then formula cell, making sure has correct Dependents
        /// </summary>
        [TestMethod]
        public void TestAddFormulaToSpreedSheetWithMultipleCellsReferencedAddNewCellToExistingCell()
        {
            ISet<String> tempsVariablesReturned = new HashSet<String>();
            List<String> expectedVaribles = new List<string>
            {
                "B1",
                "A1"
            };

            AbstractSpreadsheet sheet = new Spreadsheet();
            tempsVariablesReturned = sheet.SetContentsOfCell("A1", "String");
            tempsVariablesReturned = sheet.SetContentsOfCell("B1", ("A1"));

            for (int i = 0; i < tempsVariablesReturned.Count; i++)
                Assert.AreEqual(expectedVaribles.ElementAt(i), tempsVariablesReturned.ElementAt(i));
        }

        /// <summary>
        /// Tests adding text cell then formula cell, making sure has correct Dependents
        /// </summary>
        [TestMethod]
        public void TestAddFormulaAndAddTwoOtherCellsTheDependOnFormulaCell()
        {
            ISet<String> tempsVariablesReturned = new HashSet<String>();
            List<String> expectedVaribles = new List<string>
            {
                "A1",
                "B1",
                "C1"
            };

            AbstractSpreadsheet sheet = new Spreadsheet();
            sheet.SetContentsOfCell("B1", ("A1*2"));
            sheet.SetContentsOfCell("C1", ("B1+A1"));
            tempsVariablesReturned = sheet.SetContentsOfCell("A1", ("2"));

            for (int i = 0; i < tempsVariablesReturned.Count; i++)
                Assert.AreEqual(expectedVaribles.ElementAt(i), tempsVariablesReturned.ElementAt(i));
        }


        /// <summary>
        /// Tests to see if name is invalid fails, should
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(InvalidNameException))]
        public void TestGetCellContentsInvalidCellName()
        {
            AbstractSpreadsheet sheet = new Spreadsheet();

            Assert.IsTrue(sheet.GetCellContents("1A").GetType().Equals((typeof(Formula))));
        }


        /// <summary>
        /// Tests to see if null name for text field fails
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(InvalidNameException))]
        public void TestForNullNameForTextCell()
        {
            AbstractSpreadsheet sheet = new Spreadsheet();
            sheet.SetContentsOfCell(null, "String");
        }

        /// <summary>
        /// Tests to see if null text for text field fails
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void TestForNullTextForTextCell()
        {
            AbstractSpreadsheet sheet = new Spreadsheet();
            String tempString = null;
            sheet.SetContentsOfCell("A1", tempString);
        }

        /// <summary>
        /// Tests add a cell with formula then replace that cell with text
        /// </summary>
        [TestMethod]
        public void TestAddFormulaCellThenReplaceWithTextCell()
        {
            AbstractSpreadsheet sheet = new Spreadsheet();

            sheet.SetContentsOfCell("A1", ("=B1 + C1"));
            sheet.SetContentsOfCell("A1", "String");
            Assert.IsTrue(sheet.GetCellContents("A1").GetType().Equals((typeof(String))));
        }

        /// <summary>
        /// Tests add a cell with formula then replace that cell with text
        /// </summary>
        [TestMethod]
        public void TestAddFormulaCellThenReplaceWithTextCellTestReturnedSet()
        {
            HashSet<String> testReturnedSet;

            AbstractSpreadsheet sheet = new Spreadsheet();

            sheet.SetContentsOfCell("A1", ("=B1 + C1"));
            testReturnedSet = (HashSet<String>)(sheet.SetContentsOfCell("A1", "String"));

            foreach (String st in testReturnedSet)
                Assert.AreEqual("A1", st);
        }

        /// <summary>
        /// Tests add a cell with formul that depends on text of other cell
        /// </summary>
        [TestMethod]
        public void TestAddTextAndFormulaSet()
        {
            AbstractSpreadsheet sheet = new Spreadsheet();

            sheet.SetContentsOfCell("A1", ("String"));
            sheet.SetContentsOfCell("B1", ("=A1"));
        }

        /// <summary>
        /// Tests add a formula, then replace the formula with a new formula
        /// </summary>
        [TestMethod]
        public void TestAddFormulThenReplaceWithNewFormula()
        {
            AbstractSpreadsheet sheet = new Spreadsheet();

            List<String> expectedResults = new List<String>
            {
                "A1",
                "Z1"
            };

            sheet.SetContentsOfCell("A1", ("=C1 + B1 - 2"));
            List<String> actualResults = sheet.SetContentsOfCell("A1", ("=Z1 - 2")).ToList();

            for (int i = 0; i < actualResults.Count; i++)
                Assert.AreEqual(expectedResults.ElementAt(i), actualResults.ElementAt(i));
        }

        /// <summary>
        /// Tests try add a double cell
        /// </summary>
        [TestMethod]
        public void TestAddDoubleCell()
        {
            AbstractSpreadsheet sheet = new Spreadsheet();

            List<String> actualResults = sheet.SetContentsOfCell("A1", "5.2").ToList();

            foreach (String st in actualResults)
                Assert.AreEqual("A1", st);
        }

        /// <summary>
        /// Tests try add a formula cell then replace with double cell
        /// </summary>
        [TestMethod]
        public void TestAddFormulaCellThenReplaceWithDoubleCell()
        {
            AbstractSpreadsheet sheet = new Spreadsheet();
            List<String> expectedFirstResults = new List<string>
            {
                "A1",
                "D1",
                "Z1",
                "B1"
            };

            List<String> actualFirstResults = sheet.SetContentsOfCell("A1", ("=B1 + Z1 + D1")).ToList();

            for (int i = 0; i < actualFirstResults.Count; i++)
                Assert.AreEqual(expectedFirstResults.ElementAt(i), actualFirstResults.ElementAt(i));

            List<String> actualSecondResults = sheet.SetContentsOfCell("A1", "5.2").ToList();

            foreach (String st in actualSecondResults)
                Assert.AreEqual("A1", st);
        }

        /// <summary>
        /// Tests empty cell
        /// </summary>
        [TestMethod]
        public void TestEmptyCell()
        {
            AbstractSpreadsheet sheet = new Spreadsheet();

            Assert.AreEqual("", sheet.GetCellContents("A1"));
        }

        /// <summary>
        /// Tests GetAllNonEmptyCells
        /// </summary>
        [TestMethod]
        public void TestGetAllNonEmptyCells()
        {
            AbstractSpreadsheet sheet = new Spreadsheet();
            List<String> expectedResults = new List<string>
            {
                "A1",
                "B1",
                "C1"
            };

            sheet.SetContentsOfCell("A1", ("=B1 + C1"));
            sheet.SetContentsOfCell("B1", "5.2");
            sheet.SetContentsOfCell("C1", "6.2");

            List<String> returnedResutls = sheet.GetNamesOfAllNonemptyCells().ToList();

            for (int i = 0; i < returnedResutls.Count; i++)
                Assert.AreEqual(expectedResults.ElementAt(i), returnedResutls.ElementAt(i));
        }

        /// <summary>
        /// Tests GetAllNonEmptyCells with strings
        /// </summary>
        [TestMethod]
        public void TestGetAllNonEmptyCellsWithStrings()
        {
            AbstractSpreadsheet sheet = new Spreadsheet();
            List<String> expectedResults = new List<string>
            {
                "A1",
                "B1",
                "C1"
            };

            sheet.SetContentsOfCell("A1", ("=B1 + C1"));
            sheet.SetContentsOfCell("B1", "String");
            sheet.SetContentsOfCell("C1", "String2");

            List<String> returnedResutls = sheet.GetNamesOfAllNonemptyCells().ToList();

            for (int i = 0; i < returnedResutls.Count; i++)
                Assert.AreEqual(expectedResults.ElementAt(i), returnedResutls.ElementAt(i));
        }

        /// <summary>
        /// Tests to see if formula tests for circular dep, should fail
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(CircularException))]
        public void TestForCircularDepCreatTextThenConvertToFormulaCell()
        {
            AbstractSpreadsheet sheet = new Spreadsheet();

            sheet.SetContentsOfCell("B1", ("=A1 + C1"));
            sheet.SetContentsOfCell("C1", ("=A1"));
            sheet.SetContentsOfCell("A1", ("=B1"));
        }

        /// <summary>
        /// Tests to see if formula tests for circular dep, should fail
        /// creates the cell first as a double then trys to change it to
        /// a formula that should throw a circular exception
        /// 
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(CircularException))]
        public void TestForCircularDepCreatDoubleThenConvertToFormulaCell()
        {
            AbstractSpreadsheet sheet = new Spreadsheet();
            sheet.SetContentsOfCell("A1", "5.10");
            Assert.IsTrue(sheet.GetCellContents("A1").GetType().Equals((typeof(Double))));

            sheet.SetContentsOfCell("B1", ("=A1 + C1"));
            sheet.SetContentsOfCell("C1", ("=A1"));
            sheet.SetContentsOfCell("A1", ("=B1"));
        }

        /// <summary>
        /// Tests creating multiple cells that are dependent on a text
        /// cell, then check the cells that depend on it to check the set
        /// </summary>
        [TestMethod]
        public void TestForDepOnTextCell()
        {
            List<String> expectedResults = new List<string>
            {
                "A1",
                "C1",
                "B1"
            };

            AbstractSpreadsheet sheet = new Spreadsheet();
            sheet.SetContentsOfCell("B1", ("=A1"));
            sheet.SetContentsOfCell("C1", ("=A1"));
            List<String> actualResults = sheet.SetContentsOfCell("A1", "String").ToList();

            for (int i = 0; i < actualResults.Count; i++)
                Assert.AreEqual(expectedResults.ElementAt(i), actualResults.ElementAt(i));

        }

        /// <summary>
        /// Tests for a valid cell name
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(InvalidNameException))]
        public void TestForInvalidCellName()
        {
            AbstractSpreadsheet sheet = new Spreadsheet();
            sheet.SetContentsOfCell("$A1$", ("=A1"));
        }

        /// <summary>
        /// Tests for a valid cell name with "2x"
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(InvalidNameException))]
        public void TestForInvalidCellName2x()
        {
            AbstractSpreadsheet sheet = new Spreadsheet();
            sheet.SetContentsOfCell("2x", ("=A1"));
        }

        
        // EMPTY SPREADSHEETS
        [TestMethod()]
        [ExpectedException(typeof(InvalidNameException))]
        public void Test1()
        {
            Spreadsheet s = new Spreadsheet();
            s.GetCellContents(null);
        }

        [TestMethod()]
        [ExpectedException(typeof(InvalidNameException))]
        public void Test2()
        {
            Spreadsheet s = new Spreadsheet();
            s.GetCellContents("1AA");
        }

        [TestMethod()]
        public void Test3()
        {
            Spreadsheet s = new Spreadsheet();
            Assert.AreEqual("", s.GetCellContents("A2"));
        }

        // SETTING CELL TO A DOUBLE
        [TestMethod()]
        [ExpectedException(typeof(InvalidNameException))]
        public void Test4()
        {
            Spreadsheet s = new Spreadsheet();
            s.SetContentsOfCell(null, "1.5");
        }

        [TestMethod()]
        [ExpectedException(typeof(InvalidNameException))]
        public void Test5()
        {
            Spreadsheet s = new Spreadsheet();
            s.SetContentsOfCell("1A1A", "1.5");
        }

        [TestMethod()]
        public void Test6()
        {
            Spreadsheet s = new Spreadsheet();
            s.SetContentsOfCell("Z7", "1.5");
            Assert.AreEqual(1.5, (double)s.GetCellContents("Z7"), 1e-9);
        }

        // SETTING CELL TO A STRING
        [TestMethod()]
        [ExpectedException(typeof(ArgumentNullException))]
        public void Test7()
        {
            Spreadsheet s = new Spreadsheet();
            s.SetContentsOfCell("A8", (string)null);
        }

        [TestMethod()]
        [ExpectedException(typeof(InvalidNameException))]
        public void Test8()
        {
            Spreadsheet s = new Spreadsheet();
            s.SetContentsOfCell(null, "hello");
        }

        [TestMethod()]
        [ExpectedException(typeof(InvalidNameException))]
        public void Test9()
        {
            Spreadsheet s = new Spreadsheet();
            s.SetContentsOfCell("1AZ", "hello");
        }

        [TestMethod()]
        public void Test10()
        {
            Spreadsheet s = new Spreadsheet();
            s.SetContentsOfCell("Z7", "hello");
            Assert.AreEqual("hello", s.GetCellContents("Z7"));
        }

        // SETTING CELL TO A FORMULA
        [TestMethod()]
        [ExpectedException(typeof(ArgumentNullException))]
        public void Test11()
        {
            Spreadsheet s = new Spreadsheet();
            s.SetContentsOfCell("A8", null);
        }

        [TestMethod()]
        [ExpectedException(typeof(InvalidNameException))]
        public void Test12()
        {
            Spreadsheet s = new Spreadsheet();
            s.SetContentsOfCell(null, ("=2"));
        }

        [TestMethod()]
        [ExpectedException(typeof(InvalidNameException))]
        public void Test13()
        {
            Spreadsheet s = new Spreadsheet();
            s.SetContentsOfCell("1AZ", ("=2"));
        }

        // CIRCULAR FORMULA DETECTION
        [TestMethod()]
        [ExpectedException(typeof(CircularException))]
        public void Test15()
        {
            Spreadsheet s = new Spreadsheet();
            s.SetContentsOfCell("A1", ("=A2"));
            s.SetContentsOfCell("A2", ("=A1"));
        }

        [TestMethod()]
        [ExpectedException(typeof(CircularException))]
        public void Test16()
        {
            Spreadsheet s = new Spreadsheet();
            s.SetContentsOfCell("A1", ("=A2+A3"));
            s.SetContentsOfCell("A3", ("=A4+A5"));
            s.SetContentsOfCell("A5", ("=A6+A7"));
            s.SetContentsOfCell("A7", ("=A1+A1"));
        }

        [TestMethod()]
        [ExpectedException(typeof(CircularException))]
        public void Test17()
        {
            Spreadsheet s = new Spreadsheet();
            try
            {
                s.SetContentsOfCell("A1", ("=A2+A3"));
                s.SetContentsOfCell("A2", "15");
                s.SetContentsOfCell("A3", "30");
                s.SetContentsOfCell("A2", ("=A3*A1"));
            }
            catch (CircularException e)
            {
                Assert.AreEqual(15, (double)s.GetCellContents("A2"), 1e-9);
                throw e;
            }
        }

        // NONEMPTY CELLS
        [TestMethod()]
        public void Test18()
        {
            Spreadsheet s = new Spreadsheet();
            Assert.IsFalse(s.GetNamesOfAllNonemptyCells().GetEnumerator().MoveNext());
        }

        [TestMethod()]
        public void Test19()
        {
            Spreadsheet s = new Spreadsheet();
            s.SetContentsOfCell("B1", "");
            Assert.IsFalse(s.GetNamesOfAllNonemptyCells().GetEnumerator().MoveNext());
        }

        [TestMethod()]
        public void Test20()
        {
            Spreadsheet s = new Spreadsheet();
            s.SetContentsOfCell("B1", "hello");
            Assert.IsTrue(new HashSet<string>(s.GetNamesOfAllNonemptyCells()).SetEquals(new HashSet<string>() { "B1" }));
        }

        [TestMethod()]
        public void Test21()
        {
            Spreadsheet s = new Spreadsheet();
            s.SetContentsOfCell("B1", "52.25");
            Assert.IsTrue(new HashSet<string>(s.GetNamesOfAllNonemptyCells()).SetEquals(new HashSet<string>() { "B1" }));
        }

        [TestMethod()]
        public void Test22()
        {
            Spreadsheet s = new Spreadsheet();
            s.SetContentsOfCell("B1", ("=3.5"));
            Assert.IsTrue(new HashSet<string>(s.GetNamesOfAllNonemptyCells()).SetEquals(new HashSet<string>() { "B1" }));
        }

        [TestMethod()]
        public void Test23()
        {
            Spreadsheet s = new Spreadsheet();
            s.SetContentsOfCell("A1", "17.2");
            s.SetContentsOfCell("C1", "hello");
            s.SetContentsOfCell("B1", ("=3.5"));
            Assert.IsTrue(new HashSet<string>(s.GetNamesOfAllNonemptyCells()).SetEquals(new HashSet<string>() { "A1", "B1", "C1" }));
        }

        // RETURN VALUE OF SET CELL CONTENTS
        [TestMethod()]
        public void Test24()
        {
            Spreadsheet s = new Spreadsheet();
            s.SetContentsOfCell("B1", "hello");
            s.SetContentsOfCell("C1", ("=5"));
            Assert.IsTrue(s.SetContentsOfCell("A1", "17.2").SetEquals(new HashSet<string>() { "A1" }));
        }

        [TestMethod()]
        public void Test25()
        {
            Spreadsheet s = new Spreadsheet();
            s.SetContentsOfCell("A1", "17.2");
            s.SetContentsOfCell("C1", ("=5"));
            Assert.IsTrue(s.SetContentsOfCell("B1", "hello").SetEquals(new HashSet<string>() { "B1" }));
        }

        [TestMethod()]
        public void Test26()
        {
            Spreadsheet s = new Spreadsheet();
            s.SetContentsOfCell("A1", "17.2");
            s.SetContentsOfCell("B1", "hello");
            Assert.IsTrue(s.SetContentsOfCell("C1", ("=5")).SetEquals(new HashSet<string>() { "C1" }));
        }

        [TestMethod()]
        public void Test27()
        {
            Spreadsheet s = new Spreadsheet();
            s.SetContentsOfCell("A1", ("=A2+A3"));
            s.SetContentsOfCell("A2", "6");
            s.SetContentsOfCell("A3", ("=A2+A4"));
            s.SetContentsOfCell("A4", ("=A2+A5"));
            Assert.IsTrue(s.SetContentsOfCell("A5", "82.5").SetEquals(new HashSet<string>() { "A5", "A4", "A3", "A1" }));
        }

        // CHANGING CELLS
        [TestMethod()]
        public void Test28()
        {
            Spreadsheet s = new Spreadsheet();
            s.SetContentsOfCell("A1", ("=A2+A3"));
            s.SetContentsOfCell("A1", "2.5");
            Assert.AreEqual(2.5, (double)s.GetCellContents("A1"), 1e-9);
        }

        [TestMethod()]
        public void Test29()
        {
            Spreadsheet s = new Spreadsheet();
            s.SetContentsOfCell("A1", ("=A2+A3"));
            s.SetContentsOfCell("A1", "Hello");
            Assert.AreEqual("Hello", (string)s.GetCellContents("A1"));
        }



        // STRESS TESTS
        [TestMethod()]
        public void Test31()
        {
            Spreadsheet s = new Spreadsheet();
            s.SetContentsOfCell("A1", ("=B1+B2"));
            s.SetContentsOfCell("B1", ("=C1-C2"));
            s.SetContentsOfCell("B2", ("=C3*C4"));
            s.SetContentsOfCell("C1", ("=D1*D2"));
            s.SetContentsOfCell("C2", ("=D3*D4"));
            s.SetContentsOfCell("C3", ("=D5*D6"));
            s.SetContentsOfCell("C4", ("=D7*D8"));
            s.SetContentsOfCell("D1", ("=E1"));
            s.SetContentsOfCell("D2", ("=E1"));
            s.SetContentsOfCell("D3", ("=E1"));
            s.SetContentsOfCell("D4", ("=E1"));
            s.SetContentsOfCell("D5", ("=E1"));
            s.SetContentsOfCell("D6", ("=E1"));
            s.SetContentsOfCell("D7", ("=E1"));
            s.SetContentsOfCell("D8", ("=E1"));
            ISet<String> cells = s.SetContentsOfCell("E1", "0");
            Assert.IsTrue(new HashSet<string>() { "A1", "B1", "B2", "C1", "C2", "C3", "C4", "D1", "D2", "D3", "D4", "D5", "D6", "D7", "D8", "E1" }.SetEquals(cells));
        }
        [TestMethod()]
        public void Test32()
        {
            Test31();
        }
        [TestMethod()]
        public void Test33()
        {
            Test31();
        }
        [TestMethod()]
        public void Test34()
        {
            Test31();
        }

        [TestMethod()]
        public void Test35()
        {
            Spreadsheet s = new Spreadsheet();
            ISet<String> cells = new HashSet<string>();
            for (int i = 1; i < 200; i++)
            {
                cells.Add("A" + i);
                Assert.IsTrue(cells.SetEquals(s.SetContentsOfCell("A" + i, ("=A" + (i + 1)))));
            }
        }
        [TestMethod()]
        public void Test36()
        {
            Test35();
        }
        [TestMethod()]
        public void Test37()
        {
            Test35();
        }
        [TestMethod()]
        public void Test38()
        {
            Test35();
        }
        [TestMethod()]
        public void Test39()
        {
            Spreadsheet s = new Spreadsheet();
            for (int i = 1; i < 200; i++)
            {
                s.SetContentsOfCell("A" + i, ("=A" + (i + 1)));
            }
            try
            {
                s.SetContentsOfCell("A150", ("=A50"));
                Assert.Fail();
            }
            catch (CircularException)
            {
            }
        }
        [TestMethod()]
        public void Test40()
        {
            Test39();
        }
        [TestMethod()]
        public void Test41()
        {
            Test39();
        }
        [TestMethod()]
        public void Test42()
        {
            Test39();
        }

        /*
        [TestMethod()]
        public void Test44()
        {
            Test43();
        }
        [TestMethod()]
        public void Test45()
        {
            Test43();
        }
        [TestMethod()]
        public void Test46()
        {
            Test43();
        }
        */

        [TestMethod()]
        public void Test47()
        {
            RunRandomizedTest(47, 2519);
        }
        [TestMethod()]
        public void Test48()
        {
            RunRandomizedTest(48, 2521);
        }
        [TestMethod()]
        public void Test49()
        {
            RunRandomizedTest(49, 2526);
        }
        [TestMethod()]
        public void Test50()
        {
            RunRandomizedTest(50, 2521);
        }

        public void RunRandomizedTest(int seed, int size)
        {
            Spreadsheet s = new Spreadsheet();
            Random rand = new Random(seed);
            for (int i = 0; i < 10000; i++)
            {
                try
                {
                    switch (rand.Next(3))
                    {
                        case 0:
                            s.SetContentsOfCell(randomName(rand), "3.14");
                            break;
                        case 1:
                            s.SetContentsOfCell(randomName(rand), "hello");
                            break;
                        case 2:
                            s.SetContentsOfCell(randomName(rand), randomFormula(rand));
                            break;
                    }
                }
                catch (CircularException)
                {
                }
            }
            ISet<string> set = new HashSet<string>(s.GetNamesOfAllNonemptyCells());
            Assert.AreEqual(size, set.Count);
        }

        private String randomName(Random rand)
        {
            return "ABCDEFGHIJKLMNOPQRSTUVWXYZ".Substring(rand.Next(26), 1) + (rand.Next(99) + 1);
        }

        private String randomFormula(Random rand)
        {
            String f = randomName(rand);
            for (int i = 0; i < 10; i++)
            {
                switch (rand.Next(4))
                {
                    case 0:
                        f += "+";
                        break;
                    case 1:
                        f += "-";
                        break;
                    case 2:
                        f += "*";
                        break;
                    case 3:
                        f += "/";
                        break;
                }
                switch (rand.Next(2))
                {
                    case 0:
                        f += 7.2;
                        break;
                    case 1:
                        f += randomName(rand);
                        break;
                }
            }
            return f;
        }

    }
}

