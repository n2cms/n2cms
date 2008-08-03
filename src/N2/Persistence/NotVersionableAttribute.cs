using System;
using System.Collections.Generic;
using System.Text;

namespace N2.Persistence
{
    /// <summary>
    /// When used to decorate a content class this attribute tells the edit 
    /// manager not to store versions of items of that class.
    /// </summary>
    public class NotVersionableAttribute : Attribute
    {
    }
}
