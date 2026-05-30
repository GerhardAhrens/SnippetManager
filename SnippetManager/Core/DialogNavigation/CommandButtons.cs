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
        [Description("Neuer Eintrag")]
        NewEntry = 6,
        [Description("Eintrag löschen")]
        DeleteEntry = 7,
        [Description("Eintrag kopieren")]
        CopyEntry = 8,
        [Description("Eintrag in die Zwischenablage kopieren")]
        CopyEntryToClipboard = 9,
        [Description("Eintrag speichern")]
        SaveEntry = 10,
        [Description("Als Snippet kopieren")]
        CopyAsSnippet = 11,
        [Description("Als Datei kopieren")]
        CopyAsFile = 12,
    }
}
