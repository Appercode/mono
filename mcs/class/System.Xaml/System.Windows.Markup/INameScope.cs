//
// Copyright (C) 2010 Novell Inc. http://novell.com
//
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
using System;
using System.Collections.Generic;

namespace System.Windows.Markup
{
    /// <summary>Defines a contract for how names of elements should be accessed within a particular namescope, and how to enforce uniqueness of names within that scope. </summary>
	public interface INameScope
	{
        /// <summary>Returns an object that has the provided identifying name. </summary>
        /// <returns>The object, if found. Returns null if no object of that name was found.</returns>
        /// <param name="name">The name identifier for the object being requested.</param>
		object FindName (string name);

        /// <summary>Registers the provided name into the current namescope. </summary>
        /// <param name="name">Name to register.</param>
        /// <param name="scopedElement">The specific element that the provided <paramref name="name" /> refers to.</param>
		void RegisterName (string name, object scopedElement);

        /// <summary>Unregisters the provided name from the current namescope. </summary>
        /// <param name="name">The name to unregister.</param>
		void UnregisterName (string name);
	}
}
