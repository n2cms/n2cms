using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace N2.Definitions.Runtime
{
	public class ContainerBuilder<TModel, TContainer> : ContainerBuilder<TContainer>
		where TContainer : IEditableContainer
	{
		public new ContentRegistration<TModel> Registration { get; set; }

		public ContainerBuilder(string containerName, ContentRegistration<TModel> re)
			: base(containerName, re)
		{
			Registration = re;
		}
	}

	public class ContainerBuilder<TContainer> : Builder<TContainer>, IDisposable
		where TContainer : IEditableContainer
	{
		private string containerName;
		private IDisposable scope;

		public ContainerBuilder(string containerName, ContentRegistration re)
			: base(containerName, re)
		{
			this.containerName = containerName;
		}

		public ContainerBuilder<TContainer> Begin()
		{
			scope = Registration.Context.BeginContainer(containerName);
			return this;
		}

		public ContainerBuilder<TContainer> End()
		{
			if (scope != null)
				scope.Dispose();
			return this;
		}

		void IDisposable.Dispose()
		{
			End();
		}
	}
}
