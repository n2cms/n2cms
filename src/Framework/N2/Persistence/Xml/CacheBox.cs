using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace N2.Persistence.Xml
{
	public class CacheBox<TEntity>
		where TEntity: class
	{
		public TEntity Value
		{
			get
			{
				return IsEmpty ? null : EntityFactory();
			}
			set
			{
				if (value != null)
					EntityFactory = () => value;
				else
					EntityFactory = null;
			}
		}
		// TODO: add multithreading support
		public Func<TEntity> EntityFactory { get; set; }
		public bool IsEmpty 
		{
			get { return EntityFactory == null; }
		}
		public int UnresolvedParentID { get; set; }
	}

}
