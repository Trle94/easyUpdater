using System.IO;

namespace easyUpdater.Common
{
    public static class Utilities
    {
        public static string CleanFileName(string fileName)
        {
            foreach (var c in Path.GetInvalidFileNameChars()) fileName = fileName.Replace(c, '_');
            return fileName;
        }
    }
}