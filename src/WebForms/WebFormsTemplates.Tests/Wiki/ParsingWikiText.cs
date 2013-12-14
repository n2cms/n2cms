using System;
using System.Collections.Generic;
using System.Linq;
using N2.Addons.Wiki;
using NUnit.Framework;
using N2.Web.Wiki;

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
            var fragments = parser.Parse("~~~").ToList();
            Assert.That(fragments.Count, Is.EqualTo(1));
            Assert.That(fragments[0].Command, Is.EqualTo("UserInfo"));
        }

        [Test]
        public void QuadrupleTilde_IsParsedAs_UserName()
        {
            var fragments = parser.Parse("~~~~").ToList();
            Assert.That(fragments.Count, Is.EqualTo(1));
            Assert.That(fragments[0].Command, Is.EqualTo("UserInfo"));
        }

        [Test]
        public void QuintipleTilde_IsParsedAs_UserName()
        {
            var fragments = parser.Parse("~~~~~").ToList();
            Assert.That(fragments.Count, Is.EqualTo(1));
            Assert.That(fragments[0].Command, Is.EqualTo("UserInfo"));
        }

        [Test]
        public void EmptyString_YieldsNoFragments()
        {
            var fragments = parser.Parse("").ToList();
            Assert.That(fragments.Count, Is.EqualTo(0));
        }

        [Test]
        public void CanParse_InternalLink()
        {
            var fragments = parser.Parse("[[link]]").ToList();
     
            Assert.That(fragments.Count, Is.EqualTo(1));
            Assert.That(fragments[0].Command, Is.EqualTo("InternalLink"));
            Assert.That(fragments[0].ToString(), Is.EqualTo("[[link]]"));
        }

        [Test]
        public void CanParse_InternalLink_WithTextBefore()
        {
            var fragments = parser.Parse("beginning[[link]]").ToList();
      
            Assert.That(fragments.Count, Is.EqualTo(2));
            Assert.That(fragments[0].Command, Is.EqualTo("Text"));
            Assert.That(fragments[0].ToString(), Is.EqualTo("beginning"));
            Assert.That(fragments[1].Command, Is.EqualTo("InternalLink"));
            Assert.That(fragments[1].ToString(), Is.EqualTo("[[link]]"));
        }

        [Test]
        public void CanParse_InternalLink_WithTextAfter()
        {
            var fragments = parser.Parse("[[link]]end").ToList();
    
            Assert.That(fragments.Count, Is.EqualTo(2));
            Assert.That(fragments[0].Command, Is.EqualTo("InternalLink"));
            Assert.That(fragments[0].ToString(), Is.EqualTo("[[link]]"));
            Assert.That(fragments[1].Command, Is.EqualTo("Text"));
            Assert.That(fragments[1].ToString(), Is.EqualTo("end"));
        }

        [Test]
        public void CanParse_InternalLink_WithTextBefore_AndAfter()
        {
            var fragments = parser.Parse("beginning[[link]]end").ToList();
      
            Assert.That(fragments.Count, Is.EqualTo(3));
            Assert.That(fragments[0].Command, Is.EqualTo("Text"));
            Assert.That(fragments[0].ToString(), Is.EqualTo("beginning"));
            Assert.That(fragments[1].Command, Is.EqualTo("InternalLink"));
            Assert.That(fragments[1].ToString(), Is.EqualTo("[[link]]"));
            Assert.That(fragments[2].Command, Is.EqualTo("Text"));
            Assert.That(fragments[2].ToString(), Is.EqualTo("end"));
        }

        [Test]
        public void CanParse_2InternalLinks()
        {
            var fragments = parser.Parse("[[link1]][[link2]]").ToList();

            Assert.That(fragments.Count, Is.EqualTo(2));
            Assert.That(fragments[0].Command, Is.EqualTo("InternalLink"));
            Assert.That(fragments[0].ToString(), Is.EqualTo("[[link1]]"));
            Assert.That(fragments[1].Command, Is.EqualTo("InternalLink"));
            Assert.That(fragments[1].ToString(), Is.EqualTo("[[link2]]"));
        }

        [Test]
        public void CanParse_2InternalLinks_WithTextBefore_AndAfter_AndBetween()
        {
            var fragments = parser.Parse("beginning[[link]]middle[[link2]]end").ToList();
            
            Assert.That(fragments.Count, Is.EqualTo(5));
            Assert.That(fragments[0].Command, Is.EqualTo("Text"));
            Assert.That(fragments[0].ToString(), Is.EqualTo("beginning"));
            Assert.That(fragments[1].Command, Is.EqualTo("InternalLink"));
            Assert.That(fragments[1].ToString(), Is.EqualTo("[[link]]"));
            Assert.That(fragments[2].Command, Is.EqualTo("Text"));
            Assert.That(fragments[2].ToString(), Is.EqualTo("middle"));
            Assert.That(fragments[3].Command, Is.EqualTo("InternalLink"));
            Assert.That(fragments[3].ToString(), Is.EqualTo("[[link2]]"));
            Assert.That(fragments[4].Command, Is.EqualTo("Text"));
            Assert.That(fragments[4].ToString(), Is.EqualTo("end"));
        }

        [Test]
        public void CanParse_ExternalLink()
        {
            var fragments = parser.Parse("[http://n2cms.com]").ToList();

            Assert.That(fragments.Count, Is.EqualTo(1));
            Assert.That(fragments[0].Command, Is.EqualTo("ExternalLink"));
            Assert.That(fragments[0].ToString(), Is.EqualTo("[http://n2cms.com]"));
        }

        [Test]
        public void CanParse_ExternalLink_WithNoBrackets()
        {
            var fragments = parser.Parse("http://n2cms.com").ToList();

            Assert.That(fragments.Count, Is.EqualTo(1));
            Assert.That(fragments[0].Command, Is.EqualTo("ExternalLink"));
            Assert.That(fragments[0].ToString(), Is.EqualTo("http://n2cms.com"));
        }

        [Test]
        public void CanParse_ExternalLink_WithNoBrackets_WithPath()
        {
            var fragments = parser.Parse("http://n2cms.com/something.aspx").ToList();

            Assert.That(fragments.Count, Is.EqualTo(1));
            Assert.That(fragments[0].Command, Is.EqualTo("ExternalLink"));
            Assert.That(fragments[0].ToString(), Is.EqualTo("http://n2cms.com/something.aspx"));
        }

        [Test]
        public void CanParse_ExternalLink_WithNoBrackets_WithPathAndQueryAndHash()
        {
            var fragments = parser.Parse("http://n2cms.com/something.aspx?key=value&auml;key2=value2#bookmark").ToList();

            Assert.That(fragments.Count, Is.EqualTo(1));
            Assert.That(fragments[0].Command, Is.EqualTo("ExternalLink"));
            Assert.That(fragments[0].ToString(), Is.EqualTo("http://n2cms.com/something.aspx?key=value&auml;key2=value2#bookmark"));
        }

        [Test]
        public void CanParse_ExternalLink_WithNoBrackets_InASentence()
        {
            var fragments = parser.Parse("Visit http://n2cms.com/ for more information.").ToList();

            Assert.That(fragments.Count, Is.EqualTo(3));
            Assert.That(fragments[0].ToString(), Is.EqualTo("Visit "));
            Assert.That(fragments[1].Command, Is.EqualTo("ExternalLink"));
            Assert.That(fragments[1].ToString(), Is.EqualTo("http://n2cms.com/"));
            Assert.That(fragments[2].ToString(), Is.EqualTo(" for more information."));
        }

        [Test]
        public void TreatsDoubleBrackets_AsText()
        {
            var fragments = parser.Parse("before[[some text]]after").ToList();

            Assert.That(fragments.Count, Is.EqualTo(3));
            Assert.That(fragments[0].ToString(), Is.EqualTo("before"));
            Assert.That(fragments[1].Command, Is.EqualTo("InternalLink"));
            Assert.That(fragments[1].ToString(), Is.EqualTo("[[some text]]"));
            Assert.That(fragments[2].ToString(), Is.EqualTo("after"));
        }

        [Test]
        public void CanParse_Template()
        {
            var fragments = parser.Parse("{{WikiArticleList}}").ToList();
            Assert.That(fragments.Count, Is.EqualTo(1));
            Assert.That(fragments[0].ToString(), Is.EqualTo("{{WikiArticleList}}"));
            Assert.That(fragments[0].Command, Is.EqualTo("Template"));
        }

        [Test]
        public void CanParse_Template_AndInternalLink()
        {
            var fragments = parser.Parse("{{WikiArticleList}}[[internallink]]").ToList();
            Assert.That(fragments.Count, Is.EqualTo(2));
            Assert.That(fragments[0].ToString(), Is.EqualTo("{{WikiArticleList}}"));
            Assert.That(fragments[0].Command, Is.EqualTo("Template"));
            Assert.That(fragments[1].ToString(), Is.EqualTo("[[internallink]]"));
            Assert.That(fragments[1].Command, Is.EqualTo("InternalLink"));
        }

        [Test]
        public void CanParse_Combinations()
        {
            var fragments = parser.Parse("{{WikiArticleList}}[[internallink]][external]text{{WikiArticleList2}}[[internallink2]][external2]text2").ToList();
            Assert.That(fragments.Count, Is.EqualTo(8));
            Assert.That(fragments[0].ToString(), Is.EqualTo("{{WikiArticleList}}"));
            Assert.That(fragments[0].Command, Is.EqualTo("Template"));
            Assert.That(fragments[1].ToString(), Is.EqualTo("[[internallink]]"));
            Assert.That(fragments[1].Command, Is.EqualTo("InternalLink"));
            Assert.That(fragments[2].ToString(), Is.EqualTo("[external]"));
            Assert.That(fragments[2].Command, Is.EqualTo("ExternalLink"));
            Assert.That(fragments[3].ToString(), Is.EqualTo("text"));
            Assert.That(fragments[3].Command, Is.EqualTo("Text"));
            Assert.That(fragments[4].Command, Is.EqualTo("Template"));
            Assert.That(fragments[5].Command, Is.EqualTo("InternalLink"));
            Assert.That(fragments[6].Command, Is.EqualTo("ExternalLink"));
            Assert.That(fragments[7].Command, Is.EqualTo("Text"));
        }

        [Test]
        public void CanParse_Heading1()
        {
            var fragments = parser.Parse("=Heading 1=").ToList();
            Assert.That(fragments.Count, Is.EqualTo(1));
            Assert.That(fragments[0].Command, Is.EqualTo("Heading"));
            Assert.That(fragments[0].Argument, Is.EqualTo("="));
        }

        [Test]
        public void CanParse_Heading2()
        {
            var fragments = parser.Parse("==Heading 2==").ToList();
            Assert.That(fragments.Count, Is.EqualTo(1));
            Assert.That(fragments[0].Command, Is.EqualTo("Heading"));
            Assert.That(fragments[0].Argument, Is.EqualTo("=="));
        }

        [TestCase("* List item contents", "UnorderedListItem", "*")]
        [TestCase("** List item contents", "UnorderedListItem", "**")]
        [TestCase("*** List item contents", "UnorderedListItem", "***")]
        [TestCase("# List item contents", "OrderedListItem", "#")]
        [TestCase("## List item contents", "OrderedListItem", "##")]
        [TestCase("### List item contents", "OrderedListItem", "###")]
        public void Lists_CanContainChildFragments_WithText(string input, string expectedName, string expectedValue)
        {
            var fragments = parser.Parse(input).ToList();
            Assert.That(fragments.Count, Is.EqualTo(1), "'" + input + "' resulted in too many fragments.");
            Assert.That(fragments[0].Command, Is.EqualTo(expectedName));
            Assert.That(fragments[0].Argument, Is.EqualTo(expectedValue));
            
            var children = fragments[0].Components.ToList();
            Assert.That(children.Count, Is.EqualTo(1));
            Assert.That(children[0].Command, Is.EqualTo("Text"));
            Assert.That(children[0].ToString(), Is.EqualTo("List item contents"));
        }

        [Test]
        public void List_MayContain_LinkAndText()
        {
            var fragments = parser.Parse("* [[Link]] Text").ToList();
            Assert.That(fragments.Count, Is.EqualTo(1));
            Assert.That(fragments[0].Components.Count(), Is.EqualTo(2));
            Assert.That(fragments[0].Components.ToList()[0].Command, Is.EqualTo("InternalLink"));
            Assert.That(fragments[0].Components.ToList()[1].Command, Is.EqualTo("Text"));
        }

        [Test]
        public void DoesntInsertLineFragment_BetweenListFragments()
        {
            string input = @"* Line 1
* Line 2";
            var fragments = parser.Parse(input).ToList();

            Assert.That(fragments.Count, Is.EqualTo(2));
            Assert.That(fragments[0].Command, Is.EqualTo("UnorderedListItem"));
            Assert.That(fragments[1].Command, Is.EqualTo("UnorderedListItem"));
        }

        [Test]
        public void DoesntAppendsToParentFragment_WhenParsingNestedList()
        {
            string input = @"* List 1
** List 1.1";
            var fragments = parser.Parse(input).ToList();

            Assert.That(fragments.Count, Is.EqualTo(2));
            Assert.That(fragments[0].Command, Is.EqualTo("UnorderedListItem"));
            Assert.That(fragments[0].Components.Single().Command, Is.EqualTo("Text"));
        }

        [Test]
        public void AppendsSubsequentFragment_ToMasterList_AfterSeveralNestedItems()
        {
            string input = @"* List 1
** List 1.1
** List 1.2
* List 2";
            var fragments = parser.Parse(input).ToList();

            Assert.That(fragments.Count, Is.EqualTo(4));
            Assert.That(fragments[0].Command, Is.EqualTo("UnorderedListItem"));
            Assert.That(fragments[0].Argument, Is.EqualTo("*"));
            Assert.That(fragments[1].Command, Is.EqualTo("UnorderedListItem"));
            Assert.That(fragments[1].Argument, Is.EqualTo("**"));
            Assert.That(fragments[2].Command, Is.EqualTo("UnorderedListItem"));
            Assert.That(fragments[2].Argument, Is.EqualTo("**"));
            Assert.That(fragments[3].Command, Is.EqualTo("UnorderedListItem"));
            Assert.That(fragments[3].Argument, Is.EqualTo("*"));
        }

//        [Test]
//        public void ChildFragments_ReferToEachOther()
//        {
//            string input = @"* List 1
//** List 1.1
//** List 1.2";
//            var fragments = parser.Parse(input).ToList();
//            var children = fragments[0].Components.ToList();

//            Assert.That(children[0].Next, Is.SameAs(children[1]));
//            Assert.That(children[1].Previous, Is.SameAs(children[0]));
//        }
        

        [Test]
        public void AppendsSubsequentFragment_ToMasterList_AfterHorriblyNestedList()
        {
            string input = @"* List 1
** List 1.1
*** List 1.1.1
* List 2";
            var fragments = parser.Parse(input).ToList();

            Assert.That(fragments.Count, Is.EqualTo(4));
            Assert.That(fragments.Any(f => f.Command != "UnorderedListItem"), Is.False);
        }

//        [Test]
//        public void UnderstandingNewLineMatches()
//        {
//            string input = @"hello
//world";
//            var m = System.Text.RegularExpressions.Regex.Matches(input, @"[\r\n]+", System.Text.RegularExpressions.RegexOptions.CultureInvariant | System.Text.RegularExpressions.RegexOptions.IgnoreCase | System.Text.RegularExpressions.RegexOptions.IgnorePatternWhitespace | System.Text.RegularExpressions.RegexOptions.Multiline);
//            Assert.That(m[0].Value, Is.EqualTo(Environment.NewLine));
//            Assert.That(m.Count, Is.EqualTo(1));
//            Assert.That(m[0].Index, Is.EqualTo(5));
//            Assert.That(m[0].Length, Is.EqualTo(2));
//        }

//        [Test]
//        public void UnderstandingDotMatches()
//        {
//            string txt = @"hello
//world";
//            var m = System.Text.RegularExpressions.Regex.Matches(txt, @"^.*$[\r\n]*", System.Text.RegularExpressions.RegexOptions.CultureInvariant | System.Text.RegularExpressions.RegexOptions.IgnoreCase | System.Text.RegularExpressions.RegexOptions.IgnorePatternWhitespace | System.Text.RegularExpressions.RegexOptions.Multiline);
//            Assert.That(m.Count, Is.EqualTo(2));
//            Assert.That(m[0].Value, Is.EqualTo("hello" + Environment.NewLine));
//            Assert.That(m[1].Value, Is.EqualTo("world"));
//        }

        //[Test]
        //public void Next_PointsTo_NextFragment()
        //{
        //    string input = "hello [[world]]";
        //    var fragments = parser.Parse(input).ToList();
        //    Assert.That(fragments[0].Next, Is.SameAs(fragments[1]));
        //    Assert.That(fragments[1].Next, Is.Null);
        //}

        //[Test]
        //public void Previous_PointsTo_PreviousFragment()
        //{
        //    string input = "hello [[world]]";
        //    var fragments = parser.Parse(input).ToList();
        //    Assert.That(fragments[0].Previous, Is.Null); 
        //    Assert.That(fragments[1].Previous, Is.SameAs(fragments[0]));
        //}
    }
}
