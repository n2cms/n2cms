using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using N2.Web.Parsing;
using System.IO;
using Shouldly;
using N2.Management.Content.Export;

namespace N2.Management.Tests
{
    public static class CsvParserTestsExtensions
    {
        public static IEnumerable<CsvRow> Parse(this CsvParser parser, TextReader reader)
        {
            return parser.Parse(';', reader);
        }
    }

    [TestFixture]
    public class CsvParserTests
    {
        CsvParser p = new CsvParser();

        [TestCase]
        public void SingleRow()
        {
            var row = p.Parse(new StringReader("hello;world")).Single();
            row.Columns[0].ShouldBe("hello");
            row.Columns[1].ShouldBe("world");
        }

        [TestCase]
        public void SurroundingWhiteSpace_IsTrimmed()
        {
            var row = p.Parse(new StringReader("hello  ;world  ")).Single();
            row.Columns[0].ShouldBe("hello");
            row.Columns[1].ShouldBe("world");
        }

        [TestCase]
        public void TwoRows()
        {
            var rows = p.Parse(new StringReader(@"hello
world")).ToList();
            rows[0].Columns.Single().ShouldBe("hello");
            rows[1].Columns.Single().ShouldBe("world");
        }

        [TestCase]
        public void SingleRow_EndingWithNewLine_YieldsSingleResult()
        {
            var rows = p.Parse(new StringReader(@"hello;world
")).ToList();
            rows.Single().Columns.Count.ShouldBe(2);
        }

        [TestCase]
        public void Quotes_AreOmitted()
        {
            var row = p.Parse(new StringReader("\"hello\";\"world\"")).Single();
            row.Columns[0].ShouldBe("hello");
            row.Columns[1].ShouldBe("world");
        }

        [TestCase]
        public void Quotes_MayContain_EscapedQuotes()
        {
            var row = p.Parse(new StringReader("\"hell\\\"no\"")).Single();
            row.Columns[0].ShouldBe("hell\"no");
        }

        [TestCase]
        public void Quotes_MayContain_Newline()
        {
            var row = p.Parse(new StringReader(@"""hell
no""")).Single();
            row.Columns[0].ShouldBe(@"hell
no");
        }

        [TestCase]
        public void Quotes_MayContain_EscapedNewline()
        {
            var row = p.Parse(new StringReader("\"hell\nno\"")).Single();
            row.Columns[0].ShouldBe(@"hell
no");
        }

        [TestCase]
        public void Quotes_MayContain_Separator()
        {
            var row = p.Parse(new StringReader("\"hell;no\"")).Single();
            row.Columns[0].ShouldBe(@"hell;no");
        }

        [TestCase]
        public void DoubleQuote_InQuotes_IsContent()
        {
            var row = p.Parse(new StringReader("\"\"\"\"")).Single();
            row.Columns[0].ShouldBe("\"");
        }

        [TestCase]
        public void SurroundingWhiteSpace_InQuotes_IsRetained()
        {
            var row = p.Parse(new StringReader("\"hello  \";\"world  \"")).Single();
            row.Columns[0].ShouldBe("hello  "); ;
            row.Columns[1].ShouldBe("world  ");
        }

        [TestCase]
        public void GuessBestSeparator_GuessesOn_ExistingSeparator()
        {
            var separator = p.GuessBestSeparator(() => new StringReader("hello;world\nworld;hello"), '\t', ',', ';');

            separator.ShouldBe(';');
        }

        [TestCase]
        public void GuessBestSeparator_PrefersSeparator_WithMoreThanOneOccurance()
        {
            var separator = p.GuessBestSeparator(() => new StringReader("hello;world"), '\t', ',', ';');

            separator.ShouldBe(';');
        }

        [TestCase]
        public void GuessBestSeparator_PrefersSeparator_WithSameNumberOfColumns()
        {
            var separator = p.GuessBestSeparator(() => new StringReader("hello;world,universe\nworld,universe;hello,howdy"), '\t', ',', ';');

            separator.ShouldBe(';');
        }

        [TestCase]
        public void GuessBestSeparator_PrefersSeparator_WithSameNumberOfColumns_OnThe3FirstRows()
        {
            var separator = p.GuessBestSeparator(() => new StringReader("hello;world,universe\nworld,universe;hello\nmoi;mukkolat"), '\t', ',', ';');

            separator.ShouldBe(';');
        }
    }
}
