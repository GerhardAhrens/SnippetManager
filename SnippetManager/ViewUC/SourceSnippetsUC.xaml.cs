namespace SnippetManager.View
{
    using System.ComponentModel;
    using System.Data;
    using System.Data.SQLite;
    using System.Globalization;
    using System.Windows;
    using System.Windows.Controls;

    using SnippetManager.Core;
    using SnippetManager.Core.Placeholder;
    using SnippetManager.Data;

    /// <summary>
    /// Interaktionslogik für SourceSnippetsUC.xaml
    /// </summary>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Performance", "CA1822:Member als statisch markieren", Justification = "<Ausstehend>")]
    public partial class SourceSnippetsUC : UserControlBase
    {
        public SourceSnippetsUC() : base(typeof(SourceSnippetsUC))
        {
            this.InitializeComponent();
            WeakEventManager<UserControl, RoutedEventArgs>.AddHandler(this, "Loaded", this.OnLoaded);

            this.GoBackCommand = new CommandBase(commandParam => this.OnGoBack(commandParam), () => true);
            this.NewEntryCommand = new CommandBase(commandParam => this.OnNewEntry(commandParam), () => true);
            this.EditEntryCommand = new CommandBase(commandParam => this.OnEditEntry(commandParam), () => true);
            this.DeleteEntryCommand = new CommandBase(commandParam => this.OnDeleteEntry(commandParam), () => true);
            this.CopyEntryCommand = new CommandBase(commandParam => this.OnCopyEntry(commandParam), () => true);
            this.HelpCommand = new CommandBase(commandParam => this.OnHelp(commandParam), () => true);
            this.CopyAsSnippetCommand = new CommandBase(commandParam => this.OnCopyAsSnippet(commandParam), () => true);
            this.CopyAsFileCommand = new CommandBase(commandParam => this.OnCopyAsFile(commandParam), () => true);
        }

        #region Properties
        public CommandBase GoBackCommand { get; private set; }
        public CommandBase NewEntryCommand { get; private set; }
        public CommandBase EditEntryCommand { get; private set; }
        public CommandBase DeleteEntryCommand { get; private set; }
        public CommandBase CopyEntryCommand { get; private set; }
        public CommandBase HelpCommand { get; private set; }
        public CommandBase CopyAsSnippetCommand { get; private set; }
        public CommandBase CopyAsFileCommand { get; private set; }

        public ICollectionView SnippetSource
        {
            get => base.GetValue<ICollectionView>();
            set => base.SetValue(value);
        }

        public DataRow SelectedSnippet
        {
            get => base.GetValue<DataRow>();
            set => base.SetValue(value);
        }

        public string FilterText
        {
            get => base.GetValue<string>();
            set => base.SetValue(value, this.RefreshData);
        }

        public int RowCount
        {
            get => base.GetValue<int>();
            set => base.SetValue(value);
        }

        private MessageBase Message { get; } = new MessageBase();
        #endregion Properties

        #region Windows Events
        private async void OnLoaded(object sender, RoutedEventArgs e)
        {
            try
            {
                this.DataContext = this;
                this.RowCount = 0;

                this.LoadData();

                if (App.EventAgg.IsSubscription<StatusEvent>() == true)
                {
                    await App.EventAgg.PublishAsync(new StatusEvent($"Bereit, Anzahl : {this.RowCount}"));
                }

                if (App.EventAgg.IsSubscription<WindowsTitelEvent>() == true)
                {
                    await App.EventAgg.PublishAsync(new WindowsTitelEvent("Code Snippets Übersicht"));
                }
            }
            catch (Exception ex)
            {
                string errorText = ex.Message;
                App.ErrorMessage(ex, $"Fehler in {this.GetType().Name}");
            }
        }

        private void LoadData()
        {
            using (DatabaseService ds = new DatabaseService(App.DatabasePath))
            {
                SQLiteConnection connection = ds.OpenConnection();
                this.SnippetSource = connection.RecordSet<ICollectionView>("SELECT * FROM TAB_Snippet ORDER BY Titel").Get().Result;
            }

            if (this.SnippetSource != null)
            {
                this.RowCount = this.SnippetSource.Cast<DataRow>().Count();
                this.SnippetSource.Filter = filter => this.DataDefaultFilter(filter as DataRow);
            }
        }

        private bool DataDefaultFilter(DataRow rowItem)
        {
            bool found = false;

            if (rowItem == null)
            {
                return false;
            }

            string textFilterString = (this.FilterText ?? string.Empty).ToUpperInvariant();
            if (string.IsNullOrEmpty(textFilterString) == false)
            {
                string fullRow = rowItem.ToString("Titel,Gruppe");

                if (fullRow.Contains(textFilterString))
                {
                    found = true;
                }
            }
            else
            {
                found = true;
            }

            return found;
        }

        private void RefreshData(string value, string propertyName)
        {
            if (this.SnippetSource != null)
            {
                this.SnippetSource.Refresh();
                this.RowCount = this.SnippetSource.Cast<DataRow>().Count();
            }
            else
            {
                this.RowCount = 0;
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
                    args.MenuButton = button;

                    if (App.EventAgg.IsSubscription<ChangeViewEventArgs>() == true)
                    {
                        await App.EventAgg.PublishAsync(args);
                    }
                }
            }
        }

        private async void OnHelp(object commandParam)
        {
            if (commandParam != null && commandParam is CommandButtons button)
            {
                if (button == CommandButtons.Help)
                {
                    ChangeViewEventArgs args = new();
                    args.MenuButton = button;
                    args.FromPage = DialogView.SourceSnippets;

                    if (App.EventAgg.IsSubscription<ChangeViewEventArgs>() == true)
                    {
                        await App.EventAgg.PublishAsync(args);
                    }
                }
            }
        }

        private async void OnNewEntry(object commandParam)
        {
            if (commandParam != null && commandParam is CommandButtons button)
            {
                if (button == CommandButtons.NewEntry)
                {
                    ChangeViewEventArgs args = new();
                    args.MenuButton = DialogView.SourceSnippetsDetail;
                    args.FromPage = DialogView.SourceSnippets;
                    args.EntityId = Guid.Empty;

                    if (App.EventAgg.IsSubscription<ChangeViewEventArgs>() == true)
                    {
                        await App.EventAgg.PublishAsync(args);
                    }
                }
            }
        }

        private async void OnEditEntry(object commandParam)
        {
            if (commandParam != null && commandParam is DataRow row)
            {
                Guid id = Guid.Parse(row.Field<string>("Id"));
                ChangeViewEventArgs args = new();
                args.MenuButton = DialogView.SourceSnippetsDetail;
                args.FromPage = DialogView.SourceSnippets;
                args.EntityId = id;

                if (App.EventAgg.IsSubscription<ChangeViewEventArgs>() == true)
                {
                    await App.EventAgg.PublishAsync(args);
                }
            }
        }

        private async void OnDeleteEntry(object commandParam)
        {
            try
            {
                if (this.SelectedSnippet == null)
                {
                    this.Message.Hinweis("Löschen Eintrag","Bitte wählen Sie einen Eintrag aus.");
                    return;
                }

                Guid id = Guid.Parse(this.SelectedSnippet.Field<string>("Id"));
                string titel = this.SelectedSnippet.Field<string>("Titel");
                MessageBoxResult result = this.Message.Question("Löschen Eintrag", $"Möchten Sie den Eintrag '{titel}' wirklich löschen?");
                if (result == MessageBoxResult.Yes)
                {
                    using (DatabaseService ds = new DatabaseService(App.DatabasePath))
                    {
                        ds.Delete(this.DeleteSnippet, id);
                    }
                }


                this.LoadData();
            }
            catch (Exception ex)
            {
                string errorText = ex.Message;
                App.ErrorMessage(ex, $"Fehler in {this.GetType().Name}");
            }
        }

        private async void DeleteSnippet(SQLiteConnection sqliteConnection, object arg2)
        {
            try
            {
                if (arg2 != null && arg2 is Guid id)
                {
                    string sqlText = "DELETE FROM TAB_Snippet WHERE Id = @Id";
                    Dictionary<string, object> parameterCollection = new();
                    parameterCollection.Add("@Id", arg2.ToString());
                    int updatedRows = sqliteConnection.RecordSet<int>(sqlText, parameterCollection).Execute().Result;
                    if (updatedRows > 0)
                    {
                        if (App.EventAgg.IsSubscription<StatusEvent>() == true)
                        {
                            await App.EventAgg.PublishAsync(new StatusEvent("letzte Änderung gespeichert"));
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                string errorText = ex.Message;
                App.ErrorMessage(ex, $"Fehler in {this.GetType().Name}");
            }
        }

        private void OnCopyEntry(object commandParam)
        {
            try
            {
                if (this.SelectedSnippet == null)
                {
                    this.Message.Hinweis("Eintrag kopieren", "Bitte wählen Sie einen Eintrag aus.");
                    return;
                }

                SnippetItem copySnippet = new();
                copySnippet.Titel = $"{this.SelectedSnippet.Field<string>("Titel")} (Kopie)";
                copySnippet.Gruppe = this.SelectedSnippet.Field<string>("Gruppe");
                copySnippet.SnippetTyp = this.SelectedSnippet.Field<string>("SnippetTyp");
                copySnippet.Beschreibung = this.SelectedSnippet.Field<string>("Beschreibung");
                copySnippet.SnippetContent = this.SelectedSnippet.Field<string>("SnippetContent");

                Guid id = Guid.Parse(this.SelectedSnippet.Field<string>("Id"));
                string titel = this.SelectedSnippet.Field<string>("Titel");
                MessageBoxResult result = this.Message.Question("Eintrag kopieren", $"Möchten Sie den Eintrag '{titel}' wirklich kopieren?");
                if (result == MessageBoxResult.Yes)
                {
                    using (DatabaseService ds = new DatabaseService(App.DatabasePath))
                    {
                        ds.Insert(this.InsertSnippet, copySnippet);
                    }
                }

                this.LoadData();
            }
            catch (Exception ex)
            {
                string errorText = ex.Message;
                App.ErrorMessage(ex, $"Fehler in {this.GetType().Name}");
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
                        await App.EventAgg.PublishAsync(new StatusEvent("Kopie erstellt"));
                    }
                }
            }
            catch (Exception ex)
            {
                string errorText = ex.Message;
                App.ErrorMessage(ex, "Fehler beim Insert des Snippets in die Tabelle.");
            }
        }

        private void OnCopyAsFile(object commandParam)
        {
            try
            {

            }
            catch (Exception ex)
            {
                string errorText = ex.Message;
                App.ErrorMessage(ex, $"Fehler in {this.GetType().Name}");
            }
        }

        private async void OnCopyAsSnippet(object commandParam)
        {
            try
            {
                if (this.SelectedSnippet != null)
                {
                    string snippetContent = this.SelectedSnippet.Field<string>("SnippetContent");
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
            }
            catch (Exception ex)
            {
                string errorText = ex.Message;
                App.ErrorMessage(ex, $"Fehler in {this.GetType().Name}");
            }
        }
        #endregion Command Events
    }
}
