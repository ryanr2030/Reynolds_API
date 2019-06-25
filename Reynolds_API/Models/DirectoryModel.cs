using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Reynolds_API.Models
{
    public class DirectoryModel
    {
        //The path to the directory serving as an ID
        public string path { get; set; }

        //File Count for subdirectories
        public long fileCount { get; set; } = -1;

        //Subdirectory count
        public long directoryCount { get; set; } = -1;

        //Average size of all files in sub directory 
        public long avgFileSize { get; set; } = -1;

        //Sum of all files in sub directory
        public long sumFileSizes { get; set; } = -1;

        //Stores filenames and directories that were deleted or lacked requested permissions
        public List<string> errors { get; set; }

    }
}