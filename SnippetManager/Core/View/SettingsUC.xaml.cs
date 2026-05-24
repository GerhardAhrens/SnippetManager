namespace SnippetManager.Core.View
{
    using System.Windows;
    using System.Windows.Controls;

    /// <summary>
    /// Interaktionslogik für SettingsUC.xaml
    /// </summary>
    public partial class SettingsUC : UserControl
    {
        public SettingsUC()
        {
            this.InitializeComponent();
            WeakEventManager<UserControl, RoutedEventArgs>.AddHandler(this, "Loaded", this.OnLoaded);
            this.DataContext = this;
        }

        #region WindowEventHandler
        private void OnLoaded(object sender, RoutedEventArgs e)
        {
        }
        #endregion WindowEventHandler
    }
}
