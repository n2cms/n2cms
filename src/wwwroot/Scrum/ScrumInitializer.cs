using N2.Engine;
using N2.Plugin;
using N2.Templates.Scrum;

namespace N2.Templates.Scrum
{
	public class ScrumInitializer : IPluginInitializer
	{
		public void Initialize(IEngine engine)
		{
			engine.AddComponent("n2.templates.sprint.makeCurrent", typeof(Commands.MakeCurrentSprint));
			engine.Resolve<N2.Web.AjaxRequestDispatcher>().AddHandler(engine.Resolve<Commands.MakeCurrentSprint>());
		}
	}
}