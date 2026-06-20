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
        [Description("WPF Window ohne Kontruktor Parameter")]
        Window = 3,
        [Description("WPF Dialog Window ohne Kontruktor Parameter")]
        DialogWindow = 4,
        [Description("Enum Klasse")]
        EnumClass = 5,
        [Description("Default Klasse")]
        DefaultClass = 6,
    }
}
