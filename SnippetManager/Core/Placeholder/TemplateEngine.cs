namespace SnippetManager.Core.Placeholder
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.Text.RegularExpressions;

    public static class TemplateEngine
    {
        private static readonly string pattern = new(@"\[\[(.*?)\]\]");

        public static string Replace(string template, IEnumerable<PlaceholderItem> items)
        {
            foreach (var item in items)
            {
                string replacement = ConvertValue(item);

                template = Regex.Replace(template, pattern, replacement);
            }

            return template;
        }

        private static string ConvertValue(PlaceholderItem item)
        {
            return item.Type switch
            {
                PlaceholderType.Date => ((DateTime?)item.Value)?.ToString("dd.MM.yyyy",CultureInfo.CurrentCulture) ?? "",

                PlaceholderType.Number => Convert.ToDecimal(item.Value,CultureInfo.CurrentCulture).ToString("0.##",CultureInfo.CurrentCulture),

                PlaceholderType.Boolean =>
                    ((bool?)item.Value) == true
                        ? "Ja"
                        : "Nein",

                _ => item.Value?.ToString() ?? ""
            };
        }
    }
}
