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
            this.ExportXamlIconCommand = new CommandBase(commandParam => this.OnExportXamlIcon(commandParam), this.OnCanExportXamlIcon);
            this.ImageDoubleClickCommand = new CommandBase(commandParam => this.OnImageDoubleClick(commandParam), () => true);
            this.HelpCommand = new CommandBase(commandParam => this.OnHelp(commandParam), () => true);

        }

        #region Properties
        public CommandBase GoBackCommand { get; private set; }
        public CommandBase ExportXamlIconCommand { get; private set; }
        public CommandBase ImageDoubleClickCommand { get; private set; }
        public CommandBase HelpCommand { get; private set; }

        public ObservableCollection<XamlTileItem> XamlItemSource
        {
            get => base.GetValue<ObservableCollection<XamlTileItem>>();
            set => base.SetValue(value);
        }

        public XamlTileItem SelectedXamlItem
        {
            get => base.GetValue<XamlTileItem>();
            set => base.SetValue(value);
        }

        private int CountSelectedItem { get; set; }

        private ResourceDictionary ResourcesDic { get; set; }
        #endregion Properties

        #region Windows Events
        private async void OnLoaded(object sender, RoutedEventArgs e)
        {
            this.DataContext = this;

            if (App.EventAgg.IsSubscription<WindowsTitelEvent>() == true)
            {
                await App.EventAgg.PublishAsync(new WindowsTitelEvent("XamlGrafik Übersicht"));
            }

            this.XamlItemSource = new ObservableCollection<XamlTileItem>();
            this.XamlItemSource.CollectionChanged += (s, ev) =>
            {
                if (ev.NewItems != null)
                {
                    foreach (XamlTileItem newItem in ev.NewItems)
                    {
                        newItem.PropertyChanged += async (s2, ev2) =>
                        {
                            if (ev2.PropertyName == nameof(XamlTileItem.IsSelectedItem))
                            {
                                this.CountSelectedItem = this.XamlItemSource.Count(x => x.IsSelectedItem == true);
                                if (this.CountSelectedItem > 0)
                                {
                                    if (App.EventAgg.IsSubscription<StatusEvent>() == true)
                                    {
                                        await App.EventAgg.PublishAsync(new StatusEvent($"Bereit: Anzahl der XAML-Icons: {this.XamlItemSource.Count} / Ausgewählt: {this.CountSelectedItem}"));
                                    }

                                    this.ExportXamlIconCommand.RaiseCanExecuteChanged();
                                }
                                else
                                {
                                    if (App.EventAgg.IsSubscription<StatusEvent>() == true)
                                    {
                                        await App.EventAgg.PublishAsync(new StatusEvent("Bereit: Anzahl der XAML-Icons: " + this.XamlItemSource.Count));
                                    }

                                    this.ExportXamlIconCommand.RaiseCanExecuteChanged();
                                }
                            }
                        };
                    }
                }
            };

            const string DICTIONARYNAME = "Resources\\Style\\XamlIcon.xaml";

            this.ResourcesDic = Application.Current.Resources.MergedDictionaries.Where(md => md.Source.OriginalString.EndsWith(DICTIONARYNAME, StringComparison.CurrentCulture)).FirstOrDefault();
            List<string> valueList = GetResourceKeys(this.ResourcesDic).OrderBy(k => k).ToList();
            foreach (string key in valueList)
            {
                var value = this.ResourcesDic.Cast<DictionaryEntry>().FirstOrDefault(f => f.Key.ToString().Equals(key, StringComparison.OrdinalIgnoreCase)).Value;
                if (value is DrawingImage drawingImage)
                {
                    XamlItemSource.Add(new XamlTileItem() { Title = key, ImageContent = drawingImage });
                }
            }

            if (App.EventAgg.IsSubscription<StatusEvent>() == true)
            {
                await App.EventAgg.PublishAsync(new StatusEvent("Bereit: Anzahl der XAML-Icons: " + this.XamlItemSource.Count));
            }
        }

        private static List<string> GetResourceKeys(ResourceDictionary dictionary)
        {
            return dictionary.Keys.OfType<string>().ToList();
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

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Performance", "CA1822:Member als statisch markieren", Justification = "<Ausstehend>")]
        private bool OnCanExportXamlIcon()
        {
            return this.CountSelectedItem > 0;
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Performance", "CA1822:Member als statisch markieren", Justification = "<Ausstehend>")]
        private void OnExportXamlIcon(object commandParam)
        {
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Performance", "CA1822:Member als statisch markieren", Justification = "<Ausstehend>")]
        private void OnImageDoubleClick(object commandParam)
        {
            string key = SelectedXamlItem.Title;

            string xamlSource = GetXamlSourceFromKey(this.ResourcesDic, key);
            xamlSource = xamlSource.Replace("xmlns=\"http://schemas.microsoft.com/winfx/2006/xaml/presentation\"", $"x:Key=\"{key}\"");
            Clipboard.SetText(xamlSource);
        }
        #endregion Command Events

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

    }
}
