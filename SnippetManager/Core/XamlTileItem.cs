namespace SnippetManager.Core
{
    using System.Diagnostics;
    using System.Windows.Media;

    [DebuggerDisplay("Tite: {this.Title}")]
    public class XamlTileItem
    {
        public string Title { get; set; }
        public DrawingImage ImageContent { get; set; }
    }
}
