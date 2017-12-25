using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace easyUpdater.Interfaces
{
    public interface IParsedFile
    {
        string FileName { get; }
        string Md5 { get; }
        Uri Url { get; }

        string TempPath { get; set; }
    }
}
