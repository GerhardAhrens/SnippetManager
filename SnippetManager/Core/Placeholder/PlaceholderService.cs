namespace SnippetManager.Core.Placeholder
{
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Text.RegularExpressions;

    public static class PlaceholderService
    {
        private static readonly Regex pattern = new(@"\[\[(.*?)\]\]");

        public static List<PlaceholderItem> Extract(string text)
        {
            var result = new List<PlaceholderItem>();

            foreach (Match match in pattern.Matches(text))
            {
                string content = match.Groups[1].Value.Trim();

                string name;
                string defaultValue = "";

                int index = content.IndexOf('=');

                if (index >= 0)
                {
                    name = content[..index].Trim();
                    defaultValue = content[(index + 1)..].Trim();
                }
                else
                {
                    name = content;
                }

                if (result.Any(x => x.Name == name))
                    continue;

                result.Add(new PlaceholderItem
                {
                    Name = name,
                    DefaultValue = defaultValue,
                    Value = defaultValue
                });
            }

            return result;
        }

        public static string ReplacePlaceholders(string text, IEnumerable<PlaceholderItem> items)
        {
            foreach (var item in items)
            {
                text = text.Replace($"{item.Name}", item.Value.ToString() ?? string.Empty ,StringComparison.OrdinalIgnoreCase);
            }

            return text.Trim().Replace("[[", string.Empty).Replace("]]", string.Empty);
        }

        public static string Replace(string template, IEnumerable<PlaceholderItem> placeholders)
        {
            foreach (var placeholder in placeholders)
            {
                string pattern = $@"\[\[{Regex.Escape(placeholder.Name)}(?:=.*?)?\]\]";

                template = Regex.Replace(template, pattern, (string)(placeholder.Value ?? ""));
            }

            return template;
        }

        public static List<PlaceholderItem> ExtractByTyp(string text)
        {
            var result = new List<PlaceholderItem>();

            foreach (Match match in pattern.Matches(text))
            {
                string content = match.Groups[1].Value.Trim();

                var item = ParseByTyp(content);

                if (result.All(x => x.Name != item.Name))
                {
                    result.Add(item);
                }
            }

            return result;
        }

        private static PlaceholderItem ParseByTyp(string content)
        {
            // Name:Type
            var parts = content.Split(':', 2);

            string name = parts[0];

            if (parts.Length == 1)
            {
                return new PlaceholderItem
                {
                    Name = name,
                    Type = PlaceholderType.Text
                };
            }

            string typePart = parts[1];

            // Selection(...)
            if (typePart.StartsWith("Selection(", StringComparison.OrdinalIgnoreCase))
            {
                string values = typePart.Replace("Selection(", "").Replace(")", "");

                return new PlaceholderItem
                {
                    Name = name,
                    Type = PlaceholderType.Selection,
                    Options = new ObservableCollection<string>(values.Split('|'))
                };
            }

            return new PlaceholderItem
            {
                Name = name,
                Type = Enum.Parse<PlaceholderType>(typePart)
            };
        }
    }
}
