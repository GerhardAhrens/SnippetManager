namespace System.Windows.Documents
{
    using System.Globalization;
    using System.IO;
    using System.Runtime.CompilerServices;
    using System.Text;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Input;
    using System.Windows.Media;
    using System.Windows.Threading;

    using Microsoft.Win32;

    using static System.Windows.Forms.VisualStyles.VisualStyleElement;

    /// <summary>
    /// Interaktionslogik für TextEditor.xaml
    /// </summary>
    public partial class TextEditor : UserControl
    {
        private const string DATEIFILTER = "Markdown (*.md)|*.md|Textdateien (*.txt)|*.txt|Alle Dateien (*.*)|*.*";
        private ScrollViewer editorScrollViewer;
        private double lineHeight;

        public TextEditor()
        {
            this.InitializeComponent();

            Application.Current.Dispatcher.BeginInvoke(
                DispatcherPriority.Background, 
                new Action(() => 
                { 
                    this.Editor.Focus(); 
                }));
            

            /*
            InputBindings.Add(new KeyBinding(
            new EditorRelayCommand(o => OpenFileDialog()),
            new KeyGesture(Key.O, ModifierKeys.Control)));

            InputBindings.Add(new KeyBinding(
                    new EditorRelayCommand(o => this.Save()),
                    new KeyGesture(Key.S, ModifierKeys.Control)));
            */
            InputBindings.Add(new KeyBinding(
                        new EditorRelayCommand(o => this.InsertCurrentDate(),null),
                        new KeyGesture(Key.D, ModifierKeys.Control)));

            InputBindings.Add(new KeyBinding(
                        new EditorRelayCommand(o => this.InsertPlaceholder(), null),
                        new KeyGesture(Key.P, ModifierKeys.Control)));

            InputBindings.Add(new KeyBinding(
                        new EditorRelayCommand(o => this.WrapSelection("[[", "]]"), null),
                        new KeyGesture(Key.H, ModifierKeys.Control)));

            InputBindings.Add(new KeyBinding(
                        new EditorRelayCommand(o => this.WrapSelection("/*", "*/"), null),
                        new KeyGesture(Key.K, ModifierKeys.Control)));
        }

        private bool IsModified { get; set; }
        private static string TextFromOut { get; set; }

        public static readonly DependencyProperty TextProperty =
                DependencyProperty.Register(
                    nameof(Text),
                    typeof(string),
                    typeof(TextEditor),
                    new PropertyMetadata(OnTextChanged));

        public string Text
        {
            get => (string)GetValue(TextProperty);
            set => SetValue(TextProperty, value);
        }

        private static void OnTextChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            TextEditor editor = (TextEditor)d;
            editor.Text = e.NewValue as string;
            TextFromOut = e.NewValue as string;
            editor.UpdateEditorVisuals();
        }


        private void Editor_Loaded(object sender, RoutedEventArgs e)
        {
            this.editorScrollViewer = FindScrollViewer(this.Editor);

            this.Editor.Text = TextFromOut;

            if (this.editorScrollViewer != null)
            {
                this.editorScrollViewer.ScrollChanged += EditorScrollChanged;
            }

            this.lineHeight = this.Editor.GetRectFromCharacterIndex(0).Height;

            if (this.lineHeight <= 0)
            {
                this.lineHeight = this.Editor.FontSize * 1.4;
            }

            this.UpdateEditorVisuals();
        }

        private void Editor_TextChanged(object sender, TextChangedEventArgs e)
        {
            this.IsModified = true;
            this.UpdateEditorVisuals();
            this.Text = Editor.Text;
        }

        private void Editor_SelectionChanged(object sender, RoutedEventArgs e)
        {
            this.UpdateEditorVisuals();
        }

        private void EditorScrollChanged(object sender, ScrollChangedEventArgs e)
        {
            this.UpdateLineNumbers();
        }

        private void UpdateStatus()
        {
            int caret = Editor.CaretIndex;

            int line = Editor.GetLineIndexFromCharacterIndex(caret);
            int column = caret - Editor.GetCharacterIndexFromLineIndex(line);

            this.StatusCursor.Text = $"Ln {line + 1}, Col {column + 1}";

            int totalLines = Editor.LineCount;
            this.StatusLines.Text = $"Lines: {totalLines}";

            int selection = Editor.SelectionLength;
            this.StatusSelection.Text = $"Sel: {selection}";

            int asciiCode = 0;
            if (caret > 0 && caret <= Editor.Text.Length)
            {
                char zeichen = Editor.Text[caret-1];
                asciiCode = (int)zeichen;
            }

            int bytePos = Encoding.UTF8.GetByteCount(Editor.Text.AsSpan(0, caret));

            this.StatusUtf.Text = $"ASCII: {asciiCode}  Bytes: {bytePos}";

            string name = string.Empty;

            if (IsModified == true)
            {
                name += " *";
            }

            StatusFile.Text = name;
        }

        private void UpdateLineNumbers()
        {
            if (this.editorScrollViewer == null || lineHeight <= 0)
            {
                return;
            }

            this.LineNumberCanvas.Children.Clear();

            double offset = editorScrollViewer.VerticalOffset;
            double viewport = editorScrollViewer.ViewportHeight;

            int firstLine = (int)(offset / lineHeight);
            int visibleLines = (int)(viewport / lineHeight) + 2;

            int lastLine = Math.Min(Editor.LineCount, firstLine + visibleLines);

            for (int i = firstLine; i < lastLine; i++)
            {
                TextBlock tb = new TextBlock
                {
                    Text = (i + 1).ToString(CultureInfo.CurrentCulture), FontFamily = Editor.FontFamily, FontSize = Editor.FontSize, Foreground = Brushes.Gray
                };

                double y = (i * lineHeight) - offset;

                Canvas.SetTop(tb, y);
                Canvas.SetRight(tb, 5);

                this.LineNumberCanvas.Children.Add(tb);
            }
        }

        private void UpdateCurrentLineHighlight()
        {
            if (this.editorScrollViewer == null)
            {
                return;
            }

            int line = this.Editor.GetLineIndexFromCharacterIndex(Editor.CaretIndex);

            double y = line * this.lineHeight - this.editorScrollViewer.VerticalOffset;

            this.CurrentLineHighlight.Height = this.lineHeight + 8;
            this.CurrentLineHighlight.Margin = new Thickness(0, y, 0, 0);
        }


        private static ScrollViewer FindScrollViewer(DependencyObject d)
        {
            if (d is ScrollViewer)
            {
                return (ScrollViewer)d;
            }

            for (int i = 0; i < VisualTreeHelper.GetChildrenCount(d); i++)
            {
                var child = VisualTreeHelper.GetChild(d, i);
                var result = FindScrollViewer(child);
                if (result != null)
                {
                    return result;
                }
            }

            return null;
        }

        private void UpdateEditorVisuals()
        {
            this.UpdateCurrentLineHighlight();
            this.UpdateLineNumbers();
            this.UpdateStatus();
        }

        #region Text per Doppelklick markieren
        private void Editor_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            int caret = Editor.CaretIndex;

            if (caret >= Editor.Text.Length)
                return;

            string text = Editor.Text;

            if (char.IsWhiteSpace(text[caret]))
                return;

            int start = caret;
            int end = caret;

            while (start > 0 && !char.IsWhiteSpace(text[start - 1]))
                start--;

            while (end < text.Length && !char.IsWhiteSpace(text[end]))
                end++;

            Editor.SelectionStart = start;
            Editor.SelectionLength = end - start;

            e.Handled = true;
        }
        #endregion Text per Doppelklick markieren

        #region Bereich für Kontextmenü
        private void InsertCurrentDate_Click(object sender, RoutedEventArgs e)
        {
            this.InsertCurrentDate();
        }

        private void WrapWithStar_Click(object sender, RoutedEventArgs e)
        {
            MenuItem menuItem = sender as MenuItem;
            if (menuItem != null)
            {
                string parameter = menuItem.Tag.ToString();
                string left = parameter.Split(',')[0];
                string right = parameter.Split(',')[1];
                this.WrapSelection(left, right);
            }
        }

        private void InsertCurrentDate()
        {
            string dateText = DateTime.Now.ToString("dd.MM.yyyy",CultureInfo.CurrentCulture);

            int caret = Editor.CaretIndex;

            if (Editor.SelectionLength > 0)
            {
                caret = Editor.SelectionStart;
                Editor.Text = Editor.Text.Remove(Editor.SelectionStart, Editor.SelectionLength);
            }

            Editor.Text = Editor.Text.Insert(caret, dateText);
            Editor.CaretIndex = caret + dateText.Length;
        }

        private void InsertPlaceholder()
        {
            string dateText = "[[placeholder]]";

            int caret = Editor.CaretIndex;

            if (Editor.SelectionLength > 0)
            {
                caret = Editor.SelectionStart;
                Editor.Text = Editor.Text.Remove(Editor.SelectionStart, Editor.SelectionLength);
            }

            Editor.Text = Editor.Text.Insert(caret, dateText);
            Editor.CaretIndex = caret + dateText.Length;
        }

        private void WrapSelection(string wrapperLeft, string wrapperRight)
        {
            int start = Editor.SelectionStart;
            int length = Editor.SelectionLength;

            if (length == 0)
            {
                return;
            }

            string selectedText = Editor.SelectedText;
            string newText = string.Empty;
            if (wrapperLeft == "/*" && wrapperRight == "*/")
            {
                newText = $"{wrapperLeft}\n{selectedText}\n{wrapperRight}";
            }
            else
            {
                newText = $"{wrapperLeft}{selectedText}{wrapperRight}";
            }

            Editor.Text = Editor.Text.Remove(start, length);
            Editor.Text = Editor.Text.Insert(start, newText);

            Editor.SelectionStart = start;
            Editor.SelectionLength = newText.Length;
        }
        #endregion Bereich für Kontextmenü
    }

    public class EditorRelayCommand : ICommand
    {
        private readonly Action<object> execute;
        private readonly Predicate<object> canExecute;

        public EditorRelayCommand(Action<object> execute, Predicate<object> canExecute = null)
        {
            this.execute = execute;
            this.canExecute = canExecute;
        }

        public bool CanExecute(object parameter)
        {
            return canExecute == null || canExecute(parameter);
        }

        // Weiterleitung an CommandManager verhindert CS0067 und ermöglicht automatische Requery
        public event EventHandler CanExecuteChanged
        {
            add { CommandManager.RequerySuggested += value; }
            remove { CommandManager.RequerySuggested -= value; }
        }

        public void Execute(object parameter)
        {
            execute(parameter);
        }
    }
}
