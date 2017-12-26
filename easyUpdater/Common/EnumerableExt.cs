namespace easyUpdater.Common
{
    public static class EnumerableExt
    {
        public static string[] TrimAll(this string[] strings)
        {
            var trimmedStrings = new string[strings.Length];

            for (var i = 0; i < strings.Length; i++)
            {
                var s = strings[i].Trim();
                trimmedStrings[i] = s;
            }

            return trimmedStrings;
        }
    }
}