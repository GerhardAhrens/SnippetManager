namespace SnippetManager.View
{
    using System.Windows;
    using System.Windows.Controls;

    using SnippetManager.Core;

    /// <summary>
    /// Interaktionslogik für HelpUC.xaml
    /// </summary>
    public partial class HelpUC : UserControlBase
    {
        public HelpUC() : base(typeof(HelpUC))
        {
            this.InitializeComponent();
            WeakEventManager<UserControl, RoutedEventArgs>.AddHandler(this, "Loaded", this.OnLoaded);

            this.GoBackCommand = new CommandBase(commandParam => this.OnGoBack(commandParam), () => true);
        }

        #region Properties
        public CommandBase GoBackCommand { get; private set; }

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
                await App.EventAgg.PublishAsync(new WindowsTitelEvent("Hilfe"));
            }
        }
        #endregion Windows Events

        #region Command Events
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Performance", "CA1822:Member als statisch markieren", Justification = "<Ausstehend>")]
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
        #endregion Command Events
    }
}
