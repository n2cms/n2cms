
namespace N2.Edit.Workflow
{
    /// <summary>
    /// Binds values between an object and it's interface.
    /// </summary>
    /// <typeparam name="T">The type of object to bind.</typeparam>
    public interface IContentForm<T>
    {
        /// <summary>Updates the object from the interface.</summary>
        /// <param name="value">The object to update.</param>
        /// <returns>True if the object was updated.</returns>
        bool UpdateObject(T value);

        /// <summary>Updates the interface from the object.</summary>
        /// <param name="value">The value to use for updating the interface.</param>
        void UpdateInterface(T value);
    }
}
