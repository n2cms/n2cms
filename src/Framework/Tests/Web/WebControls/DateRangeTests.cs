using System;
using System.Globalization;
using System.Threading;
using N2.Web.UI.WebControls;
using NUnit.Framework;

namespace N2.Tests.Web.WebControls
{
    [TestFixture]
    public class DateRangeTests
    {
        CultureInfo originalCulture;

        [SetUp]
        public  void SetUp()
        {
            originalCulture = Thread.CurrentThread.CurrentCulture;
        }

        [TearDown]
        public void TearDown()
        {
            Thread.CurrentThread.CurrentCulture = originalCulture;
        }



        [Test]
        public void DateRange_DisplaysPm_OnEnglishUSCultures()
        {
            Thread.CurrentThread.CurrentCulture = CultureInfo.GetCultureInfo("en-US");

            DateRange rangeControl = new DateRange();
            rangeControl.From = new DateTime(2009, 02, 27, 20, 12, 24);

            Assert.That(rangeControl.FromDatePicker.DatePickerBox.Text, Is.EqualTo("2/27/2009"));
            Assert.That(rangeControl.FromDatePicker.TimePickerBox.Text, Is.EqualTo("8:12:24 PM"));
        }

        [Test]
        public void DateRange_Display24HourTime_OnSwedishCultures()
        {
            Thread.CurrentThread.CurrentCulture = CultureInfo.GetCultureInfo("sv-SE");

            DateRange rangeControl = new DateRange();
            rangeControl.From = new DateTime(2009, 02, 27, 20, 12, 24);

            Assert.That(rangeControl.ToDatePicker.DatePickerBox.Text, Is.Empty);
            Assert.That(rangeControl.ToDatePicker.TimePickerBox.Text, Is.Empty);
            Assert.That(rangeControl.FromDatePicker.DatePickerBox.Text, Is.EqualTo("2009-02-27"));
            Assert.That(rangeControl.FromDatePicker.TimePickerBox.Text, Is.EqualTo("20:12:24"));
        }

        [Test]
        public void DateRange_Display24HourTime_OnToControl_InSwedishCultures()
        {
            Thread.CurrentThread.CurrentCulture = CultureInfo.GetCultureInfo("sv-SE");

            DateRange rangeControl = new DateRange();
            rangeControl.To = new DateTime(2009, 02, 27, 20, 12, 24);

            Assert.That(rangeControl.ToDatePicker.DatePickerBox.Text, Is.EqualTo("2009-02-27"));
            Assert.That(rangeControl.ToDatePicker.TimePickerBox.Text, Is.EqualTo("20:12:24"));
            Assert.That(rangeControl.FromDatePicker.DatePickerBox.Text, Is.Empty);
            Assert.That(rangeControl.FromDatePicker.TimePickerBox.Text, Is.Empty);
        }

        [Test]
        public void DateRange_StripsZeroSeconds_FromTimeBoxes()
        {
            Thread.CurrentThread.CurrentCulture = CultureInfo.GetCultureInfo("sv-SE");

            DateRange rangeControl = new DateRange();
            rangeControl.From = new DateTime(2009, 02, 27, 20, 15, 00);
            rangeControl.To = new DateTime(2009, 02, 27, 20, 30, 00);

            Assert.That(rangeControl.FromDatePicker.TimePickerBox.Text, Is.EqualTo("20:15"));
            Assert.That(rangeControl.ToDatePicker.TimePickerBox.Text, Is.EqualTo("20:30"));
        }

        [Test]
        public void DateRange_StripsZeroSeconds_FromEnUSTimeBoxes()
        {
            Thread.CurrentThread.CurrentCulture = CultureInfo.GetCultureInfo("en-US");

            DateRange rangeControl = new DateRange();
            rangeControl.From = new DateTime(2009, 02, 27, 8, 15, 00);
            rangeControl.To = new DateTime(2009, 02, 27, 20, 30, 00);

            Assert.That(rangeControl.FromDatePicker.TimePickerBox.Text, Is.EqualTo("8:15 AM"));
            Assert.That(rangeControl.ToDatePicker.TimePickerBox.Text, Is.EqualTo("8:30 PM"));
        }

        [Test]
        public void DateRange_DisplayDate_OnFromControl_InRussianCultures()
        {
            Thread.CurrentThread.CurrentCulture = CultureInfo.GetCultureInfo("ru-RU");

            DateRange rangeControl = new DateRange();
            rangeControl.From = new DateTime(2014, 03, 06, 20, 12, 24);

            Assert.That(rangeControl.FromDatePicker.DatePickerBox.Text, Is.EqualTo("06.03.2014"));
            Assert.That(rangeControl.FromDatePicker.TimePickerBox.Text, Is.EqualTo("20:12:24"));
            Assert.That(rangeControl.ToDatePicker.DatePickerBox.Text, Is.Empty);
            Assert.That(rangeControl.ToDatePicker.TimePickerBox.Text, Is.Empty);
        }

    }
}
