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
    using System.Windows.Input;

    using SnippetManager.Core;
    using SnippetManager.View;

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

            this.RegisterFactory();

            this.WindowTitel = $"{LocalizationValue.Get("WindowsTitelZeile")} ({base.ApplicationVersion})";
            this.SetVectorIcon("IconSnippetManager", 64);
            this.DataContext = this;
        }

        #region Properties
        public string WindowTitel
        {
            get => base.GetValue<string>();
            set => base.SetValue(value);
        }

        public System.Windows.Controls.UserControl WorkContent
        {
            get { return base.GetValue<System.Windows.Controls.UserControl>(); }
            set { base.SetValue(value); }
        }

        private MessageBase Message { get; } = new MessageBase();
        #endregion Properties

        private async void OnLoaded(object sender, RoutedEventArgs e)
        {
            App.EventAgg.Subscribe<ChangeViewEventArgs>(async (evt, ct) => this.ChangeControl(evt));
            App.EventAgg.Subscribe<WindowsTitelEvent>(async (evt, ct) => this.OnUpdateWindowTitel(evt));
            App.EventAgg.Subscribe<StatusEvent>(async (evt, ct) => this.OnUpdateStatusBar(evt));

            StatusbarMain.Statusbar.DatabaseInfo = "Keine";
            StatusbarMain.Statusbar.DatabaseInfoTooltip = "Keine Datenbank verbunden";
            StatusbarMain.Statusbar.Notification = "Bereit";

            ChangeViewEventArgs args = new();
            args.MenuButton = CommandButtons.Home;
            args.FromPage = CommandButtons.Home;
            this.ChangeControl(args);
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
        private void OnUpdateStatusBar(StatusEvent evt)
        {
            StatusbarMain.Statusbar.Notification = evt.Notification;
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Performance", "CA1822:Member als statisch markieren", Justification = "<Ausstehend>")]
        private void OnUpdateWindowTitel(WindowsTitelEvent evt)
        {
            if (string.IsNullOrEmpty(evt.DialogTitel) == true)
            {
                this.WindowTitel = $"{LocalizationValue.Get("WindowsTitelZeile")} ({base.ApplicationVersion})";
                return;
            }
            else
            {
                this.WindowTitel = $"{LocalizationValue.Get("WindowsTitelZeile")} ({base.ApplicationVersion}) [{evt.DialogTitel}]";
            }
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Performance", "CA1822:Member als statisch markieren", Justification = "<Ausstehend>")]
        private async void ChangeControl(ChangeViewEventArgs commandParam)
        {
            this.Dispatcher.Invoke(() => Mouse.OverrideCursor = Cursors.Wait);

            if (commandParam != null && commandParam.MenuButton is DialogView view)
            {
                if (view.In(DialogView.SourceSnippets, DialogView.XamlGrafik))
                {
                    this.WorkContent = null;
                    this.WorkContent = (System.Windows.Controls.UserControl)Factory.Get<UserControlBase, DialogView>((DialogView)commandParam.MenuButton, commandParam);
                }
            }
            else if (commandParam != null && commandParam.MenuButton is CommandButtons button)
            {
                if (button == CommandButtons.AppQuit)
                {
                    this.OnQuit();
                }
                else if (button.In(CommandButtons.Home, CommandButtons.Help))
                {
                    this.WorkContent = null;
                    this.WorkContent = (System.Windows.Controls.UserControl)Factory.Get<UserControlBase, CommandButtons>((CommandButtons)commandParam.MenuButton, commandParam);
                }
                else if (button.In(CommandButtons.GoBack))
                {
                    if (App.EventAgg.IsSubscription<WindowsTitelEvent>() == true)
                    {
                        await App.EventAgg.PublishAsync(new WindowsTitelEvent(string.Empty));
                    }

                    this.WorkContent = null;
                    this.WorkContent = (System.Windows.Controls.UserControl)Factory.Get<UserControlBase, CommandButtons>((CommandButtons)CommandButtons.Home, commandParam);
                }
            }

            this.Dispatcher.Invoke(() => Mouse.OverrideCursor = Cursors.Arrow);
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Performance", "CA1822:Member als statisch markieren", Justification = "<Ausstehend>")]
        private void RegisterFactory()
        {
            Factory.RegisterSingleton<CommandButtons>(CommandButtons.Home, () => new HelloUC());
            Factory.RegisterSingleton<CommandButtons>(CommandButtons.Help, () => new HelpUC());
            Factory.RegisterSingleton<DialogView>(DialogView.SourceSnippets, () => new SourceSnippetsUC());
            Factory.RegisterSingleton<DialogView>(DialogView.XamlGrafik, () => new XamlGrafikUC());
        }

    }
}