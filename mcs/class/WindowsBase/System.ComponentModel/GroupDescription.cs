// Permission is hereby granted, free of charge, to any person obtaining
// a copy of this software and associated documentation files (the
// "Software"), to deal in the Software without restriction, including
// without limitation the rights to use, copy, modify, merge, publish,
// distribute, sublicense, and/or sell copies of the Software, and to
// permit persons to whom the Software is furnished to do so, subject to
// the following conditions:
// 
// The above copyright notice and this permission notice shall be
// included in all copies or substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND,
// EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF
// MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND
// NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE
// LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION
// OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION
// WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
//
// Copyright (c) 2008 Novell, Inc. (http://www.novell.com)
//
// Authors:
//	Chris Toshok (toshok@ximian.com)
//

using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Globalization;

namespace System.ComponentModel {

    /// <summary>
    /// Provides an abstract base class for types that describe how to divide the items in a collection into groups.
    /// </summary>
	public abstract class GroupDescription : INotifyPropertyChanged
	{
		readonly ObservableCollection<object> groupNames;

        /// <summary>
        /// Initializes a new instance of the GroupDescription class.
        /// </summary>
		protected GroupDescription ()
		{
			groupNames = new ObservableCollection<object> ();
            groupNames.CollectionChanged += OnGroupNamesChanged;
		}

        /// <summary>
        /// Gets the collection of names that are used to initialize a group with a set of subgroups with the given names.
        /// </summary>
		public ObservableCollection<object> GroupNames {
			get { return groupNames; }
		}

        /// <summary>
        /// Occurs when a property value changes.
        /// </summary>
		event PropertyChangedEventHandler INotifyPropertyChanged.PropertyChanged {
			add { PropertyChanged += value; }
			remove { PropertyChanged -= value; }
		}

        /// <summary>
        /// Occurs when a property value changes.
        /// </summary>
        protected internal virtual event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        /// Returns a value that indicates whether the group name and the item name match such that the item belongs to the group.
        /// </summary>
        /// <param name="groupName">The name of the group to check.</param>
        /// <param name="itemName">The name of the item to check.</param>
		public virtual bool NamesMatch (object groupName, object itemName)
		{
			return Equals (groupName, itemName);
		}

        /// <summary>
        /// Raises the <see cref="PropertyChanged"/> event.
        /// </summary>
		protected virtual void OnPropertyChanged (PropertyChangedEventArgs e)
		{
			if (PropertyChanged != null)
				PropertyChanged (this, e);
		}

        /// <summary>
        /// Returns whether serialization processes should serialize the effective value of the <see cref="GroupNames"/> property on instances of this class.
        /// </summary>
		[EditorBrowsable (EditorBrowsableState.Never)]
		public bool ShouldSerializeGroupNames ()
		{
			return GroupNames.Count != 0;
		}

        /// <summary>
        /// Returns the group name(s) for the given item.
        /// </summary>
        /// <param name="item">The item to return group names for.</param>
        /// <param name="level">The level of grouping.</param>
        /// <param name="culture">The <see cref="System.Globalization.CultureInfo"/> to supply to the converter.</param>
		public abstract object GroupNameFromItem (object item, int level, CultureInfo culture);

        private void OnGroupNamesChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            this.OnPropertyChanged(new PropertyChangedEventArgs("GroupNames"));
        }
	}
}
