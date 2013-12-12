using System;
using System.Diagnostics;
using NUnit.Framework;
using Shouldly;
using System.Linq;

namespace N2.Tests.Definitions
{
    [TestFixture]
    public class AttributeReference
    {
        public interface IVeichle
        {
            string GetDescription();
        }

        public class OffRoadAttribute : Attribute, IVeichle
        {
            public string GetDescription()
            {
                return "OffRoad";
            }
        }

        public class ReleaseYear : Attribute, IVeichle
        {
            private readonly int year;

            public ReleaseYear(int year)
            {
                this.year = year;
            }

            public string GetDescription()
            {
                return "Y" + year;
            }
        }

        [AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
        public class FourWheelerAttribute : Attribute, IVeichle
        {
            private string description = "Y";

            public string Description
            {
                get { return description; }
                set { description = value; }
            }

            public string GetDescription()
            {
                return description;
            }
        }

        [AttributeUsage(AttributeTargets.Class, Inherited = false)]
        public class OddAttribute : Attribute
        {
        }

        [OffRoad]
        [Odd]
        public class Car
        {
        }

        [FourWheeler]
        [Odd]
        public class Volvo : Car
        {
        }

        [OffRoad, FourWheeler(Description = "4wd optional")]
        public class VolvoV70 : Volvo
        {
        }

        [Test]
        public void AttributeExploration()
        {
            object[] a1 = typeof(Car).GetCustomAttributes(typeof(IVeichle), false);
            Assert.AreEqual(1, a1.Length);
            foreach (IVeichle i in a1)
                Debug.WriteLine("a1: " + i.GetDescription());
            Debug.WriteLine("");

            object[] a2 = typeof(Car).GetCustomAttributes(typeof(IVeichle), true);
            Assert.AreEqual(1, a2.Length);
            foreach (IVeichle i in a2)
                Debug.WriteLine("a2: " + i.GetDescription());
            Debug.WriteLine("");

            object[] b1 = typeof(Volvo).GetCustomAttributes(typeof(IVeichle), false);
            Assert.AreEqual(1, b1.Length);
            foreach (IVeichle i in b1)
                Debug.WriteLine("b1: " + i.GetDescription());
            Debug.WriteLine("");

            object[] b2 = typeof(Volvo).GetCustomAttributes(typeof(IVeichle), true);
            Assert.AreEqual(2, b2.Length);
            foreach (IVeichle i in b2)
                Debug.WriteLine("b2: " + i.GetDescription());
            Debug.WriteLine("");

            object[] c1 = typeof(VolvoV70).GetCustomAttributes(typeof(IVeichle), false);
            Assert.AreEqual(2, c1.Length);
            foreach (IVeichle i in c1)
                Debug.WriteLine("c1: " + i.GetDescription());
            Debug.WriteLine("");

            object[] c2 = typeof(VolvoV70).GetCustomAttributes(typeof(IVeichle), true);
            Assert.AreEqual(3, c2.Length);
            foreach (IVeichle i in c2)
                Debug.WriteLine("c2: " + i.GetDescription());

            int oddCar = typeof(Car).GetCustomAttributes(typeof(OddAttribute), true).Length;
            int oddVolvo = typeof(Volvo).GetCustomAttributes(typeof(OddAttribute), true).Length;
            int oddVolvoV70 = typeof(VolvoV70).GetCustomAttributes(typeof(OddAttribute), true).Length;
            Assert.That(oddCar, Is.EqualTo(1));
            Assert.That(oddVolvo, Is.EqualTo(1));
            Assert.That(oddVolvoV70, Is.EqualTo(0));

            int offCar = typeof(Car).GetCustomAttributes(typeof(OffRoadAttribute), true).Length;
            int offVolvo = typeof(Volvo).GetCustomAttributes(typeof(OffRoadAttribute), true).Length;
            Assert.That(offCar, Is.EqualTo(1));
            Assert.That(offVolvo, Is.EqualTo(1));

            int offCarNonInherit = typeof(Car).GetCustomAttributes(typeof(OffRoadAttribute), false).Length;
            int offVolvoNonInherit = typeof(Volvo).GetCustomAttributes(typeof(OffRoadAttribute), false).Length;
            Assert.That(offCarNonInherit, Is.EqualTo(1));
            Assert.That(offVolvoNonInherit, Is.EqualTo(0));
        }

        [Inherited, Uninherited, AllowMultiple, DisallowMultiple, Default]
        public class A { }

        public class B : A { }

        [Inherited, Uninherited, AllowMultiple, DisallowMultiple, Default]
        public class C : B { }

        public class Default : Attribute { }

        [AttributeUsage(AttributeTargets.Class, Inherited = true)]
        public class Inherited : Attribute { }

        [AttributeUsage(AttributeTargets.Class, Inherited = false)]
        public class Uninherited : Attribute { }

        [AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
        public class AllowMultiple : Attribute { }

        [AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
        public class DisallowMultiple : Attribute { }

        Type at = typeof(A);
        Type bt = typeof(B);
        Type ct = typeof(C);
        
        [Test]
        public void AttributesOnInheritedClasses()
        {
            at.GetCustomAttributes(true).Length.ShouldBe(5);
            at.GetCustomAttributes(false).Length.ShouldBe(5);

            bt.GetCustomAttributes(true).Length.ShouldBe(4);
            bt.GetCustomAttributes(true).OfType<Uninherited>().ShouldBeEmpty();
            bt.GetCustomAttributes(false).Length.ShouldBe(0);

            ct.GetCustomAttributes(true).Length.ShouldBe(6);
            ct.GetCustomAttributes(true).OfType<AllowMultiple>().Count().ShouldBe(2);
            ct.GetCustomAttributes(false).Length.ShouldBe(5);
        }

        [AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
        public class BaseA : Attribute { }
        public class SuperA : BaseA { }

        [SuperA, BaseA]
        public class X { }
        [SuperA, BaseA]
        public class Y : X { }

        Type xt = typeof(X);
        Type yt = typeof(Y);

        [Test]
        public void AttributesInheritingOtherAttributes()
        {
            xt.GetCustomAttributes(true).Length.ShouldBe(2);
            yt.GetCustomAttributes(true).Length.ShouldBe(3);
            yt.GetCustomAttributes(true).OfType<SuperA>().Count().ShouldBe(1);

        }
    }
}
