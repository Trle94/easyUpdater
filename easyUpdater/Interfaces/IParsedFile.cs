using System;

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