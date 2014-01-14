using N2.Configuration;
using N2.Edit;

namespace N2.Tests.Fakes
{
    public class FakeEditUrlManager : EditUrlManager
    {
        public FakeEditUrlManager()
            : base(null, new EditSection())
        {
        }
    }
}
