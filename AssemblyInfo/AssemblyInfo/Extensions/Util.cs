using System;

namespace AssemblyInfo.Extensions
{
    public static class Util
    {
        public static string BytesToString(long byteCount, int decimalPlaces = 1)
        {
            string[] suf = { "B", "KB", "MB", "GB", "TB", "PB", "EB" }; //Longs run out around EB
            if (byteCount == 0)
                return "0" + suf[0];
            var bytes = Math.Abs(byteCount);
            var place = Convert.ToInt32(Math.Floor(Math.Log(bytes, 1024)));
            var num = Math.Round(bytes / Math.Pow(1024, place), decimalPlaces);
            return (Math.Sign(byteCount) * num).ToString() + suf[place];
        }
    }
}
