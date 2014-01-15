namespace N2.Edit.Installation
{
    public enum SystemStatusLevel
    {
        Unknown = 0,
        NoConnection = 1,
        NoSchema = 2,
        OldVersion = 3,
        NoRootNode = 4,
        NoStartNode = 5,
        Unconfirmed = 88,
        UpAndRunning = 99
    }
}
