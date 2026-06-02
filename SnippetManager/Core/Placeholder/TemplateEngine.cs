namespace SnippetManager.Core.Placeholder
{
    using System.Collections.Generic;
    using System.Text.RegularExpressions;

    public static class TemplateEngine
    {
        public static string Replace(string template, IEnumerable<PlaceholderItem> placeholders)
        {
            foreach (var placeholder in placeholders)
            {
                string pattern = $@"\[\[{Regex.Escape(placeholder.Name)}(?:=.*?)?\]\]";
                template = Regex.Replace(template, pattern, (string)(placeholder.Value ?? ""));
            }

            return template;
        }
    }
}
