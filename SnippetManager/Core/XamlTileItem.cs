namespace SnippetManager.Core
{
    using System.ComponentModel;
    using System.Diagnostics;
    using System.Runtime.CompilerServices;
    using System.Windows.Media;

    [DebuggerDisplay("Title: {this.Title}; IsSelectedItem: {this.IsSelectedItem}")]
    public class XamlTileItem : INotifyPropertyChanged
    {
        private string title;
        private ImageSource imageContent;
        private bool isSelectedItem;
        public event PropertyChangedEventHandler PropertyChanged;
        public string Title
        {
            get => this.title;
            set
            {
                if (this.title != value)
                {
                    this.title = value;
                    this.OnPropertyChanged();
                }
            }
        }
        public ImageSource ImageContent
        {
            get => this.imageContent;
            set
            {
                if (this.imageContent != value)
                {
                    this.imageContent = value;
                    this.OnPropertyChanged();
                }
            }
        }
        public bool IsSelectedItem
        {
            get => this.isSelectedItem;
            set
            {
                if (this.isSelectedItem != value)
                {
                    this.isSelectedItem = value;
                    this.OnPropertyChanged();
                }
            }
        }
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
