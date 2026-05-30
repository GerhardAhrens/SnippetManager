namespace SnippetManager.Core.Placeholder
{
    using System.Collections.ObjectModel;

    public class PlaceholderItem
    {
        public string Name { get; set; }

        public PlaceholderType Type { get; set; }

        public object Value { get; set; }

        // Für ComboBox
        public ObservableCollection<string> Options { get; set; } = new();
    }
}
