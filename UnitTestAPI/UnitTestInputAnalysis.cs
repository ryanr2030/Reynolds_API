using System;
using System.IO;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json;
using Reynolds_API.Controllers;
using Reynolds_API.Models;

namespace UnitTestAPI
{
    [TestClass]
    class UnitTestInputAnalysis
    {

        [TestMethod]
        public void CheckAll()
        {

            string path = "C:\\Users\\Ryan\\Desktop\\Reynolds_API\\Reynolds_API\\Files";
            string grountTruthPath = "C:\\Users\\Ryan\\Desktop\\Reynolds_API\\UnitTestAPI\\TestFiles\\InputAnalysis";
            string[] files = { "TestAllNumbers.txt", "TestAllText.txt", "TestNestedNumbers.txt", "TestNoNestedNumbers.txt" };
            string[] groundtruth = { "TestAllNumbers.json", "TestAllText.json", "TestNestedNumbers.json", "TestNoNestedNumbers.json" };


            InputAnalysisController input = new InputAnalysisController();
            for (int i = 0; i < files.Count(); i++)
            {
                FileModel expected;
                FileModel actual;

                //open uploaded data
                using (StreamReader r = new StreamReader(path + "\\" + files[i] + "\\analytics.json"))
                {
                    string json = r.ReadToEnd();
                    actual = JsonConvert.DeserializeObject<FileModel>(json);

                }

                //open groundtruth data
                using (StreamReader r = new StreamReader(grountTruthPath + "\\" + groundtruth[i]))
                {
                    string json = r.ReadToEnd();
                    expected = JsonConvert.DeserializeObject<FileModel>(json);
                }
                Assert.AreEqual(expected.fname, actual.fname);
                Assert.AreEqual(expected.allStrings, actual.allStrings);
                Assert.AreEqual(expected.RevStrings, actual.RevStrings);
                Assert.AreEqual(expected.numList, actual.numList);
                Assert.AreEqual(expected.sum, actual.sum);
                Assert.AreEqual(expected.numCount, actual.numCount);
                Assert.AreEqual(expected.avg, actual.avg);
                Assert.AreEqual(expected.median, actual.median);
                Assert.AreEqual(expected.percentNum, actual.percentNum);

            }

        }
    }

}