using System;
using System.Reflection;

namespace N2.Persistence
{
    public interface IPersistableProperty
    {
        string Column { get; set; }
        int Length { get; set; }
        PropertyPersistenceLocation PersistAs { get; set; }
        object GetPropertyMapping(PropertyInfo info, Func<string, string> formatter);
    }
}
