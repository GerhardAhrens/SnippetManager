namespace SnippetManager.Core
{
    using System.ComponentModel;

    public enum SourceTyp
    {
        [Description("Keine Auswahl")]
        None = 0,
        [Description("UserControl mit Kontruktor Parameter")]
        UserControlWithArgs = 1,
        [Description("UserControl ohne Kontruktor Parameter")]
        UserControlWithoutArgs = 2,
    }
}
