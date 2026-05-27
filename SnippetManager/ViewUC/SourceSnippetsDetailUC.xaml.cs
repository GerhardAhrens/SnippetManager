namespace SnippetManager.View
{
    using System.ComponentModel;
    using System.Globalization;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Data;

    using SnippetManager.Core;

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
        }

        #region Properties
        public CommandBase GoBackCommand { get; private set; }
        public CommandBase SaveEntryCommand { get; private set; }
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

        #endregion Properties

        #region Windows Events
        private async void OnLoaded(object sender, RoutedEventArgs e)
        {
            this.GruppenFilter.AddHandler(System.Windows.Controls.Primitives.TextBoxBase.TextChangedEvent, new System.Windows.Controls.TextChangedEventHandler(OnComboBoxTextChanged));

            this.DataContext = this;

            if (App.EventAgg.IsSubscription<WindowsTitelEvent>() == true)
            {
                await App.EventAgg.PublishAsync(new WindowsTitelEvent(DialogView.SourceSnippetsDetail.ToDescription()));
            }

            this.GruppenSource = new();
            this.GruppenSource.Add("Allgemein");
            this.GruppenSource.Add("Linksammlung");
            this.GruppenSource.Add("C#");
            this.GruppenSource.Add("Visual Basic .Net");

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
                comboBox.IsDropDownOpen = true;
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
            if (App.EventAgg.IsSubscription<StatusEvent>() == true)
            {
                await App.EventAgg.PublishAsync(new StatusEvent("letzte Änderung gespeichert"));
            }
        }

        #endregion Command Events
    }
}
