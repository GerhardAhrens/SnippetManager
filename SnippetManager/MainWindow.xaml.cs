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

    using MinimalWPF.Beispiel;

    using SnippetManager.Core;
    using SnippetManager.View;

    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Performance", "CA1822:Member als statisch markieren", Justification = "<Ausstehend>")]
    public partial class MainWindow : WindowBase
    {
        public MainWindow()
        {
            this.InitializeComponent();
            this.WindowStartupLocation = WindowStartupLocation.CenterScreen;
            this.ResizeMode = ResizeMode.CanResizeWithGrip;
            this.ShowInTaskbar = true;
            this.MinWidth = 400;
            this.MinHeight = 300;

            WeakEventManager<WindowBase, RoutedEventArgs>.AddHandler(this, "Loaded", this.OnLoaded);
            WeakEventManager<WindowBase, CancelEventArgs>.AddHandler(this, "Closing", this.OnWindowClosing);

            this.RegisterFactory();

            this.WindowTitel = $"{LocalizationValue.Get("WindowsTitelZeile")} ({base.ApplicationVersion})";
            this.SetVectorIcon("IconDatabase_Code", 64);
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

            if (App.Settings.QuestionExit == false)
            {
                App.ApplicationExit();
                return;
            }

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

        private void OnUpdateStatusBar(StatusEvent evt)
        {
            StatusbarMain.Statusbar.Notification = evt.Notification;

            if (string.IsNullOrEmpty(evt.DatabaseInfo) == false)
            {
                StatusbarMain.Statusbar.DatabaseInfo = evt.DatabaseInfo;
                StatusbarMain.Statusbar.DatabaseInfoTooltip = evt.DatabaseInfoTooltip;
            }
        }

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

        private async void ChangeControl(ChangeViewEventArgs commandParam)
        {
            this.Dispatcher.Invoke(() => Mouse.OverrideCursor = Cursors.Wait);

            if (commandParam != null && commandParam.MenuButton is DialogView view)
            {
                if (view.In(DialogView.SourceSnippets, DialogView.XamlGrafik, DialogView.SourceSnippetsDetail, DialogView.ShowSourceGen))
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
                    if (App.EventAgg.IsSubscription<WindowsTitelEvent>() == true)
                    {
                        await App.EventAgg.PublishAsync(new WindowsTitelEvent(string.Empty));
                    }

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
            Factory.RegisterTransient<CommandButtons>(CommandButtons.Help, (param) => new HelpUC((ChangeViewEventArgs)param!));
            Factory.RegisterTransient<DialogView>(DialogView.SourceSnippets, (param) => new SourceSnippetsUC((ChangeViewEventArgs)param!));
            Factory.RegisterTransient<DialogView>(DialogView.SourceSnippetsDetail, (param) => new SourceSnippetsDetailUC((ChangeViewEventArgs)param!));
            Factory.RegisterSingleton<DialogView>(DialogView.XamlGrafik, () => new XamlGrafikUC());
            Factory.RegisterTransient<DialogView>(DialogView.ShowSourceGen, (param) => new SourceGenUC((ChangeViewEventArgs)param!));
        }

    }
}