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
using System.ComponentModel;
using DaveSexton.DocProject.DocSites;

namespace N2.DocSite.Controls
{
    public partial class DocSiteLetterBar : System.Web.UI.UserControl
    {
        #region Public Properties
        public char[] Letters
        {
            get
            {
                return DocSiteManager.Settings.LetterBar.ToCharArray();
            }
        }

        public int LettersCount
        {
            get
            {
                return DocSiteManager.Settings.LetterBar.Length;
            }
        }
        #endregion

        #region Private / Protected
        #endregion

        #region Constructors
        /// <summary>
        /// Constructs a new instance of the <see cref="DocSiteLetterBar" /> class.
        /// </summary>
        public DocSiteLetterBar()
        {
        }
        #endregion

        #region Methods
        protected void Page_Load(object sender, EventArgs e)
        {
            DataBind();
        }
        #endregion

        #region Events
        private readonly object LetterClickEvent = new object();

        /// <summary>
        /// Event raised when a letter is clicked.
        /// </summary>
        [Category("Action")]
        [Description("Event raised when a letter is clicked.")]
        public event EventHandler<CommandEventArgs> LetterClick
        {
            add
            {
                lock (LetterClickEvent)
                {
                    Events.AddHandler(LetterClickEvent, value);
                }
            }
            remove
            {
                lock (LetterClickEvent)
                {
                    Events.RemoveHandler(LetterClickEvent, value);
                }
            }
        }

        /// <summary>
        /// Raises the <see cref="LetterClick" /> event.
        /// </summary>
        /// <param name="e"><see cref="CommandEventArgs" /> object that provides the arguments for the event.</param>
        protected virtual void OnLetterClick(CommandEventArgs e)
        {
            EventHandler<CommandEventArgs> handler = null;

            lock (LetterClickEvent)
            {
                handler = (EventHandler<CommandEventArgs>)Events[LetterClickEvent];
            }

            if (handler != null)
                handler(this, e);
        }
        #endregion

        #region Event Handlers
        protected void letter_Command(object sender, CommandEventArgs e)
        {
            OnLetterClick(e);
        }
        #endregion
    }
}