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

using System.Collections;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Globalization;

namespace System.ComponentModel {

    /// <summary>
    /// Enables collections to have the functionalities of current record management, custom sorting, filtering, and grouping.
    /// </summary>
	public interface ICollectionView : IEnumerable, INotifyCollectionChanged
	{
        /// <summary>
        /// Gets a value that indicates whether this view supports filtering via the <see cref="Filter"/> property.
        /// </summary>
		bool CanFilter { get; }

        /// <summary>
        /// Gets a value that indicates whether this view supports grouping via the <see cref="GroupDescriptions"/> property.
        /// </summary>
		bool CanGroup { get; }

        /// <summary>
        /// Gets a value that indicates whether this view supports sorting via the <see cref="SortDescriptions"/> property.
        /// </summary>
		bool CanSort { get; }

        /// <summary>
        /// Gets or sets the cultural info for any operations of the view that may differ by culture, such as sorting.
        /// </summary>
		CultureInfo Culture { get; set; }

        /// <summary>
        /// Gets the current item in the view.
        /// </summary>
		object CurrentItem { get; }

        /// <summary>
        /// Gets the ordinal position of the <see cref="CurrentItem"/> within the view.
        /// </summary>
		int CurrentPosition { get; }

        /// <summary>
        /// Gets or sets a callback used to determine if an item is suitable for inclusion in the view.
        /// </summary>
		Predicate<object> Filter { get; set; }

        /// <summary>
        /// Gets a collection of <see cref="GroupDescription"/> objects that describe how the items in the collection are grouped in the view.
        /// </summary>
		ObservableCollection<GroupDescription> GroupDescriptions { get; }

        /// <summary>
        /// Gets the top-level groups.
        /// </summary>
		ReadOnlyObservableCollection<object> Groups { get; }

        /// <summary>
        /// Gets a value that indicates whether the <see cref="CurrentItem"/> of the view is beyond the end of the collection.
        /// </summary>
		bool IsCurrentAfterLast { get; }

        /// <summary>
        /// Gets a value that indicates whether the <see cref="CurrentItem"/> of the view is beyond the beginning of the collection.
        /// </summary>
		bool IsCurrentBeforeFirst { get; }

        /// <summary>
        /// Returns a value that indicates whether the resulting view is empty.
        /// </summary>
		bool IsEmpty { get; }

        /// <summary>
        /// Gets a collection of <see cref="SortDescription"/> objects that describe how the items in the collection are sorted in the view.
        /// </summary>
		SortDescriptionCollection SortDescriptions { get; }

        /// <summary>
        /// Returns the underlying collection.
        /// </summary>
		IEnumerable SourceCollection { get; }

        /// <summary>
        /// When implementing this interface, raise this event after the current item has been changed.
        /// </summary>
		event EventHandler CurrentChanged;

        /// <summary>
        /// When implementing this interface, raise this event before changing the current item. Event handler can cancel this event.
        /// </summary>
		event CurrentChangingEventHandler CurrentChanging;

        /// <summary>
        /// Returns a value that indicates whether a given item belongs to this collection view.
        /// </summary>
        /// <param name="item">The object to check.</param>
        /// <returns></returns>
		bool Contains (object item);

        /// <summary>
        /// Enters a defer cycle that you can use to merge changes to the view and delay automatic refresh.
        /// </summary>
        /// <returns>An <see cref="IDisposable"/> object that you can use to dispose of the calling object.</returns>
		IDisposable DeferRefresh ();

        /// <summary>
        /// Sets the specified item to be the <see cref="CurrentItem"/> in the view.
        /// </summary>
        /// <param name="item">The item to set as the <see cref="CurrentItem"/>.</param>
        /// <returns>true if the resulting <see cref="CurrentItem"/> is within the view; otherwise, false.</returns>
		bool MoveCurrentTo (object item);

        /// <summary>
        /// Sets the first item in the view as the <see cref="CurrentItem"/>.
        /// </summary>
        /// <returns>true if the resulting <see cref="CurrentItem"/> is within the view; otherwise, false.</returns>
		bool MoveCurrentToFirst ();

        /// <summary>
        /// Sets the last item in the view as the <see cref="CurrentItem"/>.
        /// </summary>
        /// <returns>true if the resulting <see cref="CurrentItem"/> is within the view; otherwise, false.</returns>
		bool MoveCurrentToLast ();

        /// <summary>
        /// Sets the item after the <see cref="CurrentItem"/> in the view as the <see cref="CurrentItem"/>.
        /// </summary>
        /// <returns>true if the resulting <see cref="CurrentItem"/> is within the view; otherwise, false.</returns>
		bool MoveCurrentToNext ();

        /// <summary>
        /// Sets the item at the specified index to be the <see cref="CurrentItem"/> in the view.
        /// </summary>
        /// <param name="position">The index to set the <see cref="CurrentItem"/> to.</param>
        /// <returns>true if the resulting <see cref="CurrentItem"/> is within the view; otherwise, false.</returns>
		bool MoveCurrentToPosition (int position);

        /// <summary>
        /// Sets the item before the <see cref="CurrentItem"/> in the view as the <see cref="CurrentItem"/>.
        /// </summary>
        /// <returns>true if the resulting <see cref="CurrentItem"/> is within the view; otherwise, false.</returns>
		bool MoveCurrentToPrevious ();

        /// <summary>
        /// Recreates the view.
        /// </summary>
		void Refresh ();
	}
}