//-----------------------------------------------------------------------
// <copyright file="XamlGrafikUC.xaml.cs" company="Lifeprojects.de">
//     Class: XamlGrafikUC
//     Copyright © Lifeprojects.de GmbH 2026
// </copyright>
//
// <author>Gerhard Ahrens - Lifeprojects.de</author>
// <email>developer@lifeprojects.de</email>
// <date>25.05.2026</date>
//
// <summary>
// Code Behinde für die XamlGrafikUC.xaml
// </summary>
//-----------------------------------------------------------------------

namespace SnippetManager.View
{
    using System.Collections;
    using System.Collections.ObjectModel;
    using System.Data;
    using System.Data.SQLite;
    using System.IO;
    using System.Text;
    using System.Text.RegularExpressions;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Markup;
    using System.Windows.Media;
    using System.Xml;

    using SnippetManager.Converter;
    using SnippetManager.Core;

    /// <summary>
    /// Interaktionslogik für XamlGrafikUC.xaml
    /// </summary>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Performance", "CA1822:Member als statisch markieren", Justification = "<Ausstehend>")]
    public partial class XamlGrafikUC : UserControlBase
    {
        public XamlGrafikUC() : base(typeof(XamlGrafikUC))
        {
            this.InitializeComponent();
            WeakEventManager<UserControl, RoutedEventArgs>.AddHandler(this, "Loaded", this.OnLoaded);

            this.GoBackCommand = new CommandBase(commandParam => this.OnGoBack(commandParam), () => true);
            this.ExportXamlIconCommand = new CommandBase(commandParam => this.OnExportXamlIcon(commandParam), this.OnCanExportXamlIcon);
            this.ImportXamlIconCommand = new CommandBase(commandParam => this.OnImportXamlIcon(commandParam), this.OnCanImportXamlIcon);
            this.ImageDoubleClickCommand = new CommandBase(commandParam => this.OnImageDoubleClick(commandParam), () => true);
            this.ConvertCommand = new CommandBase(commandParam => this.OnConvert(commandParam), () => true);
            this.HelpCommand = new CommandBase(commandParam => this.OnHelp(commandParam), () => true);

        }

        #region Properties
        public CommandBase GoBackCommand { get; private set; }
        public CommandBase ExportXamlIconCommand { get; private set; }
        public CommandBase ImportXamlIconCommand { get; private set; }
        public CommandBase ImageDoubleClickCommand { get; private set; }
        public CommandBase ConvertCommand { get; private set; }
        public CommandBase HelpCommand { get; private set; }

        public ObservableCollection<XamlTileItem> XamlItemAlleSource
        {
            get => base.GetValue<ObservableCollection<XamlTileItem>>();
            set => base.SetValue(value);
        }

        public FilteredObservableCollection<XamlTileItem> XamlItemSource
        {
            get => base.GetValue<FilteredObservableCollection<XamlTileItem>>();
            set => base.SetValue(value);
        }

        public XamlTileItem SelectedXamlItem
        {
            get => base.GetValue<XamlTileItem>();
            set => base.SetValue(value);
        }

        public string FilterText
        {
            get => base.GetValue<string>();
            set => base.SetValue(value,this.RefreshData);
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

            this.XamlItemAlleSource = new ObservableCollection<XamlTileItem>();
            this.XamlItemAlleSource.CollectionChanged += (s, ev) =>
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
                                        await App.EventAgg.PublishAsync(new StatusEvent($"Bereit: Anzahl der XAML-Icons: {this.XamlItemAlleSource.Count} / Ausgewählt: {this.CountSelectedItem}"));
                                    }

                                    this.ExportXamlIconCommand.RaiseCanExecuteChanged();
                                }
                                else
                                {
                                    if (App.EventAgg.IsSubscription<StatusEvent>() == true)
                                    {
                                        await App.EventAgg.PublishAsync(new StatusEvent("Bereit: Anzahl der XAML-Icons: " + this.XamlItemAlleSource.Count));
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
                    this.XamlItemAlleSource.Add(new XamlTileItem() { Key = key, Title = $"{key}", ImageContent = drawingImage, XamlTyp = value.GetType().Name, Tooltip = $"{key} ({value.GetType().Name})", Quelle = "Resources\\Style\\XamlIcon.xaml" });
                }
                else if (value is Viewbox viewBox)
                {
                    if (double.IsNaN(viewBox.Width) == false && double.IsNaN(viewBox.Height) == false)
                    {
                        DrawingImage img = ConvertViewboxToDrawingImage(viewBox);
                        if (img.Height > 0 && img.Width > 0)
                        {
                            this.XamlItemAlleSource.Add(new XamlTileItem() {Key = key, Title = $"{key}", ImageContent = img , XamlTyp = value.GetType().Name, Tooltip = $"{key} ({value.GetType().Name})", Quelle = "Resources\\Style\\XamlIcon.xaml" });
                        }
                    }
                }
            }

            /* Weitere XAML-Icons aus anderen Quellen können hier geladen und der XamlItemAlleSource hinzugefügt werden */
            string sql = "SELECT Id, Gruppe, Titel, XamlContent FROM TAB_Xaml";
            using (DatabaseService ds = new DatabaseService(App.DatabasePath))
            {
                SQLiteConnection connection = ds.OpenConnection();
                DataTable dtSeletWhere = connection.RecordSet<DataTable>(sql).Get().Result;
                foreach (DataRow row in dtSeletWhere.Rows)
                {
                    string key = row["Titel"].ToString();
                    string xamlContent = row["XamlContent"].ToString();
                    XamlTileItem item = new XamlTileItem() { Key = key, Title = key, XamlContent = xamlContent, XamlTyp = "DrawingImage", Tooltip = $"{key} (DrawingImage)", Quelle = "Import" };
                    item.ImageContent = LoadDrawingImage(xamlContent);
                    this.XamlItemAlleSource.Add(item);
                }
            }


            if (App.EventAgg.IsSubscription<StatusEvent>() == true)
            {
                await App.EventAgg.PublishAsync(new StatusEvent("Bereit: Anzahl der XAML-Icons: " + this.XamlItemAlleSource.Count));
            }

            this.XamlItemSource = new FilteredObservableCollection<XamlTileItem>(this.XamlItemAlleSource, this.DataDefaultFilter);
            this.XamlItemSource.Filter = this.DataDefaultFilter;
        }

        private bool DataDefaultFilter(XamlTileItem item)
        {
            if (string.IsNullOrEmpty(this.FilterText) == true)
            {
                return true;
            }
            
            bool isInKey = item.Key.Contains(this.FilterText, StringComparison.CurrentCultureIgnoreCase);

            return isInKey || item.Title.Contains(this.FilterText, StringComparison.CurrentCultureIgnoreCase);
        }

        private void RefreshData(string arg1, string arg2)
        {
            this.XamlItemSource.Refilter();
        }

        private static List<string> GetResourceKeys(ResourceDictionary dictionary)
        {
            return dictionary.Keys.OfType<string>().ToList();
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
                    args.MenuButton = button;

                    if (App.EventAgg.IsSubscription<ChangeViewEventArgs>() == true)
                    {
                        await App.EventAgg.PublishAsync(args);
                    }
                }
            }
        }

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

        private bool OnCanExportXamlIcon()
        {
            return this.CountSelectedItem > 0;
        }

        private async void OnExportXamlIcon(object commandParam)
        {
            StringBuilder exportXaml = new StringBuilder();
            if (this.XamlItemSource != null && this.XamlItemSource.Count > 0)
            {
                exportXaml.AppendLine("<ResourceDictionary xmlns=\"http://schemas.microsoft.com/winfx/2006/xaml/presentation\" xmlns:x=\"http://schemas.microsoft.com/winfx/2006/xaml\">");
                exportXaml.AppendLine($"\n");
                exportXaml.AppendLine("<!--#region Farbige Icon Symbole auf Basis von DrawingImage-->");
                exportXaml.AppendLine($"\n");

                foreach (var item in this.XamlItemSource.Where(x => x.IsSelectedItem == true))
                {
                    if (item.ImageContent is DrawingImage drawingImage)
                    {
                        string xamlSource = GetXamlSourceFromKey(this.ResourcesDic, item.Key);
                        xamlSource = xamlSource.Replace("xmlns=\"http://schemas.microsoft.com/winfx/2006/xaml/presentation\"", $"x:Key=\"{item.Key}\"");
                        exportXaml.Append( xamlSource );
                        exportXaml.AppendLine($"\n");
                    }
                }

                exportXaml.AppendLine("</ResourceDictionary>");
                Clipboard.SetText(exportXaml.ToString());

                if (App.EventAgg.IsSubscription<StatusEvent>() == true)
                {
                    await App.EventAgg.PublishAsync(new StatusEvent("XAML-Icons in Zwischenablage kopiert"));
                }
            }
        }

        private bool OnCanImportXamlIcon()
        {
            return true;
        }

        private void OnImportXamlIcon(object commandParam)
        {
            const string DATEIFILTER = "XAML-Dateien (*.xaml)|*.xaml|Textdateien (*.txt)|*.txt|Alle Dateien (*.*)|*.*";
            Microsoft.Win32.OpenFileDialog dlg = new Microsoft.Win32.OpenFileDialog();
            dlg.AddExtension = true;
            dlg.CheckPathExists = true;
            dlg.CheckFileExists = true;
            dlg.DefaultExt = ".xaml";
            dlg.Title = "Datei mit XAML - Icons auswählen";
            dlg.Filter = DATEIFILTER;

            if (dlg.ShowDialog() == true)
            {
                this.LoadFileToImport(dlg.FileName);
            }
        }

        private async void OnImageDoubleClick(object commandParam)
        {
            string key = SelectedXamlItem.Key;

            if (this.SelectedXamlItem.Quelle == "Resources\\Style\\XamlIcon.xaml")
            {
                string xamlSource = GetXamlSourceFromKey(this.ResourcesDic, key);
                xamlSource = xamlSource.Replace("xmlns=\"http://schemas.microsoft.com/winfx/2006/xaml/presentation\"", $"x:Key=\"{key}\"");
                Clipboard.SetText(xamlSource);
            }
            else
            {
                string xamlContent = SelectedXamlItem.XamlContent;
                Clipboard.SetText(xamlContent);
            }

            if (App.EventAgg.IsSubscription<StatusEvent>() == true)
            {
                await App.EventAgg.PublishAsync(new StatusEvent("XAML-Icons in Zwischenablage kopiert"));
            }
        }

        private void OnConvert(object commandParam)
        {
            const string DATEIFILTER = "XAML-Dateien (*.xaml)|*.xaml|Textdateien (*.txt)|*.txt|Alle Dateien (*.*)|*.*";
            Microsoft.Win32.OpenFileDialog dlg = new Microsoft.Win32.OpenFileDialog();
            dlg.AddExtension = true;
            dlg.CheckPathExists = true;
            dlg.CheckFileExists = true;
            dlg.DefaultExt = ".xaml";
            dlg.Title = "Datei mit XAML - Icons auswählen";
            dlg.Filter = DATEIFILTER;

            if (dlg.ShowDialog() == true)
            {
                this.LoadFileToConvert(dlg.FileName);
            }
        }
        #endregion Command Events

        private void LoadFileToImport(string path)
        {
            try
            {
                string sourceXaml = File.ReadAllText(path);
                if (string.IsNullOrEmpty(sourceXaml) == false)
                {
                    string xamlConvert = ViewBoxToDrawingImageConverter.Convert(sourceXaml, Path.GetFileNameWithoutExtension(path));

                    XamlTileItem importXaml = new XamlTileItem();
                    importXaml.Key = Path.GetFileNameWithoutExtension(path);
                    importXaml.Title = Path.GetFileNameWithoutExtension(path);
                    importXaml.XamlContent = xamlConvert;
                    importXaml.XamlTyp = "DrawingImage";
                    importXaml.Tooltip = $"{importXaml.Key}\n({importXaml.XamlTyp}";
                    importXaml.Quelle = "Import";

                    using (DatabaseService ds = new DatabaseService(App.DatabasePath))
                    {
                        ds.Insert(this.ImportXaml, importXaml);
                    }

                    ObservableCollection<XamlTileItem> tempCollection = new ObservableCollection<XamlTileItem>(this.XamlItemAlleSource);
                    importXaml.ImageContent = LoadDrawingImage(importXaml.XamlContent);
                    tempCollection.Add(importXaml);

                    this.XamlItemSource = new FilteredObservableCollection<XamlTileItem>(tempCollection, this.DataDefaultFilter);
                }
            }
            catch (Exception ex)
            {
                string errorText = ex.Message;
                App.ErrorMessage(ex, "Fehler beim Importieren des XAML-Icons.");
            }
        }

        private async void ImportXaml(SQLiteConnection sqliteConnection, object importXaml)
        {
            XamlTileItem import = importXaml as XamlTileItem;

            try
            {
                string sqlText = "INSERT INTO TAB_Xaml (Id, Gruppe, Titel, XamlContent,CreatedOn,CreatedBy) VALUES (@Id, @Gruppe, @Titel, @XamlContent,@CreatedOn,@CreatedBy)";
                Dictionary<string, object> parameterCollection = new();
                parameterCollection.Add("@Id", Guid.CreateVersion7().ToString());
                parameterCollection.Add("@Gruppe", "Import");
                parameterCollection.Add("@Titel", import.Title);
                parameterCollection.Add("@XamlContent", import.XamlContent);
                parameterCollection.Add("@CreatedOn", DateTime.Now);
                parameterCollection.Add("@CreatedBy", Environment.UserName);
                int insertedRows = sqliteConnection.RecordSet<int>(sqlText, parameterCollection).Execute().Result;
                if (insertedRows > 0)
                {
                    if (App.EventAgg.IsSubscription<StatusEvent>() == true)
                    {
                        await App.EventAgg.PublishAsync(new StatusEvent($"XAML-Icon '{import.Title}' erfolgreich importiert"));
                    }
                }
            }
            catch (Exception ex)
            {
                string errorText = ex.Message;
                App.ErrorMessage(ex, "Fehler beim Insert des XAML-Icons in die Tabelle.");
            }
        }

        public async void LoadFileToConvert(string path)
        {
            try
            {
                string sourceXaml = File.ReadAllText(path);
                if (string.IsNullOrEmpty(sourceXaml) == false)
                {
                    string xamlConvert = ViewBoxToDrawingImageConverter.Convert(sourceXaml, Path.GetFileNameWithoutExtension(path));
                    Clipboard.SetText(xamlConvert);

                    if (App.EventAgg.IsSubscription<StatusEvent>() == true)
                    {
                        await App.EventAgg.PublishAsync(new StatusEvent("XAML-Icons in Zwischenablage kopiert"));
                    }
                }
            }
            catch (Exception ex)
            {
                string errorText = ex.Message;
                App.ErrorMessage(ex, "Fehler beim Importieren des XAML-Icons.");
            }
        }

        private static string GetXamlSourceFromKey(ResourceDictionary dictionary, string key)
        {
            try
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
            catch (Exception ex)
            {
                string errorText = ex.Message;
                App.ErrorMessage(ex, "Fehler beim Importieren des XAML-Icons.");
            }

            return string.Empty;
        }

        private static DrawingImage ConvertViewboxToDrawingImage(Viewbox viewbox)
        {
            try
            {
                // 1. Größe der Viewbox auslesen
                double width = viewbox.Width;
                double height = viewbox.Height;

                // 2. VisualBrush für den Inhalt der Viewbox erstellen
                VisualBrush visualBrush = new VisualBrush(viewbox.Child);

                // 3. DrawingVisual instanziieren und den Brush zeichnen
                DrawingVisual drawingVisual = new DrawingVisual();
                using (DrawingContext context = drawingVisual.RenderOpen())
                {
                    context.DrawRectangle(visualBrush, null, new Rect(0, 0, width, height));
                }

                // 4. Das DrawingImage erzeugen
                return new DrawingImage(drawingVisual.Drawing);
            }
            catch (Exception ex)
            {
                string errorText = ex.Message;
                App.ErrorMessage(ex, "Fehler beim Importieren des XAML-Icons.");
            }

            return null ;
        }

        public static DrawingImage LoadDrawingImage(string xaml)
        {
            string cleanedXaml = Regex.Replace(xaml, @"\s+x:Key\s*=\s*""[^""]*""", " xmlns=\"http://schemas.microsoft.com/winfx/2006/xaml/presentation\"");
            using StringReader stringReader = new(cleanedXaml);
            using XmlReader xmlReader = XmlReader.Create(stringReader);

            object obj = XamlReader.Load(xmlReader);

            return obj as DrawingImage ?? throw new InvalidOperationException("Kein DrawingImage.");
        }
    }
}
