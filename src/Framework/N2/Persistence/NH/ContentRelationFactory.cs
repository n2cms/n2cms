using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NHibernate.UserTypes;
using NHibernate;
using System.Data;

namespace N2.Persistence.NH
{
    public class ContentRelationFactory : ICompositeUserType
    {
        public object Assemble(object cached, NHibernate.Engine.ISessionImplementor session, object owner)
        {
            throw new NotImplementedException();
        }

        public object DeepCopy(object value)
        {
            var cr = value as ContentRelation;
            if (cr == null)
                return value;

            return new ContentRelation { ForeignID = cr.ForeignID, ValueAccessor = cr.ValueAccessor };
        }

        public object Disassemble(object value, NHibernate.Engine.ISessionImplementor session)
        {
            throw new NotImplementedException();
        }

        public new bool Equals(object x, object y)
        {
            var cx = x as ContentRelation;
            var cy = y as ContentRelation;
            if (cx == cy) return true;
            if (cx == null || cy == null) return false;
            return cx.ForeignID == cy.ForeignID;
        }

        public int GetHashCode(object x)
        {
            throw new NotImplementedException();
        }

        public object GetPropertyValue(object component, int property)
        {
            ContentRelation cr = component as ContentRelation;
            if (cr == null)
                return null;
            
            return cr.ForeignID;
        }

        public bool IsMutable
        {
            get { return false; }
        }

        public object NullSafeGet(IDataReader dr, string[] names, NHibernate.Engine.ISessionImplementor session, object owner)
        {
            var value = dr[names[0]];
            if (value == DBNull.Value)
                return new ContentRelation();

            var id = (int)value;
            return new ContentRelation
            {
                ForeignID = id,
                ValueAccessor = () => ((ISession)session).Get<ContentItem>(id)
            };
        }

        public void NullSafeSet(IDbCommand cmd, object value, int index, bool[] settable, NHibernate.Engine.ISessionImplementor session)
        {
            NHibernateUtil.Int32.NullSafeSet(cmd, (value == null) ? (object)null : ((ContentRelation)value).ForeignID, index);
            //if(value == null)
            //    cmd.Parameters[index] = DBNull.Value;
            //else
            //    cmd.Parameters[index] = ((ContentRelation)value).ForeignID;
        }

        public string[] PropertyNames
        {
            get { return new string[] { "VersionOfID" }; }
        }

        public NHibernate.Type.IType[] PropertyTypes
        {
            get { return new NHibernate.Type.IType[] { NHibernateUtil.Int32 }; }
        }

        public object Replace(object original, object target, NHibernate.Engine.ISessionImplementor session, object owner)
        {
            throw new NotImplementedException();
        }

        public Type ReturnedClass
        {
            get { return typeof(ContentRelation); }
        }

        public void SetPropertyValue(object component, int property, object value)
        {
            throw new NotImplementedException();
        }
    }
}
