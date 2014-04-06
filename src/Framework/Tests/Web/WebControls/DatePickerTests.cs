using System;
using System.Globalization;
using System.Threading;
using N2.Web.UI.WebControls;
using NUnit.Framework;

namespace N2.Tests.Web.WebControls
{
    [TestFixture]
    public class DatePickerTests
    {
        CultureInfo startCulture = Thread.CurrentThread.CurrentCulture;

        [TestFixtureTearDown]
        public void TestFixtureTearDown()
        {
            Thread.CurrentThread.CurrentCulture = startCulture;
        }

        [Test]
        public void DatePickerSetSelectedDate_enUSCulture_ShouldCorrectlyFormatDateAndTimePart()
        {
            Thread.CurrentThread.CurrentCulture = CultureInfo.GetCultureInfo("en-US");

            DatePicker picker = new DatePicker();
            picker.SelectedDate = new DateTime(2010, 08, 29, 13, 12, 33);

            Assert.That(picker.DatePickerBox.Text, Is.EqualTo("8/29/2010"));
            Assert.That(picker.TimePickerBox.Text, Is.EqualTo("1:12:33 PM"));
        }

        [Test]
        public void DatePickerSetSelectedDate_skSkCulture_ShouldCorrectlyFormatDateAndTimePart()
        {
            Thread.CurrentThread.CurrentCulture = CultureInfo.GetCultureInfo("sk-Sk");

            DatePicker picker = new DatePicker();
            var date = new DateTime(2010, 08, 29, 13, 12, 33);
            picker.SelectedDate = date;

            Assert.That(picker.DatePickerBox.Text, Is.EqualTo(date.ToShortDateString()));
            Assert.That(picker.TimePickerBox.Text, Is.EqualTo(date.ToString("HH:mm:ss")));
        }

        [Test]
        public void DatePickerSetSelectedDate_svSECulture_ShouldCorrectlyFormatDateAndTimePart()
        {
            Thread.CurrentThread.CurrentCulture = CultureInfo.GetCultureInfo("sv-SE");

            DatePicker picker = new DatePicker();
            picker.SelectedDate = new DateTime(2010, 08, 29, 13, 12, 33);

            Assert.That(picker.DatePickerBox.Text, Is.EqualTo("2010-08-29"));
            Assert.That(picker.TimePickerBox.Text, Is.EqualTo("13:12:33"));
        }

        [Test]
        public void DatePickerSetSelectedDate_enUSCultureAndZeroSeconds_ShouldTrimSeconds()
        {
            Thread.CurrentThread.CurrentCulture = CultureInfo.GetCultureInfo("en-US");

            DatePicker picker = new DatePicker();
            picker.SelectedDate = new DateTime(2010, 08, 29, 13, 12, 00);

            Assert.That(picker.TimePickerBox.Text, Is.EqualTo("1:12 PM"));
        }

        [Test]
        public void DatePickerSetSelectedDate_skSKCultureAndZeroSeconds_ShouldTrimSeconds()
        {
            Thread.CurrentThread.CurrentCulture = CultureInfo.GetCultureInfo("sk-Sk");

            DatePicker picker = new DatePicker();
            picker.SelectedDate = new DateTime(2010, 08, 29, 13, 12, 00);

            Assert.That(picker.TimePickerBox.Text, Is.EqualTo("13:12"));
        }
    }
}
