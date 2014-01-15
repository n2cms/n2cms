using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using N2.Definitions;
using N2.Details;
using N2.Definitions.Static;
using N2.Configuration;

namespace N2.Tests.Definitions
{
    [TestFixture]
    public class AttributeTransformerTests : TypeFindingBase
    {
        DefinitionBuilder builder;

        [SetUp]
        public override void SetUp()
        {
            base.SetUp();

            builder = new DefinitionBuilder(new DefinitionMap(), typeFinder, new TransformerBase<IUniquelyNamed>[] { new TitleTransformer() }, TestSupport.SetupEngineSection());
        }

        protected override Type[] GetTypes()
        {
            return new Type[] { typeof(Items.DefinitionStartPage) };
        }

        [Test]
        public void Transforms_Editable_OfCorrectType()
        {
            IList<IEditable> editables = builder.GetDefinitions().Single(d => d.ItemType == typeof(Items.DefinitionStartPage)).Editables;
            
            Assert.That(editables.OfType<WithEditableTitleAttribute>().Single().Title.EndsWith(" Transformed"));
        }

        [Test]
        public void DoesntTransform_Editables_OfIncorrectTypes()
        {
            IList<IEditable> editables = builder.GetDefinitions().Single(d => d.ItemType == typeof(Items.DefinitionStartPage)).Editables;
            
            Assert.That(!editables.OfType<WithEditableNameAttribute>().Single().Title.EndsWith(" Transformed"));
        }
    }
}
