namespace SnippetManager.View
{
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
            this.DeleteEntryCommand = new CommandBase(commandParam => this.OnDeleteEntry(commandParam), () => true);
            this.CopyEntryCommand = new CommandBase(commandParam => this.OnCopyEntry(commandParam), () => true);
            this.CopyToClipboardCommand = new CommandBase(commandParam => this.OnCopyToClipboard(commandParam), () => true);
            this.HelpCommand = new CommandBase(commandParam => this.OnHelp(commandParam), () => true);
        }

        #region Properties
        public CommandBase GoBackCommand { get; private set; }
        public CommandBase NewEntryCommand { get; private set; }
        public CommandBase DeleteEntryCommand { get; private set; }
        public CommandBase CopyEntryCommand { get; private set; }
        public CommandBase CopyToClipboardCommand { get; private set; } 
        public CommandBase HelpCommand { get; private set; }

        #endregion Properties

        #region Windows Events
        private async void OnLoaded(object sender, RoutedEventArgs e)
        {
            this.DataContext = this;

            if (App.EventAgg.IsSubscription<StatusEvent>() == true)
            {
                await App.EventAgg.PublishAsync(new StatusEvent("Bereit"));
            }

            if (App.EventAgg.IsSubscription<WindowsTitelEvent>() == true)
            {
                await App.EventAgg.PublishAsync(new WindowsTitelEvent("Code Snippets Übersicht"));
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
                    args.EntityId = Guid.CreateVersion7();

                    if (App.EventAgg.IsSubscription<ChangeViewEventArgs>() == true)
                    {
                        await App.EventAgg.PublishAsync(args);
                    }
                }
            }
        }

        private void OnDeleteEntry(object commandParam)
        {
        }

        private void OnCopyToClipboard(object commandParam)
        {
        }

        private void OnCopyEntry(object commandParam)
        {
        }

        #endregion Command Events
    }
}
