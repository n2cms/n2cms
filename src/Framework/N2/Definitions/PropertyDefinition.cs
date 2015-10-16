using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
using N2.Details;
using N2.Persistence;
using N2.Persistence.Proxying;

namespace N2.Definitions
{
    /// <summary>
    /// Stores metadata about a property on a content item.
    /// </summary>
    public class PropertyDefinition : ICloneable
    {
        public PropertyDefinition(PropertyInfo property)
        {
            Info = property;
            Name = property.Name;
            PropertyType = property.PropertyType;
            Attributes = property.GetCustomAttributes(true);
            foreach (var a in Attributes)
            {
                if (a is IUniquelyNamed)
                    (a as IUniquelyNamed).Name = Name;
            }
            Getter = (instance) => Info.GetValue(instance, null);
            Setter = (instance, value) => Info.SetValue(instance, value, null);
            Editable = Attributes.OfType<IEditable>().FirstOrDefault();
            Displayable = Attributes.OfType<IDisplayable>().FirstOrDefault();
            Persistable = Attributes.OfType<IPersistableProperty>().FirstOrDefault()
                ?? new PersistableAttribute 
                { 
                    PersistAs = property.DeclaringType == typeof(ContentItem)
                        ? PropertyPersistenceLocation.Column
                        : Editable != null
                            ? PropertyPersistenceLocation.Detail
                            : PropertyPersistenceLocation.Ignore
                };
            DefaultValue = Attributes.OfType<IInterceptableProperty>().Select(ip => ip.DefaultValue).FirstOrDefault(v => v != null);
        }

        public PropertyDefinition(string name, Type propertyType)
        {
            Name = name;
            PropertyType = propertyType;
            Attributes = new object[0];
            Getter = (instance) => Utility.GetProperty(instance, name);
            Setter = (instance, value) => Utility.SetProperty(instance, name, value);
        }
        
        public string Name { get; private set; }
        public PropertyInfo Info { get; private set; }
        public Type PropertyType { get; set; }
        
        public object[] Attributes { get; set; }
        public Func<object, object> Getter { get; set; }
        public Action<object, object> Setter { get; set; }

        public IEditable Editable { get; set; }
        public IDisplayable Displayable { get; set; }

        object ICloneable.Clone()
        {
            return Clone();
        }

        public PropertyDefinition Clone()
        {
            var pd = (PropertyDefinition)MemberwiseClone();
            pd.Attributes = pd.Attributes.Select(a => a.TryClone()).ToArray();
            return pd;
        }

        public IPersistableProperty Persistable { get; set; }

        public object DefaultValue { get; set; }
    }
}
