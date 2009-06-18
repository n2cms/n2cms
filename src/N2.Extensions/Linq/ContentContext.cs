//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using NHibernate;
//using NHibernate.Linq;
//using N2.Details;
//using N2.Persistence.NH;

//namespace N2.Linq
//{
//    public class ContentContext
//    {
//        private ISessionProvider sessionProvider;

//        public ContentContext(ISessionProvider sessionProvider)
//        {
//            this.sessionProvider = sessionProvider;
//        }

//        public IOrderedQueryable<ContentItem> ContentItems
//        {
//            get 
//            {
//                return sessionProvider.OpenSession.Session.Linq<ContentItem>();
//            }
//        }

//        public IOrderedQueryable<T> Elements<T>()
//        {
//            return sessionProvider.OpenSession.Session.Linq<T>();
//        }
//    }
//}
