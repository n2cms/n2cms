using System;
using System.Data;
using System.Configuration;
using System.Collections;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;
using System.Collections.Generic;
using DaveSexton.DocProject.DocSites;

namespace N2.DocSite
{
    public partial class DocSiteBrowseIndex : System.Web.UI.Page
    {
        #region Public Properties
        public char? KeywordFirstLetter
        {
            get
            {
                return ViewState["_$KeywordFirstLetter"] as char?;
            }
            set
            {
                ViewState["_$KeywordFirstLetter"] = value;

                letterLabel.Visible = value != null;
            }
        }

        public List<string> Keywords
        {
            get
            {
                List<string> keywords = ViewState["_$Keywords"] as List<string>;

                if (keywords == null)
                    ViewState["_$Keywords"] = keywords = new List<string>();

                return keywords;
            }
        }

        public string DocSiteFramePath
        {
            get
            {
                return DocSiteManager.Settings.DocSiteFramePath;
            }
        }

        public bool ShowSearchButton
        {
            get
            {
                return DocSiteManager.Settings.SearchEnabled || User.Identity.IsAuthenticated;
            }
        }
        #endregion

        #region Private / Protected
        #endregion

        #region Constructors
        /// <summary>
        /// Constructs a new instance of the <see cref="DocSiteBrowseIndex" /> class.
        /// </summary>
        public DocSiteBrowseIndex()
        {
        }
        #endregion

        #region Methods
        private void UpdateKeywordsList()
        {
            IList<string> keywords = Keywords;

            if (keywords.Count > 0)
            {
                userKeywordsRepeater.DataBind();
                userKeywordsRepeater.Visible = true;

                // # Occurrences and Weight only have meaningful data when one keyword is selected for the filter
                keywordEntriesGridView.Columns[3].Visible = keywords.Count == 1;
                keywordEntriesGridView.Columns[4].Visible = keywords.Count == 1;

                keywordEntriesGridView.DataBind();
                keywordEntriesPanel.Visible = !keywordsListPanel.Visible;
            }
            else
            {
                userKeywordsRepeater.Visible = false;
                keywordEntriesPanel.Visible = false;
            }
        }

        protected bool IsKeywordIndexed(string keyword)
        {
            return DaveSexton.DocProject.DocSites.DocSiteSearch.IsKeywordIndexed(keyword);
        }
        #endregion

        #region Event Handlers
        protected override void OnInit(EventArgs e)
        {
            if (!User.Identity.IsAuthenticated && !DocSiteManager.Settings.BrowseIndexEnabled)
                Response.Redirect(DocSiteManager.Settings.DocSiteFramePath);

            ScriptManager.GetCurrent(Page).AsyncPostBackTimeout = DocSiteManager.Settings.CreateIndexRefreshTimeout;

            base.OnInit(e);
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            if (!Page.IsPostBack)
            {
                string query = Request.QueryString["q"];

                if (!string.IsNullOrEmpty(query))
                {
                    QueryExpressionFactory factory = new QueryExpressionFactory();
                    factory.DefaultOperator = '&';
                    factory.SplitQueryCharacters = DaveSexton.DocProject.DocSites.DocSiteSearch.DefaultSearchProvider.SplitQueryCharacters;
                    factory.IgnoredWords = DocSiteManager.Settings.SearchExcludedKeywords.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
                    factory.MinimumKeywordLength = DocSiteManager.Settings.SearchMinimumKeywordLength;
                    factory.Optimize = false;

                    QueryExpression expression = factory.CreateExpression(query);

                    if (!(expression is EmptyQueryExpression))
                    {
                        expression.Evaluate(delegate(QueryExpression expr)
                        {
                            TermQueryExpression term = expr as TermQueryExpression;

                            if (term != null && !Keywords.Contains(term.Term))
                                Keywords.Add(term.Term);
                        });
                    }

                    // The index must always be created when a query is specified otherwise it will be generated 
                    // automatically _after_ the keywords are rendered, which means they'll all appear in "red".
                    DaveSexton.DocProject.DocSites.DocSiteSearch.EnsureIndex();

                    keywordsListPanel.Visible = false;
                    KeywordFirstLetter = null;
                }
            }
        }

        protected override void OnPreRender(EventArgs e)
        {
            base.OnPreRender(e);

            char? firstLetter = KeywordFirstLetter;

            if (firstLetter.HasValue)
            {
                keywordsDataSource.SelectParameters["firstLetter"].DefaultValue = firstLetter.ToString();
                keywordEntriesPanel.Visible = false;
                keywordsListPanel.Visible = true;
            }

            UpdateKeywordsList();

            bool indexCreated = DaveSexton.DocProject.DocSites.DocSiteSearch.IndexCreated;

            keywordsCreateIndexProgressPlaceholder.Visible = !indexCreated;
            keywordsPleaesWaitProgressPlaceholder.Visible = indexCreated;
        }

        protected void allKeywordsLinkButton_Click(object sender, EventArgs e)
        {
            keywordsDataSource.SelectParameters["firstLetter"].DefaultValue = null;
            KeywordFirstLetter = null;
            keywordEntriesPanel.Visible = false;
            keywordsListPanel.Visible = true;
            keywordsGridView.PageIndex = 0;
        }

        protected void letterBar_LetterClick(object sender, CommandEventArgs e)
        {
            KeywordFirstLetter = e.CommandName[0];
            keywordsGridView.PageIndex = 0;
            letterLabel.DataBind();
        }

        protected void keywordsLinkButton_Click(object sender, EventArgs e)
        {
            keywordsListPanel.Visible = false;
            KeywordFirstLetter = null;
        }

        protected void keyword_Command(object sender, CommandEventArgs e)
        {
            if (string.Equals(e.CommandName, "Keyword", StringComparison.OrdinalIgnoreCase))
            {
                string word = e.CommandArgument as string;

                if (!string.IsNullOrEmpty(word))
                {
                    if (!Keywords.Contains(word))
                        Keywords.Add(word);

                    keywordsListPanel.Visible = false;
                    KeywordFirstLetter = null;
                }
            }
        }

        protected void removeKeywordImageButton_Command(object sender, CommandEventArgs e)
        {
            if (string.Equals(e.CommandName, "Remove", StringComparison.OrdinalIgnoreCase))
            {
                string word = e.CommandArgument as string;

                if (!string.IsNullOrEmpty(word))
                {
                    Keywords.Remove(word);

                    keywordsListPanel.Visible = false;
                    KeywordFirstLetter = null;
                }
            }
        }

        protected void keywordEntriesDataSource_Selecting(object sender, ObjectDataSourceSelectingEventArgs e)
        {
            e.InputParameters.Add("keywords", Keywords.ToArray());
        }

        protected void searchImageButton_Click(object sender, ImageClickEventArgs e)
        {
            DocSiteNavigator.NavigateToSearchResults(string.Join(" ", Keywords.ToArray()));
        }
        #endregion
    }
}
