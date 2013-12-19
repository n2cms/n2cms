namespace N2.Edit.Settings
{
    /// <summary>
    /// When used on a settings editor user control this interface defines 
    /// the method invoked when it's time to save changes.
    /// </summary>
    public interface ISettingsEditor
    {
        void Save();
    }
}
