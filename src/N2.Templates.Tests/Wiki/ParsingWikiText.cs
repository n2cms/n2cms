using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using N2.Templates.Wiki;
using NUnit.Framework.SyntaxHelpers;
using N2.Templates.Wiki.Fragmenters;

namespace N2.Templates.Tests.Wiki
{
    [TestFixture]
    public class ParsingWikiText
    {
        WikiParser parser;

        [SetUp]
        public void SetUp()
        {
            parser = new WikiParser();
        }

        [Test]
        public void TripleTilde_IsParsedAs_UserName()
        {
            IList<Fragment> fragments = parser.Parse("~~~").ToList();
            Assert.That(fragments.Count, Is.EqualTo(1));
            Assert.That(fragments[0].Name, Is.EqualTo("UserInfo"));
        }

        [Test]
        public void QuadrupleTilde_IsParsedAs_UserName()
        {
            IList<Fragment> fragments = parser.Parse("~~~~").ToList();
            Assert.That(fragments.Count, Is.EqualTo(1));
            Assert.That(fragments[0].Name, Is.EqualTo("UserInfo"));
        }

        [Test]
        public void QuintipleTilde_IsParsedAs_UserName()
        {
            IList<Fragment> fragments = parser.Parse("~~~~~").ToList();
            Assert.That(fragments.Count, Is.EqualTo(1));
            Assert.That(fragments[0].Name, Is.EqualTo("UserInfo"));
        }

        [Test]
        public void EmptyString_YieldsNoFragments()
        {
            IList<Fragment> fragments = parser.Parse("").ToList();
            Assert.That(fragments.Count, Is.EqualTo(0));
        }

        [Test]
        public void CanParse_InternalLink()
        {
            IList<Fragment> fragments = parser.Parse("[[link]]").ToList();
     
            Assert.That(fragments.Count, Is.EqualTo(1));
            Assert.That(fragments[0].Name, Is.EqualTo("InternalLink"));
            Assert.That(fragments[0].Value, Is.EqualTo("[[link]]"));
        }

        [Test]
        public void CanParse_InternalLink_WithTextBefore()
        {
            IList<Fragment> fragments = parser.Parse("beginning[[link]]").ToList();
      
            Assert.That(fragments.Count, Is.EqualTo(2));
            Assert.That(fragments[0].Name, Is.EqualTo("Text"));
            Assert.That(fragments[0].Value, Is.EqualTo("beginning"));
            Assert.That(fragments[1].Name, Is.EqualTo("InternalLink"));
            Assert.That(fragments[1].Value, Is.EqualTo("[[link]]"));
        }

        [Test]
        public void CanParse_InternalLink_WithTextAfter()
        {
            IList<Fragment> fragments = parser.Parse("[[link]]end").ToList();
    
            Assert.That(fragments.Count, Is.EqualTo(2));
            Assert.That(fragments[0].Name, Is.EqualTo("InternalLink"));
            Assert.That(fragments[0].Value, Is.EqualTo("[[link]]"));
            Assert.That(fragments[1].Name, Is.EqualTo("Text"));
            Assert.That(fragments[1].Value, Is.EqualTo("end"));
        }

        [Test]
        public void CanParse_InternalLink_WithTextBefore_AndAfter()
        {
            IList<Fragment> fragments = parser.Parse("beginning[[link]]end").ToList();
      
            Assert.That(fragments.Count, Is.EqualTo(3));
            Assert.That(fragments[0].Name, Is.EqualTo("Text"));
            Assert.That(fragments[0].Value, Is.EqualTo("beginning"));
            Assert.That(fragments[1].Name, Is.EqualTo("InternalLink"));
            Assert.That(fragments[1].Value, Is.EqualTo("[[link]]"));
            Assert.That(fragments[2].Name, Is.EqualTo("Text"));
            Assert.That(fragments[2].Value, Is.EqualTo("end"));
        }

        [Test]
        public void CanParse_2InternalLinks()
        {
            IList<Fragment> fragments = parser.Parse("[[link1]][[link2]]").ToList();

            Assert.That(fragments.Count, Is.EqualTo(2));
            Assert.That(fragments[0].Name, Is.EqualTo("InternalLink"));
            Assert.That(fragments[0].Value, Is.EqualTo("[[link1]]"));
            Assert.That(fragments[1].Name, Is.EqualTo("InternalLink"));
            Assert.That(fragments[1].Value, Is.EqualTo("[[link2]]"));
        }

        [Test]
        public void CanParse_2InternalLinks_WithTextBefore_AndAfter_AndBetween()
        {
            IList<Fragment> fragments = parser.Parse("beginning[[link]]middle[[link2]]end").ToList();
            
            Assert.That(fragments.Count, Is.EqualTo(5));
            Assert.That(fragments[0].Name, Is.EqualTo("Text"));
            Assert.That(fragments[0].Value, Is.EqualTo("beginning"));
            Assert.That(fragments[1].Name, Is.EqualTo("InternalLink"));
            Assert.That(fragments[1].Value, Is.EqualTo("[[link]]"));
            Assert.That(fragments[2].Name, Is.EqualTo("Text"));
            Assert.That(fragments[2].Value, Is.EqualTo("middle"));
            Assert.That(fragments[3].Name, Is.EqualTo("InternalLink"));
            Assert.That(fragments[3].Value, Is.EqualTo("[[link2]]"));
            Assert.That(fragments[4].Name, Is.EqualTo("Text"));
            Assert.That(fragments[4].Value, Is.EqualTo("end"));
        }

        [Test]
        public void CanParse_ExternalLink()
        {
            IList<Fragment> fragments = parser.Parse("[http://n2cms.com]").ToList();

            Assert.That(fragments.Count, Is.EqualTo(1));
            Assert.That(fragments[0].Name, Is.EqualTo("ExternalLink"));
            Assert.That(fragments[0].Value, Is.EqualTo("[http://n2cms.com]"));
        }

        [Test]
        public void CanParse_ExternalLink_WithNoBrackets()
        {
            IList<Fragment> fragments = parser.Parse("http://n2cms.com").ToList();

            Assert.That(fragments.Count, Is.EqualTo(1));
            Assert.That(fragments[0].Name, Is.EqualTo("ExternalLink"));
            Assert.That(fragments[0].Value, Is.EqualTo("http://n2cms.com"));
        }

        [Test]
        public void CanParse_ExternalLink_WithNoBrackets_WithPath()
        {
            IList<Fragment> fragments = parser.Parse("http://n2cms.com/something.aspx").ToList();

            Assert.That(fragments.Count, Is.EqualTo(1));
            Assert.That(fragments[0].Name, Is.EqualTo("ExternalLink"));
            Assert.That(fragments[0].Value, Is.EqualTo("http://n2cms.com/something.aspx"));
        }

        [Test]
        public void CanParse_ExternalLink_WithNoBrackets_WithPathAndQueryAndHash()
        {
            IList<Fragment> fragments = parser.Parse("http://n2cms.com/something.aspx?key=value&auml;key2=value2#bookmark").ToList();

            Assert.That(fragments.Count, Is.EqualTo(1));
            Assert.That(fragments[0].Name, Is.EqualTo("ExternalLink"));
            Assert.That(fragments[0].Value, Is.EqualTo("http://n2cms.com/something.aspx?key=value&auml;key2=value2#bookmark"));
        }

        [Test]
        public void CanParse_ExternalLink_WithNoBrackets_InASentence()
        {
            IList<Fragment> fragments = parser.Parse("Visit http://n2cms.com/ for more information.").ToList();

            Assert.That(fragments.Count, Is.EqualTo(3));
            Assert.That(fragments[0].Value, Is.EqualTo("Visit "));
            Assert.That(fragments[1].Name, Is.EqualTo("ExternalLink"));
            Assert.That(fragments[1].Value, Is.EqualTo("http://n2cms.com/"));
            Assert.That(fragments[2].Value, Is.EqualTo(" for more information."));
        }

        [Test]
        public void TreatsDoubleBrackets_AsText()
        {
            IList<Fragment> fragments = parser.Parse("before[[some text]]after").ToList();

            Assert.That(fragments.Count, Is.EqualTo(3));
            Assert.That(fragments[0].Value, Is.EqualTo("before"));
            Assert.That(fragments[1].Name, Is.EqualTo("InternalLink"));
            Assert.That(fragments[1].Value, Is.EqualTo("[[some text]]"));
            Assert.That(fragments[2].Value, Is.EqualTo("after"));
        }

        [Test]
        public void CanParse_Template()
        {
            IList<Fragment> fragments = parser.Parse("{{WikiArticleList}}").ToList();
            Assert.That(fragments.Count, Is.EqualTo(1));
            Assert.That(fragments[0].Value, Is.EqualTo("{{WikiArticleList}}"));
            Assert.That(fragments[0].Name, Is.EqualTo("Template"));
        }

        [Test]
        public void CanParse_Template_AndInternalLink()
        {
            IList<Fragment> fragments = parser.Parse("{{WikiArticleList}}[[internallink]]").ToList();
            Assert.That(fragments.Count, Is.EqualTo(2));
            Assert.That(fragments[0].Value, Is.EqualTo("{{WikiArticleList}}"));
            Assert.That(fragments[0].Name, Is.EqualTo("Template"));
            Assert.That(fragments[1].Value, Is.EqualTo("[[internallink]]"));
            Assert.That(fragments[1].Name, Is.EqualTo("InternalLink"));
        }

        [Test]
        public void CanParse_Combinations()
        {
            IList<Fragment> fragments = parser.Parse("{{WikiArticleList}}[[internallink]][external]text{{WikiArticleList2}}[[internallink2]][external2]text2").ToList();
            Assert.That(fragments.Count, Is.EqualTo(8));
            Assert.That(fragments[0].Value, Is.EqualTo("{{WikiArticleList}}"));
            Assert.That(fragments[0].Name, Is.EqualTo("Template"));
            Assert.That(fragments[1].Value, Is.EqualTo("[[internallink]]"));
            Assert.That(fragments[1].Name, Is.EqualTo("InternalLink"));
            Assert.That(fragments[2].Value, Is.EqualTo("[external]"));
            Assert.That(fragments[2].Name, Is.EqualTo("ExternalLink"));
            Assert.That(fragments[3].Value, Is.EqualTo("text"));
            Assert.That(fragments[3].Name, Is.EqualTo("Text"));
            Assert.That(fragments[4].Name, Is.EqualTo("Template"));
            Assert.That(fragments[5].Name, Is.EqualTo("InternalLink"));
            Assert.That(fragments[6].Name, Is.EqualTo("ExternalLink"));
            Assert.That(fragments[7].Name, Is.EqualTo("Text"));
        }

        [Test]
        public void CanParse_Heading1()
        {
            IList<Fragment> fragments = parser.Parse("=Heading 1=").ToList();
            Assert.That(fragments.Count, Is.EqualTo(1));
            Assert.That(fragments[0].Name, Is.EqualTo("Heading"));
            Assert.That(fragments[0].Value, Is.EqualTo("=Heading 1="));
        }

        [Test]
        public void CanParse_Heading2()
        {
            IList<Fragment> fragments = parser.Parse("==Heading 2==").ToList();
            Assert.That(fragments.Count, Is.EqualTo(1));
            Assert.That(fragments[0].Name, Is.EqualTo("Heading"));
            Assert.That(fragments[0].Value, Is.EqualTo("==Heading 2=="));
        }
    }
}
