namespace SnippetManager.View
{
    using System.Data.SQLite;
    using System.Windows;
    using System.Windows.Controls;

    using SnippetManager.Core;

    /// <summary>
    /// Interaktionslogik für HelloUC.xaml
    /// </summary>
    public partial class HelloUC : UserControlBase
    {
        public HelloUC() : base(typeof(HelloUC))

        {
            this.InitializeComponent();
            WeakEventManager<UserControl, RoutedEventArgs>.AddHandler(this, "Loaded", this.OnLoaded);

            this.QuitCommand = new CommandBase(commandParam => this.OnQuit(commandParam), () => true);
            this.HomeCommand = new CommandBase(commandParam => this.OnHome(commandParam), () => true);
            this.HelpCommand = new CommandBase(commandParam => this.OnHelp(commandParam), () => true);
            this.SourceSnippetsCommand = new CommandBase(commandParam => this.ChangeView(commandParam), () => true);
            this.IconGrafikCommand = new CommandBase(commandParam => this.ChangeView(commandParam), () => true);

            this.InformationCommand = new CommandBase(this.OnInformationPopup);
            this.SettingsCommand = new CommandBase(this.OnSettingsPopup);
            this.CloseInformationPopupCommand = new CommandBase(this.OnCloseInformation);
            this.CloseSettingsPopupCommand = new CommandBase(this.OnCloseSettingsPopup);

            this.WindowTitel = LocalizationValue.Get("WindowsTitelZeile");
            this.ApplikationVersion = base.ApplicationVersion.ToString();
            this.LaufzeitVersion = base.RuntimeVersion;
            this.WinVersion = base.WindowsVersion;
        }

        #region Properties
        public CommandBase QuitCommand { get; private set; }
        public CommandBase HomeCommand { get; private set; }
        public CommandBase HelpCommand { get; private set; }
        public CommandBase SourceSnippetsCommand { get; private set; }
        public CommandBase IconGrafikCommand { get; private set; }

        public CommandBase InformationCommand { get; private set; }
        public CommandBase SettingsCommand { get; private set; }
        public CommandBase CloseInformationPopupCommand { get; private set; }
        public CommandBase CloseSettingsPopupCommand { get; private set; }
        public CommandBase ShowMessageCommand { get; private set; }

        public string WindowTitel
        {
            get => base.GetValue<string>();
            set => base.SetValue(value);
        }

        public string ApplikationVersion
        {
            get => base.GetValue<string>();
            set => base.SetValue(value);
        }

        public string LaufzeitVersion
        {
            get => base.GetValue<string>();
            set => base.SetValue(value);
        }

        public string WinVersion
        {
            get => base.GetValue<string>();
            set => base.SetValue(value);
        }

        private MessageBase Message { get; } = new MessageBase();
        #endregion Properties

        #region Windows Events
        private async void OnLoaded(object sender, RoutedEventArgs e)
        {
            this.DataContext = this;

            string databaseName = string.Empty;
            string databaseVersion = string.Empty;
            string databaseTooltip = string.Empty;
            using (DatabaseService ds = new DatabaseService(App.DatabasePath))
            {
                var dbVersion = ds.MetadataInformation();
                databaseName = ((List<Tuple<string, string, object, Type>>)dbVersion).FirstOrDefault()?.Item3.ToString() ?? string.Empty;
                databaseVersion = ((List<Tuple<string, string, object, Type>>)dbVersion).FirstOrDefault(f => f.Item1 == "ServerVersion")?.Item3.ToString() ?? string.Empty;
            }

            if (App.EventAgg.IsSubscription<StatusEvent>() == true)
            {
                databaseTooltip = $"{databaseName} - {databaseVersion}";
                await App.EventAgg.PublishAsync(new StatusEvent("Bereit",databaseName, databaseTooltip));
            }
        }
        #endregion Windows Events

        #region Command Events
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Performance", "CA1822:Member als statisch markieren", Justification = "<Ausstehend>")]
        private async void OnQuit(object commandParam)
        {
            if (commandParam != null && commandParam is CommandButtons button)
            {
                if (button == CommandButtons.AppQuit)
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

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Performance", "CA1822:Member als statisch markieren", Justification = "<Ausstehend>")]
        private async void OnHome(object commandParam)
        {
            if (commandParam != null && commandParam is CommandButtons button)
            {
                if (button == CommandButtons.Home)
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

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Performance", "CA1822:Member als statisch markieren", Justification = "<Ausstehend>")]
        private async void OnHelp(object commandParam)
        {
            if (commandParam != null && commandParam is CommandButtons button)
            {
                if (button == CommandButtons.Help)
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

        private void OnInformationPopup()
        {
            this.InformationPopup.SetValue(MaskLayerBehavior.IsOpenProperty, true);
        }

        private void OnCloseInformation()
        {
            this.InformationPopup.SetValue(MaskLayerBehavior.IsOpenProperty, false);
        }

        private void OnSettingsPopup()
        {
            this.SettingsPopup.SetValue(MaskLayerBehavior.IsOpenProperty, true);
        }

        private void OnCloseSettingsPopup()
        {
            this.SettingsPopup.SetValue(MaskLayerBehavior.IsOpenProperty, false);
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Performance", "CA1822:Member als statisch markieren", Justification = "<Ausstehend>")]
        private async void ChangeView(object commandParam)
        {
            if (commandParam != null && commandParam is DialogView view)
            {
                if (view == DialogView.SourceSnippets)
                {
                    ChangeViewEventArgs args = new();
                    args.MenuButton = DialogView.SourceSnippets;
                    args.FromPage = CommandButtons.Home;

                    if (App.EventAgg.IsSubscription<ChangeViewEventArgs>() == true)
                    {
                        await App.EventAgg.PublishAsync(args);
                    }
                }
                else if (view == DialogView.XamlGrafik)
                {
                    ChangeViewEventArgs args = new();
                    args.MenuButton = DialogView.XamlGrafik;
                    args.FromPage = CommandButtons.Home;

                    if (App.EventAgg.IsSubscription<ChangeViewEventArgs>() == true)
                    {
                        await App.EventAgg.PublishAsync(args);
                    }
                }
            }
        }

        #endregion Command Events
    }
}
