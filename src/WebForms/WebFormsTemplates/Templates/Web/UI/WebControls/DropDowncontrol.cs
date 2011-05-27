using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using N2.Templates.Items;
using N2.Persistence;

namespace N2.Templates.Web.UI.WebControls
{
    public class DropDowncontrol : Control, IQuestionControl
    {


        public readonly Label mlable;
        public readonly DropDownList mDropdown;
        public readonly RequiredFieldValidator rfv;
                
        public DropDowncontrol(DropSelect question)
        {
            mDropdown = new DropDownList();
            mDropdown.CssClass = "alternatives";
            mDropdown.ID = question.ID.ToString();
            mDropdown.DataTextField = "Title";
            mDropdown.DataValueField = "ID";
           

            if (question["Required"] != null && (bool)question["Required"] == true)
            {
                Option select = (Option)question.GetChild("Select") ;
                if (select == null)
                {
                     select = N2.Context.Current.Resolve<ContentActivator>().CreateInstance<Option>(question);
                    select.Title = "Select";
                    select.Name = "Select";
                    N2.Context.Persister.Save(select);
                    mDropdown.DataSource = question.Options;
                    mDropdown.DataBind();
                    mDropdown.Items.FindByValue(select.ID.ToString()).Selected = true;
                }
                else {
                    mDropdown.DataSource = question.Options;
                    mDropdown.DataBind();
                    mDropdown.Items.FindByValue(select.ID.ToString()).Selected = true;
                }

                mDropdown.CausesValidation = true;
                mDropdown.ValidationGroup = "Form";
                rfv = new RequiredFieldValidator();
                rfv.ControlToValidate = mDropdown.ID;
                rfv.ErrorMessage = "Required Field";
                rfv.ValidationGroup = "Form";
                rfv.ID = "Required" + mDropdown.ID;
                rfv.SetFocusOnError = true;
                rfv.Enabled = true;
                rfv.Display = ValidatorDisplay.Dynamic;
                rfv.EnableClientScript = true;
                rfv.InitialValue = select.ID.ToString();
                Controls.Add(rfv);
                //Page.Validators.Add(rfv);
            }
            else
            {
                mDropdown.DataSource = question.GetChildren();
                mDropdown.DataBind();
            }

            

            mlable = new Label();
            mlable.CssClass = "label";
            mlable.Text = question.Title;
            mlable.AssociatedControlID = mDropdown.ID;



            Controls.Add(mlable);
            Controls.Add(mDropdown);
        }

        #region " IQuestionControl Members "

        public string AnswerText
        {
            get { return mDropdown.SelectedItem.Text; }
        }

        public string Question
        {
            get { return mlable.Text; }
        }

        #endregion

        protected override void Render(HtmlTextWriter writer)
        {
            writer.Write("<div class='question cf'>");
            base.Render(writer);
            writer.Write("</div>");
        }


    }
}