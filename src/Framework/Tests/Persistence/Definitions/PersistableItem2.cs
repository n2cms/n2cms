using System;
using N2.Definitions;
using N2.Engine.Globalization;

namespace N2.Tests.Persistence.Definitions
{
    [SortChildren(SortBy.Expression, SortExpression = "Name DESC")]
    public class PersistableItem2 : PersistableItem, ILanguage
    {
        [N2.Persistence.Persistable]
        public virtual bool BoolPersistableProperty { get; set; }
        [N2.Persistence.Persistable]
        public virtual int IntPersistableProperty { get; set; }
        [N2.Persistence.Persistable]
        public virtual DateTime DateTimePersistableProperty { get; set; }
        [N2.Persistence.Persistable]
        public virtual double DoublePersistableProperty { get; set; }
        [N2.Persistence.Persistable]
        public virtual string StringPersistableProperty { get; set; }
        [N2.Persistence.Persistable]
        public virtual ContentItem LinkPersistableProperty { get; set; }
        [N2.Persistence.Persistable]
        public virtual Base64FormattingOptions EnumPersistableProperty { get; set; }

        #region ILanguage Members

        public virtual string LanguageTitle { get; set; }

        public virtual string LanguageCode { get; set; }

        #endregion
    }
}
