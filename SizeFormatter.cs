using System.Globalization;

namespace c2flux
{
    public static class SizeFormatter
    {
        private static readonly string[] Units = { "B", "KB", "MB", "GB", "TB", "PB" };

        public static string Format(long bytes)
        {
            double value = bytes;
            int unitIndex = 0;

            while (value >= 1024 && unitIndex < Units.Length - 1)
            {
                value /= 1024;
                unitIndex++;
            }

            if (unitIndex == 0)
            {
                return bytes.ToString(CultureInfo.InvariantCulture) + " " + Units[unitIndex];
            }

            return value.ToString("0.##", CultureInfo.InvariantCulture) + " " + Units[unitIndex];
        }
    }
}