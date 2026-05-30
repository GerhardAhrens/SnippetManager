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
            return pattern.Matches(text)
                .Select(m => m.Groups[1].Value)
                .Distinct()
                .Select(name => new PlaceholderItem
                {
                    Name = name
                })
                .ToList();
        }

        public static string ReplacePlaceholders(string text, IEnumerable<PlaceholderItem> items)
        {
            foreach (var item in items)
            {
                text = text.Replace($"{item.Name}", item.Value.ToString() ?? string.Empty ,StringComparison.OrdinalIgnoreCase);
            }

            return text;
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
