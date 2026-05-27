//-----------------------------------------------------------------------
// <copyright file="FilteredObservableCollection.cs" company="Lifeprojects.de">
//     Class: FilteredObservableCollection<T>
//     Copyright © Lifeprojects.de GmbH 2026
// </copyright>
//
// <author>Gerhard Ahrens - Lifeprojects.de</author>
// <email>developer@lifeprojects.de</email>
// <date>27.05.2026</date>
//
// <summary>
// Die Klasse 'FilteredObservableCollection<T>' ist ein Wrapper um die ObservableCollection<T> und implementiert zusätzlich die INotifyPropertyChanged-Schnittstelle.
// Diese Variante biet auch die Möglichkeit einen Filter zu setzen, um die angezeigten Elemente zu steuern.
// </summary>
//-----------------------------------------------------------------------

namespace SnippetManager.Core
{
    using System;
    using System.Collections.ObjectModel;
    using System.Collections.Specialized;
    using System.ComponentModel;

    public class FilteredObservableCollection<T> : ObservableCollection<T>
    {
        private readonly ObservableCollection<T> _originalCollection;
        private Func<T, bool> _filter;

        public FilteredObservableCollection(ObservableCollection<T> originalCollection, Func<T, bool> filter)
        {
            _originalCollection = originalCollection ?? throw new ArgumentNullException(nameof(originalCollection));
            _filter = filter ?? throw new ArgumentNullException(nameof(filter));

            _originalCollection.CollectionChanged += OnOriginalCollectionChanged;

            // Event-Handler für bereits existierende Elemente registrieren
            RegisterPropertyChangedHandler(_originalCollection);

            Refilter();
        }

        public Func<T, bool> Filter
        {
            get => _filter;
            set
            {
                if (_filter != value)
                {
                    _filter = value ?? throw new ArgumentNullException(nameof(value));
                    Refilter();
                }
            }
        }

        public void Refilter()
        {
            this.Clear();
            foreach (var item in _originalCollection.Where(_filter))
            {
                this.Add(item);
            }
        }

        private void OnOriginalCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            switch (e.Action)
            {
                case NotifyCollectionChangedAction.Add:
                    if (e.NewItems != null)
                    {
                        RegisterPropertyChangedHandler(e.NewItems);
                        foreach (T newItem in e.NewItems)
                        {
                            if (_filter(newItem)) this.Add(newItem);
                        }
                    }
                    break;

                case NotifyCollectionChangedAction.Remove:
                    if (e.OldItems != null)
                    {
                        UnregisterPropertyChangedHandler(e.OldItems);
                        foreach (T oldItem in e.OldItems) this.Remove(oldItem);
                    }
                    break;

                case NotifyCollectionChangedAction.Replace:
                    if (e.OldItems != null)
                    {
                        UnregisterPropertyChangedHandler(e.OldItems);
                        foreach (T oldItem in e.OldItems) this.Remove(oldItem);
                    }
                    if (e.NewItems != null)
                    {
                        RegisterPropertyChangedHandler(e.NewItems);
                        foreach (T newItem in e.NewItems)
                        {
                            if (_filter(newItem)) this.Add(newItem);
                        }
                    }
                    break;

                case NotifyCollectionChangedAction.Reset:
                    // Bei Reset müssen alle alten Handler entfernt werden
                    // Da wir nicht wissen, was vorher drin war, versuchen wir alle bekannten zu bereinigen
                    UnregisterPropertyChangedHandler(this);
                    Refilter();
                    RegisterPropertyChangedHandler(_originalCollection);
                    break;
            }
        }

        // Höre auf Änderungen im Objekt
        private void OnItemPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (sender is T item)
            {
                bool passtInFilter = _filter(item);
                bool istBereitsInAnsicht = this.Contains(item);

                if (passtInFilter && !istBereitsInAnsicht)
                {
                    this.Add(item); // Element erfüllt jetzt den Filter -> Hinzufügen
                }
                else if (!passtInFilter && istBereitsInAnsicht)
                {
                    this.Remove(item); // Element erfüllt den Filter nicht mehr -> Entfernen
                }
            }
        }

        // Hilfsmethoden zum Registrieren der Events
        private void RegisterPropertyChangedHandler(System.Collections.IEnumerable items)
        {
            foreach (var item in items)
            {
                if (item is INotifyPropertyChanged npc)
                {
                    npc.PropertyChanged += OnItemPropertyChanged;
                }
            }
        }

        // Hilfsmethoden zum Entfernen der Events (wichtig gegen Speicherlecks!)
        private void UnregisterPropertyChangedHandler(System.Collections.IEnumerable items)
        {
            foreach (var item in items)
            {
                if (item is INotifyPropertyChanged npc)
                {
                    npc.PropertyChanged -= OnItemPropertyChanged;
                }
            }
        }
    }
}
