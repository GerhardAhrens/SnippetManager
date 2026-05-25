//-----------------------------------------------------------------------
// <copyright file="MainWindow.cs" company="Lifeprojects.de">
//     Class: MainWindow
//     Copyright © Lifeprojects.de 2026
// </copyright>
//
// <author>Gerhard Ahrens - Lifeprojects.de</author>
// <email>developer@lifeprojects.de</email>
// <date>05.03.2026 18:21:36</date>
//
// <summary>
// WPF Template mit Minimalfunktionen
// </summary>
//-----------------------------------------------------------------------

namespace SnippetManager
{
    using System.ComponentModel;
    using System.Windows;

    using SnippetManager.Core;

    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : WindowBase
    {
        public MainWindow()
        {
            this.InitializeComponent();
            WeakEventManager<WindowBase, RoutedEventArgs>.AddHandler(this, "Loaded", this.OnLoaded);
            WeakEventManager<WindowBase, CancelEventArgs>.AddHandler(this, "Closing", this.OnWindowClosing);

            this.SetVectorIcon("IconSnippetManager", 64);

            this.QuitCommand = new CommandBase(this.OnQuit, () => true);
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
            this.DataContext = this;
        }

        public CommandBase QuitCommand { get; private set; }
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

        private void OnLoaded(object sender, RoutedEventArgs e)
        {
            StatusbarMain.Statusbar.DatabaseInfo = "Keine";
            StatusbarMain.Statusbar.DatabaseInfoTooltip = "Keine Datenbank verbunden";
            StatusbarMain.Statusbar.Notification = "Bereit";
        }

        private void OnCloseApplication(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void OnQuit()
        {
            this.Tag = null;
            this.Close();
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

        private void OnWindowClosing(object sender, CancelEventArgs e)
        {
            e.Cancel = false;

            /*
            if (App.Settings.FrageExit == false)
            {
                App.ApplicationExit();
                return;
            }
            */

            MessageBoxResult msgYN;
            if (this.Tag != null)
            {
                msgYN = this.Message.AppExitMessage(this.Tag.ToString());
            }
            else
            {
                msgYN = this.Message.AppExitMessage();
            }

            if (msgYN == MessageBoxResult.Yes)
            {
                App.ApplicationExit();
            }
            else
            {
                e.Cancel = true;
            }
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Performance", "CA1822:Member als statisch markieren", Justification = "<Ausstehend>")]
        private void ChangeView(object commandParam)
        {
            if (commandParam != null && commandParam is DialogView view)
            {
                if (view == DialogView.SourceSnippets)
                {
                }
                else if (view == DialogView.XamlGrafik)
                {
                }
            }
        }


    }
}