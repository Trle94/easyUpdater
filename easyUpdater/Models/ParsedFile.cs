using System;
using easyUpdater.Interfaces;

namespace easyUpdater.Models
{
    public class ParsedFile : IParsedFile
    {
        public ParsedFile(string fileName = null, string md5 = null, Uri url = null)
        {
            FileName = fileName;
            Md5 = md5;
            Url = url;
        }

        /// <summary>
        ///     The file name of the binary
        ///     for use on local computer
        /// </summary>
        public string FileName { get; set; }

        /// <summary>
        ///     The MD5 of the update's binary
        /// </summary>
        public string Md5 { get; set; }

        /// <summary>
        ///     The location of the file
        /// </summary>
        public Uri Url { get; set; }

        /// <summary>
        ///     Temporary path
        /// </summary>
        public string TempPath { get; set; }
    }
}