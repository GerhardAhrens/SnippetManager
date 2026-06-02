namespace SnippetManager.Core.Placeholder
{
    using System.Collections.ObjectModel;
    using System.ComponentModel;
    using System.Printing;
    using System.Runtime.CompilerServices;

    public class PlaceholderItem : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        private string _name;
        private object _value;
        private PlaceholderType _type;
        private object _defaultValue;

        public string Name
        {
            get => this._name;
            set
            {
                if (this._name != value)
                {
                    this._name = value;
                    this.OnPropertyChanged();
                }
            }
        }

        public object Value
        {
            get => this._value;
            set
            {
                if (this._value != value)
                {
                    this._value = value;
                    this.OnPropertyChanged();
                }
            }
        }

        public PlaceholderType Type
        {
            get => this._type;
            set
            {
                if (this._type != value)
                {
                    this._type = value;
                    this.OnPropertyChanged();
                }
            }
        }

        public object DefaultValue
        {
            get => this._defaultValue;
            set
            {
                if (this._defaultValue != value)
                {
                    this._defaultValue = value;
                    this.OnPropertyChanged();
                }
            }
        }

        // Für ComboBox
        public ObservableCollection<string> Options { get; set; } = new();

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
