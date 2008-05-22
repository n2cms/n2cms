using System;
using System.Collections.Generic;
using System.Text;
using N2.Edit.Settings;

namespace N2.Tests.Edit.Settings.Services
{
	public class VeryUsefulService : IVeryUsefulService
	{
		private bool beUseful;

		[EditableCheckBox("Be useful", -100)]
		public bool BeUseful
		{
			get { return beUseful; }
			set { beUseful = value; }
		}
	}
}
