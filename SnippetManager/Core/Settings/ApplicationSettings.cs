namespace SnippetManager.Core
{
    using System.Windows;

    public sealed class ApplicationSettings : SettingsBase
    {
        public string Username { get; set; }
        public DateTime LastAccess { get; set; }
        public bool QuestionExit { get; set; }
        public bool SaveAndClose { get; set; }
        public string TemplateCompany { get; set; }
        public string TemplateName { get; set; }
        public string TemplateEmail { get; set; }
        public string DefaultClassName { get; set; }
        public string DefaultEnumName { get; set; }
    }
}
