namespace SnippetManager.View
{
    using System.ComponentModel;
    using System.Data;
    using System.Data.SQLite;
    using System.Windows;
    using System.Windows.Controls;

    using SnippetManager.Core;

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

        #endregion Properties

        #region Windows Events
        private async void OnLoaded(object sender, RoutedEventArgs e)
        {
            try
            {
                this.DataContext = this;

                using (DatabaseService ds = new DatabaseService(App.DatabasePath))
                {
                    SQLiteConnection connection = ds.OpenConnection();
                    this.SnippetSource = connection.RecordSet<ICollectionView>("SELECT * FROM TAB_Snippet").Get().Result;
                }

                if (App.EventAgg.IsSubscription<StatusEvent>() == true)
                {
                    await App.EventAgg.PublishAsync(new StatusEvent("Bereit"));
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

        private void OnDeleteEntry(object commandParam)
        {
        }

        private void OnCopyEntry(object commandParam)
        {
        }

        private void OnCopyAsFile(object commandParam)
        {
        }

        private void OnCopyAsSnippet(object commandParam)
        {
        }

        #endregion Command Events
    }
}
