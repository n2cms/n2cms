using System.Drawing;
using N2.Web.Drawing;
using NUnit.Framework;

namespace N2.Tests.Web
{
    [TestFixture]
    public class ImageResizerTests
    {
        [Test]
        public void Fill_Landscape_CropVertically()
        {
            var rect = ImageResizer.GetFillDestinationRectangle(new Size(3, 1), new Size(1, 1));
            var expected = new Rectangle(-1, 0, 3, 1);
            Assert.That(rect, Is.EqualTo(expected));
        }

        [Test]
        public void Fill_Landscape_CropHorizontally()
        {
            var rect = ImageResizer.GetFillDestinationRectangle(new Size(6, 3), new Size(6, 1));
            var expected = new Rectangle(0, -1, 6, 3);
            Assert.That(rect, Is.EqualTo(expected));
        }

        [Test]
        public void Fill_Portrait_CropVertically()
        {
            var rect = ImageResizer.GetFillDestinationRectangle(new Size(3, 6), new Size(1, 6));
            var expected = new Rectangle(-1, 0, 3, 6);
            Assert.That(rect, Is.EqualTo(expected));
        }

        [Test]
        public void Fill_Portrait_CropHorizontally()
        {
            var rect = ImageResizer.GetFillDestinationRectangle(new Size(1, 3), new Size(1, 1));
            var expected = new Rectangle(0, -1, 1, 3);
            Assert.That(rect, Is.EqualTo(expected));
        }


        [Test]
        public void Fill_Landscape_NoCrop_Resize()
        {
            var rect = ImageResizer.GetFillDestinationRectangle(new Size(9, 6), new Size(6, 4));
            var expected = new Rectangle(0, 0, 6, 4);
            Assert.That(rect, Is.EqualTo(expected));
        }

        [Test]
        public void Fill_Landscape_NoCrop_NoResize()
        {
            var rect = ImageResizer.GetFillDestinationRectangle(new Size(6, 4), new Size(6, 4));
            var expected = new Rectangle(0, 0, 6, 4);
            Assert.That(rect, Is.EqualTo(expected));
        }

        [Test]
        public void Fill_Landscape_CropVertically_AndResize()
        {
            var rect = ImageResizer.GetFillDestinationRectangle(new Size(9, 6), new Size(4, 4));
            var expected = new Rectangle(-1, 0, 6, 4);
            Assert.That(rect, Is.EqualTo(expected));
        }

        [Test]
        public void Fill_Landscape_CropHorizontally_AndResize()
        {
            var rect = ImageResizer.GetFillDestinationRectangle(new Size(9, 6), new Size(2, 3));
            var expected = new Rectangle(-1, 0, 4, 3);
            Assert.That(rect, Is.EqualTo(expected));
        }

        [Test]
        public void Fill_Portrait_CropVertically_AndResize()
        {
            var rect = ImageResizer.GetFillDestinationRectangle(new Size(6, 9), new Size(3, 2));
            var expected = new Rectangle(0, -1, 3, 4);
            Assert.That(rect, Is.EqualTo(expected));
        }

        [Test]
        public void Fill_Portrait_CropHorizontally_AndResize()
        {
            var rect = ImageResizer.GetFillDestinationRectangle(new Size(6, 9), new Size(4, 4));
            var expected = new Rectangle(0, -1, 4, 6);
            Assert.That(rect, Is.EqualTo(expected));
        }
    }
}
