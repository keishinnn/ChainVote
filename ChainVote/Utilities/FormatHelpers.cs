namespace ChainVote.Utilities
{
    public static class FormatHelpers
    {
        // Returns the given year number with an appropriate ordinal suffix (e.g., "1st Year", "2nd Year").
        public static string GetYearWithSuffix(string year)
        {
            // Attempt to parse the input string to an integer.
            if (!int.TryParse(year, out int yearNum))
                return $"{year} Year"; // If parsing fails, return the input with "Year" appended.

            // Determine the appropriate suffix based on common English ordinal rules.
            string suffix = yearNum switch
            {
                1 => "st",
                2 => "nd",
                3 => "rd",
                _ => "th"
            };

            // Return the formatted string with the suffix and "Year" appended.
            return $"{yearNum}{suffix} Year";
        }

        // Concatenates the year and section into a single string (e.g., "3A", "2B").
        public static string GetSectionWithYear(string year, string section)
        {
            // Combine the year and section directly.
            return $"{year}{section}";
        }
    }
}
