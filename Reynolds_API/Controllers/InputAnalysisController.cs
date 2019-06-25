using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Web;
using System.Web.Mvc;
using Reynolds_API.Models;
using System.Text;
using MiscUtil.Collections;
using Newtonsoft.Json;
using System.Diagnostics;
using System.Text.RegularExpressions;

namespace Reynolds_API.Controllers
{
    /* Controller to handle requests for Input and Analysis*/
    public class InputAnalysisController : Controller
    {

        // /InputAnalysis
        // /InputAnalysis/index
        /// <summary>
        ///Takes in posted files and calls the /InputAnalysis/Upload enpoint. Displays the resulting list of FileModels and their 
        ///values to the index if the upload is successfull.
        /// </summary>
        /// <param name="fileInput"></param>
        /// <returns>ActionResult</returns>
        public ActionResult Index(HttpPostedFileBase fileInput)
        {
            JsonResult result = Upload(fileInput);
            return View(result.Data);
        }

        // /InputAnalysis/Documentation
        /// <summary>
        /// Displays the documentation page for the Input and Analysis endpoint. 
        /// </summary>
        /// <param name="fileInput"></param>
        /// <returns></returns>
        public ActionResult Documentation()
        {
            return View();
        }

        // /InputpAnalysis/containsString
        /// <summary>
        /// Checks each files analytics.json allStrings Dictionary against an input string to see if the file contains it.
        /// Returns a json response with a boolean whether it contains the word, the number of occurences, and the filename. Note this 
        /// checks for individual word occurences.An additional endpoint is needed for line occurences.
        /// </summary>
        /// <param name="lookupString"></param>
        /// <param name="filenames"></param>
        /// <returns> Json result Boolean, Filename, and Number of Occurences</returns>
        public JsonResult ContainsString(string lookupString, string[] filenames)
        {

            List<object> responses = new List<object>();
            if (lookupString != null)
            {
                if (filenames != null)
                {
                    foreach (string file in filenames)
                    {
                        try
                        {
                            Dictionary<string, string> response = new Dictionary<string, string>();
                            string path = Server.MapPath("~") + "Files\\" + file;
                            using (StreamReader r = new StreamReader(path + "\\analytics.json"))
                            {
                                int count = 0;
                                string json = r.ReadToEnd();
                                FileModel fmodel = JsonConvert.DeserializeObject<FileModel>(json);
                                response["fname"] = fmodel.fname;
                                response["lookupString"] = lookupString;
                                response["isContained"] = fmodel.allStrings.TryGetValue(lookupString, out count).ToString() ;
                                response["count"] = count.ToString();
                            }
                            responses.Add(response);
                        }
                        catch (IOException e)
                        {
                            Dictionary<string, string> response = new Dictionary<string, string>();
                            response["Error"] = "filename: " + file + " not found.";
                            responses.Add(response);
                            Console.WriteLine(response["Error"]);
                            Console.WriteLine(e.Message);
                        }
                    }
                    return Json(responses, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    try
                    {   // Open the recent uploads cache
                        string lastuploads;
                        using (StreamReader sr = new StreamReader(Server.MapPath("~") + "Files\\lastupload.txt"))
                        {
                            lastuploads = sr.ReadToEnd();
                        }
                        filenames = lastuploads.ToString().Split(
                             new[] { Environment.NewLine },
                             StringSplitOptions.RemoveEmptyEntries
                             );
                        foreach (string file in filenames)
                        {
                            try
                            {
                                Dictionary<string, string> response = new Dictionary<string, string>();
                                using (StreamReader r = new StreamReader(file + "\\analytics.json"))
                                {
                                    string json = r.ReadToEnd();
                                    FileModel fmodel = JsonConvert.DeserializeObject<FileModel>(json);
                                    int count = 0;
                                    response["fname"] = fmodel.fname;
                                    response["lookupString"] = lookupString;
                                    response["isContained"] = fmodel.allStrings.TryGetValue(lookupString, out count).ToString();
                                    response["count"] = count.ToString();
                                }
                                responses.Add(response);
                            }
                            catch (IOException e)
                            {
                                Dictionary<string, string> response = new Dictionary<string, string>();
                                //TODO: get a substring after last occurence of \
                                response["Error"] = "filename not found.";
                                responses.Add(response);
                                Console.WriteLine(response["Error"]);
                                Console.WriteLine(e.Message);
                            }
                        }
                        return Json(responses, JsonRequestBehavior.AllowGet);
                    }
                    catch (IOException e)
                    {
                        responses.Add("Error: no files uploaded please upload and retry.");
                        Console.WriteLine("No files in the recent upload cache.");
                        Console.WriteLine(e.Message);
                        return Json(responses, JsonRequestBehavior.AllowGet);
                    }
                }
            }
            return Json("Error: No lookup String Specified. Please input a string to lookup.", JsonRequestBehavior.AllowGet);
        }


        // InputAnalysis/getSum/filenames
        /// <summary>
        ///Retrieves the sum of all numbers stored as the 'sum' variable in the analytics.json file.
        ///Returns a json response with the filename and the sum value.
        /// </summary>
        /// <param name="filenames"> Array of filenames to search</param>
        /// <returns> Json response with the sum</returns>
       [HttpGet]
        public JsonResult getSum(string[] filenames)
        {
            List<object> responses = new List<object>();
            if (filenames != null)
            {
                foreach (string file in filenames)
                {
                    try
                    {
                        Dictionary<string, string> response = new Dictionary<string, string>();
                        string path = Server.MapPath("~") + "Files\\" + file;
                        using (StreamReader r = new StreamReader(path + "\\analytics.json"))
                        {
                            string json = r.ReadToEnd();
                            FileModel fmodel = JsonConvert.DeserializeObject<FileModel>(json);
                            response["fname"] = fmodel.fname;
                            response["sum"] = fmodel.sum.ToString();
                        }
                        responses.Add(response);
                    }
                    catch (IOException e)
                    {
                        Dictionary<string, string> response = new Dictionary<string, string>();
                        response["Error"] = "filename: " + file + " not found.";
                        responses.Add(response);
                        Console.WriteLine(response["Error"]);
                        Console.WriteLine(e.Message);
                    }
                }
                return Json(responses, JsonRequestBehavior.AllowGet);
            }
            else
            {
                try
                {   // Open the recent uploads cache
                    string lastuploads;
                    using (StreamReader sr = new StreamReader(Server.MapPath("~") + "Files\\lastupload.txt"))
                    {
                        lastuploads = sr.ReadToEnd();
                    }
                   filenames = lastuploads.ToString().Split(
                        new[] { Environment.NewLine },
                        StringSplitOptions.RemoveEmptyEntries
                        );
                    foreach (string file in filenames)
                    {
                        try
                        {
                            Dictionary<string, string> response = new Dictionary<string, string>();
                            using (StreamReader r = new StreamReader(file + "\\analytics.json"))
                            {
                                string json = r.ReadToEnd();
                                FileModel fmodel = JsonConvert.DeserializeObject<FileModel>(json);
                                response["fname"] = fmodel.fname;
                                response["sum"] = fmodel.sum.ToString();
                            }
                            responses.Add(response);
                        }
                        catch (IOException e)
                        {
                            Dictionary<string, string> response = new Dictionary<string, string>();
                            //TODO: get a substring after last occurence of \
                            response["Error"] = "filename not found.";
                            responses.Add(response);
                            Console.WriteLine(response["Error"]);
                            Console.WriteLine(e.Message);
                        }
                    }
                    return Json(responses, JsonRequestBehavior.AllowGet);
                }
                catch (IOException e)
                {
                    responses.Add("Error: no files uploaded please upload and retry.");
                    Console.WriteLine("No files in the recent upload cache.");
                    Console.WriteLine(e.Message);
                    return Json(responses, JsonRequestBehavior.AllowGet);

                }

            }

        }



        // InputAnalysis/getNumCount/

        /// <summary>
        ///Retrieves the count of numbers stored as the 'numCount' in the analytics.json file.
        ///Returns a json response with the file name and count of numbers.
        ///Defaults to the last uploaded file(s) if no input name is given.
        /// </summary>
        /// <param name="filenames"> Array of strings containing filenames to lookup</param>
        /// <returns> Json response with the count of numbers in the file</returns>
        public JsonResult getNumCount(string[] filenames)
        {
            List<object> responses = new List<object>();
            if (filenames != null)
            {
                foreach (string file in filenames)
                {
                    try
                    {
                        Dictionary<string, string> response = new Dictionary<string, string>();
                        string path = Server.MapPath("~") + "Files\\" + file;
                        using (StreamReader r = new StreamReader(path + "\\analytics.json"))
                        {
                            string json = r.ReadToEnd();
                            FileModel fmodel = JsonConvert.DeserializeObject<FileModel>(json);
                            response["fname"] = fmodel.fname;
                            response["NumCount"] = fmodel.numCount.ToString();
                            responses.Add(response);
                        }
                    }
                    catch (IOException e)
                    {
                        Dictionary<string, string> response = new Dictionary<string, string>();
                        response["Error"] = "filename: " + file + " not found.";
                        responses.Add(response);
                        Console.WriteLine(response["Error"]);
                        Console.WriteLine(e.Message);
                    }
                }
                return Json(responses, JsonRequestBehavior.AllowGet);
            }
            else
            {
                try
                {   // Open the recent uploads cache
                    string lastuploads;
                    using (StreamReader sr = new StreamReader(Server.MapPath("~") + "Files\\lastupload.txt"))
                    {
                        lastuploads = sr.ReadToEnd();
                    }
                    filenames = lastuploads.ToString().Split(
                         new[] { Environment.NewLine },
                         StringSplitOptions.RemoveEmptyEntries
                         );
                    foreach (string file in filenames)
                    {
                        try
                        {
                            Dictionary<string, string> response = new Dictionary<string, string>();
                            using (StreamReader r = new StreamReader(file + "\\analytics.json"))
                            {
                                string json = r.ReadToEnd();
                                FileModel fmodel = JsonConvert.DeserializeObject<FileModel>(json);
                                response["fname"] = fmodel.fname;
                                response["NumCount"] = fmodel.numCount.ToString();
                            }
                            responses.Add(response);
                        }
                        catch (IOException e)
                        {
                            Dictionary<string, string> response = new Dictionary<string, string>();
                            //TODO: get a substring after last occurence of \
                            response["Error"] = "filename not found.";
                            responses.Add(response);
                            Console.WriteLine(response["Error"]);
                            Console.WriteLine(e.Message);
                        }
                    }
                    return Json(responses, JsonRequestBehavior.AllowGet);
                }
                catch (IOException e)
                {
                    responses.Add("Error: No files uploaded please upload and retry.");
                    Console.WriteLine("Error: No files uploaded please upload and retry.");
                    Console.WriteLine(e.Message);
                    return Json(responses, JsonRequestBehavior.AllowGet);

                }
            }

        }

        /// <summary>
        ///Takes in Http file inputs. Calls parseFiles() to parse the files and return FileModel Objects for each file.
        ///Each FileModel is saved as a json file in an endpoint /Files/filename/analytics.json.
        ///   Each original input file is also saved in memory under /Files/filename/filename.fileextension.
        ///  A list of FileModels is returned to be consumed by /InputAnalysis/index or another client. 
        ///  See parseFiles() for more information on how the FileModels are populated.
        /// </summary>
        /// <param name="fileInput"> Http Files attached via a post request</param>
        /// <returns> Json Result with a List of FileModels</returns>
        [HttpPost]
        public JsonResult Upload(HttpPostedFileBase fileInput)
        {
            List<FileModel> fileList = new List<FileModel>();

            //Try catch to let the user know if the upload was successful
            try
            {
                //Server side validation to make sure a file was input and it is not empty
                if ( fileInput!=null && fileInput.ContentLength > 0)
                {

                    
                    //Retrieve the Collection of Files uploaded from Client
                    HttpFileCollectionBase files = Request.Files;

                    //List of file data
                    //iterate through each file
                    for (int i = 0; i<files.Count; i++)
                    {
                        //retrieve an individual file object
                        HttpPostedFileBase file = files[i];
                       
                        //create a unique path based on the filename
                        string path = Server.MapPath("~") + "Files\\" + Path.GetFileNameWithoutExtension(file.FileName);
                        
                        // Create a directory for the filename if it does not already exist
                        if (!Directory.Exists(path))
                        {
                            Directory.CreateDirectory(path);
                        }

                        //save the file in memory
                        //could remove 
                        file.SaveAs(path + "\\" + file.FileName);

                        //parse the file and extract the information
                        FileModel fmodel = parseFiles(file, path);
                        fileList.Add(fmodel);

                        //save the analytical data as a json
                        string fileJson = JsonConvert.SerializeObject(fmodel, Formatting.Indented);
                        System.IO.File.WriteAllText(path + "\\analytics.json", fileJson);
                    }

                    //Write each files path into the last upload cache
                    using (StreamWriter writer = new StreamWriter(Server.MapPath("~") + "Files\\lastupload.txt", false))
                    {
                        foreach (FileModel fmode in fileList){
                            writer.WriteLine(fmode.path);
                        }
                    }
                    return Json(fileList, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    Console.WriteLine("Error: File Content Empty.");
                    return Json(fileList, JsonRequestBehavior.AllowGet);
                }

            }
            catch
            {
                Console.WriteLine("Error: File failed to upload.");
                return Json(fileList, JsonRequestBehavior.AllowGet);
            }
        }

        /// <summary>
        ///Loads a posted file into a byte array.Each line is encoded into a UTF8 string as lowercase.
        /// The string is split into a word array and a number array using two regexes.Nested numbers are extracted from strings
        /// Example: "you32.1 \n hello world\n 28.1" The numbers extracted will be 28.1 and 32.1. The word occurences would be you:1, world:1, hello:1
        /// </summary>
        /// <param name="file"> Inputs a single Post File</param>
        /// <param name="path"> path of the file on the server</param>
        /// <returns> Returns an populated FileModel Object</returns>
        private FileModel parseFiles(HttpPostedFileBase file, string path) {

            //string builder containing input from the file stream
            StringBuilder result = new StringBuilder();
            List<string> lines = new List<string>();
            List<double> numbers = new List<double>();
            SortedDictionary<string, int> words = new SortedDictionary<string, int>(new RevComparer<string>(StringComparer.InvariantCulture));
            SortedDictionary<string, int> revWords;
            int wordcount = 0;
            double sum=0;
            var fmodel = new FileModel() { fname = file.FileName, path = path };

            string numPattern = @"[-+]?[0-9]*\.?[0-9]+(?:[eE][-+]?[0-9]+)?";
            string wordPattern = @"[a-zA-Z]+";
            Regex regex = new Regex(numPattern);

            //Convert the file stream to byte array then encode to UTF8 String
            //then add to string builder
            using (BinaryReader b = new BinaryReader(file.InputStream))
            {
                byte[] binData = b.ReadBytes(file.ContentLength);

                //gets a single line string, parses to lower case

                string tmp = Encoding.UTF8.GetString(binData).ToLower();
                
   

                MatchCollection matches = Regex.Matches(tmp, numPattern);
                foreach (Match match in matches)
                {
                    double num = double.Parse(match.Value);
                    sum += num;
                    numbers.Add(num);
                }

                matches = Regex.Matches(tmp, wordPattern);
                foreach(Match match in matches){
                    if (words.ContainsKey(match.Value))
                        words[match.Value] += 1;
                    else
                        words[match.Value] = 1;
                     wordcount += 1;
                }

            }


            //sort the list of numbers (ascending)
            numbers.Sort();
            revWords = new SortedDictionary<string, int>(words);

            //set the model values: 
            fmodel.avg = Math.Round(sum / numbers.Count(), 2);
            fmodel.numList = numbers;
            fmodel.median = Math.Round(median(numbers),2);
            fmodel.allStrings = words;
            fmodel.RevStrings = reverseStrings(revWords);
            fmodel.sum = (int)Math.Round(sum,0);
            fmodel.numCount = numbers.Count();
            fmodel.percentNum = Math.Round((100.0*numbers.Count()) / (numbers.Count() + wordcount), 2);

            //Write Values to standard Out
            Console.WriteLine("Sum: "+ fmodel.sum);
            Console.WriteLine("Average: " + fmodel.avg);
            Console.WriteLine("Median: " + fmodel.median);
            Console.WriteLine("Percent Numbers: " + fmodel.percentNum);
            Console.WriteLine("Word Occurences in Reverse Alphabetical Order: " + fmodel.percentNum);
            foreach (KeyValuePair<string, int> kvp in fmodel.allStrings)
            {
                Console.Write(kvp.Key + ":" + kvp.Value + ", ");
            }

            return fmodel;
        }


        //Method to calculate the median from a list of numbers
        private double median(List<double> numbers)
        {
            if (numbers.Count > 0)
            {
                int index = ((numbers.Count() + 1) / 2) - 1;
                if (numbers.Count() % 2 == 0)
                {
                    return (numbers[index + 1] + numbers[index]) / 2;
                }
                return numbers[index];
            }
            return 0;
        }

        //Method to update the key value of a dictionary
        private SortedDictionary<string, int> reverseStrings(SortedDictionary<string, int> words)
        {
            var keyarray = words.Keys.ToArray();
            foreach (string key in keyarray)
            {
                if (key.Count() == 1)
                    continue;
                words[ReverseWord(key)] = words[key];
                words.Remove(key);
            }
            return words;
        }

        //Method to reverse individual chars in strings
        private string ReverseWord(string word)
        {
            char[] charArray = word.ToCharArray();
            for(int i=0, j=charArray.Count()-1; i<j; j--, i++)
            {
                char tmp = charArray[i];
                charArray[i] = charArray[j];
                charArray[j] = tmp;
            }
            return new string(charArray);
        }
    }
}


