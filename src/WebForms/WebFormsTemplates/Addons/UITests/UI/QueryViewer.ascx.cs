using System;
using System.Collections.Generic;
using System.Web;
using log4net;
using log4net.Appender;
using log4net.Core;
using log4net.Repository.Hierarchy;
using log4net.Config;

namespace N2.Addons.UITests.UI
{
    public partial class QueryViewer : N2.Web.UI.ContentUserControl<ContentItem, Items.QueryViewerItem>
    {
    
        static QueryViewer()
        {
            AddLogger("NHibernate", () => All);
            AddLogger("NHibernate.SQL", () => Queries);
            
        }

        private static void AddLogger(string loggerName, Func<List<string>> listGetter)
        {
            Logger logger = (Logger)LogManager.GetLogger(loggerName).Logger;
            logger.Level = Level.All;
            logger.AddAppender(new CountToContextItemsAppender(listGetter));
            logger.Level = logger.Hierarchy.LevelMap["DEBUG"];
            BasicConfigurator.Configure(logger.Repository);
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            
        }

        const string queriesTextKey = "queriesTextKey";
        public static List<String> Queries
        {
            get
            {
                List<String> queries = ((List<string>)HttpContext.Current.Items[queriesTextKey]);
                if (queries == null)
                {
                    HttpContext.Current.Items[queriesTextKey] = queries = new List<String>();
                }
                return queries;
            }
        }

        const string allTextKey = "allTextKey";
        public static List<String> All
        {
            get
            {
                List<String> queries = ((List<string>)HttpContext.Current.Items[allTextKey]);
                if (queries == null)
                {
                    HttpContext.Current.Items[allTextKey] = queries = new List<String>();
                }
                return queries;
            }
        }

        #region CountToContextItemsAppender
        
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
        
        public class CountToContextItemsAppender : IAppender
        {
            Func<List<string>> getListToUse;

            public CountToContextItemsAppender(Func<List<string>> getListToUse)
            {
                this.getListToUse = getListToUse;
            }

            private string name;

            #region IAppender Members

            public void Close()
            {
            }

            public void DoAppend(LoggingEvent loggingEvent)
            {
                if (string.Empty.Equals(loggingEvent.MessageObject))
                    return;//can happen for batch queries, this is a noise issue, basically.
                
                if (loggingEvent.MessageObject != null)
                {
                    getListToUse().Add(loggingEvent.MessageObject.ToString());
                }
            }

            public string Name
            {
                get { return name; }
                set { name = value; }
            }

            #endregion
        }

        #endregion
    }
}
