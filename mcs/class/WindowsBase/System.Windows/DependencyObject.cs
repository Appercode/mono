//
// DependencyObject.cs
//
// Author:
//   Iain McCoy (iain@mccoy.id.au)
//   Chris Toshok (toshok@ximian.com)
//
// (C) 2005 Iain McCoy
// (C) 2007 Novell, Inc.
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

using Appercode.UI.Controls;
using Appercode.UI.Data;
using Appercode.UI.Internals;
using System.Collections.Generic;
using System.Threading;
using System.Windows.Threading;

namespace System.Windows {
	public class DependencyObject : DispatcherObject {
        internal object ContextStorage;
        private static readonly Dictionary<Type, Dictionary<string, DependencyProperty>> propertyDeclarations =
            new Dictionary<Type, Dictionary<string, DependencyProperty>>();
        private readonly Dictionary<DependencyProperty, object> properties = new Dictionary<DependencyProperty, object>();
        private readonly Dictionary<DependencyProperty, Expression> expressions = new Dictionary<DependencyProperty, Expression>();
        private bool isSealed = false;

        internal event EventHandler InheritanceContextChanged = delegate { };
        internal event DependencyPropertyChangedEventHandler DPChanged;

        public virtual bool IsSealed
        {
            get { return this.isSealed; }
		}

		public DependencyObjectType DependencyObjectType { 
			get { return DependencyObjectType.FromSystemType (GetType()); }
		}

		public void ClearValue(DependencyProperty dp)
		{
			if (IsSealed)
				throw new InvalidOperationException ("Cannot manipulate property values on a sealed DependencyObject");

            this.properties.Remove(dp);
		}

		public void ClearValue(DependencyPropertyKey key)
		{
			ClearValue (key.DependencyProperty);
		}

		public void CoerceValue (DependencyProperty dp)
		{
			PropertyMetadata pm = dp.GetMetadata (this);
			if (pm.CoerceValueCallback != null)
				pm.CoerceValueCallback (this, GetValue (dp));
		}

		public sealed override bool Equals (object obj)
		{
            return base.Equals(obj);
		}

		public sealed override int GetHashCode ()
		{
            return base.GetHashCode();
		}

		public LocalValueEnumerator GetLocalValueEnumerator()
		{
			return new LocalValueEnumerator(properties);
		}

        public virtual object GetValue(DependencyProperty dp)
        {
            PropertyMetadata metadata = null;
            if (dp.ReadOnly)
            {
                metadata = dp.GetMetadata(GetType());
                var callback = metadata.GetReadOnlyValueCallback;
                if (callback != null)
                {
                    BaseValueSourceInternal valueSource;
                    return callback(this, out valueSource);
                }
            }

            object value;
            if (properties.TryGetValue(dp, out value))
            {
                return value;
            }

            if (dp.ReadOnly == false)
            {
                // have not tried to load metadata yet
                metadata = dp.GetMetadata(GetType());
            }

            if (metadata != null)
            {
                return metadata.DefaultValue;
            }

            return dp.DefaultMetadata.DefaultValue;
        }

		public void InvalidateProperty(DependencyProperty dp)
		{
			throw new NotImplementedException("InvalidateProperty(DependencyProperty dp)");
		}

		protected virtual void OnPropertyChanged(DependencyPropertyChangedEventArgs e)
		{
            PropertyMetadata pm = null;
            if (e.Property.IsAttached)
            {
                pm = e.Property.DefaultMetadata;
            }
            else
            {
                pm = e.Property.GetMetadata(this);
            }
            if (pm != null && pm.PropertyChangedCallback != null && (e.NewValue == null || !e.NewValue.Equals(e.OldValue)))
            {
                pm.PropertyChangedCallback(this, e);
            }
		}

		public object ReadLocalValue(DependencyProperty dp)
		{
            object value;
            if (properties.TryGetValue(dp, out value))
            {
                return value;
            }

            return DependencyProperty.UnsetValue;
		}

		public void SetValue(DependencyProperty dp, object value)
		{
            SetValue(dp, value, false);
		}

		public void SetValue(DependencyPropertyKey key, object value)
		{
			SetValue (key.DependencyProperty, value);
		}

		protected virtual bool ShouldSerializeProperty (DependencyProperty dp)
		{
			throw new NotImplementedException ();
		}

        internal static void Register(Type t, DependencyProperty dp)
        {
            Dictionary<string, DependencyProperty> typeDeclarations;
            if (propertyDeclarations.TryGetValue(t, out typeDeclarations) == false)
            {
                typeDeclarations = new Dictionary<string, DependencyProperty>();
                propertyDeclarations[t] = typeDeclarations;
            }

			if (!typeDeclarations.ContainsKey(dp.Name))
				typeDeclarations[dp.Name] = dp;
			else
				throw new ArgumentException("A property named " + dp.Name + " already exists on " + t.Name);
		}

        internal virtual DependencyObject InheritanceContext
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        public object SetBinding(DependencyProperty dp, Binding binding)
        {
            if (dp == null)
            {
                throw new ArgumentNullException("dp");
            }
            if (binding == null)
            {
                throw new ArgumentNullException("binding");
            }
            if (binding.Mode == BindingMode.TwoWay && (string.IsNullOrEmpty(binding.Path.Path) || binding.Path.Path.Trim() == "."))
            {
                throw new ArgumentException("TwoWayBindingRequiresPath");
            }
            BindingExpression bindingExpression = binding.CreateBindingExpression();
            try
            {
                bindingExpression.BeginSetBinding();
                this.SetExpression(dp, bindingExpression);
            }
            finally
            {
                bindingExpression.EndSetBinding();
            }
            return bindingExpression;
        }

        internal static DependencyProperty GetDependencyProperty(Type t, string dependencyPropertyName)
        {
            if (!propertyDeclarations.ContainsKey(t))
            {
                return null;
            }
            Dictionary<string, DependencyProperty> typeDeclarations = propertyDeclarations[t];
            if (!typeDeclarations.ContainsKey(dependencyPropertyName))
            {
                return null;
            }
            return typeDeclarations[dependencyPropertyName];
        }

        internal static void ValidateSources(DependencyObject d, DependencySource[] newSources, Expression expression)
        {
            throw new NotImplementedException();
        }

        internal static void ChangeExpressionSources(Expression expression, DependencyObject d, DependencyProperty dp, DependencySource[] newSources)
        {
            throw new NotImplementedException();
        }

        internal virtual void Seal()
        {
            this.isSealed = true;
        }

        internal void NotifySubPropertyChange(DependencyProperty dependencyProperty)
        {
            throw new NotImplementedException();
        }

        internal void SetValue(DependencyProperty dp, object value, bool fromExpression)
        {
            if (this.IsSealed)
            {
                throw new InvalidOperationException(string.Format("Cannot manipulate property values on a sealed DependencyObject({0}", dp.OwnerType));
            }

            if (value == DependencyProperty.UnsetValue)
            {
                this.ClearValue(dp);
                return;
            }

            if (!dp.IsValidType(value))
            {
                throw new ArgumentException(string.Format("value not of the correct type for {0} DependencyProperty. Got {1}, expected {2}", dp.Name, value == null ? "(null)" : value.GetType().ToString(), dp.PropertyType));
            }

            ValidateValueCallback validate = dp.ValidateValueCallback;
            if (validate != null && !validate(value))
            {
                throw new Exception(string.Format("Value {0} does not validate on DependencyProperty {1}", value ?? "(null)", dp.Name));
            }

            Expression expression;
            if (!fromExpression && this.expressions.TryGetValue(dp, out expression))
            {
                var bindingExpression = expression as BindingExpression;
                if (bindingExpression == null || !bindingExpression.CanSetValue)
                {
                    this.RemoveExpression(dp, expression);
                }
            }

            object oldValue;
            if (this.properties.TryGetValue(dp, out oldValue) == false)
            {
                oldValue = dp.DefaultMetadata.DefaultValue;
            }

            this.properties[dp] = value;
            if (oldValue != value)
            {
#if !WINDOWS_PHONE
                if (Dispatcher.Thread != Thread.CurrentThread)
                {
                    this.Dispatcher.BeginInvoke(() => this.NotifyPropertyChange(dp, value, oldValue));
                }
                else
#endif
                {
                    this.NotifyPropertyChange(dp, value, oldValue);
                }
            }
        }

        internal void SetExpression(DependencyProperty dp, Expression expression)
        {
            Expression oldExpression;
            if (this.expressions.TryGetValue(dp, out oldExpression))
            {
                if (oldExpression == expression)
                {
                    this.RefreshExpression(dp, expression);
                    return;
                }
                this.RemoveExpression(dp, oldExpression);
            }

            expression.OnAttach(this, dp);
            bool wasError = true;
            try
            {
                var value = expression.GetValue(this, dp);

                this.SetValue(dp, value, true);
                this.expressions[dp] = expression;
                wasError = false;
            }
            finally
            {
                if (wasError)
                {
                    expression.OnDetach(this, dp);
                }
            }
        }

        internal bool RefreshExpression(DependencyProperty dependencyProperty)
        {
            var expression = this.expressions[dependencyProperty];
            this.RefreshExpression(dependencyProperty, expression);
            return true;
        }

        internal Expression GetExpression(DependencyProperty dp)
        {
            Expression result;
            this.expressions.TryGetValue(dp, out result);
            return result;
        }

        /// <summary>
        /// Returns UIElement DO associated with
        /// </summary>
        /// <returns></returns>
        internal UIElement GetMentor()
        {
            // We have no way to set binding to DO expect UIElement right now
            return (UIElement)this;
        }

        /// <summary>
        /// Determines if a value is set for a given <see cref="DependencyProperty" />.
        /// </summary>
        internal virtual bool ContainsValue(DependencyProperty dp)
        {
            return properties.ContainsKey(dp);
        }

        protected void NotifyPropertyChange(DependencyProperty dp, object newValue, object oldValue)
        {
            var args = new DependencyPropertyChangedEventArgs(dp, oldValue, newValue);
            this.OnPropertyChanged(args);
            if (this.DPChanged != null)
            {
                this.DPChanged(this, args);
            }
        }

        private void RefreshExpression(DependencyProperty dependencyProperty, Expression expression)
        {
            this.SetValue(dependencyProperty, expression.GetValue(this, dependencyProperty), true);
        }

        private void RemoveExpression(DependencyProperty dp, Expression expression)
        {
            expression.OnDetach(this, dp);
            this.expressions.Remove(dp);
        }
	}
}
