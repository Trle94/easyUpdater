using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
