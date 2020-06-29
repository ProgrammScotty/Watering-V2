using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Text;

namespace Watering2.Utils
{
    public class RangeAddableObservableCollection<T> : ObservableCollection<T>
    {
        public RangeAddableObservableCollection() : base()
        {
        }

        public RangeAddableObservableCollection(IEnumerable<T> items) : base()
        {
            InsertRange(items);
        }

        public void InsertRange(IEnumerable<T> items)
        {
            this.CheckReentrancy();
            int startIndex = Count;

            var enumerable = items as T[] ?? items.ToArray();

            foreach (var item in enumerable)
                this.Items.Add(item);

            //funktioniert hier, da vorher immer die Collection gelöscht wird
            //this.OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));

            //Zu testen
            this.OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add, new List<T>(enumerable), startIndex));
            OnPropertyChanged(new PropertyChangedEventArgs("Count"));
            //OnPropertyChanged(new PropertyChangedEventArgs("Item[]"));
        }
    }

}
