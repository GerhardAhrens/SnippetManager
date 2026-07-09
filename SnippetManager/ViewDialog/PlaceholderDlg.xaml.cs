namespace SnippetManager.View
{
    using System.Collections.ObjectModel;
    using System.ComponentModel;
    using System.Windows;

    using SnippetManager.Core.Placeholder;

    /// <summary>
    /// Interaktionslogik für PlaceholderDlg.xaml
    /// </summary>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Performance", "CA1822:Member als statisch markieren", Justification = "<Ausstehend>")]
    public partial class PlaceholderDlg : WindowBase
    {
        public ObservableCollection<PlaceholderItem> Placeholders { get; private set; }

        public PlaceholderDlg()
        {
            this.InitializeComponent();
            WeakEventManager<WindowBase, RoutedEventArgs>.AddHandler(this, "Loaded", this.OnLoaded);
            WeakEventManager<WindowBase, CancelEventArgs>.AddHandler(this, "Closing", this.OnWindowClosing);
            this.WindowTitel = LocalizationValue.Get("PlaceholderTitelZeile");

            this.CloseDialogCommand = new CommandBase(commandParam => this.OnCloseDialog(commandParam), () => true);
            this.ApplyChangesCommand = new CommandBase(commandParam => this.OnApplyChanges(commandParam), () => true);
            this.DiscardInputCommand = new CommandBase(commandParam => this.OnDiscardInput(commandParam), () => true);

            this.DataContext = this;
        }

        public PlaceholderDlg(List<PlaceholderItem> param)
        {
            this.InitializeComponent();
            WeakEventManager<WindowBase, RoutedEventArgs>.AddHandler(this, "Loaded", this.OnLoaded);
            WeakEventManager<WindowBase, CancelEventArgs>.AddHandler(this, "Closing", this.OnWindowClosing);

            this.CloseDialogCommand = new CommandBase(commandParam => this.OnCloseDialog(commandParam), () => true);
            this.ApplyChangesCommand = new CommandBase(commandParam => this.OnApplyChanges(commandParam), () => true);
            this.DiscardInputCommand = new CommandBase(commandParam => this.OnDiscardInput(commandParam), () => true);

            this.WindowTitel = LocalizationValue.Get("PlaceholderTitelZeile");

            this.DataContext = this;

            this.Placeholders = new ObservableCollection<PlaceholderItem>(param);
        }

        public CommandBase CloseDialogCommand { get; private set; }
        public CommandBase ApplyChangesCommand { get; private set; }
        public CommandBase DiscardInputCommand { get; private set; }

        public string WindowTitel
        {
            get => base.GetValue<string>();
            set => base.SetValue(value);
        }

        private MessageBase Message { get; } = new MessageBase();
        #region WindowEventHandler
        private void OnLoaded(object sender, RoutedEventArgs e)
        {
        }

        private void OnCloseApplication(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void OnWindowClosing(object sender, CancelEventArgs e)
        {
            e.Cancel = false;
        }
        #endregion WindowEventHandler

        private void OnCloseDialog(object commandParam)
        {
            this.DialogResult = false;
            this.Close();
        }

        private void OnApplyChanges(object commandParam)
        {
            foreach (var placeholder in Placeholders)
            {
                if (placeholder.Value == null)
                {
                    this.Message.Warnung("Übernehmen Platzhalter","Der Platzhalterwert darf nicht leer sein.");
                    return;
                }
            }

            this.DialogResult = true;
            this.Tag = this.Placeholders.ToList();
            this.Close();
        }

        private void OnDiscardInput(object commandParam)
        {
            try
            {
                foreach (var placeholder in Placeholders)
                {
                    switch (placeholder.Type)
                    {
                        case PlaceholderType.Boolean:
                            placeholder.Value = false;
                            break;

                        case PlaceholderType.Number:
                            placeholder.Value = null;
                            break;

                        case PlaceholderType.Text:
                            placeholder.Value = string.Empty;
                            break;

                        case PlaceholderType.Date:
                            placeholder.Value = null;
                            break;

                        default:
                            placeholder.Value = string.Empty;
                            break;
                    }
                }

                this.Close();
            }
            catch (Exception ex)
            {
                string errorText = ex.Message;
                App.ErrorMessage(ex, $"Fehler in {this.GetType().Name}");
            }
        }
    }
}
