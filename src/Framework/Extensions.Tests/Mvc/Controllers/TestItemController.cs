using System.Web.Mvc;
using N2.Extensions.Tests.Mvc.Models;
using N2.Web;
using N2.Web.Mvc;
using N2.Web.UI;

namespace N2.Extensions.Tests.Mvc.Controllers
{
    [Controls(typeof (TestItem))]
    public class TestItemController : ContentController<TestItem>
    {
        public ActionResult UsingView()
        {
            return View("Index");
        }

        public ActionResult WithModel()
        {
            var model = new TestItemModel(CurrentItem);

            return View("Index", model);
        }

        public ActionResult Submit(string q)
        {
            return View(new string[q.Length]);
        }

        #region Nested type: TestItemModel

        public class TestItemModel : IItemContainer<TestItem>
        {
            private readonly TestItem _item;

            public TestItemModel(TestItem item)
            {
                _item = item;
            }

            #region IItemContainer<TestItem> Members

            ContentItem IItemContainer.CurrentItem
            {
                get { return CurrentItem; }
            }

            public TestItem CurrentItem
            {
                get { return _item; }
            }

            #endregion
        }

        #endregion
    }
}
