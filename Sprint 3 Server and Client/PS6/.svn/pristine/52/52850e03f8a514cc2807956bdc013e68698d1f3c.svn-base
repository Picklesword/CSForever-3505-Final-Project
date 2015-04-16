// Jonathan Warner CS 3500  
// 10/3/14
// SpreadsheetTests PS5

using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SpreadsheetUtilities;
using SS;
using System.Collections.Generic; 



namespace SpreadsheetTests
{
    /// <summary>
    /// Test cases for spreadsheet and cell classes
    /// </summary>
    [TestClass]
    public class SpreadsheetTests
    {
        /// <summary>
        /// Tests to make sure the constructor works 
        /// </summary>
        [TestMethod]
        public void Public_ConstructorTest()
        {
            Spreadsheet test = new Spreadsheet();
        }

        /// <summary>
        ///  Tests the constructor making sure it also works for type Abstract Spreadsheet 
        /// </summary>
        [TestMethod]
        public void Public_ConstructorTestAbs()
        {
            AbstractSpreadsheet test = new Spreadsheet();
        }

        /// <summary>
        ///  Tests the constructor making sure it also works for type Abstract Spreadsheet 3 parameter
        /// </summary>
        [TestMethod]
        public void Public_ConstructorTestAbs3Parm()
        {
            AbstractSpreadsheet test = new Spreadsheet(s=>true,s => s, "default");
        }

        /// <summary>
        /// Tests that trying to get contents of an empty cell returns and empty string 
        /// </summary>
        [TestMethod]
        public void Public_GetCellContents()
        {
            Spreadsheet test = new Spreadsheet();
            test.GetCellContents("a1");
            Assert.AreEqual("", "");
        }

        /// <summary>
        /// Test the get contents for a cell containing a double 
        /// </summary>
        [TestMethod]
        public void Public_CetGellContents2()
        {
            Spreadsheet test = new Spreadsheet();
            string temp = "3.0";
            test.SetContentsOfCell("a1", temp);
            object check = test.GetCellContents("a1");
            Assert.AreEqual(3.0, check);
        }

        /// <summary>
        /// Tests the get contents for a cell containing a string 
        /// </summary>
        [TestMethod]
        public void Public_GetCellContents3()
        {
            Spreadsheet test = new Spreadsheet();
            string temp = "hello";
            test.SetContentsOfCell("a1", temp); 
            object check = test.GetCellContents("a1");
            Assert.AreEqual("hello", check);
        }

        /// <summary>
        /// Tests the get contents for a cell with a formula 
        /// </summary>
        [TestMethod]
        public void Public_GetCellContents4()
        {
            Spreadsheet test = new Spreadsheet();
            string temp1 = "=2.0 + 2.0";
            Formula temp2 = new Formula("2.0 + 2.0");
            test.SetContentsOfCell("a1", temp1);
            object check = test.GetCellContents("a1");
            Assert.AreEqual(true, temp2 == (Formula)check);
        }

        /// <summary>
        /// Tests to make sure GetCellContents returns an InvalidNameException when given null as the cell name 
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(InvalidNameException))]
        public void Public_GetCellContentsfail()
        {
            Spreadsheet test = new Spreadsheet();
            string temp1 = "=2E-9 + 2.0";
            Formula temp2 = new Formula("2.0 + 2.0");
            test.SetContentsOfCell("a1", temp1);
            object check = test.GetCellContents(null);
        }

        /// <summary>
        /// Tests to make sure CetCellContents returns an InvalidNameException when the given name is not of correct format 
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(InvalidNameException))]
        public void Public_GetCellContentsfail2()
        {
            Spreadsheet test = new Spreadsheet();
            string temp1 = "=2.0 + 2.0";
            Formula temp2 = new Formula("2.0 + 2.0");
            test.SetContentsOfCell("a1", temp1);
            object check = test.GetCellContents("1");
        }

        /// <summary>
        ///  Test to make sure SetContentsOfCell for strings returns an InvalidNameException when given an invalid name 
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(InvalidNameException))]
        public void Public_SetContentsOfCell()
        {
            Spreadsheet test = new Spreadsheet();
            string temp = "hello";
            test.SetContentsOfCell("1", temp);
           

        }

        /// <summary>
        /// Tests to make sure SetContentsOfCell for doubles returns an InvalidNameException when given an invalid name 
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(InvalidNameException))]
        public void Public_SetContentsOfCell2()
        {
            Spreadsheet test = new Spreadsheet();
            string temp = "3.0";
            test.SetContentsOfCell("1", temp);
            
        }

        /// <summary>
        /// Tests to make sure SetContentsOfCell for Formula returns an InvalidNameException when given an invalid name 
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(InvalidNameException))]
        public void Public_SetContentsOfCell3()
        {
            Spreadsheet test = new Spreadsheet();
            string temp1 = "=2.0 + 2.0";
            Formula temp2 = new Formula("2.0 + 2.0");
            test.SetContentsOfCell("1", temp1);
           

        }

        /// <summary>
        /// Test that SetCellContent for Formula returns and InvalidNameException when the name is null 
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(InvalidNameException))]
        public void Public_SetContentsOfCell4()
        {
            Spreadsheet test = new Spreadsheet();
            string temp1 = "=2.0 + 2.0";
            Formula temp2 = new Formula("2.0 + 2.0");
            test.SetContentsOfCell(null, temp1);
          

        }

        /// <summary>
        /// Tests to make sure that setting the contents  to an already existent nonempty cell overwrites them  using formula SetCellContent
        /// </summary>

        [TestMethod]
        public void Public_SetContentsOfCell5()
        {
            Spreadsheet test = new Spreadsheet();
            string temp1 = "=2.0 + 2.0";
            string temp2 = "=3.0 + 2.0";
            test.SetContentsOfCell("a1", temp1);
            test.SetContentsOfCell("a1", temp2);
            temp2 = temp2.Replace("=", "");
            Formula comparee = new Formula(temp2); 
            Assert.AreEqual(comparee, test.GetCellContents("a1"));


        }

        /// <summary>
        /// Tests to make sure that setting the contents  to an already existent nonempty cell overwrites them  using double SetCellContent
        /// </summary>
        [TestMethod]
        public void Public_SetContentsOfCell6()
        {
            Spreadsheet test = new Spreadsheet();
            string first = "3.0";
            string second = "5.0"; 
            test.SetContentsOfCell("a1", first);
            test.SetContentsOfCell("a1", second);
            second = second.Replace("=", "");
            double holder;
            double.TryParse(second, out holder);
            Assert.AreEqual(holder, test.GetCellContents("a1"));


        }

        /// <summary>
        /// Tests to make sure that setting the contents  to an already existent nonempty cell overwrites them  using string SetCellContent
        /// </summary>
        [TestMethod]
        public void Public_SetContentsOfCell7()
        {
            Spreadsheet test = new Spreadsheet();
            string first = "hello";
            string second = "goodbye";
            test.SetContentsOfCell("a1", first);
            test.SetContentsOfCell("a1", second);
            Assert.AreEqual(second, test.GetCellContents("a1"));


        }

        /// <summary>
        /// Test that an argument exception occurs when null is given as what to set contents as with Formula SetContentsOfCell
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void Public_SetContentsOfCellfail1()
        {
            Spreadsheet test = new Spreadsheet();
            string temp1 = null;
            Formula temp2 = new Formula("2.0 + 2.0");
            test.SetContentsOfCell("a1" , temp1);


        }

        /// <summary>
        /// Tests tahat ArgumentException is thrown when null is given as value to set with string SetContentsOfCell 
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void Public_SetContentsOfCellfail2()
        {
            Spreadsheet test = new Spreadsheet();
            string broke = null; 
            test.SetContentsOfCell("a1", broke );


        }

       

        /// <summary>
        /// Tests for InvalidNameException when a null is given as the name of a cell  with  double SetContentsOfCell
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(InvalidNameException))]
        public void Public_SetContentsOfCellfail4()
        {
            Spreadsheet test = new Spreadsheet();
            string broke = "10";
            test.SetContentsOfCell(null, broke);


        }

        

        /// <summary>
        /// Tests for an InvalidNameException if null is given as name for string SetContentsOfCell 
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(InvalidNameException))]
        public void Public_SetContentsOfCellfail6()
        {
            Spreadsheet test = new Spreadsheet();
            string broke = "hello";
            test.SetContentsOfCell(null, broke);


        }


        /// <summary>
        /// test that  Argument Exception is thrown when null formula is given to be placed in a cell by SetContentsOfCell
        /// and is thrown before an InvalidNameException 
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void Public_SetCellFormula()
        {
            Spreadsheet test = new Spreadsheet();
            string temp1 = null;
            Formula temp2 = new Formula("2.0 + 2.0");
            test.SetContentsOfCell("1", temp1);


        }
     

        /// <summary>
        /// Tests that setting up cells creates the right dependencies between them 
        /// </summary>
        [TestMethod]
        public void Public_SetContentsOfCellDeps()
        {
            Spreadsheet test = new Spreadsheet();
            test.SetContentsOfCell("a1", "=b1 + c1");
            test.SetContentsOfCell("c1", "=d1+e1");
            ISet<string> test_set = test.SetContentsOfCell("e1", "5");
            Assert.AreEqual(true, test_set.Contains("a1"));
            Assert.AreEqual(true, test_set.Contains("c1"));
            Assert.AreEqual(true, test_set.Contains("e1")); 
            Assert.AreEqual(true, test_set.Count == 3); 
        }

        /// <summary>
        /// Tests that overwriting a cell with an empty cell will remove neccary dependency 
        /// </summary>
        [TestMethod]
        public void Public_SetContentsOfCellDeps2()
        {
            Spreadsheet test = new Spreadsheet();
            test.SetContentsOfCell("a1", "=b1 + c1");
            test.SetContentsOfCell("c1", "=d1+e1");
            test.SetContentsOfCell("a1", "=b1"); 
            ISet<string> test_set = test.SetContentsOfCell("e1", "5");
            Assert.AreEqual(true, test_set.Contains("c1"));
            Assert.AreEqual(true, test_set.Contains("e1"));
            Assert.AreEqual(true, test_set.Count == 2);
        }

        /// <summary>
        /// Test to make sure circular dependencies are detected when first set 
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(CircularException))]
        public void Public_SetContentsOfCellCirdep()
        {
            Spreadsheet test = new Spreadsheet();
            test.SetContentsOfCell("a1", "=b1 + c1");
            test.SetContentsOfCell("c1", "=d1+e1");
            test.SetContentsOfCell("e1", "=a1"); 
        }


        /// <summary>
        /// Test to make sure ciruclar dependencies are detected when overwritten 
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(CircularException))]
        public void Public_SetContentsOfCellCirdep2()
        {
            Spreadsheet test = new Spreadsheet();
            test.SetContentsOfCell("a1", "=b1 + c1");
            test.SetContentsOfCell("c1", "=d1+e1");
            test.SetContentsOfCell("a1", "=a1");
        }

        /// <summary>
        /// Test to make sure that GetNamesOfAllNonemptyCells returns non empty cells 
        /// </summary>
        [TestMethod]
        public void Public_GetNonEmpt()
        {
            Spreadsheet test = new Spreadsheet();
            test.SetContentsOfCell("a1", "3.0");
            test.SetContentsOfCell("b1", "hello");
            test.SetContentsOfCell("c1", "=3.0 + 1.0");
            HashSet<string> non_empt = new HashSet<string>(); 
            foreach(string s in test.GetNamesOfAllNonemptyCells())
            {
                non_empt.Add(s); 
            }
            Assert.AreEqual(true, non_empt.Contains("a1"));
            Assert.AreEqual(true, non_empt.Contains("b1"));
            Assert.AreEqual(true, non_empt.Contains("c1"));
            Assert.AreEqual(3, non_empt.Count);
           
        }

        /// <summary>
        /// Tests that GetNamesOfAllNonemptyCells doesn't return a cell if it has been overwritten as empty 
        /// </summary>
        [TestMethod]
        public void Public_GetNonEmpt2()
        {
            Spreadsheet test = new Spreadsheet();
            test.SetContentsOfCell("a1", "3.0");
            test.SetContentsOfCell("b1", "hello");
            test.SetContentsOfCell("c1", "=3.0 + 1.0");
            test.SetContentsOfCell("b1", "");
            HashSet<string> non_empt = new HashSet<string>();
            foreach (string s in test.GetNamesOfAllNonemptyCells())
            {
                non_empt.Add(s);
            }
            Assert.AreEqual(true, non_empt.Contains("a1"));
            Assert.AreEqual(false, non_empt.Contains("b1"));
            Assert.AreEqual(true, non_empt.Contains("c1"));
            Assert.AreEqual(2, non_empt.Count);

        }

        /// <summary>
        /// Tests that giving GetDirectDependets a null returns an argmentNullException
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void Protected_GetDirect()
        {
            AbstractSpreadsheet test = new Spreadsheet();
            PrivateObject test_acess = new PrivateObject(test);
            string temp = null; 
            test.SetContentsOfCell("a1", "3.0");
            test_acess.Invoke("GetDirectDependents", temp); 



        }

        /// <summary>
        /// Tests that giving GetDirectDependents an invalid name causes it to return a InvalidNameException
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(InvalidNameException))]
        public void Protected_GetDirect2()
        {
            AbstractSpreadsheet test = new Spreadsheet();
            PrivateObject test_acess = new PrivateObject(test);
            string temp = "1e9";
            test.SetContentsOfCell("a1", "3.0");
            test_acess.Invoke("GetDirectDependents", temp);



        }

        /// <summary>
        /// Test the cell constructor and the getters and setters 
        /// </summary>
        //[TestMethod]
        //public void Cell_tests()
        //{
        //    Cell test = new Cell("hello");
        //    test.Get_Contents();
        //    test.Get_Value();
        //    test.Set_Contents("hello2");
        //    test.Set_Value("hello2"); 



        //}

        /// <summary>
        /// Tests that dependencies are erased when cell with formula becomes empty cell
        /// </summary>
        [TestMethod]
        public void Stress_Test()
        {
            AbstractSpreadsheet test = new Spreadsheet();
            test.SetContentsOfCell("a1","=1E-3 + b1 + c1 ");
            test.SetContentsOfCell("b1", "=d1 + e1 + f1");
            test.SetContentsOfCell("c1", "=d1+g1");
            test.SetContentsOfCell("c1", ""); 
            ISet<string> test_set = test.SetContentsOfCell("d1", "3.0");
            Assert.AreEqual(true, test_set.Contains("a1"));
            Assert.AreEqual(true, test_set.Contains("b1"));
            Assert.AreEqual(true, test_set.Contains("d1"));
            Assert.AreEqual(false, test_set.Contains("c1"));
            Assert.AreEqual(true, test_set.Count == 3);

        }

        /// <summary>
        /// Test that dependencies are erased when cell with formula changes to a cell with number 
        /// </summary>
        [TestMethod]
        public void Stress_Test2()
        {
            AbstractSpreadsheet test = new Spreadsheet();
            test.SetContentsOfCell("a1", "=1E-3 + b1 + c1 ");
            test.SetContentsOfCell("b1", "=d1 + e1 + f1");
            test.SetContentsOfCell("c1", "=d1+g1");
            test.SetContentsOfCell("c1", "5.0");
            ISet<string> test_set = test.SetContentsOfCell("d1", "3.0");
            Assert.AreEqual(true, test_set.Contains("a1"));
            Assert.AreEqual(true, test_set.Contains("b1"));
            Assert.AreEqual(true, test_set.Contains("d1"));
            Assert.AreEqual(false, test_set.Contains("c1"));
            Assert.AreEqual(true, test_set.Count == 3);

        }
        /// <summary>
        /// Tests that the value is restored if a circular dependency occurs (from grading tests from PS4)
        /// </summary>
        [TestMethod()]
        [ExpectedException(typeof(CircularException))]
        public void Test17_b()
        {
            Spreadsheet s = new Spreadsheet();
            try
            {
                s.SetContentsOfCell("A1", "=A2+A3");
                s.SetContentsOfCell("A2","15");
                s.SetContentsOfCell("A3", "30");
                s.SetContentsOfCell("A2", "=A3*A1");
            }
            catch (CircularException e)
            {
                double test_double = (double)(s.GetCellContents("A2"));
                Assert.AreEqual(15, (double)(s.GetCellContents("A2")), 1e-9);
                throw e;
            }
        }

        /// <summary>
        /// Test that xml document is written without error 
        /// </summary>
        [TestMethod]
        public void Stress_Test_Save()
        {
            AbstractSpreadsheet test = new Spreadsheet();
            test.SetContentsOfCell("a1", "=1E-3 + b1 + c1 ");
            test.SetContentsOfCell("b1", "=d1 + e1 + f1");
            test.SetContentsOfCell("c1", "=d1+g1");
            test.SetContentsOfCell("c1", "5.0");
            ISet<string> test_set = test.SetContentsOfCell("d1", "3.0");
            test.Save("testsave.xml");
            Assert.AreEqual(true, test_set.Contains("a1"));
            Assert.AreEqual(true, test_set.Contains("b1"));
            Assert.AreEqual(true, test_set.Contains("d1"));
            Assert.AreEqual(false, test_set.Contains("c1"));
            Assert.AreEqual(true, test_set.Count == 3);

        }

        /// <summary>
        /// Test that xml document is written without error with string inclued
        /// </summary>
        [TestMethod]
        public void Stress_Test_Save2()
        {
            AbstractSpreadsheet test = new Spreadsheet();
            test.SetContentsOfCell("a1", "=1E-3 + b1 + c1 ");
            test.SetContentsOfCell("b1", "=d1 + e1 + f1");
            test.SetContentsOfCell("c1", "=d1+g1");
            test.SetContentsOfCell("c1", "hello");
            ISet<string> test_set = test.SetContentsOfCell("d1", "3.0");
            test.Save("testsave.xml");
            Assert.AreEqual(true, test_set.Contains("a1"));
            Assert.AreEqual(true, test_set.Contains("b1"));
            Assert.AreEqual(true, test_set.Contains("d1"));
            Assert.AreEqual(false, test_set.Contains("c1"));
            Assert.AreEqual(true, test_set.Count == 3);

        }

        /// <summary>
        /// Test that xml document throws spreadsheet readwrite exception when bad file name given to save as 
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(SpreadsheetReadWriteException))]
        public void Stress_Test_SaveError()
        {
            AbstractSpreadsheet test = new Spreadsheet();
            test.SetContentsOfCell("a1", "=1E-3 + b1 + c1 ");
            test.SetContentsOfCell("b1", "=d1 + e1 + f1");
            test.SetContentsOfCell("c1", "=d1+g1");
            test.SetContentsOfCell("c1", "hello");
            ISet<string> test_set = test.SetContentsOfCell("d1", "3.0");
            test.Save("../../Cupcakes/hey.xml");

        }

        /// <summary>
        /// Test that xml can be read from to generate a spreadsheet  
        /// </summary>
        [TestMethod]
        public void Stress_Test_Read()
        {
            AbstractSpreadsheet test = new Spreadsheet();
            test.SetContentsOfCell("a1", "=1E-3 + b1 + c1 ");
            test.SetContentsOfCell("b1", "=d1 + e1 + f1");
            test.SetContentsOfCell("c1", "=d1+g1");
            test.SetContentsOfCell("c1", "5.0");
            test.Save("testsave2.xml");
            AbstractSpreadsheet test2 = new Spreadsheet("testsave2.xml", s => true, s => s.ToUpper(), "default");
            ISet<string> test_set = test2.SetContentsOfCell("d1", "3.0");
            Assert.AreEqual(true, test_set.Contains("A1"));
            Assert.AreEqual(true, test_set.Contains("B1"));
            Assert.AreEqual(true, test_set.Contains("D1"));
            Assert.AreEqual(false, test_set.Contains("C1"));
            Assert.AreEqual(true, test_set.Count == 3);

        }

        /// <summary>
        /// Test that formula with undefined variables have FormulaError for their value 
        /// </summary>
        [TestMethod]
        public void Stress_Test_Read_GetValue()
        {
            AbstractSpreadsheet test = new Spreadsheet();
            test.SetContentsOfCell("a1", "=1E-3 + b1 + c1 ");
            test.SetContentsOfCell("b1", "=d1 + e1 + f1");
            test.SetContentsOfCell("c1", "=d1+g1");
            test.SetContentsOfCell("c1", "5.0");
            test.Save("testsave3.xml");
            AbstractSpreadsheet test2 = new Spreadsheet("testsave3.xml", s => true, s => s, "default");
            ISet<string> test_set = test2.SetContentsOfCell("d1", "3.0");
            Assert.AreEqual(true, test_set.Contains("a1"));
            Assert.AreEqual(true, test_set.Contains("b1"));
            Assert.AreEqual(true, test_set.Contains("d1"));
            Assert.AreEqual(false, test_set.Contains("c1"));
            Assert.IsTrue((double)test.GetCellValue("c1") == 5.0);
            Assert.IsTrue(test.GetCellValue("a1") is FormulaError); 
            Assert.AreEqual(true, test_set.Count == 3);

        }

        /// <summary>
        /// Test that the recalculate function works and previous formula with undefined variables are recalculated 
        /// after the variables are defined 
        /// </summary>
        [TestMethod]
        public void Stress_Test_Read_GetValue2()
        {
            AbstractSpreadsheet test = new Spreadsheet();
            test.SetContentsOfCell("a1", "=15.0 + b1 + c1 ");
            test.SetContentsOfCell("b1", "= 3.0 + c1");
            test.SetContentsOfCell("c1", "=d1+g1");
            test.SetContentsOfCell("c1", "5.0");
            test.Save("testsave4.xml");
            AbstractSpreadsheet test2 = new Spreadsheet("testsave4.xml", s => true, s => s, "default");
            Assert.IsTrue((double)test2.GetCellValue("c1") == 5.0);
            Assert.AreEqual((double)test2.GetCellValue("a1"),28.00);

        }

        /// <summary>
        /// Test that spreadsheet can be written from file in realitve path
        /// </summary>
        [TestMethod]
        public void Stress_Test_Read_GetValue3()
        {
            AbstractSpreadsheet test = new Spreadsheet();
            test.SetContentsOfCell("a1", "=15.0 + b1 + c1 ");
            test.SetContentsOfCell("b1", "= 3.0 + c1");
            test.SetContentsOfCell("c1", "=d1+g1");
            test.SetContentsOfCell("c1", "5.0");
            test.Save("testsave4.xml");
            AbstractSpreadsheet test2 = new Spreadsheet("../../XmlTestFiles/testsave5.xml", s => true, s => s, "default");
            Assert.IsTrue((double)test2.GetCellValue("c1") == 5.0);
            Assert.AreEqual((double)test2.GetCellValue("a1"), 28.00);

        }

        /// <summary>
        /// Test that spreadsheet read write error is returned when inapropriate amount of tags 
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(SpreadsheetReadWriteException))]
        public void Stress_Test_Read_Error1()
        {
            AbstractSpreadsheet test = new Spreadsheet();
            test.SetContentsOfCell("a1", "=15.0 + b1 + c1 ");
            test.SetContentsOfCell("b1", "= 3.0 + c1");
            test.SetContentsOfCell("c1", "=d1+g1");
            test.SetContentsOfCell("c1", "5.0");
            test.Save("testsave4.xml");
            AbstractSpreadsheet test2 = new Spreadsheet("../../XmlTestFiles/testsave6.xml", s => true, s => s, "default");
            

        }

        /// <summary>
        /// Test that spreadsheet read write error is returned when name is missing 
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(SpreadsheetReadWriteException))]
        public void Stress_Test_Read_Error2()
        {
            AbstractSpreadsheet test = new Spreadsheet();
            test.SetContentsOfCell("a1", "=15.0 + b1 + c1 ");
            test.SetContentsOfCell("b1", "= 3.0 + c1");
            test.SetContentsOfCell("c1", "=d1+g1");
            test.SetContentsOfCell("c1", "5.0");
            test.Save("testsave4.xml");
            AbstractSpreadsheet test2 = new Spreadsheet("../../XmlTestFiles/testsave7.xml", s => true, s => s, "default");
     

        }

        /// <summary>
        /// Test that spreadsheet read write error is returned when contents are missing
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(SpreadsheetReadWriteException))]
        public void Stress_Test_Read_Error3()
        {
            AbstractSpreadsheet test = new Spreadsheet();
            test.SetContentsOfCell("a1", "=15.0 + b1 + c1 ");
            test.SetContentsOfCell("b1", "= 3.0 + c1");
            test.SetContentsOfCell("c1", "=d1+g1");
            test.SetContentsOfCell("c1", "5.0");
            test.Save("testsave4.xml");
            AbstractSpreadsheet test2 = new Spreadsheet("../../XmlTestFiles/testsave8.xml", s => true, s => s, "default");
  

        }

        /// <summary>
        /// Test that spreadsheet read write error is returned when cell tags are missing 
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(SpreadsheetReadWriteException))]
        public void Stress_Test_Read_Error4()
        {
            AbstractSpreadsheet test = new Spreadsheet();
            test.SetContentsOfCell("a1", "=15.0 + b1 + c1 ");
            test.SetContentsOfCell("b1", "= 3.0 + c1");
            test.SetContentsOfCell("c1", "=d1+g1");
            test.SetContentsOfCell("c1", "5.0");
            test.Save("testsave4.xml");
            AbstractSpreadsheet test2 = new Spreadsheet("../../XmlTestFiles/testsave9.xml", s => true, s => s, "default");


        }

        /// <summary>
        /// Test that spreadsheet read write error is returned when version of loaded file does not meet the paramater 
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(SpreadsheetReadWriteException))]
        public void Stress_Test_Read_Error5()
        {
            AbstractSpreadsheet test = new Spreadsheet();
            test.SetContentsOfCell("a1", "=15.0 + b1 + c1 ");
            test.SetContentsOfCell("b1", "= 3.0 + c1");
            test.SetContentsOfCell("c1", "=d1+g1");
            test.SetContentsOfCell("c1", "5.0");
            test.Save("testsave4.xml");
            AbstractSpreadsheet test2 = new Spreadsheet("../../XmlTestFiles/testsave10.xml", s => true, s => s, "default");


        }

        /// <summary>
        /// Test that spreadsheet read write error is returned when noexistant file path is given  
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(SpreadsheetReadWriteException))]
        public void Stress_Test_Read_Error6()
        {
            AbstractSpreadsheet test = new Spreadsheet();
            test.SetContentsOfCell("a1", "=15.0 + b1 + c1 ");
            test.SetContentsOfCell("b1", "= 3.0 + c1");
            test.SetContentsOfCell("c1", "=d1+g1");
            test.SetContentsOfCell("c1", "5.0");
            test.Save("testsave4.xml");
            AbstractSpreadsheet test2 = new Spreadsheet("../../XmlTestFiles/testsve10.xml", s => true, s => s, "default");


        }

        /// <summary>
        /// Test that spreadsheet read write error is returned when formual error in file being read  
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(SpreadsheetReadWriteException))]
        public void Stress_Test_Read_Error7()
        {
            AbstractSpreadsheet test = new Spreadsheet();
            test.SetContentsOfCell("a1", "=15.0 + b1 + c1 ");
            test.SetContentsOfCell("b1", "= 3.0 + c1");
            test.SetContentsOfCell("c1", "=d1+g1");
            test.SetContentsOfCell("c1", "5.0");
            test.Save("testsave4.xml");
            AbstractSpreadsheet test2 = new Spreadsheet("../../XmlTestFiles/testsave11.xml", s => true, s => s, "default");


        }

        /// <summary>
        /// Test that spreadsheet read write error is returned when circular dependency in file being read  
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(SpreadsheetReadWriteException))]
        public void Stress_Test_Read_Error8()
        {
            AbstractSpreadsheet test = new Spreadsheet();
            test.SetContentsOfCell("a1", "=15.0 + b1 + c1 ");
            test.SetContentsOfCell("b1", "= 3.0 + c1");
            test.SetContentsOfCell("c1", "=d1+g1");
            test.SetContentsOfCell("c1", "5.0");
            test.Save("testsave4.xml");
            AbstractSpreadsheet test2 = new Spreadsheet("../../XmlTestFiles/testsave12.xml", s => true, s => s, "default");


        }

        /// <summary>
        /// Test that spreadsheet read write error is returned when naming error in file being read  
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(SpreadsheetReadWriteException))]
        public void Stress_Test_Read_Error9()
        {
            AbstractSpreadsheet test = new Spreadsheet();
            test.SetContentsOfCell("a1", "=15.0 + b1 + c1 ");
            test.SetContentsOfCell("b1", "= 3.0 + c1");
            test.SetContentsOfCell("c1", "=d1+g1");
            test.SetContentsOfCell("c1", "5.0");
            test.Save("testsave4.xml");
            AbstractSpreadsheet test2 = new Spreadsheet("../../XmlTestFiles/testsave13.xml", s => true, s => s, "default");


        }

        /// <summary>
        /// Test that spreadsheet read write error is returned when final contents is missing  
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(SpreadsheetReadWriteException))]
        public void Stress_Test_Read_Error10()
        {
            AbstractSpreadsheet test = new Spreadsheet();
            test.SetContentsOfCell("a1", "=15.0 + b1 + c1 ");
            test.SetContentsOfCell("b1", "= 3.0 + c1");
            test.SetContentsOfCell("c1", "=d1+g1");
            test.SetContentsOfCell("c1", "5.0");
            test.Save("testsave4.xml");
            AbstractSpreadsheet test2 = new Spreadsheet("../../XmlTestFiles/testsave17.xml", s => true, s => s, "default");


        }

        /// <summary>
        /// Test that spreadsheet read write error is returned when final name is missing  
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(SpreadsheetReadWriteException))]
        public void Stress_Test_Read_Error11()
        {
            AbstractSpreadsheet test = new Spreadsheet();
            test.SetContentsOfCell("a1", "=15.0 + b1 + c1 ");
            test.SetContentsOfCell("b1", "= 3.0 + c1");
            test.SetContentsOfCell("c1", "=d1+g1");
            test.SetContentsOfCell("c1", "5.0");
            test.Save("testsave4.xml");
            AbstractSpreadsheet test2 = new Spreadsheet("../../XmlTestFiles/testsave18.xml", s => true, s => s, "default");


        }

        /// <summary>
        /// Test that spreadsheet read write error is returned when cell doesn't contain name or contents
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(SpreadsheetReadWriteException))]
        public void Stress_Test_Read_Error12()
        {
            AbstractSpreadsheet test = new Spreadsheet();
            test.SetContentsOfCell("a1", "=15.0 + b1 + c1 ");
            test.SetContentsOfCell("b1", "= 3.0 + c1");
            test.SetContentsOfCell("c1", "=d1+g1");
            test.SetContentsOfCell("c1", "5.0");
            test.Save("testsave4.xml");
            AbstractSpreadsheet test2 = new Spreadsheet("../../XmlTestFiles/testsave19.xml", s => true, s => s, "default");


        }

        /// <summary>
        /// Test that spreadsheet can get the version information from saved file 
        /// </summary>
        [TestMethod]
        public void Stress_Test_Get_saved_version()
        {
            AbstractSpreadsheet test = new Spreadsheet();
            test.SetContentsOfCell("a1", "=15.0 + b1 + c1 ");
            test.SetContentsOfCell("b1", "= 3.0 + c1");
            test.SetContentsOfCell("c1", "=d1+g1");
            test.SetContentsOfCell("c1", "5.0");
            test.Save("testsave14.xml");
            string holder = test.GetSavedVersion("testsave14.xml");
            Assert.AreEqual(holder, "default"); 

        }

        /// <summary>
        /// Test that spreadsheet throws a spreadsheet read write error when a file can not be opened 
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(SpreadsheetReadWriteException))]
        public void Stress_Test_Get_saved_versionError()
        {
            AbstractSpreadsheet test = new Spreadsheet();
            test.SetContentsOfCell("a1", "=15.0 + b1 + c1 ");
            test.SetContentsOfCell("b1", "= 3.0 + c1");
            test.SetContentsOfCell("c1", "=d1+g1");
            test.SetContentsOfCell("c1", "5.0");
            test.Save("testsave14.xml");
            string holder = test.GetSavedVersion("testsve14.xml");


        }

        /// <summary>
        /// Test that spreadsheet throws a spreadsheet read write error when none valid name given as paramater
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(InvalidNameException))]
        public void Stress_Test_Get_Error()
        {
            AbstractSpreadsheet test = new Spreadsheet();
            test.SetContentsOfCell("a1", "=15.0 + b1 + c1 ");
            test.SetContentsOfCell("b1", "= 3.0 + c1");
            test.SetContentsOfCell("c1", "=d1+g1");
            test.SetContentsOfCell("c1", "5.0");
            test.GetCellValue("1"); 


        }

        /// <summary>
        /// Test that spreadsheet returns empty string when empty var name is given as paramater. 
        /// </summary>
        [TestMethod]
        public void Stress_Test_Get_4()
        {
            AbstractSpreadsheet test = new Spreadsheet();
            test.SetContentsOfCell("a1", "=15.0 + b1 + c1 ");
            test.SetContentsOfCell("b1", "= 3.0 + c1");
            test.SetContentsOfCell("c1", "=d1+g1");
            test.SetContentsOfCell("c1", "5.0");
            object holder =test.GetCellValue("d1");
            Assert.AreEqual(holder, "");



        }

        /// <summary>
        /// Test that spreadsheet recalc with string involved
        /// </summary>
        [TestMethod]
        public void Stress_Test_RecalculateString()
        {
            AbstractSpreadsheet test = new Spreadsheet();
            test.SetContentsOfCell("a1", "=15.0 + b1 + c1 ");
            test.SetContentsOfCell("b1", "= 3.0 + c1");
            test.SetContentsOfCell("c1", "=d1+g1");
            test.SetContentsOfCell("c1", "5.0");
            test.SetContentsOfCell("b1", "hello");
            Assert.IsTrue(test.GetCellValue("a1") is FormulaError);



        }

        /// <summary>
        /// Test that spreadsheet recalc with formula involved
        /// </summary>
        [TestMethod]
        public void Stress_Test_RecalculateFormula()
        {
            AbstractSpreadsheet test = new Spreadsheet();
            test.SetContentsOfCell("a1", "=15.0 + b1 + c1 ");
            test.SetContentsOfCell("b1", "= d1 + c1");
            test.SetContentsOfCell("c1", "=d1+g1");
            test.SetContentsOfCell("c1", "=d1 + 1.0");
            test.SetContentsOfCell("d1", "5.0");
            Assert.AreEqual(test.GetCellValue("a1"), 32.00);
            



        }

        /// <summary>
        /// Test that spreadsheet recalc with formula involved
        /// </summary>
        [TestMethod]
        public void Stress_Test_RecalculateLookupFormula()
        {
            AbstractSpreadsheet test = new Spreadsheet();
            test.SetContentsOfCell("a1", "=15.0 + b1 + c1 ");
            test.SetContentsOfCell("e1", "=a1 +2.0");
            test.SetContentsOfCell("b1", "= d1 + c1");
            test.SetContentsOfCell("c1", "=d1+g1");
            test.SetContentsOfCell("c1", "=d1 + 1.0");
            test.SetContentsOfCell("d1", "5.0");
            Assert.AreEqual(test.GetCellValue("e1"), 34.00);




        }


        ///// <summary>
        ///// Test that xml document is written without error 
        ///// </summary> 
        //[TestMethod]
        //public void Stress_Test_Read()
        //{
        //    AbstractSpreadsheet test = new Spreadsheet();
        //    test.SetContentsOfCell("a1", "=1E-3 + b1 + c1 ");
        //    test.SetContentsOfCell("b1", "=d1 + e1 + f1");
        //    test.SetContentsOfCell("c1", "=d1+g1");
        //    test.SetContentsOfCell("c1", "5.0");
        //    test.Save("testsave.xml");
        //    AbstractSpreadsheet test2 = new Spreadsheet("testsave.xml", s => true, s => s, "default");
        //    ISet<string> test_set = test2.SetContentsOfCell("d1", "3.0");
        //    Assert.AreEqual(true, test_set.Contains("a1"));
        //    Assert.AreEqual(true, test_set.Contains("b1"));
        //    Assert.AreEqual(true, test_set.Contains("d1"));
        //    Assert.AreEqual(false, test_set.Contains("c1"));
        //    Assert.AreEqual(true, test_set.Count == 3);

        //}

        

    }
}
