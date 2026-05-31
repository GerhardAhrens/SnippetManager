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

        public string WindowTitel
        {
            get => base.GetValue<string>();
            set => base.SetValue(value);
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

        public string TemplateCompany
        {
            get => base.GetValue<string>();
            set => base.SetValue(value, this.SetStringSettingHandler);
        }

        private ApplicationSettings Settings { get; set; }
        #region WindowEventHandler
        private void OnLoaded(object sender, RoutedEventArgs e)
        {
            this.DataContext = this;

            if (System.ComponentModel.DesignerProperties.GetIsInDesignMode(this) == false)
            {
                this.WindowTitel = LocalizationValue.Get("WindowsTitelZeile");

                this.Settings = App.Settings;
                this.SelectionExitAnswer = this.Settings.QuestionExit;
                this.SelectionSaveAnswer = this.Settings.QuestionSaveClose;
                this.TemplateCompany = this.Settings.TemplateCompany;
            }
        }
        #endregion WindowEventHandler

        private void SetBoolSettingHandler(bool arg1, string arg2)
        {
            if (arg2 == nameof(this.SelectionExitAnswer))
            {
                App.Settings.QuestionExit = arg1;
            }
            else if (arg2 == nameof(this.SelectionSaveAnswer))
            {
                App.Settings.QuestionSaveClose = arg1;
            }

        }

        private void SetStringSettingHandler(string arg1, string arg2)
        {
            if (arg2 == nameof(this.TemplateCompany))
            {
                App.Settings.TemplateCompany = arg1;
            }
        }
    }
}

