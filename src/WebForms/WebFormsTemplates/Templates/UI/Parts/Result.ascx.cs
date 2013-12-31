using System;
using N2.Templates.Items;

namespace N2.Templates.UI.Parts
{
    public partial class Result : Templates.Web.UI.TemplateUserControl<ContentItem, N2.Templates.Items.Poll>
    {
        private int total = 0;

        public int Total
        {
            get { return total; }
            set { total = value; }
        }
    
        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);
            Resources.Register.StyleSheet(Page, "~/Templates/UI/Css/Poll.css", N2.Resources.Media.All);

            LoadTotal();

            rptAnswers.DataSource = CurrentItem.Question.Options;
            rptAnswers.DataBind();
        }

        private void LoadTotal()
        {
            if (CurrentItem.Question != null)
            {
                foreach (Option o in CurrentItem.Question.Options)
                {
                    Total += o.Answers;
                }
            }
            if (Total == 0)
                Total = 100;
        }
    }
}
