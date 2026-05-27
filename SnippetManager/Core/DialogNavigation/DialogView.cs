namespace SnippetManager.Core
{
    using System.ComponentModel;

    public enum DialogView
    {
        [Description("Keine Auswahl")]
        None = 0,
        [Description("Übersicht Source Snippets")]
        SourceSnippets = 1,
        [Description("Detailansicht Source Snippets")]
        SourceSnippetsDetail = 2,
        [Description("XAML-Grafik")]
        XamlGrafik = 3,
    }
}
