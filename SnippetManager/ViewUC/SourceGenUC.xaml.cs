//-----------------------------------------------------------------------
// <copyright file="SourceGenUC.cs" company="Lifeprojects.de">
//     Class: SourceGenUC
//     Copyright © Lifeprojects.de 2026
// </copyright>
//
// <author>Gerhard Ahrens - Lifeprojects.de</author>
// <email>developer@lifeprojects.de</email>
// <date>18.06.2026</date>
//
// <summary>
// Template für eine neues UserControl
// </summary>
//-----------------------------------------------------------------------

namespace MinimalWPF.Beispiel
{
    using SnippetManager;
    using SnippetManager.Core;
    using SnippetManager.Core.Helper;
    using SnippetManager.Core.Placeholder;
    using SnippetManager.View;

    using System.Collections;
    using System.Collections.Specialized;
    using System.Globalization;
    using System.IO;
    using System.Reflection;
    using System.Resources;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Resources;

    /// <summary>
    /// Interaktionslogik für SourceGenUC.xaml
    /// </summary>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Performance", "CA1822:Member als statisch markieren", Justification = "<Ausstehend>")]
    public partial class SourceGenUC : UserControlBase
    {
        public SourceGenUC(ChangeViewEventArgs args) : base(typeof(SourceGenUC))

        {
            this.InitializeComponent();
            WeakEventManager<UserControl, RoutedEventArgs>.AddHandler(this, "Loaded", this.OnLoaded);

            this.CurrentCtorArgs = args;

            this.GoBackCommand = new CommandBase(commandParam => this.OnGoBack(commandParam), () => true);
            this.CreateSourceCommand = new CommandBase(commandParam => OnCreateSource(commandParam), () => true);

            this.DataContext = this;
        }

        #region Properties
        public CommandBase GoBackCommand { get; private set; }
        public CommandBase CreateSourceCommand { get; private set; }
        private ChangeViewEventArgs CurrentCtorArgs { get; set; }
        private MessageBase Message { get; } = new MessageBase();
        private string TemplatePath { get; set; } = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Template");
        #endregion Properties

        #region Windows Events

        private async void OnLoaded(object sender, RoutedEventArgs e)
        {
            if (App.EventAgg.IsSubscription<StatusEvent>() == true)
            {
                await App.EventAgg.PublishAsync(new StatusEvent("Bereit"));
            }

            if (App.EventAgg.IsSubscription<WindowsTitelEvent>() == true)
            {
                await App.EventAgg.PublishAsync(new WindowsTitelEvent("Code Template Generator"));
            }

            /*
            _ = new UsedEmbeddetSource().GetResources();
            _ = new UsedEmbeddetSource().IsResourceExist("NeuEnum");
            (string,string) file = new UsedEmbeddetSource().GetSourceFromResources("NeuEnum");
            */
        }
        #endregion Windows Events

        #region Command Events
        private async void OnGoBack(object commandParam)
        {
            if (commandParam != null && commandParam is CommandButtons button)
            {
                if (button == CommandButtons.GoBack)
                {
                    ChangeViewEventArgs args = new();
                    args.MenuButton = this.CurrentCtorArgs.FromPage;
                    args.FromPage = this.CurrentCtorArgs.MenuButton;
                    if (App.EventAgg.IsSubscription<ChangeViewEventArgs>() == true)
                    {
                        await App.EventAgg.PublishAsync(args);
                    }
                }
            }
        }

        private void OnCreateSource(object commandParam)
        {
            if (commandParam != null && commandParam is SourceTyp button)
            {
                if (button == SourceTyp.UserControlWithArgs)
                {
                    this.CreateUserControl("noargs");
                }
                else if (button == SourceTyp.UserControlWithoutArgs)
                {
                    this.CreateUserControl("withargs");
                }
                else if (button == SourceTyp.UserControlDefault)
                {
                    this.CreateUserControl("default");
                }
                else if (button == SourceTyp.Window)
                {
                    this.CreateWindow(button);
                }
                else if (button == SourceTyp.DialogWindow)
                {
                    this.CreateWindow(button);
                }
                else if (button == SourceTyp.EnumClass)
                {
                    this.CreateEnumClass();
                }
                else if (button == SourceTyp.DefaultClass)
                {
                    this.CreateDefaultClass();
                }
                else if (button == SourceTyp.ExtensionClass)
                {
                    this.CreateExtensionClass();
                }
                else if (button == SourceTyp.ExtensionBlockClass)
                {
                    this.CreateExtensionBlockClass();
                }
                else if (button == SourceTyp.InterfaceClass)
                {
                    this.CreateInterfaceClass();
                }
                else if (button == SourceTyp.RecordClass)
                {
                    this.Message.Warnung("Source Generator", "Die Funktion wurde noch nicht implementiert.");
                }
                else if (button == SourceTyp.StructClass)
                {
                    this.Message.Warnung("Source Generator", "Die Funktion wurde noch nicht implementiert.");
                }
                else if (button == SourceTyp.CustomDataTypeClass)
                {
                    this.Message.Warnung("Source Generator", "Die Funktion wurde noch nicht implementiert.");
                }
                else if (button == SourceTyp.DependencyProperty)
                {
                    this.Message.Warnung("Source Generator", "Die Funktion wurde noch nicht implementiert.");
                }
                else if (button == SourceTyp.DependencyPropertyCallback)
                {
                    this.Message.Warnung("Source Generator", "Die Funktion wurde noch nicht implementiert.");
                }
            }
        }

        #endregion Command Events

        private async void CreateUserControl(string typ)
        {
            (string, string) file = (string.Empty,string.Empty);
            if (typ.ToLower(CultureInfo.CurrentCulture).Equals("noargs", StringComparison.OrdinalIgnoreCase) == true)
            {
                file = new UsedEmbeddetSource().GetSourceFromResources("NeuUC");
            }
            else if (typ.ToLower(CultureInfo.CurrentCulture).Equals("withargs", StringComparison.OrdinalIgnoreCase) == true)
            {
                file = new UsedEmbeddetSource().GetSourceFromResources("NeuWithArgsUC");
            }
            else if (typ.ToLower(CultureInfo.CurrentCulture).Equals("default", StringComparison.OrdinalIgnoreCase) == true)
            {
                file = new UsedEmbeddetSource().GetSourceFromResources("NeuDefaultUC");
            }

            if (string.IsNullOrEmpty(file.Item1) == false || string.IsNullOrEmpty(file.Item2) == false)
            {
                StringCollection files = new StringCollection();
                string sourceContent = new ReplaceContent().Replace(file.Item1);
                List<PlaceholderItem> pl = PlaceholderService.Extract(sourceContent);
                if (pl != null && pl.Count > 0)
                {
                    DialogResponse<PlaceholderDlg> response = new DialogService<PlaceholderDlg>(pl)
                        .WithOwner(Application.Current.MainWindow)
                        .ShowDialog();
                    if (response.DialogResult == true)
                    {
                        string snippetContent = PlaceholderService.Replace(sourceContent, (List<PlaceholderItem>)response.ResponseObject);
                        string fileName = ExtractHelper.ExtractClassNames(snippetContent).FirstOrDefault();
                        string templatePath = Path.Combine(App.TemplatePath, $"{fileName}.xaml.cs");
                        File.WriteAllText(templatePath, snippetContent);
                        files.Add(templatePath);

                        sourceContent = file.Item2;
                        snippetContent = PlaceholderService.Replace(sourceContent, (List<PlaceholderItem>)response.ResponseObject);
                        templatePath = Path.Combine(App.TemplatePath, $"{fileName}.xaml");
                        File.WriteAllText(templatePath, snippetContent);
                        files.Add(templatePath);

                        /* Datei in Zwischenablage legen, damit sie in einem Explorer-Fenster mit STRG+V eingefügt werden kann. */
                        if (files.Count > 0)
                        {
                            ClipboardHelper.CutFilesToClipboard(files);

                            if (App.EventAgg.IsSubscription<StatusEvent>() == true)
                            {
                                await App.EventAgg.PublishAsync(new StatusEvent($"Snippet {Path.GetFileName(templatePath)} wurde in die Zwischenablage kopiert"));
                            }
                        }
                    }
                }
            }
            else
            {
                this.Message.Warnung("Source Generator", "Die Resource 'NeuUC / NeuWithArgsUC' wurde nicht gefunden.");
            }
        }

        private async void CreateWindow(SourceTyp styp)
        {
            (string, string) file = (string.Empty,string.Empty);
            if (styp == SourceTyp.Window)
            {
                file = new UsedEmbeddetSource().GetSourceFromResources("NeuWindow");
            }
            else if (styp == SourceTyp.DialogWindow)
            {
                file = new UsedEmbeddetSource().GetSourceFromResources("NeuDefaultWindow");
            }

            if (string.IsNullOrEmpty(file.Item1) == false || string.IsNullOrEmpty(file.Item2) == false)
            {
                StringCollection files = new StringCollection();
                string sourceContent = new ReplaceContent().Replace(file.Item1);
                List<PlaceholderItem> pl = PlaceholderService.Extract(sourceContent);
                if (pl != null && pl.Count > 0)
                {
                    DialogResponse<PlaceholderDlg> response = new DialogService<PlaceholderDlg>(pl)
                        .WithOwner(Application.Current.MainWindow)
                        .ShowDialog();
                    if (response.DialogResult == true)
                    {
                        string snippetContent = PlaceholderService.Replace(sourceContent, (List<PlaceholderItem>)response.ResponseObject);
                        string fileName = ExtractHelper.ExtractClassNames(snippetContent).FirstOrDefault();
                        string templatePath = Path.Combine(App.TemplatePath, $"{fileName}.xaml.cs");
                        File.WriteAllText(templatePath, snippetContent);
                        files.Add(templatePath);

                        sourceContent = file.Item2;
                        snippetContent = PlaceholderService.Replace(sourceContent, (List<PlaceholderItem>)response.ResponseObject);
                        templatePath = Path.Combine(App.TemplatePath, $"{fileName}.xaml");
                        File.WriteAllText(templatePath, snippetContent);
                        files.Add(templatePath);

                        /* Datei in Zwischenablage legen, damit sie in einem Explorer-Fenster mit STRG+V eingefügt werden kann. */
                        if (files.Count > 0)
                        {
                            ClipboardHelper.CutFilesToClipboard(files);

                            if (App.EventAgg.IsSubscription<StatusEvent>() == true)
                            {
                                await App.EventAgg.PublishAsync(new StatusEvent($"Snippet {Path.GetFileName(templatePath)} wurde in die Zwischenablage kopiert"));
                            }
                        }
                    }
                }
            }
            else
            {
                this.Message.Warnung("Source Generator", "Die Resource 'NeuWindow' wurde nicht gefunden.");
            }
        }

        private async void CreateEnumClass()
        {
            (string, string) file = new UsedEmbeddetSource().GetSourceFromResources("NeuEnum");
            if (string.IsNullOrEmpty(file.Item1) == false || string.IsNullOrEmpty(file.Item2) == false)
            {
                StringCollection files = new StringCollection();

                string sourceContent = new ReplaceContent().Replace(file.Item1);
                List<PlaceholderItem> pl = PlaceholderService.Extract(sourceContent);
                if (pl != null && pl.Count > 0)
                {
                    DialogResponse<PlaceholderDlg> response = new DialogService<PlaceholderDlg>(pl)
                        .WithOwner(Application.Current.MainWindow)
                        .ShowDialog();
                    if (response.DialogResult == true)
                    {
                        string snippetContent = PlaceholderService.Replace(sourceContent, (List<PlaceholderItem>)response.ResponseObject);
                        string fileName = ExtractHelper.ExtractClassNames(snippetContent).FirstOrDefault();
                        string templatePath = Path.Combine(App.TemplatePath, $"{fileName}.cs");
                        File.WriteAllText(templatePath, snippetContent);

                        /* Datei in Zwischenablage legen, damit sie in einem Explorer-Fenster mit STRG+V eingefügt werden kann. */
                        ClipboardHelper.CutFilesToClipboard(templatePath);

                        if (App.EventAgg.IsSubscription<StatusEvent>() == true)
                        {
                            await App.EventAgg.PublishAsync(new StatusEvent($"Snippet {Path.GetFileName(templatePath)} wurde in die Zwischenablage kopiert"));
                        }
                    }
                }
            }
            else
            {
                this.Message.Warnung("Source Generator", "Die Resource 'NeuEnum' wurde nicht gefunden.");
            }
        }

        private async void CreateDefaultClass()
        {
            (string, string) file = new UsedEmbeddetSource().GetSourceFromResources("NeuPublicClass");
            if (string.IsNullOrEmpty(file.Item1) == false || string.IsNullOrEmpty(file.Item2) == false)
            {
                string sourceContent = new ReplaceContent().Replace(file.Item1);
                List<PlaceholderItem> pl = PlaceholderService.Extract(sourceContent);
                if (pl != null && pl.Count > 0)
                {
                    DialogResponse<PlaceholderDlg> response = new DialogService<PlaceholderDlg>(pl)
                        .WithOwner(Application.Current.MainWindow)
                        .ShowDialog();
                    if (response.DialogResult == true)
                    {
                        string snippetContent = PlaceholderService.Replace(sourceContent, (List<PlaceholderItem>)response.ResponseObject);
                        string fileName = ExtractHelper.ExtractClassNames(snippetContent).FirstOrDefault();
                        string templatePath = Path.Combine(App.TemplatePath, $"{fileName}.cs");
                        File.WriteAllText(templatePath, snippetContent);

                        /* Datei in Zwischenablage legen, damit sie in einem Explorer-Fenster mit STRG+V eingefügt werden kann. */
                        ClipboardHelper.CutFilesToClipboard(templatePath);

                        if (App.EventAgg.IsSubscription<StatusEvent>() == true)
                        {
                            await App.EventAgg.PublishAsync(new StatusEvent($"Snippet {Path.GetFileName(templatePath)} wurde in die Zwischenablage kopiert"));
                        }
                    }
                }
            }
            else
            {
                this.Message.Warnung("Source Generator", "Die Resource 'NeuPublicClass' wurde nicht gefunden.");
            }
        }

        private async void CreateExtensionClass()
        {
            (string, string) file = new UsedEmbeddetSource().GetSourceFromResources("NeuStaticExtension");
            if (string.IsNullOrEmpty(file.Item1) == false || string.IsNullOrEmpty(file.Item2) == false)
            {
                string sourceContent = new ReplaceContent().Replace(file.Item1);
                List<PlaceholderItem> pl = PlaceholderService.Extract(sourceContent);
                if (pl != null && pl.Count > 0)
                {
                    DialogResponse<PlaceholderDlg> response = new DialogService<PlaceholderDlg>(pl)
                        .WithOwner(Application.Current.MainWindow)
                        .ShowDialog();
                    if (response.DialogResult == true)
                    {
                        string snippetContent = PlaceholderService.Replace(sourceContent, (List<PlaceholderItem>)response.ResponseObject);
                        string fileName = ExtractHelper.ExtractClassNames(snippetContent).FirstOrDefault();
                        string templatePath = Path.Combine(App.TemplatePath, $"{fileName}.cs");
                        File.WriteAllText(templatePath, snippetContent);

                        /* Datei in Zwischenablage legen, damit sie in einem Explorer-Fenster mit STRG+V eingefügt werden kann. */
                        ClipboardHelper.CutFilesToClipboard(templatePath);

                        if (App.EventAgg.IsSubscription<StatusEvent>() == true)
                        {
                            await App.EventAgg.PublishAsync(new StatusEvent($"Snippet {Path.GetFileName(templatePath)} wurde in die Zwischenablage kopiert"));
                        }
                    }
                }
            }
            else
            {
                this.Message.Warnung("Source Generator", "Die Resource 'NeuStaticExtension' wurde nicht gefunden.");
            }
        }

        private async void CreateExtensionBlockClass()
        {
            (string, string) file = new UsedEmbeddetSource().GetSourceFromResources("NeuStaticExtensionBlock");
            if (string.IsNullOrEmpty(file.Item1) == false || string.IsNullOrEmpty(file.Item2) == false)
            {
                string sourceContent = new ReplaceContent().Replace(file.Item1);
                List<PlaceholderItem> pl = PlaceholderService.Extract(sourceContent);
                if (pl != null && pl.Count > 0)
                {
                    DialogResponse<PlaceholderDlg> response = new DialogService<PlaceholderDlg>(pl)
                        .WithOwner(Application.Current.MainWindow)
                        .ShowDialog();
                    if (response.DialogResult == true)
                    {
                        string snippetContent = PlaceholderService.Replace(sourceContent, (List<PlaceholderItem>)response.ResponseObject);
                        string fileName = ExtractHelper.ExtractClassNames(snippetContent).FirstOrDefault();
                        string templatePath = Path.Combine(App.TemplatePath, $"{fileName}.cs");
                        File.WriteAllText(templatePath, snippetContent);

                        /* Datei in Zwischenablage legen, damit sie in einem Explorer-Fenster mit STRG+V eingefügt werden kann. */
                        ClipboardHelper.CutFilesToClipboard(templatePath);

                        if (App.EventAgg.IsSubscription<StatusEvent>() == true)
                        {
                            await App.EventAgg.PublishAsync(new StatusEvent($"Snippet {Path.GetFileName(templatePath)} wurde in die Zwischenablage kopiert"));
                        }
                    }
                }
            }
            else
            {
                this.Message.Warnung("Source Generator", "Die Resource 'NeuStaticExtensionBlock' wurde nicht gefunden.");
            }
        }

        private async void CreateInterfaceClass()
        {
            (string, string) file = new UsedEmbeddetSource().GetSourceFromResources("INeu");
            if (string.IsNullOrEmpty(file.Item1) == false || string.IsNullOrEmpty(file.Item2) == false)
            {
                string sourceContent = new ReplaceContent().Replace(file.Item1);
                List<PlaceholderItem> pl = PlaceholderService.Extract(sourceContent);
                if (pl != null && pl.Count > 0)
                {
                    DialogResponse<PlaceholderDlg> response = new DialogService<PlaceholderDlg>(pl)
                        .WithOwner(Application.Current.MainWindow)
                        .ShowDialog();
                    if (response.DialogResult == true)
                    {
                        string snippetContent = PlaceholderService.Replace(sourceContent, (List<PlaceholderItem>)response.ResponseObject);
                        string fileName = ExtractHelper.ExtractClassNames(snippetContent).FirstOrDefault();
                        string templatePath = Path.Combine(App.TemplatePath, $"{fileName}.cs");
                        File.WriteAllText(templatePath, snippetContent);

                        /* Datei in Zwischenablage legen, damit sie in einem Explorer-Fenster mit STRG+V eingefügt werden kann. */
                        ClipboardHelper.CutFilesToClipboard(templatePath);

                        if (App.EventAgg.IsSubscription<StatusEvent>() == true)
                        {
                            await App.EventAgg.PublishAsync(new StatusEvent($"Snippet {Path.GetFileName(templatePath)} wurde in die Zwischenablage kopiert"));
                        }
                    }
                }
            }
            else
            {
                this.Message.Warnung("Source Generator", "Die Resource 'NeuStaticExtensionBlock' wurde nicht gefunden.");
            }
        }
    }

    [System.Diagnostics.CodeAnalysis.SuppressMessage("Performance", "CA1822:Member als statisch markieren", Justification = "<Ausstehend>")]
    public class UsedEmbeddetSource
    {
        private Assembly _Assembly;
        public UsedEmbeddetSource()
        {
            this._Assembly = Assembly.GetExecutingAssembly();

            if (Directory.Exists(TemplatePath) == false)
            {
                Directory.CreateDirectory(TemplatePath);
            }
            else
            {
                foreach (string filePath in Directory.EnumerateFiles(TemplatePath))
                {
                    File.Delete(filePath);
                }
            }
        }

        public static string TemplatePath { get; private set; } = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Template");

        public string[] GetEmbeddedResources()
        {
            if (_Assembly == null)
            {
                return Array.Empty<string>();
            }

            try
            {
                string[] allResources = _Assembly.GetManifestResourceNames();
                return allResources;
            }
            catch (Exception ex)
            {
                string errorText = ex.Message;
                throw;
            }
        }

        public string[] GetResources(string filter = "resources/source/")
        {
            List<string> files = new();

            if (_Assembly == null)
            {
                return Array.Empty<string>();
            }

            try
            {
                string rootNamespace = AppDomain.CurrentDomain.FriendlyName;

                using Stream stream = _Assembly.GetManifestResourceStream($"{this._Assembly.GetName().Name}.g.resources");
                if (stream != null)
                {
                    using var reader = new ResourceReader(stream);

                    foreach (DictionaryEntry entry in reader)
                    {
                        string key = entry.Key.ToString();
                        if (key.Contains(filter, StringComparison.OrdinalIgnoreCase) == true)
                        {
                            files.Add(entry.Key.ToString());
                        }
                    }
                }

                string[] allResources = files.ToArray();
                return allResources;
            }
            catch (Exception ex)
            {
                string errorText = ex.Message;
                throw;
            }
        }

        public bool IsResourceExist(Uri resourceUri)
        {
            try
            {
                // Versucht, den Stream der Ressource abzurufen
                var resourceStream = Application.GetResourceStream(resourceUri);

                // Wenn kein Fehler auftritt und der Stream existiert
                return resourceStream != null;
            }
            catch (IOException)
            {
                // FileNotFoundException (bzw. IOException in WPF) wird geworfen, wenn die Ressource fehlt
                return false;
            }
            catch (ArgumentException)
            {
                // Tritt auf, wenn die Uri fehlerhaft oder ungültig ist
                return false;
            }
        }

        public bool IsResourceExist(string resourceName, bool isXamlCs = false)
        {
            Uri uriPath;
            try
            {
                if (isXamlCs == true)
                {
                    uriPath = new Uri($"pack://application:,,,/Resources/Source/{resourceName}.xaml.cs.source", UriKind.Absolute);
                }
                else
                {
                    uriPath = new Uri($"pack://application:,,,/Resources/Source/{resourceName}.cs.source", UriKind.Absolute);
                }

                // Versucht, den Stream der Ressource abzurufen
                var resourceStream = Application.GetResourceStream(uriPath);

                // Wenn kein Fehler auftritt und der Stream existiert
                return resourceStream != null;
            }
            catch (IOException)
            {
                // FileNotFoundException (bzw. IOException in WPF) wird geworfen, wenn die Ressource fehlt
                return false;
            }
            catch (ArgumentException)
            {
                // Tritt auf, wenn die Uri fehlerhaft oder ungültig ist
                return false;
            }
        }

        public (string, string) GetSourceFromResources(string className)
        {
            Uri uriXAMLCS;
            Uri uriXAML;
            Uri uriCS;
            string outCodeCS = string.Empty;
            string outCodeXAML = string.Empty;

            uriXAMLCS = new Uri($"pack://application:,,,/Resources/Source/{className}.xaml.cs.source", UriKind.Absolute);
            if (this.IsResourceExist(uriXAMLCS) == true)
            {
                StreamResourceInfo sri = Application.GetResourceStream(uriXAMLCS);
                using StreamReader reader = new StreamReader(sri.Stream);
                outCodeCS = reader.ReadToEnd();
            }

            uriCS = new Uri($"pack://application:,,,/Resources/Source/{className}.cs.source", UriKind.Absolute);
            if (this.IsResourceExist(uriCS) == true)
            {
                StreamResourceInfo sri = Application.GetResourceStream(uriCS);
                using StreamReader reader = new StreamReader(sri.Stream);
                outCodeCS = reader.ReadToEnd();
            }

            uriXAML = new Uri($"pack://application:,,,/Resources/Source/{className}.xaml.source", UriKind.Absolute);
            if (this.IsResourceExist(uriXAML) == true)
            {
                StreamResourceInfo sri = Application.GetResourceStream(uriXAML);
                using StreamReader reader = new StreamReader(sri.Stream);
                outCodeXAML = reader.ReadToEnd();
            }

            return (outCodeCS, outCodeXAML);
        }

        public void CreateSourceFile(string className, string newClassName)
        {
            string rootNamespace = AppDomain.CurrentDomain.FriendlyName;
            StringCollection files = new StringCollection();

            (string, string) sources = this.GetSourceFromResources(className);
            if (string.IsNullOrEmpty(sources.Item1) == false && string.IsNullOrEmpty(sources.Item2) == false)
            {
                string csFilePath = Path.Combine(TemplatePath, $"{newClassName}.xaml.cs");
                if (string.IsNullOrEmpty(sources.Item1) == false)
                {
                    string csContent = sources.Item1.Replace("[[ClassName]]", newClassName).Replace("[[RootNamespace]]", $"{rootNamespace}.Beispiel");
                    File.WriteAllText(csFilePath, csContent);
                    files.Add(csFilePath);
                }

                string xamlFilePath = Path.Combine(TemplatePath, $"{newClassName}.xaml");
                if (string.IsNullOrEmpty(sources.Item2) == false)
                {
                    string xamlContent = sources.Item2.Replace("[[ClassName]]", newClassName).Replace("[[RootNamespace]]", $"{rootNamespace}.Beispiel");
                    File.WriteAllText(xamlFilePath, xamlContent);
                    files.Add(xamlFilePath);
                }
            }
            else
            {
                string csFilePath = Path.Combine(TemplatePath, $"{className}.cs");
                string csContent = sources.Item1.Replace("[[ClassName]]", newClassName).Replace("[[RootNamespace]]", $"{rootNamespace}.Beispiel");
                File.WriteAllText(csFilePath, csContent);
                files.Add(csFilePath);
            }

            if (files.Count > 0)
            {
                ClipboardHelper.CutFilesToClipboard(files);
            }
        }

    }

    internal sealed class ReplaceContent
    {
        private const string FIRMA = "Lifeprojects.de";
        private const string FULLNAME = "Gerhard Ahrens";
        private const string EMAIL = "developer@lifeprojects.de";

        private readonly List<ReplaceValues> _replaceValues;

        public ReplaceContent()
        {
            this.Settings = App.Settings;

            this._replaceValues = new List<ReplaceValues>();
            this._replaceValues.Add(new ReplaceValues() { Placeholder = "$company$", Value = FallbackContent(App.Settings.TemplateCompany,FIRMA) });
            this._replaceValues.Add(new ReplaceValues() { Placeholder = "$Firma$", Value = FallbackContent(App.Settings.TemplateCompany,FIRMA) });
            this._replaceValues.Add(new ReplaceValues() { Placeholder = "$name$", Value = FallbackContent(App.Settings.TemplateName,FULLNAME) });
            this._replaceValues.Add(new ReplaceValues() { Placeholder = "$email$", Value = FallbackContent(App.Settings.TemplateEmail,EMAIL) });
            this._replaceValues.Add(new ReplaceValues() { Placeholder = "$year$", Value = DateTime.Now.Year.ToString(CultureInfo.CurrentCulture) });
            this._replaceValues.Add(new ReplaceValues() { Placeholder = "$date$", Value = DateTime.Now.ToString("dd.MM.yyyy", CultureInfo.CurrentCulture) });
        }

        public ApplicationSettings Settings { get; private set; }

        private static string FallbackContent(string value, string fallBack)
        {
            return string.IsNullOrEmpty(value) == true ? fallBack : value;
        }

        public string Replace(string content)
        {
            if (this._replaceValues != null && this._replaceValues.Count > 0)
            {
                foreach (ReplaceValues item in this._replaceValues)
                {
                    content = content.Replace(item.Placeholder, item.Value, StringComparison.OrdinalIgnoreCase);
                }
            }

            return content;
        }

        private sealed class ReplaceValues
        {
            public string Placeholder { get; set; }
            public string Value { get; set; }
        }
    }
}
