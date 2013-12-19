namespace N2.Resources
{
    /// <summary>
    /// An enum of recognized media descriptors.
    /// </summary>
    public enum Media
    {
        /// <summary>Intended for non-paged computer screens.</summary>
        Screen,
        /// <summary>Intended for media using a fixed-pitch character grid, such as teletypes, terminals, or portable devices with limited display capabilities.</summary>
        TTY,
        /// <summary>Intended for television-type devices (low resolution, color, limited scrollability).</summary>
        TV,
        /// <summary>Intended for projectors.</summary>
        Projection,
        /// <summary>Intended for handheld devices (small screen, monochrome, bitmapped graphics, limited bandwidth).</summary>
        Handheld,
        /// <summary>Intended for paged, opaque material and for documents viewed on screen in print preview mode.</summary>
        Print,
        /// <summary>Intended for braille tactile feedback devices.</summary>
        Braille,
        /// <summary>Intended for speech synthesizers.</summary>
        Aural,
        /// <summary>Suitable for all devices.</summary>
        All
    }
}
