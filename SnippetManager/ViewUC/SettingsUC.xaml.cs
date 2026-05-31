namespace SnippetManager.View
{
    using System.Diagnostics;
    using System.Windows;
    using System.Windows.Controls;

    using SnippetManager.Core;

    /// <summary>
    /// Interaktionslogik für SettingsUC.xaml
    /// </summary>
    public partial class SettingsUC : UserControlBase
    {
        public SettingsUC()
        {
            this.InitializeComponent();
            WeakEventManager<UserControl, RoutedEventArgs>.AddHandler(this, "Loaded", this.OnLoaded);
        }

        public bool SelectionExitAnswer
        {
            get => base.GetValue<bool>();
            set => base.SetValue(value, this.SetBoolSettingHandler);
        }

        public bool SelectionSaveAnswer
        {
            get => base.GetValue<bool>();
            set => base.SetValue(value, this.SetBoolSettingHandler);
        }

        private ApplicationSettings Settings { get; set; }
        #region WindowEventHandler
        private void OnLoaded(object sender, RoutedEventArgs e)
        {
            this.DataContext = this;

            if (System.ComponentModel.DesignerProperties.GetIsInDesignMode(this) == false)
            {
                this.Settings = App.Settings;
                this.SelectionExitAnswer = this.Settings.QuestionExit;
                this.SelectionSaveAnswer = this.Settings.QuestionSaveClose;
            }
        }
        #endregion WindowEventHandler

        private void SetBoolSettingHandler(bool arg1, string arg2)
        {
            Debug.WriteLine($"SettingsUC: {arg2} = {arg1}");
        }
    }
}
