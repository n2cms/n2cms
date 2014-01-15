using System.Web.UI;

namespace N2.Web.Parts
{
    /// <summary>
    /// Content items implementing this interface are detected by the part 
    /// content adapter and added with this method instead of the default.
    /// </summary>
    public interface IAddablePart
    {
        /// <summary>Adds the content item's interface control to the container.</summary>
        /// <param name="container">The containing zone.</param>
        /// <returns>The control that was added.</returns>
        Control AddTo(Control container);
    }
}
