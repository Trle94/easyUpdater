using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace easyUpdater.Models
{
    public class File
    {
        public string url { get; set; }
        public string fileName { get; set; }
        public string md5 { get; set; }
    }

    public class Files
    {
        public List<File> file { get; set; }
    }

    public class Update
    {
        public string appID { get; set; }
        public string version { get; set; }
        public Files files { get; set; }
        public string description { get; set; }
    }

    public class EUpdate
    {
        public List<Update> update { get; set; }
    }

    public class RootObject
    {
        public EUpdate eUpdate { get; set; }
    }
}
