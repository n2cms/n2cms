using System;

namespace N2.Resources
{
    [Flags]
    public enum ScriptOptions
    {
        /// <summary>Add the script exactly as given.</summary>
        None = 1,
        /// <summary>Embed the script in script tags.</summary>
        ScriptTags = 2,
        /// <summary>Embed the script in script tags and use jQuery to await document loaded event.</summary>
        DocumentReady = 4,
        /// <summary>The script is located at the supplied url.</summary>
        Include = 8,
        /// <summary>Try to register this before any other scripts.</summary>
        Prioritize = 16
    }
}
