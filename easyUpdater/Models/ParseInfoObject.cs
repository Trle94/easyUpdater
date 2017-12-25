using System;
using System.Collections.Generic;
using easyUpdater.Interfaces;

namespace easyUpdater.Models
{
    public class ParseInfoObject : IParseInfo
    {
        public ParseInfoObject(Version version = null, IList<IParsedFile> files = null,
            string description = null, string launchargs = null)
        {
            Version = version;
            Description = description;
            Files = files;
            LaunchArgs = launchargs;
        }

        /// <summary>
        ///     The update version #
        /// </summary>
        public Version Version { get; set; }


        /// <summary>
        ///     Parsed files including the Filenames and md5 hashes
        /// </summary>
        public IList<IParsedFile> Files { get; set; }


        /// <summary>
        ///     The update's description
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        ///     The arguments to pass to the updated application on startup
        /// </summary>
        public string LaunchArgs { get; set; }
    }
}
