#region license
// Copyright (c) 2005 - 2007 Ayende Rahien (ayende@ayende.com)
// All rights reserved.
// 
// Redistribution and use in source and binary forms, with or without modification,
// are permitted provided that the following conditions are met:
// 
//     * Redistributions of source code must retain the above copyright notice,
//     this list of conditions and the following disclaimer.
//     * Redistributions in binary form must reproduce the above copyright notice,
//     this list of conditions and the following disclaimer in the documentation
//     and/or other materials provided with the distribution.
//     * Neither the name of Ayende Rahien nor the names of its
//     contributors may be used to endorse or promote products derived from this
//     software without specific prior written permission.
// 
// THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS "AS IS" AND
// ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, THE IMPLIED
// WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR PURPOSE ARE
// DISCLAIMED. IN NO EVENT SHALL THE COPYRIGHT OWNER OR CONTRIBUTORS BE LIABLE
// FOR ANY DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, OR CONSEQUENTIAL
// DAMAGES (INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE GOODS OR
// SERVICES; LOSS OF USE, DATA, OR PROFITS; OR BUSINESS INTERRUPTION) HOWEVER
// CAUSED AND ON ANY THEORY OF LIABILITY, WHETHER IN CONTRACT, STRICT LIABILITY,
// OR TORT (INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN ANY WAY OUT OF THE USE OF
// THIS SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY OF SUCH DAMAGE.
#endregion

using NHibernate;
using NHibernate.Criterion;

namespace N2.Persistence.NH
{
    internal class RepositoryHelper<T>
    {
        internal static IQuery CreateQuery(ISession session, string namedQuery, Parameter[] parameters)
        {
            IQuery query = session.GetNamedQuery(namedQuery);
            foreach (Parameter parameter in parameters)
            {
                query.SetParameter(parameter.Name, parameter.Value);
            }
            return query;
        }

        public static ICriteria GetExecutableCriteria(ISession session, DetachedCriteria criteria, NHibernate.Criterion.Order[] orders)
        {
            ICriteria executableCriteria;
            if (criteria != null)
            {
                executableCriteria = criteria.GetExecutableCriteria(session);
            }
            else
            {
                executableCriteria = session.CreateCriteria(typeof(T));
            }

            if (orders != null)
            {
                foreach (NHibernate.Criterion.Order order in orders)
                {
                    executableCriteria.AddOrder(order);
                }
            }
            return executableCriteria;
        }

        public static ICriteria CreateCriteriaFromArray(ISession session, ICriterion[] criteria)
        {
            ICriteria crit = session.CreateCriteria(typeof(T));
            foreach (ICriterion criterion in criteria)
            {
                //allow some fancy antics like returning possible return 
                // or null to ignore the criteria
                if (criterion == null)
                    continue;
                crit.Add(criterion);
            }
            return crit;
        }
    }
}
