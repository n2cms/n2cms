using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

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
		/// <summary>Crop portions of the image outside the box</summary>
		Fill
	}
}
