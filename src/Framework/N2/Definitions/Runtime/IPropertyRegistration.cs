using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace N2.Definitions.Runtime
{
    public interface IPropertyRegistration<TContent, TProperty> : IPropertyRegistration<TProperty>
    {
        new IContentRegistration<TContent> Registration { get; }
    }

    public interface IPropertyRegistration<TProperty> : IPropertyRegistration
    {
    }

    public interface IPropertyRegistration
    {
        string PropertyName { get; }
        IContentRegistration Registration { get; }
    }
}
