using System;

namespace easyUpdater.Common
{
    public static class VersionExt
    {
        public static bool IsNewerThan(this Version version, Version assemblyVersion)
        {
            return version > assemblyVersion;
        }
    }
}