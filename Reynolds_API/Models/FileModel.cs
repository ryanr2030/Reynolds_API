using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Reynolds_API.Models
{
    public class FileModel
    {
        //List to store the numbers from the file
        public List<double> numList { get; set; }

        //Sum of all numbers as an integer
        //(2 decimal double rounded to int)
        public int sum { get; set; }

        //Count of all the numbers in the file
        public int numCount { get; set; }

        //Average of all numbers 
        public double avg { get; set; }

        //Median of all the numbers in the list
        public double median { get; set; }

        //Percent numbers
        public double percentNum { get; set; }

        //Sorted Dictionary of all strings reversed
        public SortedDictionary<string, int> RevStrings { get; set; }

        //Sorted Dictionary of all strings in reverse alphabetical order
        public SortedDictionary<string, int> allStrings { get; set; }

        //path to the file end point
        //serving as the id
        //endpoint contains original text file and post processed data
        public string path { get; set; }

        //file name
        public string fname { get; set; }

    }
}