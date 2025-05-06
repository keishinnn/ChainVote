// File: Utilities/FormatHelpers.cs
namespace ChainVote.Utilities
{
    public static class FormatHelpers
    {
        public static string GetYearWithSuffix(string year)
        {
            if (!int.TryParse(year, out int yearNum))
                return $"{year} Year";

            string suffix = yearNum switch
            {
                1 => "st",
                2 => "nd",
                3 => "rd",
                _ => "th"
            };

            return $"{yearNum}{suffix} Year";
        }

        public static string GetSectionWithYear(string year, string section)
        {
            return $"{year}{section}";
        }
    }
}
