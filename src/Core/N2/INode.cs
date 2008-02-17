using System;
using System.Collections.Generic;
using System.Text;

namespace N2
{
	public interface INode
	{
		string Title { get; set; }
		string Name { get; set; }
		string Path { get; }
		IEnumerable<INode> GetChildren();
		//void AddTo(INode parent);
	}
}
