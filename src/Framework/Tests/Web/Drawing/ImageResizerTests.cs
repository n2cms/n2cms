using System;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using N2.Web.Drawing;
using NUnit.Framework;

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

        [TestCase("lav.jpeg", /*x*/000, /*y*/000, /*w*/100, /*h*/200, /*mw*/50, /*mh*/100, "top_high")]
        [TestCase("lav.jpeg", /*x*/100, /*y*/100, /*w*/300, /*h*/100, /*mw*/150, /*mh*/050, "middle_wide")]
        [TestCase("lav.jpeg", /*x*/300, /*y*/200, /*w*/100, /*h*/100, /*mw*/050, /*mh*/050, "bottom_square")]
        [TestCase("lav.jpeg", /*x*/200, /*y*/100, /*w*/100, /*h*/100, /*mw*/200, /*mh*/200, "upsize")]
        [TestCase("lav.jpeg", /*x*/100, /*y*/50, /*w*/300, /*h*/200, /*mw*/450, /*mh*/300, "middle_zoom")]
        [TestCase("copter_original.jpg", /*x*/0, /*y*/80, /*w*/960, /*h*/480, /*mw*/960, /*mh*/480, "copter_middle")]
        [TestCase("copter_original.jpg", /*x*/0, /*y*/0, /*w*/960, /*h*/480, /*mw*/960, /*mh*/480, "copter_top")]
        [TestCase("copter_original.jpg", /*x*/0, /*y*/160, /*w*/960, /*h*/480, /*mw*/960, /*mh*/480, "copter_bottom")]
        [TestCase("copter_original.jpg", /*x*/500, /*y*/260, /*w*/240, /*h*/120, /*mw*/960, /*mh*/480, "copter_upsize")]
        public void ResizeFormats_Fill_SourceRectangle(string img, int x, int y, int w, int h, int maxW, int maxH, string name)
        {
            string path = Environment.CurrentDirectory + @"\Web\Drawing\" + img;
            using (var input = File.OpenRead(path))
            using (var output = new MemoryStream())
            {
                ir.Resize(input, ImageResizeParameters.Fill(new Rectangle(x, y, w, h), maxW, maxH), output);

                if (Debugger.IsAttached)
                {
                    path = path.Substring(0, path.LastIndexOf('.')) 
                        + "_fill_src_" + name + " " + path.Substring(path.LastIndexOf('.'));
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
