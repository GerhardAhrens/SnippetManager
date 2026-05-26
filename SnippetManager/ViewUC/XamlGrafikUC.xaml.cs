namespace SnippetManager.View
{
    using System.Collections;
    using System.Collections.ObjectModel;
    using System.Text;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Media;
    using System.Windows.Markup;
    using System.Xml;

    using SnippetManager.Core;

    /// <summary>
    /// Interaktionslogik für XamlGrafikUC.xaml
    /// </summary>
    public partial class XamlGrafikUC : UserControlBase
    {
        public XamlGrafikUC() : base(typeof(XamlGrafikUC))
        {
            this.InitializeComponent();
            WeakEventManager<UserControl, RoutedEventArgs>.AddHandler(this, "Loaded", this.OnLoaded);

            this.GoBackCommand = new CommandBase(commandParam => this.OnGoBack(commandParam), () => true);
            this.HelpCommand = new CommandBase(commandParam => this.OnHelp(commandParam), () => true);

        }

        #region Properties
        public CommandBase GoBackCommand { get; private set; }
        public CommandBase HelpCommand { get; private set; }

        public ObservableCollection<XamlTileItem> XamlItemSource
        {
            get => base.GetValue<ObservableCollection<XamlTileItem>>();
            set => base.SetValue(value);
        }

        #endregion Properties

        #region Windows Events
        private async void OnLoaded(object sender, RoutedEventArgs e)
        {
            this.DataContext = this;

            if (App.EventAgg.IsSubscription<StatusEvent>() == true)
            {
                await App.EventAgg.PublishAsync(new StatusEvent("Bereit"));
            }

            if (App.EventAgg.IsSubscription<WindowsTitelEvent>() == true)
            {
                await App.EventAgg.PublishAsync(new WindowsTitelEvent("XamlGrafik Übersicht"));
            }

            XamlItemSource = new ObservableCollection<XamlTileItem>();

            const string DICTIONARYNAME = "Resources\\Style\\XamlIcon.xaml";

            ResourceDictionary resourcesDic = Application.Current.Resources.MergedDictionaries.Where(md => md.Source.OriginalString.EndsWith(DICTIONARYNAME, StringComparison.CurrentCulture)).FirstOrDefault();
            List<string> valueList = GetResourceKeys(resourcesDic);
            foreach (string key in valueList)
            {
                var value = resourcesDic.Cast<DictionaryEntry>().FirstOrDefault(f => f.Key.ToString().Equals(key, StringComparison.OrdinalIgnoreCase)).Value;
                if (value is DrawingImage drawingImage)
                {
                    XamlItemSource.Add(new XamlTileItem() { Title = key, ImageContent = drawingImage });
                    /*
                    string aa = GetXamlSourceFromKey(resourcesDic, key);
                    */
                }
            }
        }

        private static List<string> GetResourceKeys(ResourceDictionary dictionary)
        {
            return dictionary.Keys.OfType<string>().ToList();
        }

        private static string GetXamlSourceFromKey(ResourceDictionary dictionary, string key)
        {
            // 1. Objekt aus dem ResourceDictionary laden
            if (dictionary.Contains(key))
            {
                object resource = dictionary[key];

                // 2. Objekt in XAML-String konvertieren
                XmlWriterSettings settings = new XmlWriterSettings
                {
                    Indent = true,
                    OmitXmlDeclaration = true
                };

                StringBuilder sb = new StringBuilder();
                using (XmlWriter writer = XmlWriter.Create(sb, settings))
                {
                    XamlWriter.Save(resource, writer);
                }

                return sb.ToString();
            }

            return $"Key '{key}' nicht gefunden.";
        }
        #endregion Windows Events

        #region Command Events
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Performance", "CA1822:Member als statisch markieren", Justification = "<Ausstehend>")]
        private async void OnGoBack(object commandParam)
        {
            if (commandParam != null && commandParam is CommandButtons button)
            {
                if (button == CommandButtons.GoBack)
                {
                    ChangeViewEventArgs args = new();
                    args.MenuButton = button;

                    if (App.EventAgg.IsSubscription<ChangeViewEventArgs>() == true)
                    {
                        await App.EventAgg.PublishAsync(args);
                    }
                }
            }
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Performance", "CA1822:Member als statisch markieren", Justification = "<Ausstehend>")]
        private async void OnHelp(object commandParam)
        {
            if (commandParam != null && commandParam is CommandButtons button)
            {
                if (button == CommandButtons.Help)
                {
                    ChangeViewEventArgs args = new();
                    args.MenuButton = button;
                    args.FromPage = DialogView.XamlGrafik;

                    if (App.EventAgg.IsSubscription<ChangeViewEventArgs>() == true)
                    {
                        await App.EventAgg.PublishAsync(args);
                    }
                }
            }
        }
        #endregion Command Events
    }
}
