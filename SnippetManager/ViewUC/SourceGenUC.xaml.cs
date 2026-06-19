//-----------------------------------------------------------------------
// <copyright file="SourceGenUC.cs" company="Lifeprojects.de">
//     Class: SourceGenUC
//     Copyright © Lifeprojects.de 2026
// </copyright>
//
// <author>Gerhard Ahrens - Lifeprojects.de</author>
// <email>developer@lifeprojects.de</email>
// <date>18.06.2026</date>
//
// <summary>
// Template für eine neues UserControl
// </summary>
//-----------------------------------------------------------------------

namespace MinimalWPF.Beispiel
{
    using System.Windows;
    using System.Windows.Controls;

    using SnippetManager;
    using SnippetManager.Core;

    /// <summary>
    /// Interaktionslogik für SourceGenUC.xaml
    /// </summary>
    public partial class SourceGenUC : UserControlBase
    {
        public SourceGenUC(ChangeViewEventArgs args) : base(typeof(SourceGenUC))

        {
            this.InitializeComponent();
            WeakEventManager<UserControl, RoutedEventArgs>.AddHandler(this, "Loaded", this.OnLoaded);

            this.CurrentCtorArgs = args;

            this.GoBackCommand = new CommandBase(commandParam => this.OnGoBack(commandParam), () => true);
            this.CreateSourceCommand = new CommandBase(commandParam => OnCreateSource(commandParam), () => true);

            this.DataContext = this;
        }

        #region Properties
        public CommandBase GoBackCommand { get; private set; }
        public CommandBase CreateSourceCommand { get; private set; }
        private ChangeViewEventArgs CurrentCtorArgs { get; set; }

        #endregion Properties

        #region Windows Events

        private async void OnLoaded(object sender, RoutedEventArgs e)
        {
            if (App.EventAgg.IsSubscription<StatusEvent>() == true)
            {
                await App.EventAgg.PublishAsync(new StatusEvent("Bereit"));
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

        private static void OnCreateSource(object commandParam)
        {
            if (commandParam != null && commandParam is SourceTyp button)
            {
                if (button == SourceTyp.UserControlWithArgs)
                {

                }
                else if (button == SourceTyp.UserControlWithoutArgs)
                {

                }
            }
        }

        #endregion Command Events

    }
}
