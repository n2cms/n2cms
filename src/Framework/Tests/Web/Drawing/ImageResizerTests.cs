using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using N2.Web.Drawing;
using System.IO;
using System.Diagnostics;
using System.Drawing;

namespace N2.Tests.Web.Drawing
{
	[TestFixture]
	public class ImageResizerTests
	{
		ImageResizer ir = new ImageResizer();

		[TestCase(@"\Web\Drawing\ajax-loader.gif")]
		[TestCase(@"\Web\Drawing\bullet_star.gif")]
		[TestCase(@"\Web\Drawing\copter_icon.jpg")]
		[TestCase(@"\Web\Drawing\lav.jpeg")]
		[TestCase(@"\Web\Drawing\n2logo.png")]
		public void ResizeFormats_Fit(string path)
		{
			path = Environment.CurrentDirectory + path;
			using(var input = File.OpenRead(path))
			using(var output = new MemoryStream())
			{
				ir.Resize(input, new ImageResizeParameters(8, 16, ImageResizeMode.Fit), output);

				Assert.That(Bitmap.FromStream(output, true, true).Width, Is.LessThanOrEqualTo(8));
				Assert.That(Bitmap.FromStream(output, true, true).Width, Is.GreaterThan(0));
				Assert.That(Bitmap.FromStream(output, true, true).Height, Is.LessThanOrEqualTo(16));
				Assert.That(Bitmap.FromStream(output, true, true).Height, Is.GreaterThan(0));
				
				if (Debugger.IsAttached)
				{
					path = path.Substring(0, path.LastIndexOf('.')) + "_fit" + path.Substring(path.LastIndexOf('.'));
					File.WriteAllBytes(path, output.GetBuffer());
				}
			}
		}

		[TestCase(@"\Web\Drawing\ajax-loader.gif")]
		[TestCase(@"\Web\Drawing\bullet_star.gif")]
		[TestCase(@"\Web\Drawing\copter_icon.jpg")]
		[TestCase(@"\Web\Drawing\lav.jpeg")]
		[TestCase(@"\Web\Drawing\n2logo.png")]
		public void ResizeFormats_Fill(string path)
		{
			path = Environment.CurrentDirectory + path;
			using (var input = File.OpenRead(path))
			using (var output = new MemoryStream())
			{
				ir.Resize(input, new ImageResizeParameters(8, 16, ImageResizeMode.Fill), output);

				Assert.That(Bitmap.FromStream(output, true, true).Width, Is.EqualTo(8));
				Assert.That(Bitmap.FromStream(output, true, true).Height, Is.EqualTo(16));

				if (Debugger.IsAttached)
				{
					path = path.Substring(0, path.LastIndexOf('.')) + "_fill" + path.Substring(path.LastIndexOf('.'));
					File.WriteAllBytes(path, output.GetBuffer());
				}
			}
		}

		[TestCase(@"\Web\Drawing\ajax-loader.gif")]
		[TestCase(@"\Web\Drawing\bullet_star.gif")]
		[TestCase(@"\Web\Drawing\copter_icon.jpg")]
		[TestCase(@"\Web\Drawing\lav.jpeg")]
		[TestCase(@"\Web\Drawing\n2logo.png")]
		public void ResizeFormats_Stretch(string path)
		{
			path = Environment.CurrentDirectory + path;
			using (var input = File.OpenRead(path))
			using (var output = new MemoryStream())
			{
				ir.Resize(input, new ImageResizeParameters(8, 16, ImageResizeMode.Stretch), output);

				Assert.That(Bitmap.FromStream(output, true, true).Width, Is.EqualTo(8));
				Assert.That(Bitmap.FromStream(output, true, true).Height, Is.EqualTo(16));

				if (Debugger.IsAttached)
				{
					path = path.Substring(0, path.LastIndexOf('.')) + "_stretch" + path.Substring(path.LastIndexOf('.'));
					File.WriteAllBytes(path, output.GetBuffer());
				}
			}
		}

	}
}
