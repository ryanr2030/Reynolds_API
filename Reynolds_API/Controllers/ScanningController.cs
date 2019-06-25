using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Data;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Diagnostics;
using Reynolds_API.Models;
namespace Reynolds_API.Controllers
{
    public class ScanningController : Controller
    {
        // /Scanning
        /// <summary>
        /// Reads form submission and calls the enpoints that corelate with the checkbox form data on the entered path.
        /// </summary>
        /// <returns>View</returns>
        public ActionResult Index()
        {
            DirectoryModel result = new DirectoryModel();
            
            NameValueCollection nvc = Request.Form;
            if (nvc["pathInput"] != null)
            {
                result.path = nvc["pathInput"];
                Console.WriteLine(nvc["pathInput"]);
                if (nvc["FileCount"] != null)
                {
                    result.fileCount = getNumberOfFiles(nvc["pathInput"]);
                    Console.WriteLine("File Count: " + result.fileCount+" files");

                }
                if (nvc["DirectoryCount"] != null)
                {
                    result.directoryCount = getNumberOfDirectories(nvc["pathInput"]);
                    Console.WriteLine("Subdirectory Count: " + result.directoryCount + " directories");
                }
                if (nvc["TotalFileSize"] != null)
                {
                    result.sumFileSizes = geFileSizeTotal(nvc["pathInput"]);
                    Console.WriteLine("Total File Sizes: " + result.sumFileSizes+" bytes");
                }
                if (nvc["AvgFileSize"] != null)
                {
                    result.avgFileSize = getAverageFileSize(nvc["pathInput"]);
                    Console.WriteLine("Average File Size: " + result.avgFileSize+" bytes");
                }
                return View(result);
            }
            return View();



        }

        // /Scanning/Documentation
        /// <summary>
        /// Displays the documentation page for the Scanning endpoint. 
        /// </summary>
        /// <param></param>
        /// <returns></returns>
        public ActionResult Documentation()
        {
            return View();
        }

        /// <summary>
        //Uses System.IO's GetFiles and GetDirectories function to parse the file
        /// of the input path. This method uses the SearOption.AllDirectories, which causes it to fail
        ///if it comes across any permission erorrs.        
        ////// </summary>
        /// <param name="path"></param>
        /// <param name="task"></param>
        /// <returns></returns>
        private dynamic ScanLinq(string path, string task)
        {
            //check that the directory exists
            if (!Directory.Exists(path))
            {
                Console.WriteLine("Error: Specified path does not exist.");
                return "Error: Specified path does not exist.";
            }

            //Record an instance of the file system
            DirectoryInfo dir = new System.IO.DirectoryInfo(path);

            IEnumerable<DirectoryInfo> dirList;
            IEnumerable<FileInfo> fileList;
            try
            {
                switch (task)
                {
                    case "numberOfFiles":
                        //Get a list of FIle Info objects for all files in all directories below path
                        //A single permission erorr will break this
                        fileList = dir.GetFiles("*.*", SearchOption.AllDirectories);
                        return fileList.Count();
                    case "numberOfDirectories":
                        //Get a list of Directory Info objects for all Subdirectories of the path directory
                        //A single permission erorr will break this
                        dirList = dir.GetDirectories("*", SearchOption.AllDirectories);
                        return dirList.Count();
                    case "totalOfFileSizes":
                        //Get a list of FIle Info objects for all files in all directories below path
                        //A single permission erorr will break this
                        fileList = dir.GetFiles("*.*",SearchOption.AllDirectories);
                        long totalFileSizes =
                            (from file in fileList
                                let size = GetFileLength(file)
                                select size).Sum();
                        return totalFileSizes;
                    case "avgFileSize":
                        //Get a list of FIle Info objects for all files in all directories below path
                        //A single permission erorr will break this
                        fileList = dir.GetFiles("*.*", SearchOption.AllDirectories);
                        long avgFileSize =
                            (from file in fileList
                                let size = GetFileLength(file)
                                select size).Sum() / fileList.Count();
                        return avgFileSize;
                    default:
                        Console.WriteLine("Error: Invalid task \'" + task + "\'");
                        return "Error: Invalid task \'" + task + "\'";
                }
            }
            catch (UnauthorizedAccessException e)
            {
                Console.WriteLine(e.Message);
                return "Error: "+e.Message;
            }
        }


        /// <summary>
        /// Function that exposes a public route that returns the 
        /// number of files below a specified directory
        /// </summary>
        /// <param name="path">path to directory to scan</param>
        /// <returns> Number of files unde the specified path</returns>
        public dynamic getNumberOfFiles(string path)
        {
            //try the Linq Based Scanner
            dynamic result = ScanLinq(path, "numberOfFiles");
            Type restype = result.GetType();

            //if Linq scan failed with errors call post order tree traversal
            //maintain a list of files the application does not have access too
            if (restype.Equals(typeof(string))){
               result = ScanTreeStack(path, "numberOfFiles");
            }

            //call Scanner with path and task
            return result;
        }

        /// <summary>
        /// Function that exposes a public route that returns the 
        /// number of directories below a specific path
        /// </summary>
        /// <param name="path"> path to directory to scan</param>
        /// <returns> Number of files below the specified path</returns>
        public dynamic getNumberOfDirectories(string path)
        {

            //try the Linq Based Scanner
            dynamic result = ScanLinq(path, "numberOfDirectories");
            Type restype = result.GetType();

            //if Linq scan failed with errors call post order tree traversal
            //maintain a list of files the application does not have access too
            if (restype.Equals(typeof(string)))
            {
                result = ScanTreeStack(path, "numberOfDirectories");
            }

            //call Scanner with path and task
            return result;
        }

        /// <summary>
        /// Function that exposes a public route that returns the integer
        /// sum of all file sizes in bytes below a specified directory.
        /// </summary>
        /// <param name="path"> path to directory to scan</param>
        /// <returns> Integer sum of file sizes below path dir</returns>
        public dynamic geFileSizeTotal(string path)
        {

            //try the Linq Based Scanner
            dynamic result = ScanLinq(path, "totalOfFileSizes");
            Type restype = result.GetType();

            //if Linq scan failed with errors call post order tree traversal
            //maintain a list of files the application does not have access too
            if (restype.Equals(typeof(string)))
            {
                result = ScanTreeStack(path, "totalOfFileSizes");
            }

            //call Scanner with path and task
            return result;
        }
      
        /// <summary>
        /// Function that exposes a public route that returns the integer
        /// average of all file sizes in bytes below a specified directory.
        /// </summary>
        /// <param name="path"> Path to directory to scan</param>
        /// <returns> Integer average of file sizes below path dir</returns>
        public dynamic getAverageFileSize(string path)
        {
            //try the Linq Based Scanner
            dynamic result = ScanLinq(path, "avgFileSize");
            Type restype = result.GetType();

            //if Linq scan failed with errors call post order tree traversal
            //maintain a list of files the application does not have access too
            if (restype.Equals(typeof(string)))
            {
                result = ScanTreeStack(path, "avgFileSize");
            }

            //call Scanner with path and task
            return result;
        }



        /// <summary>
        ///Executes the specified task and returns the value to the calling endpoint.
        ///Same functionality as ScanLinq except the file tree is parsed using a stack based LIFO traversal.
        ///Unauthorized files are passed over and no longer breaking with this implementation.
        /// </summary>
        /// <param name="path"></param>
        /// <param name="task"></param>
        /// <returns></returns>
    private dynamic ScanTreeStack(string path, string task)
        {
            //check that the directory exists
            if (!Directory.Exists(path))
            {
                Console.WriteLine("Error: Specified path does not exist.");
                return "Error: Specified path does not exist.";
            }
            Dictionary<string, dynamic> response;

            IEnumerable<DirectoryInfo> dirList;
            IEnumerable<FileInfo> fileList;
            try
            {
                switch (task)
                {
                    case "numberOfFiles":
                        //Get a list of FIle Info objects for all files in all directories below path
                        //A single permission erorr will break this
                        response = DirectoryStackTraversal(path, "Files");
                        fileList = response["FileList"];
                        return fileList.Count();
                    case "numberOfDirectories":
                        //Get a list of Directory Info objects for all Subdirectories of the path directory
                        //A single permission erorr will break this
                        response = DirectoryStackTraversal(path, "Directories");
                        dirList = response["DirectoryList"];
                        return dirList.Count();
                    case "totalOfFileSizes":
                        //Get a list of FIle Info objects for all files in all directories below path
                        //A single permission erorr will break this
                        response = DirectoryStackTraversal(path, "Files");
                        fileList = response["FileList"];
                        long totalFileSizes =
                            (from file in fileList
                             let size = GetFileLength(file)
                             select size).Sum();
                        return totalFileSizes;
                    case "avgFileSize":
                        //Get a list of FIle Info objects for all files in all directories below path
                        //A single permission erorr will break this
                        response = DirectoryStackTraversal(path, "Files");
                        fileList = response["FileList"];
                        long avgFileSize =
                            (from file in fileList
                             let size = GetFileLength(file)
                             select size).Sum() / fileList.Count();
                        return avgFileSize;
                    default:
                        Console.WriteLine("Error: Invalid task \'" + task + "\'");
                        return "Error: Invalid task \'" + task + "\'";
                }
            }
            catch (UnauthorizedAccessException e)
            {
                Console.WriteLine(e.Message);
                return "Error: " + e.Message;
            }
        }

        /// <summary>
        /// Uses a LIFO stack to parse the files and subdirectories 
        /// in pre-order traversal (subdirectories then files) 
        /// </summary>
        /// <param name="path">Path of the directory to begin scanning below</param>
        /// <param name="task">Tells the method what list objects to return (Files, Directories, or Both)</param>
        /// <returns> Returns an object containing the result data for the specified task and a list of unauthorized directories</returns>
        private dynamic DirectoryStackTraversal(string path, string task)
        {
            Dictionary<string, dynamic> response = new Dictionary<string, dynamic>();

            //Stack to hold the subfolders to traverse
            Stack<string> dirs = new Stack<string>();

            //list to store unauthorized reqs and not found messages
            List<string> errors = new List<string>();

            //List to store all of the files
            List<FileInfo> fileInfo = new List<FileInfo>();

            //List to store all of the directories 
            List<DirectoryInfo> dirInfo = new List<DirectoryInfo>();

            //Check that the path exists on the local filesystem
            //Redundant if ScanLinq is called first
            if (!Directory.Exists(path))
            {
                Console.WriteLine("Error: Specified path does not exist.");
                return "Error: Specified path does not exist.";
            }

            //push the search directory onto the stack
            dirs.Push(path);

            //Loop through each directory until there are no directories left on the stack
            while (dirs.Count() > 0)
            {
                //pop current directory to traverse off the top of the stack
                string curDir = dirs.Pop();
                //string array to store any possible sub directories
                string[] subDirs;
                try
                {
                    subDirs = Directory.GetDirectories(curDir);
                }
                catch (UnauthorizedAccessException e)
                {
                    Console.WriteLine(e.Message);
                    errors.Add(e.Message);
                    continue;
                }
                catch (DirectoryNotFoundException e)
                {
                    Console.WriteLine(e.Message);
                    errors.Add(e.Message);
                    continue;
                }
                if (task == "Directories" || task == "Both")
                {
                    foreach (string dir in subDirs)
                    {
                        dirInfo.Add(new DirectoryInfo(dir));
                    }
                }
                if (task == "Files" || task == "Both")
                {
                    string[] files = null;
                    try
                    {
                        files = Directory.GetFiles(curDir);
                    }
                    catch (UnauthorizedAccessException e)
                    {

                        Console.WriteLine(e.Message);
                        errors.Add(e.Message);
                        continue;
                    }

                    catch (DirectoryNotFoundException e)
                    {
                        Console.WriteLine(e.Message);
                        errors.Add(e.Message);
                        continue;
                    }

                    foreach (string file in files)
                    {
                        try
                        {
                            // Perform whatever action is required in your scenario.
                            fileInfo.Add(new FileInfo(file));
                        }
                        catch (FileNotFoundException e)
                        {
                            // If file was deleted by a separate application
                            //  or thread since the call to the Method
                            // then just continue.
                            Console.WriteLine(e.Message);
                            errors.Add(e.Message);
                            continue;
                        }
                        catch(PathTooLongException e)
                        {
                            Console.WriteLine(e.Message);
                            errors.Add(e.Message);
                        }
                    }
                }

                // Push the subdirectories onto the stack for traversal.
                // This could also be done before handing the files.
                foreach (string str in subDirs)
                    dirs.Push(str);

            }
            if (task == "Files" || task == "Both")
                response["FileList"] = fileInfo;

            if (task == "Directoy" || task == "Both")
                response["DirectoyList"] = dirInfo;
            response["ErrorList"] = errors;
            return response;
        }

        static long GetFileLength(FileInfo fi)
        {
            long retval;
            try
            {
                retval = fi.Length;
            }
            catch (FileNotFoundException)
            {
                // If a file is no longer present,  
                // just add zero bytes to the total.  
                retval = 0;
            }
            return retval;
        }


    }
}