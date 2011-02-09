using System;
using N2.Details;

namespace N2.Web.Rendering
{
	public abstract class DisplayableRendererBase<TDisplayable> : IDisplayableRenderer where TDisplayable : IDisplayable
	{
		public abstract void Render(RenderingContext context, TDisplayable displayable);

		#region IDisplayableRenderer Members

		public Type HandledDisplayableType
		{
			get { return typeof(TDisplayable); }
		}

		public void Render(RenderingContext context)
		{
			Render(context, (TDisplayable)context.Displayable);
		}

		#endregion
	}

}
