using System.Text;
using NHibernate;

namespace N2.Persistence.NH.Finder
{
    /// <summary>
    /// Classes implementing this interface provide hql need to build the 
    /// query.
    /// </summary>
    public interface IHqlProvider
    {
        void AppendHql(StringBuilder from, StringBuilder where, int index);
        void SetParameters(IQuery query, int index);
    }
}
