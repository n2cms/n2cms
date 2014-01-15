using N2.Definitions;

namespace N2.Persistence
{
    /// <summary>
    /// When used to decorate a content class this attribute tells the edit 
    /// manager not to store versions of items of that class.
    /// </summary>
    public class NotVersionableAttribute : VersionableAttribute
    {
        public NotVersionableAttribute()
            : base(AllowVersions.No)
        {
        }
    }
}
