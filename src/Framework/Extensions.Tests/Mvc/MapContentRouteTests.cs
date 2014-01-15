using System;
using System.Web.Mvc;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Routing;
using NUnit.Framework;
using N2.Web.Mvc;
using Rhino.Mocks;
using N2.Engine;

namespace N2.Extensions.Tests.Mvc
{
    [TestFixture]
    public class MapContentRouteTests
    {
        [Test]
        public void ContentRoute_IsAdded()
        {
            RouteCollection rc = new RouteCollection();
            rc.MapContentRoute("content", MockRepository.GenerateStub<IEngine>());

            Assert.That(rc.Count, Is.EqualTo(1));
            Assert.That(rc.First(), Is.InstanceOf<ContentRoute>());
        }

        [Test]
        public void ContentRoute_IsInsertedBefore_NonContentRoute()
        {
            RouteCollection rc = new RouteCollection();
            rc.MapRoute("mvc", "{controller}/{action}");
            rc.MapContentRoute("content", MockRepository.GenerateStub<IEngine>());

            Assert.That(rc.Count, Is.EqualTo(2));
            Assert.That(rc.First(), Is.InstanceOf<ContentRoute>());
        }

        [Test]
        public void ContentRoute_IsInsertedBetween_OtherContentRoutes_AndNonContentRoute()
        {
            RouteCollection rc = new RouteCollection();
            rc.MapRoute("mvc", "{controller}/{action}");
            rc.MapContentRoute<Models.RegularPage>("area", MockRepository.GenerateStub<IEngine>());
            rc.MapContentRoute("content", MockRepository.GenerateStub<IEngine>());

            Assert.That(rc.Count, Is.EqualTo(3));
            Assert.That(rc.First(), Is.InstanceOf<ContentRoute<Models.RegularPage>>());
            Assert.That(rc.Skip(1).First(), Is.InstanceOf<ContentRoute>());
        }

        [Test]
        public void GenericContentRoute_IsInsertedBefore_ContentRoute()
        {
            RouteCollection rc = new RouteCollection();
            rc.MapContentRoute("content", MockRepository.GenerateStub<IEngine>());
            rc.MapContentRoute<Models.RegularPage>("area", MockRepository.GenerateStub<IEngine>());

            Assert.That(rc.Count, Is.EqualTo(2));
            Assert.That(rc.First(), Is.InstanceOf<ContentRoute<Models.RegularPage>>());
            Assert.That(rc.Skip(1).First(), Is.InstanceOf<ContentRoute>());
        }

        [Test]
        public void GenericContentRoute_IsntPushedBackBy_ContentRoute()
        {
            RouteCollection rc = new RouteCollection();
            rc.MapContentRoute<Models.RegularPage>("area", MockRepository.GenerateStub<IEngine>());
            rc.MapContentRoute("content", MockRepository.GenerateStub<IEngine>());

            Assert.That(rc.Count, Is.EqualTo(2));
            Assert.That(rc.First(), Is.InstanceOf<ContentRoute<Models.RegularPage>>());
            Assert.That(rc.Skip(1).First(), Is.InstanceOf<ContentRoute>());
        }

    }
}
