using System.Text;
using NHibernate;

namespace N2.Persistence.NH.Finder
{
    /// <summary>
    /// Abstract base class for hql providers.
    /// </summary>
    /// <typeparam name="T">The type of value to compare against.</typeparam>
    public abstract class AbstractHqlProvider<T> : IHqlProvider
    {
        private Operator op;
        private string name;
        private readonly Comparison comparison;
        private T value;

        public AbstractHqlProvider(Operator op, string name, Comparison comparison, T value)
        {
            this.op = op;
            this.name = name;
            this.comparison = comparison;
            this.value = value;
        }

        #region Properties
        public Operator Op
        {
            get { return op; }
            set { op = value; }
        }

        public string Name
        {
            get { return name; }
            set { name = value; }
        }


        public T Value
        {
            get { return this.value; }
            set { this.value = value; }
        }
        #endregion

        public string GetNameParameterName(int index)
        {
            return "n" + index;
        }

        public string GetValueParameterName(int index)
        {
            return "v" + index;
        }

        protected string GetComparison()
        {
            switch (comparison)
            {
                case Comparison.Equal:
                    return " = ";
                case Comparison.GreaterOrEqual:
                    return " >= ";
                case Comparison.GreaterThan:
                    return " > ";
                case Comparison.LessOrEqual:
                    return " <= ";
                case Comparison.LessThan:
                    return " < ";
                case Comparison.Like:
                    return " like ";
                case Comparison.NotEqual:
                    return " <> ";
                case Comparison.NotLike:
                    return " not like ";
                case Comparison.In:
                    return " in ";
                case Comparison.NotIn:
                    return " not in ";
            }
            return string.Empty;
        }

        #region IHqlProvider Members

        public abstract void AppendHql(StringBuilder from, StringBuilder where, int index);

        public virtual void SetParameters(IQuery query, int index)
        {
            query.SetParameter(GetValueParameterName(index), Value);
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
