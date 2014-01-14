using N2.Configuration;
using N2.Edit.Installation;
using N2.Tests.Content;
using NUnit.Framework;
using Shouldly;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace N2.Tests.Installation
{
    [TestFixture]
    public class ImageSizesMigrationTests
    {
        [Test]
        public void SizesAreEmpty_ByDefault()
        {
            AnItem item = new AnItem();

            item.GetInstalledImageSizes().Count().ShouldBe(0);
        }

        [Test]
        public void Sizes_CanBeRecorded()
        {
            AnItem item = new AnItem();

            var sizes = new ImageSizesCollection();
            sizes.Clear();

            sizes.Add(new ImageSizeElement { Name = "x", Width = 2, Height = 1, Mode = N2.Web.Drawing.ImageResizeMode.FitCenterOnTransparent });
            sizes.Add(new ImageSizeElement { Name = "y", Width = 666, Height = 444, Mode = N2.Web.Drawing.ImageResizeMode.Fill });

            item.RecordInstalledImageSizes(sizes);

            item.GetDetailCollection(InstallationManager.installationImageSizes, false)[0].ShouldBe("x=2,1,FitCenterOnTransparent");
            item.GetDetailCollection(InstallationManager.installationImageSizes, false)[1].ShouldBe("y=666,444,Fill");
        }

        [Test]
        public void Sizes_CanBeRead()
        {
            AnItem item = new AnItem();

            var sizes = new ImageSizesCollection();
            sizes.Clear();

            sizes.Add(new ImageSizeElement { Name = "x", Width = 2, Height = 1, Mode = N2.Web.Drawing.ImageResizeMode.FitCenterOnTransparent });
            sizes.Add(new ImageSizeElement { Name = "y", Width = 666, Height = 444, Mode = N2.Web.Drawing.ImageResizeMode.Fill });

            item.RecordInstalledImageSizes(sizes);

            var readSizes = item.GetInstalledImageSizes().ToList();
            readSizes[0].Name.ShouldBe("x");
            readSizes[0].Width.ShouldBe(2);
            readSizes[0].Height.ShouldBe(1);
            readSizes[0].Mode.ShouldBe(N2.Web.Drawing.ImageResizeMode.FitCenterOnTransparent);
            readSizes[1].Name.ShouldBe("y");
            readSizes[1].Width.ShouldBe(666);
            readSizes[1].Height.ShouldBe(444);
            readSizes[1].Mode.ShouldBe(N2.Web.Drawing.ImageResizeMode.Fill);
        }
    }
}
