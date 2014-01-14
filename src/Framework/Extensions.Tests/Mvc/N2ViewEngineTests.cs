//using System;
//using System.Web;
//using System.Web.Mvc;
//using System.Web.Routing;
//using N2.Extensions.Tests.Mvc.Controllers;
//using N2.Extensions.Tests.Mvc.Models;
//using N2.Web;
//using N2.Web.Mvc;
//using NUnit.Framework;
//using Rhino.Mocks;

//namespace N2.Extensions.Tests.Mvc
//{
//    [TestFixture]
//    public class N2ViewEngineTests : N2.Tests.ItemPersistenceMockingBase
//    {
//        private IViewEngine mockViewEngine;
//        private IView mockView;
//        private IControllerMapper mockControllerMapper;

//        [SetUp]
//        public override void SetUp()
//        {
//            mockControllerMapper = MockRepository.GenerateStub<IControllerMapper>();

//            MvcConventionTemplateAttribute attribute;
//            PathDictionary.Instance[typeof (RegularPage)] = new[]
//                                                                {
//                                                                    attribute = new MvcConventionTemplateAttribute("Regular")
//                                                                        {ControllerMapper = mockControllerMapper}
//                                                                };

//            mockControllerMapper.Stub(m => m.GetControllerName(null)).IgnoreArguments().Return("Default");
//            mockControllerMapper.Stub(m => m.ControllerHasAction("Default", attribute.DefaultAction)).IgnoreArguments().Return(true);

//            mockViewEngine = MockRepository.GenerateMock<IViewEngine>();
//            mockView = MockRepository.GenerateStub<IView>();
//        }

//        [TearDown]
//        public override void TearDown()
//        {
//            PathDictionary.Instance.Remove(typeof(RegularPage));
//        }

//        [Test]
//        public void CanGetTemplateUrl_DefaultView()
//        {
//            var testViewEngine = new N2ViewEngine(new ViewEngineCollection { mockViewEngine });
//            var httpContextBase = MockRepository.GenerateStub<HttpContextBase>();
//            var routeData = new RouteData();
//            routeData.Values.Add("controller", "Default");
//            routeData.DataTokens.Add(ContentRoute.ContentItemKey, new RegularPage());
//            var controller = new RegularController();

//            var controllerContext = new ControllerContext(httpContextBase, routeData, controller);

//            mockViewEngine.Expect(engine => engine.FindView(controllerContext, "Index", null, true))
//                .Return(new ViewEngineResult(mockView, mockViewEngine)).Repeat.Once();

//            // Act
//            testViewEngine.FindView(controllerContext, "Index", null, true);

//            // Assert
//            mockViewEngine.VerifyAllExpectations();
//            Assert.That(controllerContext.RouteData.Values["controller"], Is.EqualTo("Default"));
//        }

//        [Test]
//        public void CanGetTemplateUrl_OtherView()
//        {
//            var testViewEngine = new N2ViewEngine(new ViewEngineCollection { mockViewEngine });
//            var httpContextBase = MockRepository.GenerateStub<HttpContextBase>();
//            var routeData = new RouteData();
//            routeData.Values.Add("controller", "Default");
//            routeData.DataTokens.Add(ContentRoute.ContentItemKey, new RegularPage());
//            var controller = new RegularController();

//            var controllerContext = new ControllerContext(httpContextBase, routeData, controller);

//            mockViewEngine.Expect(engine => engine.FindView(controllerContext, "OtherView", null, true))
//                .Return(new ViewEngineResult(mockView, mockViewEngine)).Repeat.Once();

//            // Act
//            testViewEngine.FindView(controllerContext, "OtherView", null, true);

//            // Assert
//            mockViewEngine.VerifyAllExpectations();
//            Assert.That(controllerContext.RouteData.Values["controller"], Is.EqualTo("Default"));
//        }

//        [Test]
//        public void CanGetTemplateUrl_FullPathToViewAsViewName()
//        {
//            var testViewEngine = new N2ViewEngine(new ViewEngineCollection { mockViewEngine });
//            var httpContextBase = MockRepository.GenerateStub<HttpContextBase>();
//            var routeData = new RouteData();
//            routeData.Values.Add("controller", "Default");
//            routeData.DataTokens.Add(ContentRoute.ContentItemKey, new RegularPage());
//            var controller = new RegularController();

//            var controllerContext = new ControllerContext(httpContextBase, routeData, controller);

//            mockViewEngine.Expect(engine => engine.FindView(controllerContext, "~/Views/Shared/OtherView.aspx", null, true))
//                .Return(new ViewEngineResult(mockView, mockViewEngine)).Repeat.Once();

//            // Act
//            testViewEngine.FindView(controllerContext, "~/Views/Shared/OtherView.aspx", null, true);

//            // Assert
//            mockViewEngine.VerifyAllExpectations();
//            Assert.That(controllerContext.RouteData.Values["controller"], Is.EqualTo("Default"));
//        }

//        [Test]
//        public void CanGetTemplateUrl_FullPathToViewAsTemplateName()
//        {
//            var testViewEngine = new N2ViewEngine(new ViewEngineCollection { mockViewEngine });
//            var httpContextBase = MockRepository.GenerateStub<HttpContextBase>();
//            var routeData = new RouteData();
//            routeData.Values.Add("controller", "Default");
//            routeData.DataTokens.Add(ContentRoute.ContentItemKey, new TemplatedItem());
//            var controller = new RegularController();

//            var controllerContext = new ControllerContext(httpContextBase, routeData, controller);

//            mockViewEngine.Expect(engine => engine.FindView(controllerContext, "~/Views/Shared/Templated.ascx", null, true))
//                .Return(new ViewEngineResult(mockView, mockViewEngine)).Repeat.Once();

//            // Act
//            testViewEngine.FindView(controllerContext, "index", null, true);

//            // Assert
//            mockViewEngine.VerifyAllExpectations();
//            Assert.That(controllerContext.RouteData.Values["controller"], Is.EqualTo("Default"));
//        }

//        [Test]
//        public void CanGetTemplateUrl_NotFound()
//        {
//            var testViewEngine = new N2ViewEngine(new ViewEngineCollection { mockViewEngine });
//            var httpContextBase = MockRepository.GenerateStub<HttpContextBase>();
//            var routeData = new RouteData();
//            routeData.Values.Add("controller", "Default");
//            routeData.DataTokens.Add(ContentRoute.ContentItemKey, new RegularPage());
//            var controller = new RegularController();

//            var controllerContext = new ControllerContext(httpContextBase, routeData, controller);

//            mockViewEngine.Expect(engine => engine.FindView(controllerContext, "NotFound", null, true))
//                .Return(new ViewEngineResult(new string[0])).Repeat.Once();

//            // Act
//            var result = testViewEngine.FindView(controllerContext, "NotFound", null, true);

//            // Assert
//            mockViewEngine.VerifyAllExpectations();
//            Assert.That(controllerContext.RouteData.Values["controller"], Is.EqualTo("Default"));
//            Assert.That(result, Is.Not.Null);
//            Assert.That(result.View, Is.Null);
//        }

//        [Definition]
//        public class TemplatedItem : ContentItem
//        {
//            public override string TemplateUrl
//            {
//                get
//                {
//                    return "~/Views/Shared/Templated.ascx";
//                }
//            }
//        }
//    }
//}
