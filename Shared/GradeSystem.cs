namespace GradeConverter.Shared
{
    using System.Linq;
    using System.Text.RegularExpressions;
    public class GradeSystem
    {
        public string abbreviatedName { get; set; }

        public Grade[] grade { get; set; }

        public string mergeOverridenBy { get; set; }

        public string parseFormatA { get; set; }

        public string type { get; set; }

        private Regex FormatARegex => string.IsNullOrWhiteSpace(parseFormatA) ? null : new Regex($"^{parseFormatA}$");

        public bool IsGradeLegal(string grade)
        {
            if (string.IsNullOrWhiteSpace(grade)) {
                return false;
            }

            return (FormatARegex?.IsMatch(grade) ?? false);
                  
        }

        public int priority { get; set; }

    }
}