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

            var regex = new Regex(
                @"(?:public|private|internal|protected)?\s*(?:static\s+)?(class|enum|struct|record)\s+([A-Za-z_][A-Za-z0-9_]*)",
                RegexOptions.Multiline);

            MatchCollection matches = regex.Matches(input);

            foreach (Match match in matches)
            {
                // Gruppe 2 enthält den Namen (Gruppe 1 ist 'class' oder 'enum')
                ergebnisse.Add(match.Groups[2].Value);
            }

            return ergebnisse;
        }
    }

    public class Test()
    {

    }
}
