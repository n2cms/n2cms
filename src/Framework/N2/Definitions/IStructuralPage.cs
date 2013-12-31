
using N2.Persistence.Search;
namespace N2.Definitions
{
    /// <summary>
    /// Interface used to denote pages that can have children. This interface 
    /// allows collaboration between modules that doesn't know about each other. 
    /// Classes implementing this interface are eligeble for having child pages.
    /// </summary>
    public interface IStructuralPage : IPage
    {
    }
}
