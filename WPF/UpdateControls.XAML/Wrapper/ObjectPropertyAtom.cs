/**********************************************************************
 * 
 * Update Controls .NET
 * Copyright 2010 Michael L Perry
 * MIT License
 * 
 * http://updatecontrols.net
 * http://updatecontrols.codeplex.com/
 * 
 **********************************************************************/

using System;
using UpdateControls;

namespace UpdateControls.XAML.Wrapper
{
    internal abstract class ObjectPropertyAtom : ObjectProperty, IUpdatable
    {
        private Dependent _depProperty;
        private object _value;
		private bool _firePropertyChanged = false;

		public ObjectPropertyAtom(IObjectInstance objectInstance, ClassMember classProperty)
			: base(objectInstance, classProperty)
		{
			if (ClassProperty.CanRead)
			{
				// When the property is out of date, update it from the wrapped object.
                _depProperty = new Dependent(() => BindingInterceptor.Current.UpdateValue(this));
				// When the property becomes out of date, trigger an update.
				// The update should have lower priority than user input & drawing,
				// to ensure that the app doesn't lock up in case a large model is 
				// being updated outside the UI (e.g. via timers or the network).
                _depProperty.Invalidated += () => UpdateScheduler.ScheduleUpdate(this);
			}
		}

		protected override void SetValue(object value)
		{
            var scheduler = UpdateScheduler.Begin();

            try
            {
                value = TranslateIncommingValue(value);
                ClassProperty.SetObjectValue(ObjectInstance.WrappedObject, value);
            }
            finally
            {
                if (scheduler != null)
                {
                    foreach (IUpdatable updatable in scheduler.End())
                        updatable.UpdateNow();
                }
            }
		}

        protected override object GetValue()
        {
            if (_depProperty.IsNotUpdating)
                _depProperty.OnGet();
            return _value;
        }

        protected override void UpdateValue()
        {
            object value = ClassProperty.GetObjectValue(ObjectInstance.WrappedObject);
            value = TranslateOutgoingValue(value);
            if (!Object.Equals(_value, value))
                _value = value;
            if (_firePropertyChanged)
                FirePropertyChanged();
            _firePropertyChanged = true;
        }

        public abstract object TranslateIncommingValue(object value);
        public abstract object TranslateOutgoingValue(object value);

        public void UpdateNow()
        {
            _depProperty.OnGet();
        }
    }
}
