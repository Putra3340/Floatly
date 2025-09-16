using System.Text.RegularExpressions;

namespace StringExt
{
    public static partial class StringExtensions
    {
        public static bool IsNotNullOrEmpty(this string pe)
        {
            if (pe == null || pe == "")
            {
                return false;
            }
            return true;
        }
        public static bool IsNullOrEmpty(this string pe)
        {
            if (pe == null || pe == "")
            {
                return true;
            }
            return false;
        }
        public static string ExtractNumbers(this string input)
        {
            if (string.IsNullOrEmpty(input))
                return string.Empty;

            // Use Regex to find all numbers in the string
            MatchCollection matches = Regex.Matches(input, @"\d+");
            return string.Join("", matches);  // Combine all numbers into a single string
        }
    }
}
