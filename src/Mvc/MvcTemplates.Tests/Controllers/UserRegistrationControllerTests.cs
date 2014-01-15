using System.Web.Mvc;
using N2.Templates.Mvc.Controllers;
using N2.Templates.Mvc.Models;
using NUnit.Framework;
using N2.Templates.Mvc.Models.Parts;

namespace N2.Templates.Mvc.Tests.Controllers
{
    [TestFixture]
    public class UserRegistrationControllerTests
    {
        [Test]
        public void Index()
        {
            var userRegistration = new UserRegistration();
            var controller = new UserRegistrationController(null, null)
                                {
                                    CurrentItem = userRegistration
                                };

            var result = controller.Index() as PartialViewResult;

            Assert.That(result.ViewData.Model, Is.TypeOf(typeof(UserRegistrationModel)));
        }
    }
}
