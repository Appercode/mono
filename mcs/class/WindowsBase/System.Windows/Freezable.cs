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
// Copyright (c) 2007 Novell, Inc. (http://www.novell.com)
//
// Authors:
//	Chris Toshok (toshok@ximian.com)
//

using Appercode.UI.Internals;

namespace System.Windows {

    /// <summary>Defines an object that has a modifiable state and a read-only (frozen) state. Classes that derive from <see cref="T:System.Windows.Freezable" /> provide detailed change notification, can be made immutable, and can clone themselves. </summary>
	public abstract class Freezable : DependencyObject, ISealable {

        [ThreadStatic]
        private static Freezable.EventStorage eventStorage;

        /// <summary>Initializes a new instance of a <see cref="T:System.Windows.Freezable" /> derived class. </summary>
		protected Freezable () {
		}

        /// <summary>Creates a modifiable clone of the <see cref="T:System.Windows.Freezable" />, making deep copies of the object's values. When copying the object's dependency properties, this method copies expressions (which might no longer resolve) but not animations or their current values. </summary>
        /// <returns>A modifiable clone of the current object. The cloned object's <see cref="P:System.Windows.Freezable.IsFrozen" /> property is false even if the source's <see cref="P:System.Windows.Freezable.IsFrozen" /> property is true.</returns>
        public Freezable Clone ()
        {
            this.ReadPreamble();
            Freezable freezable = this.CreateInstance();
            freezable.CloneCore(this);
            return freezable;
        }

        /// <summary>Makes the instance a clone (deep copy) of the specified <see cref="T:System.Windows.Freezable" /> using base (non-animated) property values. </summary>
        /// <param name="sourceFreezable">The object to clone.</param>
        protected virtual void CloneCore (Freezable sourceFreezable)
        {
            this.CloneCoreCommon(sourceFreezable, false, true);
        }

        /// <summary>Creates a modifiable clone (deep copy) of the <see cref="T:System.Windows.Freezable" /> using its current values.</summary>
        /// <returns>A modifiable clone of the current object. The cloned object's <see cref="P:System.Windows.Freezable.IsFrozen" /> property is false even if the source's <see cref="P:System.Windows.Freezable.IsFrozen" /> property is true.</returns>
        public Freezable CloneCurrentValue ()
        {
            this.ReadPreamble();
            Freezable freezable = this.CreateInstance();
            freezable.CloneCurrentValueCore(this);
            return freezable;
        }

        /// <summary>Makes the instance a modifiable clone (deep copy) of the specified <see cref="T:System.Windows.Freezable" /> using current property values.</summary>
        /// <param name="sourceFreezable">The <see cref="T:System.Windows.Freezable" /> to be cloned.</param>
        protected virtual void CloneCurrentValueCore (Freezable sourceFreezable)
        {
            this.CloneCoreCommon(sourceFreezable, true, true);
        }

        /// <summary>Initializes a new instance of the <see cref="T:System.Windows.Freezable" /> class. </summary>
        /// <returns>The new instance.</returns>
        protected Freezable CreateInstance ()
        {
            Freezable freezable = this.CreateInstanceCore();
            return freezable;
        }

        /// <summary>When implemented in a derived class, creates a new instance of the <see cref="T:System.Windows.Freezable" /> derived class. </summary>
        /// <returns>The new instance.</returns>
        protected abstract Freezable CreateInstanceCore ();

        /// <summary>Makes the current object unmodifiable and sets its <see cref="P:System.Windows.Freezable.IsFrozen" /> property to true. </summary>
        /// <exception cref="T:System.InvalidOperationException">The <see cref="T:System.Windows.Freezable" /> cannot be made unmodifiable. </exception>
        public void Freeze ()
        {
            if (!this.CanFreeze)
            {
                throw new InvalidOperationException("Freezable_CantFreeze");
            }
            this.Freeze(false);
        }

        /// <summary>If the <paramref name="isChecking" /> parameter is true, this method indicates whether the specified <see cref="T:System.Windows.Freezable" /> can be made unmodifiable. If the <paramref name="isChecking" /> parameter is false, this method attempts to make the specified <see cref="T:System.Windows.Freezable" /> unmodifiable and indicates whether the operation succeeded.</summary>
        /// <returns>If <paramref name="isChecking" /> is true, this method returns true if the specified <see cref="T:System.Windows.Freezable" /> can be made unmodifiable, or false if it cannot be made unmodifiable. If <paramref name="isChecking" /> is false, this method returns true if the specified <see cref="T:System.Windows.Freezable" /> is now unmodifiable, or false if it cannot be made unmodifiable. </returns>
        /// <param name="freezable">The object to check or make unmodifiable. If <paramref name="isChecking" /> is true, the object is checked to determine whether it can be made unmodifiable. If <paramref name="isChecking" /> is false, the object is made unmodifiable, if possible.</param>
        /// <param name="isChecking">true to return an indication of whether the object can be frozen (without actually freezing it); false to actually freeze the object.</param>
        /// <exception cref="T:System.InvalidOperationException">When <paramref name="isChecking" /> is false, the attempt to make <paramref name="freezable" /> unmodifiable was unsuccessful; the object is now in an unknown state (it might be partially frozen).  </exception>
        protected internal static bool Freeze (Freezable freezable,
            bool isChecking)
        {
            if (freezable == null)
            {
                return true;
            }
            return freezable.Freeze(isChecking);
        }

        /// <summary>Makes the <see cref="T:System.Windows.Freezable" /> object unmodifiable or tests whether it can be made unmodifiable.</summary>
        /// <returns>If <paramref name="isChecking" /> is true, this method returns true if the <see cref="T:System.Windows.Freezable" /> can be made unmodifiable, or false if it cannot be made unmodifiable. If <paramref name="isChecking" /> is false, this method returns true if the if the specified <see cref="T:System.Windows.Freezable" /> is now unmodifiable, or false if it cannot be made unmodifiable. </returns>
        /// <param name="isChecking">true to return an indication of whether the object can be frozen (without actually freezing it); false to actually freeze the object.</param>
		protected virtual bool FreezeCore (bool isChecking)
		{
			throw new NotImplementedException ();
		}

        /// <summary>Creates a frozen copy of the <see cref="T:System.Windows.Freezable" />, using base (non-animated) property values. Because the copy is frozen, any frozen sub-objects are copied by reference. </summary>
        /// <returns>A frozen copy of the <see cref="T:System.Windows.Freezable" />. The copy's <see cref="P:System.Windows.Freezable.IsFrozen" /> property is set to true. </returns>
        /// <exception cref="T:System.InvalidOperationException">The <see cref="T:System.Windows.Freezable" /> cannot be frozen because it contains expressions or animated properties.</exception>
        public Freezable GetAsFrozen ()
        {
            this.ReadPreamble();
            if (this.IsFrozenInternal)
            {
                return this;
            }
            Freezable freezable = this.CreateInstance();
            freezable.GetAsFrozenCore(this);
            freezable.Freeze();
            return freezable;
        }

        /// <summary>Makes the instance a frozen clone of the specified <see cref="T:System.Windows.Freezable" /> using base (non-animated) property values.</summary>
        /// <param name="sourceFreezable">The instance to copy.</param>
        protected virtual void GetAsFrozenCore (Freezable sourceFreezable)
        {
            this.CloneCoreCommon(sourceFreezable, false, false);
        }

        /// <summary>Creates a frozen copy of the <see cref="T:System.Windows.Freezable" /> using current property values. Because the copy is frozen, any frozen sub-objects are copied by reference.</summary>
        /// <returns>A frozen copy of the <see cref="T:System.Windows.Freezable" />. The copy's <see cref="P:System.Windows.Freezable.IsFrozen" /> property is set to true.</returns>
        public Freezable GetCurrentValueAsFrozen ()
        {
            this.ReadPreamble();
            if (this.IsFrozenInternal)
            {
                return this;
            }
            Freezable freezable = this.CreateInstance();
            freezable.GetCurrentValueAsFrozenCore(this);
            freezable.Freeze();
            return freezable;
        }

        /// <summary>Makes the current instance a frozen clone of the specified <see cref="T:System.Windows.Freezable" />. If the object has animated dependency properties, their current animated values are copied.</summary>
        /// <param name="sourceFreezable">The <see cref="T:System.Windows.Freezable" /> to copy and freeze.</param>
        protected virtual void GetCurrentValueAsFrozenCore (Freezable sourceFreezable)
        {
            this.CloneCoreCommon(sourceFreezable, true, false);
        }

        /// <summary>Called when the current <see cref="T:System.Windows.Freezable" /> object is modified. </summary>
        protected virtual void OnChanged ()
        {
        }

        /// <summary>Ensures that appropriate context pointers are established for a <see cref="T:System.Windows.DependencyObjectType" /> data member that has just been set.</summary>
        /// <param name="oldValue">The previous value of the data member.</param>
        /// <param name="newValue">The current value of the data member.</param>
        protected void OnFreezablePropertyChanged (DependencyObject oldValue,
            DependencyObject newValue)
        {
            this.OnFreezablePropertyChanged(oldValue, newValue, null);
        }

        /// <summary>This member supports the Windows Presentation Foundation (WPF) infrastructure and is not intended to be used directly from your code.</summary>
        /// <param name="oldValue"></param>
        /// <param name="newValue"></param>
        /// <param name="property"></param>
        protected void OnFreezablePropertyChanged (DependencyObject oldValue,
            DependencyObject newValue,
            DependencyProperty property)
        {
            if (newValue != null)
            {
                Freezable.EnsureConsistentDispatchers(this, newValue);
            }
            if (oldValue != null)
            {
                throw new NotImplementedException();
            }
            if (newValue != null)
            {
                throw new NotImplementedException();
            }
        }

        /// <summary>Overrides the <see cref="T:System.Windows.DependencyObject" /> implementation of <see cref="M:System.Windows.DependencyObject.OnPropertyChanged(System.Windows.DependencyPropertyChangedEventArgs)" /> to also invoke any <see cref="E:System.Windows.Freezable.Changed" /> handlers in response to a changing dependency property of type <see cref="T:System.Windows.Freezable" />.</summary>
        /// <param name="e">Event data that contains information about which property changed, and its old and new values.</param>
        protected override void OnPropertyChanged (DependencyPropertyChangedEventArgs e)
        {
            base.OnPropertyChanged(e);
        }

        /// <summary>Ensures that the <see cref="T:System.Windows.Freezable" /> is being accessed from a valid thread. Inheritors of <see cref="T:System.Windows.Freezable" /> must call this method at the beginning of any API that reads data members that are not dependency properties.</summary>
        protected void ReadPreamble ()
        {
            this.VerifyAccess();
        }

        /// <summary>Raises the <see cref="E:System.Windows.Freezable.Changed" /> event for the <see cref="T:System.Windows.Freezable" /> and invokes its <see cref="M:System.Windows.Freezable.OnChanged" /> method. Classes that derive from <see cref="T:System.Windows.Freezable" /> should call this method at the end of any API that modifies class members that are not stored as dependency properties.</summary>
        protected void WritePostscript ()
        {
            this.FireChanged();
        }

        /// <summary>Verifies that the <see cref="T:System.Windows.Freezable" /> is not frozen and that it is being accessed from a valid threading context. <see cref="T:System.Windows.Freezable" /> inheritors should call this method at the beginning of any API that writes to data members that are not dependency properties. </summary>
        /// <exception cref="T:System.InvalidOperationException">The <see cref="T:System.Windows.Freezable" /> instance is frozen and cannot have its members written to.</exception>
        protected void WritePreamble ()
        {
            ////this.VerifyAccess();
            if (this.IsFrozenInternal)
            {
                throw new InvalidOperationException(string.Format("{0} can't be frozen", this.GetType().FullName));
            }
        }

        /// <summary>Gets a value that indicates whether the object can be made unmodifiable. </summary>
        /// <returns>true if the current object can be made unmodifiable or is already unmodifiable; otherwise, false.</returns>
        public bool CanFreeze {
            get
            {
                if (IsFrozenInternal)
                {
                    return true;
                }
                return FreezeCore (true);
            }
        }

        /// <summary>Gets a value that indicates whether the object is currently modifiable. </summary>
        /// <returns>true if the object is frozen and cannot be modified; false if the object can be modified.</returns>
        public bool IsFrozen {
            get {
                this.ReadPreamble();
                return this.IsFrozenInternal;
            }
        }

        /// <summary>Occurs when the <see cref="T:System.Windows.Freezable" /> or an object it contains is modified. </summary>
        public event EventHandler Changed
        {
            add
            {
                this.WritePreamble();
                if (value != null)
                {
                    this.ChangedInternal += value;
                }
            }
            remove
            {
                this.WritePreamble();
                if (value != null)
                {
                    this.ChangedInternal -= value;
                }
            }
        }

        internal event EventHandler ChangedInternal
        {
            add
            {
                this.HandlerAdd(value);
            }
            remove
            {
                this.HandlerRemove(value);
            }
        }

        bool ISealable.CanSeal
        {
            get
            {
                return this.CanFreeze;
            }
        }

        bool ISealable.IsSealed
        {
            get
            {
                return this.IsFrozen;
            }
        }

        internal bool HasMultipleInheritanceContexts
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        internal override DependencyObject InheritanceContext
        {
            get
            {
                return null;
            }
        }

        internal bool IsFrozenInternal
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        private Freezable.EventStorage CachedEventStorage
        {
            get
            {
                if (Freezable.eventStorage == null)
                {
                    Freezable.eventStorage = new Freezable.EventStorage(4);
                }
                return Freezable.eventStorage;
            }
        }

        /*
        //private FrugalObjectList<Freezable.FreezableContextPair> ContextList
        //{
        //    get
        //    {
        //        if (!this.HasHandlers)
        //        {
        //            return (FrugalObjectList<Freezable.FreezableContextPair>)this._contextStorage;
        //        }
        //        return (FrugalObjectList<Freezable.FreezableContextPair>)((Freezable.HandlerContextStorage)this._contextStorage)._contextStorage;
        //    }
        //    set
        //    {
        //        if (!this.HasHandlers)
        //        {
        //            this._contextStorage = value;
        //            return;
        //        }
        //        ((Freezable.HandlerContextStorage)this._contextStorage)._contextStorage = value;
        //    }
        //}
        */

        /*
        //private FrugalObjectList<EventHandler> HandlerList
        //{
        //    get
        //    {
        //        if (!this.HasContextInformation)
        //        {
        //            return (FrugalObjectList<EventHandler>)this._contextStorage;
        //        }
        //        return (FrugalObjectList<EventHandler>)((Freezable.HandlerContextStorage)this._contextStorage)._handlerStorage;
        //    }
        //}
        */

        private bool HasContextInformation
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        private bool HasHandlers
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        private DependencyObject SingletonContext
        {
            get
            {
                if (!this.HasHandlers)
                {
                    return (DependencyObject)this.ContextStorage;
                }
                return (DependencyObject)((Freezable.HandlerContextStorage)this.ContextStorage).ContextStorage;
            }
        }

        private DependencyProperty SingletonContextProperty
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        private EventHandler SingletonHandler
        {
            get
            {
                if (!this.HasContextInformation)
                {
                    return (EventHandler)this.ContextStorage;
                }
                return (EventHandler)((Freezable.HandlerContextStorage)this.ContextStorage).HandlerStorage;
            }
        }

        internal bool Freeze(bool isChecking)
        {
            if (isChecking)
            {
                this.ReadPreamble();
                return this.FreezeCore(true);
            }
            if (!this.IsFrozenInternal)
            {
                this.WritePreamble();
                this.FreezeCore(false);
                PropertyMetadata.RemoveAllCachedDefaultValues(this);
                this.DetachFromDispatcher();
                this.FireChanged();
                this.ClearContextAndHandlers();
                this.WritePostscript();
            }
            return true;
        }

        void ISealable.Seal()
        {
            this.Freeze();
        }

        internal void AddContextInformation(DependencyObject context, DependencyProperty property)
        {
            throw new NotImplementedException();
        }

        internal void AddInheritanceContext(DependencyObject context, DependencyProperty property)
        {
            if (!this.IsFrozenInternal)
            {
                DependencyObject inheritanceContext = this.InheritanceContext;
                this.AddContextInformation(context, property);
                if (inheritanceContext != this.InheritanceContext)
                {
                    throw new NotImplementedException();
                }
            }
        }

        internal void ClearContextAndHandlers()
        {
            throw new NotImplementedException();
        }

        internal void FireChanged()
        {
            Freezable.EventStorage eventStorage = null;
            this.GetChangeHandlersAndInvalidateSubProperties(ref eventStorage);
            if (eventStorage != null)
            {
                int num = 0;
                int count = eventStorage.Count;
                while (num < count)
                {
                    eventStorage[num](this, EventArgs.Empty);
                    eventStorage[num] = null;
                    num++;
                }
                eventStorage.Clear();
                eventStorage.InUse = false;
            }
        }

        internal void RemoveInheritanceContext(DependencyObject context, DependencyProperty property)
        {
            if (!this.IsFrozenInternal)
            {
                DependencyObject inheritanceContext = this.InheritanceContext;
                this.RemoveContextInformation(context, property);
                if (inheritanceContext != this.InheritanceContext)
                {
                    throw new NotImplementedException();
                }
            }
        }

        internal override void Seal()
        {
        }

        private static void EnsureConsistentDispatchers(DependencyObject owner, DependencyObject child)
        {
            if (owner.Dispatcher != null && child.Dispatcher != null && owner.Dispatcher != child.Dispatcher)
            {
                throw new InvalidOperationException("Freezable_AttemptToUseInnerValueWithDifferentThread");
            }
        }

        private void AddContextToList(DependencyObject context, DependencyProperty property)
        {
            throw new NotImplementedException();
        }

        private void AddSingletonContext(DependencyObject context, DependencyProperty property)
        {
            throw new NotImplementedException();
        }

        private void AddSingletonHandler(EventHandler handler)
        {
            throw new NotImplementedException();
        }

        private void CloneCoreCommon(Freezable sourceFreezable, bool useCurrentValue, bool cloneFrozenValues)
        {
            throw new NotImplementedException();
        }

        private void ConvertToContextList()
        {
            throw new NotImplementedException();
        }

        private void ConvertToHandlerList()
        {
            throw new NotImplementedException();
        }

        private void GetChangeHandlersAndInvalidateSubProperties(ref Freezable.EventStorage calledHandlers)
        {
            throw new NotImplementedException();
        }

        private Freezable.EventStorage GetEventStorage()
        {
            Freezable.EventStorage cachedEventStorage = this.CachedEventStorage;
            if (cachedEventStorage.InUse)
            {
                cachedEventStorage = new Freezable.EventStorage(cachedEventStorage.PhysicalSize);
            }
            cachedEventStorage.InUse = true;
            return cachedEventStorage;
        }

        private void GetHandlers(ref Freezable.EventStorage calledHandlers)
        {
            throw new NotImplementedException();
        }

        private void HandlerAdd(EventHandler handler)
        {
            throw new NotImplementedException();
        }

        private void HandlerRemove(EventHandler handler)
        {
            throw new NotImplementedException();
        }

        /*
        //private void PruneContexts(FrugalObjectList<Freezable.FreezableContextPair> oldList, int numDead)
        //{
        //    int count = oldList.Count;
        //    if (count - numDead == 0)
        //    {
        //        this.RemoveContextList();
        //        return;
        //    }
        //    if (numDead > 0)
        //    {
        //        FrugalObjectList<Freezable.FreezableContextPair> frugalObjectList = new FrugalObjectList<Freezable.FreezableContextPair>(count - numDead);
        //        for (int i = 0; i < count; i++)
        //        {
        //            if (oldList[i].Owner.IsAlive)
        //            {
        //                frugalObjectList.Add(oldList[i]);
        //            }
        //        }
        //        this.ContextList = frugalObjectList;
        //    }
        //}
        */

        private void RemoveContextInformation(DependencyObject context, DependencyProperty property)
        {
            throw new NotImplementedException();
        }

        private void RemoveContextList()
        {
            if (!this.HasHandlers)
            {
                this.ContextStorage = null;
            }
            else
            {
                this.ContextStorage = ((Freezable.HandlerContextStorage)this.ContextStorage).HandlerStorage;
            }
        }

        private void RemoveHandlerList()
        {
            if (!this.HasContextInformation)
            {
                this.ContextStorage = null;
            }
            else
            {
                this.ContextStorage = ((Freezable.HandlerContextStorage)this.ContextStorage).ContextStorage;
            }
        }

        private void RemoveSingletonContext()
        {
            if (!this.HasHandlers)
            {
                this.ContextStorage = null;
            }
            else
            {
                this.ContextStorage = ((Freezable.HandlerContextStorage)this.ContextStorage).HandlerStorage;
            }
        }

        private void RemoveSingletonHandler()
        {
            if (!this.HasContextInformation)
            {
                this.ContextStorage = null;
            }
            else
            {
                this.ContextStorage = ((Freezable.HandlerContextStorage)this.ContextStorage).ContextStorage;
            }
        }

        private struct FreezableContextPair
        {
            public readonly WeakReference Owner;

            public readonly DependencyProperty Property;

            public FreezableContextPair(DependencyObject dependObject, DependencyProperty dependProperty)
            {
                this.Owner = new WeakReference(dependObject);
                this.Property = dependProperty;
            }
        }

        private class EventStorage
        {
            private EventHandler[] events;

            private int logSize;

            private int physSize;

            private bool inUse;

            public EventStorage(int initialSize)
            {
                if (initialSize <= 0)
                {
                    initialSize = 1;
                }
                this.events = new EventHandler[initialSize];
                this.logSize = 0;
                this.physSize = initialSize;
                this.inUse = false;
            }

            public int Count
            {
                get
                {
                    return this.logSize;
                }
            }

            public bool InUse
            {
                get
                {
                    return this.inUse;
                }
                set
                {
                    this.inUse = value;
                }
            }

            public int PhysicalSize
            {
                get
                {
                    return this.physSize;
                }
            }

            public EventHandler this[int idx]
            {
                get
                {
                    return this.events[idx];
                }
                set
                {
                    this.events[idx] = value;
                }
            }

            public void Add(EventHandler e)
            {
                if (this.logSize == this.physSize)
                {
                    Freezable.EventStorage eventStorage = this;
                    eventStorage.physSize = eventStorage.physSize * 2;
                    EventHandler[] eventHandlerArray = new EventHandler[this.physSize];
                    for (int i = 0; i < this.logSize; i++)
                    {
                        eventHandlerArray[i] = this.events[i];
                    }
                    this.events = eventHandlerArray;
                }
                this.events[this.logSize] = e;
                Freezable.EventStorage eventStorage1 = this;
                eventStorage1.logSize = eventStorage1.logSize + 1;
            }

            public void Clear()
            {
                this.logSize = 0;
            }
        }

        private class HandlerContextStorage
        {
            public object HandlerStorage = null;

            public object ContextStorage = null;

            public HandlerContextStorage()
            {
            }
        }
    }
}
