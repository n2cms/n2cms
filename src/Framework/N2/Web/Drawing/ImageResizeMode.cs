namespace N2.Web.Drawing
{
    /// <summary>
    /// How to fit the image in the max width and height.
    /// </summary>
    public enum ImageResizeMode
    {
        /// <summary>Stretch the image to fit</summary>
        Stretch,
        /// <summary>Fit the image inside the box.</summary>
        Fit,
        /// <summary>Fit the image inside the box best and center it.</summary>
        FitCenterOnTransparent,
        /// <summary>Crop portions of the image outside the box</summary>
        Fill
    }
}
