namespace SnippetManager.Core
{
    using System.ComponentModel;
    using System.Diagnostics;
    using System.Runtime.CompilerServices;
    using System.Windows.Media;

    [DebuggerDisplay("Title: {this.Title}; IsSelectedItem: {this.IsSelectedItem}")]
    public class XamlTileItem : INotifyPropertyChanged
    {
        private string key;
        private string title;
        private string xamlTyp;
        private string toolTip;
        private string quelle;
        private ImageSource imageContent;
        private bool isSelectedItem;
        public event PropertyChangedEventHandler PropertyChanged;

        public string Key
        {
            get => this.key;
            set
            {
                if (this.key != value)
                {
                    this.key = value;
                    this.OnPropertyChanged();
                }
            }
        }

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

        public string XamlTyp
        {
            get => this.xamlTyp;
            set
            {
                if (this.xamlTyp != value)
                {
                    this.xamlTyp = value;
                    this.OnPropertyChanged();
                }
            }
        }

        public string Tooltip
        {
            get => this.toolTip;
            set
            {
                if (this.toolTip != value)
                {
                    this.toolTip = value;
                    this.OnPropertyChanged();
                }
            }
        }

        public string Quelle
        {
            get => this.quelle;
            set
            {
                if (this.quelle != value)
                {
                    this.quelle = value;
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
