using System;
using N2.Web;
using NUnit.Framework;

namespace N2.Tests.Fakes
{
	public class FakeErrorHandler : IErrorHandler
	{
		public void Notify(Exception ex)
		{
			Assert.Fail(ex.ToString());
		}
	}
}