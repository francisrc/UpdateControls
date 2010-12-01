/**********************************************************************
 * 
 * Update Controls .NET
 * Copyright 2010 Michael L Perry
 * MIT License
 * 
 * http://updatecontrols.net
 * http://updatecontrolslight.codeplex.com/
 * 
 **********************************************************************/

using System.Windows;
using System.Windows.Threading;

namespace UpdateControls.XAML.Wrapper
{
    public interface IObjectInstance
    {
        object WrappedObject { get; }
        Dispatcher Dispatcher { get; }
        ObjectProperty LookupProperty(UpdateControls.XAML.Wrapper.ClassProperty classProperty);
        void SetValue(DependencyProperty dependencyProperty, object value);
    }
}