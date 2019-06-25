using System;
using System.Diagnostics;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Reynolds_API.Controllers;
namespace UnitTestAPI
{
    [TestClass]
    public class UnitTestScanning
    {

        /// <summary>
        /// Unit test for the Number of Files endpoint
        /// </summary>
        [TestMethod]
        public void TestGetNumberOfFiles()
        {
            //37 Files in the TestFiles/Scanning Directory

            //Arrange
            long expected = 8;
            ScanningController scan = new ScanningController();
            string path = "C:\\Users\\Ryan\\Desktop\\Reynolds_API\\UnitTestAPI\\TestFiles\\Scanning";

            //Act
            long actual = scan.getNumberOfFiles(path);
            //Assert
            Assert.AreEqual(expected, actual);

        }

        /// <summary>
        /// Unite Test for the number of directories endpoint
        /// </summary>
        [TestMethod]
        public void TestGetNumberOfDirectories()
        {
            long expected = 6;

            ScanningController scan = new ScanningController();
            string path = "C:\\Users\\Ryan\\Desktop\\Reynolds_API\\UnitTestAPI\\TestFiles\\Scanning";
            long actual = scan.getNumberOfDirectories(path);
            Assert.AreEqual(actual, expected);


        }


        /// <summary>
        /// Unit Test for the total file size enpoint
        /// </summary>
        [TestMethod]
        public void TestGetFileSizeTotal()
        {

            long expected = 62067;

            ScanningController scan = new ScanningController();
            string path = "C:\\Users\\Ryan\\Desktop\\Reynolds_API\\UnitTestAPI\\TestFiles\\Scanning";
            long actual = scan.geFileSizeTotal(path);
            Assert.AreEqual(actual, expected);


        }

        /// <summary>
        /// UNit Test to get the average file size
        /// </summary>
        [TestMethod]
        public void TestGetAverageFileSize()
        {

            long expected = 62067 / 8;

            ScanningController scan = new ScanningController();
            string path = "C:\\Users\\Ryan\\Desktop\\Reynolds_API\\UnitTestAPI\\TestFiles\\Scanning";
            long actual = scan.getAverageFileSize(path);
            Assert.AreEqual(actual, expected);


        }

    }
}
