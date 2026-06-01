namespace SnippetManager.View
{
    using System.ComponentModel;
    using System.Windows;

    using SnippetManager.Core.Placeholder;

    /// <summary>
    /// Interaktionslogik für PlaceholderDlg.xaml
    /// </summary>
    public partial class PlaceholderDlg : WindowBase
    {
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
            this.WindowTitel = LocalizationValue.Get("PlaceholderTitelZeile");
            this.DataContext = this;
        }

        public CommandBase CloseDialogCommand { get; private set; }
        public CommandBase ApplyChangesCommand { get; private set; }

        public CommandBase DiscardInputCommand { get; private set; }

        public string WindowTitel
        {
            get => base.GetValue<string>();
            set => base.SetValue(value);
        }

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
            if (this.Owner != null)
            {
                this.DialogResult = false;
            }
        }
        #endregion WindowEventHandler

        private void OnCloseDialog(object commandParam)
        {
            this.Close();
        }

        private void OnApplyChanges(object commandParam)
        {
            this.Close();
        }

        private void OnDiscardInput(object commandParam)
        {
        }
    }
}
