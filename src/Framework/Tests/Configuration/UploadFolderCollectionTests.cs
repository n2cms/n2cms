using NUnit.Framework;
using Shouldly;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace N2.Tests.Configuration
{
    [TestFixture]
    public class UploadFolderCollectionTests
    {
        [TestCase("hello.gif")]
        [TestCase("hello.png")]
        [TestCase("hello.jpeg")]
        [TestCase("hello.jpg")]
        public void Allows_ImageExtensions(string filename)
        {
            var config = new N2.Configuration.EditSection();
            config.UploadFolders.IsTrusted(filename).ShouldBe(true);
        }

        [TestCase("hello.php")]
        [TestCase("hello.cshtml")]
        [TestCase("hello.aspx")]
        public void Disallows_DangerousExtensions(string filename)
        {
            var config = new N2.Configuration.EditSection();
            config.UploadFolders.IsTrusted(filename).ShouldBe(false);
        }
    }
}
