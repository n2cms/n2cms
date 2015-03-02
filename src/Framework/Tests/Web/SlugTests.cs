using N2.Configuration;
using N2.Web;
using NUnit.Framework;
using Shouldly;

namespace N2.Tests.Web
{
    [TestFixture]
    public class SlugTests
    {
        private Slug caseInvariantSlug;
        private Slug lowercaseSlug;
        private Slug defaultSlug;
        private Slug customReplacementSlug;

        private Slug CreateCaseInvariantSlug()
        {
            NameEditorElement nameEditorElement = new NameEditorElement
            {
                WhitespaceReplacement = '-',
                Replacements = new PatternValueCollection(),
                ToLower = false
            };

            EditSection editSection = new EditSection { NameEditor = nameEditorElement };

            ConfigurationManagerWrapper cmw = new ConfigurationManagerWrapper
            {
                Sections = new ConfigurationManagerWrapper.ContentSectionTable(null, null, null, editSection)
            };

            return new Slug(cmw);
        }

        private Slug CreateLowercaseSlug()
        {
            NameEditorElement nameEditorElement = new NameEditorElement
            {
                WhitespaceReplacement = '-',
                Replacements = new PatternValueCollection(),
                ToLower = true
            };

            EditSection editSection = new EditSection { NameEditor = nameEditorElement };

            ConfigurationManagerWrapper cmw = new ConfigurationManagerWrapper
            {
                Sections = new ConfigurationManagerWrapper.ContentSectionTable(null, null, null, editSection)
            };

            return new Slug(cmw);
        }

        private Slug CreateDefaultSlug()
        {
            NameEditorElement nameEditorElement = new NameEditorElement();
            EditSection editSection = new EditSection { NameEditor = nameEditorElement };

            ConfigurationManagerWrapper cmw = new ConfigurationManagerWrapper
            {
                Sections = new ConfigurationManagerWrapper.ContentSectionTable(null, null, null, editSection)
            };

            return new Slug(cmw);
        }

        private Slug CreateCustomReplacementSlug()
        {
            PatternValueCollection patterns = new PatternValueCollection();
            patterns.Clear(); // to remove all those added by constructor
            patterns.Add(new PatternValueElement("c1", "[@]", "at", true));

            NameEditorElement nameEditorElement = new NameEditorElement
            {
                // WhitespaceReplacement = '-',
                Replacements = patterns //,
                // ToLower = true
            };

            EditSection editSection = new EditSection { NameEditor = nameEditorElement };

            ConfigurationManagerWrapper cmw = new ConfigurationManagerWrapper
            {
                Sections = new ConfigurationManagerWrapper.ContentSectionTable(null, null, null, editSection)
            };

            return new Slug(cmw);
        }

        [TestFixtureSetUp]
        public void TestFixtureSetUp()
        {
            this.caseInvariantSlug = this.CreateCaseInvariantSlug();
            this.lowercaseSlug = this.CreateLowercaseSlug();
            this.defaultSlug = this.CreateDefaultSlug();
            this.customReplacementSlug = this.CreateCustomReplacementSlug();
        }

        [TestCase("", "")]
        public void Create_Should_Handle_Edge_Cases(string value, string expected)
        {
            string result = this.defaultSlug.Create(value);
            Assert.AreEqual(expected, result);          
        }

        [TestCase("ṃ,ỹ,ṛ,è,ş,ư,ḿ,ĕ", "m-y-r-e-s-u-m-e")]
        [TestCase("á-é-í-ó-ú", "a-e-i-o-u")]
        [TestCase("à,å,á,â,ã,å,ą", "a-a-a-a-a-a-a")]
        [TestCase("ä", "a")]
        [TestCase("è,é,ê,ë,ę", "e-e-e-e-e")]
        [TestCase("ì,í,î,ï,ı", "i-i-i-i-i")]
        [TestCase("ò,ó,ô,õ,ø", "o-o-o-o-o")]
        [TestCase("ö", "o")]
        [TestCase("ù,ú,û", "u-u-u")]
        [TestCase("ü", "u")]
        [TestCase("ç,ć,č", "c-c-c")]
        [TestCase("ż,ź,ž", "z-z-z")]
        [TestCase("ś,ş,š", "s-s-s")]
        [TestCase("ñ,ń", "n-n")]
        [TestCase("ý,Ÿ", "y-Y")]
        [TestCase("ł,Ł", "l-L")]
        [TestCase("đ", "d")]
        [TestCase("ß", "ss")]
        [TestCase("ğ", "g")]
        [TestCase("Þ", "th")]
        public void Create_Should_Remove_Accents_Case_Invariant(string value, string expected)
        {
            string result = this.caseInvariantSlug.Create(value);
            Assert.AreEqual(expected, result);
        }

        [TestCase("ý,Ÿ", "y-y")]
        [TestCase("ł,Ł", "l-l")]
        [TestCase("Đ,đ", "d-d")]
        public void Create_Should_Remove_Accents_To_Lower(string value, string expected)
        {
            string result = this.lowercaseSlug.Create(value);
            Assert.AreEqual(expected, result);
        }

        [TestCase("Slug Me ", "slug-me")]
        [TestCase("Slug Me,", "slug-me")]
        [TestCase("Slug Me.", "slug-me")]
        [TestCase("Slug Me/", "slug-me")]
        [TestCase("Slug Me\\", "slug-me")]
        [TestCase("Slug Me-", "slug-me")]
        [TestCase("Slug Me_", "slug-me")]
        [TestCase("Slug Me=", "slug-me")]
        [TestCase("Slug Me--", "slug-me")]
        [TestCase("Slug Me---,", "slug-me")]
        public void Create_Should_Remove_Trailing_Punctuation(string value, string expected)
        {
            string result = this.defaultSlug.Create(value);
            Assert.AreEqual(expected, result);
        }

        [TestCase("å,á,à,â,ã", "a-a-a-a-a")]
        [TestCase("@", "at")]
        [TestCase("Å,Á,À,Â,Ã", "A-A-A-A-A")]
        [TestCase("æ,ä", "ae-a")]
        [TestCase("Æ,Ä", "Ae-A")]
        [TestCase("é,è", "e-e")]
        [TestCase("É,È", "E-E")]
        [TestCase("í,ì", "i-i")]
        [TestCase("Í,Ì", "I-I")]
        [TestCase("ø,ó,ò,ô,õ", "o-o-o-o-o")]
        [TestCase("Ø,Ó,Ò,Ô,Õ", "O-O-O-O-O")]
        [TestCase("ú,ù", "u-u")]
        [TestCase("Ú,Ù", "U-U")]
        [TestCase("ü", "u")]
        [TestCase("Ü", "U")]
        [TestCase("ß", "ss")]
        public void Create_Should_Keep_CaseInvariant_Compatibility_With_Old_N2_Slug_Implementation(string value, string expected)
        {
            string result = this.caseInvariantSlug.Create(value);
            Assert.AreEqual(expected, result);
        }   
        
        [TestCase("å,á,à,â,ã", "a-a-a-a-a")]
        [TestCase("@", "at")]
        [TestCase("Å,Á,À,Â,Ã", "a-a-a-a-a")]
        [TestCase("æ,ä", "ae-a")]
        [TestCase("Æ,Ä", "ae-a")]
        [TestCase("é,è", "e-e")]
        [TestCase("É,È", "e-e")]
        [TestCase("í,ì", "i-i")]
        [TestCase("Í,Ì", "i-i")]
        [TestCase("ø,ó,ò,ô,õ", "o-o-o-o-o")]
        [TestCase("Ø,Ó,Ò,Ô,Õ", "o-o-o-o-o")]
        [TestCase("ú,ù", "u-u")]
        [TestCase("Ú,Ù", "u-u")]
        [TestCase("ü", "u")]
        [TestCase("Ü", "u")]
        [TestCase("ß", "ss")]
        public void Create_Should_Keep_Lowercase_Compatibility_With_Old_N2_Slug_Implementation(string value, string expected)
        {
            string result = this.lowercaseSlug.Create(value);
            Assert.AreEqual(expected, result);
        }

        [TestCase("@", "at")]
        public void Create_Should_Apply_Config_Replacements(string value, string expected)
        {
            var result = this.customReplacementSlug.Create(value);
            Assert.AreEqual(expected, result);          
        }

        [TestCase("abc")]
        //[TestCase("abc%def")]
        [TestCase("abc-def")]
        [TestCase("abc_def")]
        [TestCase("abc.def")]
        [TestCase("abc~def")]
        [TestCase("abc!def")]
        [TestCase("abc$def")]
        [TestCase("abc()def")]
        [TestCase("abc,def")]
        [TestCase("abc;def")]
        [TestCase("abc=def")]
        public void IsValid_Should_Accept_Valid_Slugs(string value)
        {
            this.defaultSlug.IsValid(value).ShouldBe(true);
        }

        //[TestCase("abc def")]
        [TestCase("abc%20def")]
        [TestCase("abc+def")]
        [TestCase("abc\\")]
        [TestCase("abc#")]
        [TestCase("abc?")]
        //[TestCase("abc[]")]
        [TestCase("abc%")]
        [TestCase("abc%x")]
        [TestCase("abc%xx")]
        [TestCase("abc@")]
        [TestCase("abc&def")]
        [TestCase("abc'def")]
        [TestCase("abc*def")]
        public void IsValid_Should_Refuse_Invalid_Slugs(string value)
        {
            this.defaultSlug.IsValid(value).ShouldBe(false);
        }

        [TestCase(":")]
        [TestCase("abc:")]
        [TestCase("abc:def")]
        [TestCase(":def")]
        [TestCase("abc:80")]
        [TestCase("/")]
        [TestCase("abc/")]
        [TestCase("abc/def")]
        [TestCase("/def")]
        [TestCase("?")]
        [TestCase("abc?")]
        [TestCase("abc?def")]
        [TestCase("?def")]
        [TestCase("#")]
        [TestCase("abc#")]
        [TestCase("abc#def")]
        [TestCase("#def")]
        //[TestCase("[")]
        //[TestCase("abc[")]
        //[TestCase("abc[def")]
        //[TestCase("[def")]
        //[TestCase("]")]
        //[TestCase("abc]")]
        //[TestCase("abc]def")]
        //[TestCase("]def")]
        [TestCase("@")]
        [TestCase("abc@")]
        [TestCase("abc@def")]
        [TestCase("@def")]
        [TestCase("&")]
        [TestCase("abc&")]
        [TestCase("abc&def")]
        [TestCase("&def")]
        [TestCase("+")]
        [TestCase("abc+")]
        [TestCase("abc+def")]
        [TestCase("+def")]
        [TestCase("'")]
        [TestCase("abc'")]
        [TestCase("abc'def")]
        [TestCase("'def")]
        [TestCase("*")]
        [TestCase("abc*")]
        [TestCase("abc*def")]
        [TestCase("*def")]
        public void IsValid_Should_Refuse_GenDelims_And_Ampersand_And_Plus_And_Apostrophe_And_Star(string value)
        {
            // based on rfc3986
            // gen-delims  = ":" / "/" / "?" / "#" / "[" / "]" / "@"

            this.defaultSlug.IsValid(value).ShouldBe(false);
        }

        [TestCase("!")]
        [TestCase("abc!")]
        [TestCase("abc!def")]
        [TestCase("!def")]
        [TestCase("$")]
        [TestCase("abc$")]
        [TestCase("abc$def")]
        [TestCase("$def")]
        [TestCase("(")]
        [TestCase("abc(")]
        [TestCase("abc(def")]
        [TestCase("(def")]
        [TestCase(")")]
        [TestCase("abc)")]
        [TestCase("abc)def")]
        [TestCase(")def")]
        [TestCase(",")]
        [TestCase("abc,")]
        [TestCase("abc,def")]
        [TestCase(",def")]
        [TestCase(";")]
        [TestCase("abc;")]
        [TestCase("abc;def")]
        [TestCase(";def")]
        [TestCase("=")]
        [TestCase("abc=")]
        [TestCase("abc=def")]
        [TestCase("=def")]
        public void IsValid_Should_Accept_SubDelims_Without_Ampersand_Without_Plus_Without_Apostrophe_Without_Star(string value)
        {
            // based on rfc3986
            // sub-delims  = "!" / "$" / "&" / "'" / "(" / ")" / "*" / "+" / "," / ";" / "="

            this.defaultSlug.IsValid(value).ShouldBe(true);
        }

        [TestCase("my-pageђ")]
        [TestCase("my-pageš")]
        [TestCase("žmy-page")]
        [TestCase("mđy-page")]
        public void IsValid_Should_Allow_NonEnglish_Letters(string value)
        {
            this.defaultSlug.IsValid(value).ShouldBe(true);
        }

        [TestCase("MySlug")]
        [TestCase("My-Slug")]
        [TestCase("my-Slug")]
        [TestCase("my-SluG")]
        [TestCase("My-SluG")]
        public void IsValid_Should_Accept_Mixed_Capitals_And_Small_Letters(string value)
        {
            this.defaultSlug.IsValid(value).ShouldBe(true);
        }
    }
}
