namespace SnippetManager.View
{
    using System.ComponentModel;
    using System.Data;
    using System.Data.SQLite;
    using System.Globalization;
    using System.IO;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Data;

    using SnippetManager.Core;
    using SnippetManager.Core.Helper;
    using SnippetManager.Core.Placeholder;
    using SnippetManager.Data;

    /// <summary>
    /// Interaktionslogik für SourceSnippetsDetailUC.xaml
    /// </summary>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Performance", "CA1822:Member als statisch markieren", Justification = "<Ausstehend>")]
    public partial class SourceSnippetsDetailUC : UserControlBase
    {
        public SourceSnippetsDetailUC(ChangeViewEventArgs args) : base(typeof(SourceSnippetsDetailUC))
        {
            this.InitializeComponent();
            WeakEventManager<UserControl, RoutedEventArgs>.AddHandler(this, "Loaded", this.OnLoaded);

            this.CurrentCtorArgs = args;

            this.GoBackCommand = new CommandBase(commandParam => this.OnGoBack(commandParam), () => true);
            this.SaveEntryCommand = new CommandBase(commandParam => this.OnSaveEntry(commandParam), () => true);
            this.CopyAsFileCommand = new CommandBase(commandParam => this.OnCopyAsFile(commandParam), () => true);
            this.CopyAsSnippetCommand = new CommandBase(commandParam => this.OnCopyAsSnippet(commandParam), () => true);
        }


        #region Properties
        public CommandBase GoBackCommand { get; private set; }
        public CommandBase SaveEntryCommand { get; private set; }
        public CommandBase CopyAsSnippetCommand { get; private set; }
        public CommandBase CopyAsFileCommand { get; private set; }
        private ChangeViewEventArgs CurrentCtorArgs { get; set; }

        public List<string> GruppenSource
        {
            get => base.GetValue<List<string>>();
            set => base.SetValue(value);
        }

        public string SelectedGruppe
        {
            get => base.GetValue<string>();
            set => base.SetValue(value);
        }

        public List<string> SnippetTypSource
        {
            get => base.GetValue<List<string>>();
            set => base.SetValue(value);
        }

        public string SelectedSnippetTyp
        {
            get => base.GetValue<string>();
            set => base.SetValue(value);
        }

        public List<string> ProjektSource
        {
            get => base.GetValue<List<string>>();
            set => base.SetValue(value);
        }
        
        public string SelectedProjekt
        {
            get => base.GetValue<string>();
            set => base.SetValue(value);
        }

        public string Titel
        {
            get => base.GetValue<string>();
            set => base.SetValue(value);
        }

        public string Beschreibung
        {
            get => base.GetValue<string>();
            set => base.SetValue(value);
        }

        public string SnippetContent
        {
            get => base.GetValue<string>();
            set => base.SetValue(value);
        }

        private MessageBase Message { get; } = new MessageBase();
        #endregion Properties

        #region Windows Events
        private async void OnLoaded(object sender, RoutedEventArgs e)
        {
            this.GruppenFilter.AddHandler(System.Windows.Controls.Primitives.TextBoxBase.TextChangedEvent, new System.Windows.Controls.TextChangedEventHandler(OnComboBoxTextChanged));
            this.TxtBeschreibung.TextChanged += (s, args) =>
            {
                this.TxtBeschreibung.ScrollToEnd();
            };

            this.DataContext = this;

            if (App.EventAgg.IsSubscription<WindowsTitelEvent>() == true)
            {
                await App.EventAgg.PublishAsync(new WindowsTitelEvent(DialogView.SourceSnippetsDetail.ToDescription()));
            }

            this.GruppenSource = new();
            this.GruppenSource.Add("Allgemein");
            this.GruppenSource.Add("Web");
            this.GruppenSource.Add("C#");
            this.GruppenSource.Add("WPF");
            this.GruppenSource.Add("RegEx");

            this.SnippetTypSource = new();
            this.SnippetTypSource.Add("Snippet");
            this.SnippetTypSource.Add("File");

            this.ProjektSource = new();
            this.ProjektSource.Add("MinimalWPF");

            if (this.CurrentCtorArgs.EntityId == Guid.Empty)
            {
                this.SelectedSnippetTyp = this.SnippetTypSource.FirstOrDefault();
            }
            else
            {
                Guid snippetId = this.CurrentCtorArgs.EntityId;
                using (DatabaseService ds = new DatabaseService(App.DatabasePath))
                {
                    SQLiteConnection connection = ds.OpenConnection();
                    string sqlSelect = $"SELECT * FROM TAB_Snippet WHERE Id = '{snippetId}'";
                    DataRow row = connection.RecordSet<DataRow>(sqlSelect).Get().Result;
                    if (row != null)
                    {
                        this.SelectedGruppe = row["Gruppe"].ToString();
                        this.SelectedSnippetTyp = row["SnippetTyp"].ToString();
                        this.Titel = row["Titel"].ToString();
                        this.Beschreibung = row["Beschreibung"].ToString();
                        this.SnippetContent = row["SnippetContent"].ToString();
                        this.SelectedProjekt = row["Projekt"].ToString();
                    }
                }
            }

            if (App.EventAgg.IsSubscription<StatusEvent>() == true)
            {
                await App.EventAgg.PublishAsync(new StatusEvent("Bereit"));
            }
        }

        private void OnComboBoxTextChanged(object sender, TextChangedEventArgs e)
        {
            if (sender is ComboBox comboBox)
            {
                // Suchtext aus der ComboBox auslesen
                string searchText = comboBox.Text;

                // Standard-View der ItemsSource holen
                ICollectionView view = CollectionViewSource.GetDefaultView(comboBox.ItemsSource);

                if (view == null) return;

                if (string.IsNullOrEmpty(searchText))
                {
                    // Wenn kein Text eingegeben wurde, Filter zurücksetzen
                    view.Filter = null;
                }
                else
                {
                    // Filter-Logik anwenden (Groß-/Kleinschreibung ignorieren)
                    view.Filter = item =>
                    {
                        string currentItem = item as string;
                        return currentItem != null &&
                               currentItem.ToLower(CultureInfo.CurrentCulture).Contains(searchText.ToLower(CultureInfo.CurrentCulture), StringComparison.OrdinalIgnoreCase);
                    };
                }

                // Dropdown automatisch öffnen, während der Nutzer tippt
                if (comboBox.Text.Length < 2)
                {
                    comboBox.IsDropDownOpen = true;
                }
            }
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

        private async void OnSaveEntry(object commandParam)
        {
            if (this.CurrentCtorArgs.EntityId == Guid.Empty)
            {
                SnippetItem snippet = new()
                {
                    Id = Guid.NewGuid(),
                    Gruppe = this.SelectedGruppe,
                    SnippetTyp = this.SelectedSnippetTyp,
                    Titel = this.Titel,
                    Beschreibung = this.Beschreibung,
                    SnippetContent = this.SnippetContent,
                    Projekt = this.SelectedProjekt,
                    CreatedOn = DateTime.Now,
                    CreatedBy = Environment.UserName,
                };

                if (snippet.IsValid() == false)
                {
                    this.Message.Hinweis("Fehlerhafte Eingabe", "Bitte füllen Sie alle Pflichtfelder aus, bevor Sie den Eintrag speichern können.", true);
                    return;
                }

                using (DatabaseService ds = new DatabaseService(App.DatabasePath))
                {
                    ds.Insert(this.InsertSnippet, snippet);
                }
            }
            else
            {
                SnippetItem snippet = new()
                {
                    Id = this.CurrentCtorArgs.EntityId,
                    Gruppe = this.SelectedGruppe,
                    SnippetTyp = this.SelectedSnippetTyp,
                    Titel = this.Titel,
                    Beschreibung = this.Beschreibung,
                    SnippetContent = this.SnippetContent,
                    Projekt = this.SelectedProjekt,
                    CreatedOn = DateTime.Now,
                    CreatedBy = Environment.UserName,
                };

                if (snippet.IsValid() == false)
                {
                    this.Message.Hinweis("Fehlerhafte Eingabe", "Bitte füllen Sie alle Pflichtfelder aus, bevor Sie den Eintrag speichern können.", true);
                    return;
                }

                using (DatabaseService ds = new DatabaseService(App.DatabasePath))
                {
                    ds.Insert(this.UpdateSnippet, snippet);
                }
            }

            if (App.Settings.SaveAndClose == true)
            {
                OnGoBack(CommandButtons.GoBack);
            }
        }


        private async void InsertSnippet(SQLiteConnection sqliteConnection, object snippet)
        {
            SnippetItem insertSnipppet = snippet as SnippetItem;

            try
            {
                string sqlText = "INSERT INTO TAB_Snippet (Id, Gruppe, SnippetTyp,Titel,Beschreibung,SnippetContent,CreatedOn,CreatedBy) VALUES (@Id, @Gruppe, @SnippetTyp,@Titel,@Beschreibung,@SnippetContent,@CreatedOn,@CreatedBy)";
                Dictionary<string, object> parameterCollection = new();
                parameterCollection.Add("@Id", Guid.CreateVersion7().ToString());
                parameterCollection.Add("@Gruppe", insertSnipppet.Gruppe);
                parameterCollection.Add("@SnippetTyp", insertSnipppet.SnippetTyp);
                parameterCollection.Add("@Titel", insertSnipppet.Titel);
                parameterCollection.Add("@Beschreibung", insertSnipppet.Beschreibung);
                parameterCollection.Add("@SnippetContent", insertSnipppet.SnippetContent);
                parameterCollection.Add("@CreatedOn", DateTime.Now);
                parameterCollection.Add("@CreatedBy", Environment.UserName);
                int insertedRows = sqliteConnection.RecordSet<int>(sqlText, parameterCollection).Execute().Result;
                if (insertedRows > 0)
                {
                    if (App.EventAgg.IsSubscription<StatusEvent>() == true)
                    {
                        await App.EventAgg.PublishAsync(new StatusEvent("letzte Änderung gespeichert"));
                    }
                }
            }
            catch (Exception ex)
            {
                string errorText = ex.Message;
                App.ErrorMessage(ex, "Fehler beim Insert des Snippets in die Tabelle.");
            }
        }

        private async void UpdateSnippet(SQLiteConnection sqliteConnection, object snippet)
        {
            SnippetItem updateSnipppet = snippet as SnippetItem;

            try
            {
                string sqlText = "UPDATE TAB_Snippet SET Gruppe = @Gruppe, SnippetTyp = @SnippetTyp, Titel = @Titel, Beschreibung = @Beschreibung, SnippetContent = @SnippetContent, ModifiedOn = @ModifiedOn, ModifiedBy = @ModifiedBy WHERE Id = @Id";
                Dictionary<string, object> parameterCollection = new();
                parameterCollection.Add("@Id", updateSnipppet.Id.ToString());
                parameterCollection.Add("@Gruppe", updateSnipppet.Gruppe);
                parameterCollection.Add("@SnippetTyp", updateSnipppet.SnippetTyp);
                parameterCollection.Add("@Titel", updateSnipppet.Titel);
                parameterCollection.Add("@Beschreibung", updateSnipppet.Beschreibung);
                parameterCollection.Add("@SnippetContent", updateSnipppet.SnippetContent);
                parameterCollection.Add("@ModifiedOn", DateTime.Now);
                parameterCollection.Add("@ModifiedBy", Environment.UserName);
                int updatedRows = sqliteConnection.RecordSet<int>(sqlText, parameterCollection).Execute().Result;
                if (updatedRows > 0)
                {
                    if (App.EventAgg.IsSubscription<StatusEvent>() == true)
                    {
                        await App.EventAgg.PublishAsync(new StatusEvent("letzte Änderung gespeichert"));
                    }
                }
            }
            catch (Exception ex)
            {
                string errorText = ex.Message;
                App.ErrorMessage(ex, "Fehler beim Update des Snippets in die Tabelle.");
            }
        }

        private void OnCopyAsFile(object commandParam)
        {
            if (Directory.Exists(App.TemplatePath) == false)
            {
                Directory.CreateDirectory(App.TemplatePath);
            }

            string snippetContent = this.SnippetContent.Replace("[[", string.Empty).Replace("]]", string.Empty);
            string fileName = ExtractHelper.ExtractClassNames(snippetContent).FirstOrDefault();
            string templatePath = Path.Combine(App.TemplatePath, $"{fileName}.cs");

            File.WriteAllText(templatePath, snippetContent);

            /* Datei in Zwischenablage legen, damit sie in einem Explorer-Fenster mit STRG+V eingefügt werden kann. */
            ClipboardHelper.CutFilesToClipboard(templatePath);
        }

        private async void OnCopyAsSnippet(object commandParam)
        {
            string snippetContent = this.SnippetContent;
            if (string.IsNullOrEmpty(snippetContent) == false)
            {
                if (snippetContent.Contains("$Company$", StringComparison.OrdinalIgnoreCase) == true)
                {
                    snippetContent = snippetContent.Replace("$Company$", App.Settings.TemplateCompany, StringComparison.OrdinalIgnoreCase);
                }

                if (snippetContent.Contains("$year$", StringComparison.OrdinalIgnoreCase) == true)
                {
                    snippetContent = snippetContent.Replace("$year$", DateTime.Now.Year.ToString(CultureInfo.CurrentCulture), StringComparison.OrdinalIgnoreCase);
                }

                if (snippetContent.Contains("$name$", StringComparison.OrdinalIgnoreCase) == true)
                {
                    snippetContent = snippetContent.Replace("$name$", DateTime.Now.Year.ToString(CultureInfo.CurrentCulture), StringComparison.OrdinalIgnoreCase);
                }

                if (snippetContent.Contains("$email$", StringComparison.OrdinalIgnoreCase) == true)
                {
                    snippetContent = snippetContent.Replace("$email$", DateTime.Now.Year.ToString(CultureInfo.CurrentCulture), StringComparison.OrdinalIgnoreCase);
                }

                if (snippetContent.Contains("$date$", StringComparison.OrdinalIgnoreCase) == true)
                {
                    snippetContent = snippetContent.Replace("$date$", DateTime.Now.ToShortDateString(), StringComparison.OrdinalIgnoreCase);
                }

                List<PlaceholderItem> pl = PlaceholderService.Extract(snippetContent);
                if (pl != null && pl.Count > 0)
                {

                }

                Clipboard.SetText(snippetContent);

                if (App.EventAgg.IsSubscription<StatusEvent>() == true)
                {
                    await App.EventAgg.PublishAsync(new StatusEvent("Snippet wurde in die Zwischenablage kopiert"));
                }
            }
        }
        #endregion Command Events

    }
}
