namespace SnippetManager.Core.Helper
{
    using System.Collections.Generic;
    using System.Text.RegularExpressions;

    public static class ExtractHelper
    {
        public static List<string> ExtractClassNames(string input)
        {
            if (string.IsNullOrEmpty(input))
            {
                return new List<string>();
            }

            List<string> ergebnisse = new List<string>();

            // Der reguläre Ausdruck sucht nach 'class' oder 'enum', 
            // ignoriert Leerzeichen und fängt das Wort danach ab.
            string pattern = @"\b(class|enum)\s+(\w+)\s*\{";

            MatchCollection matches = Regex.Matches(input, pattern);

            foreach (Match match in matches)
            {
                // Gruppe 2 enthält den Namen (Gruppe 1 ist 'class' oder 'enum')
                ergebnisse.Add(match.Groups[2].Value);
            }

            return ergebnisse;
        }
    }
}
