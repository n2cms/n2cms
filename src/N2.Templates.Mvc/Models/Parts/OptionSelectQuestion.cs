using System;
using System.Collections.Generic;
using N2.Templates.Details;

namespace N2.Templates.Mvc.Models.Parts
{
	public abstract class OptionSelectQuestion : Question
	{
		[EditableOptions(Title = "Options", SortOrder = 20)]
		public virtual IList<Option> Options
		{
			get
			{
				var options = new List<Option>();
				foreach (Option o in GetChildren())
					options.Add(o);
				return options;
			}
		}
	}
}