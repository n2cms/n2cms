using N2.Details;
using System.Text;

namespace N2.Persistence.NH.Finder
{
    public abstract class AbstractInHqlProvider<T> : IHqlProvider
    {
        private readonly Operator op;
        private readonly string name;
        private readonly T[] values;

        public AbstractInHqlProvider(Operator op, string name, T[] values)
        {
            this.op = op;
            this.name = name;
            this.values = values;
        }

        #region IHqlProvider Members

        public abstract void AppendHql(StringBuilder from, StringBuilder where, int index);

        protected string GetParameters(int index)
        {
            string[] parameters = new string[Values.Length];
            for (int i = 0; i < parameters.Length; i++)
            {
                parameters[i] = ":" + GetParameterName(index, i);
            }
            return string.Join(",", parameters);
        }

        public string Name
        {
            get { return name; }
        }

        public T[] Values
        {
            get { return values; }
        }

        protected string GetParameterName(int index, int i)
        {
            return "p" + index + "i" + i;
        }

        public virtual void SetParameters(NHibernate.IQuery query, int index)
        {
            for (int i = 0; i < Values.Length; i++)
            {
                query.SetParameter(GetParameterName(index, i), ContentDetail.ExtractQueryValue(Values[i]));
            }
        }

        public string GetNameParameterName(int index)
        {
            return "n" + index;
        }

        protected virtual string GetOperator()
        {
            if (op == Operator.None)
                return string.Empty;
            else
                return op.ToString();
        }

        #endregion
    }
}
