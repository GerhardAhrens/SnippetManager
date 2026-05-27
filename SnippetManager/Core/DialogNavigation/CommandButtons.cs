namespace SnippetManager.Core
{
    using System.ComponentModel;

    public enum CommandButtons
    {
        [Description("Keine Auswahl")]
        None = 0,
        [Description("Anwendung beenden")]
        AppQuit = 1,
        [Description("Startseite")]
        Home = 2,
        [Description("Hilfe")]
        Help = 3,
        [Description("Zurück zur vorherigen Seite")]
        GoBack = 4,
        [Description("XAML-Icon exportieren")]
        ExportXamlIcon = 5,
    }
}
