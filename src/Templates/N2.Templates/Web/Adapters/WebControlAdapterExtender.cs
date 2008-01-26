using System;
using System.Data;
using System.Collections;
using System.Collections.Generic;
using System.Configuration;
using System.Reflection;
using System.Web;
using System.Web.Configuration;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.Adapters;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.Adapters;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;

namespace CSSFriendly
{
    public class WebControlAdapterExtender
    {
        private WebControl _adaptedControl = null;
        public WebControl AdaptedControl
        {
            get { return _adaptedControl; }
        }

        public bool AdapterEnabled
        {
            get
            {
                bool bReturn = true; // normally the adapters are enabled

                //  Individual controls can use the expando property called AdapterEnabled
                //  as a way to turn the adapters off.
                //  <asp:TreeView runat="server" AdapterEnabled="false" />
                if ((AdaptedControl != null) &&
                    (AdaptedControl.Attributes["AdapterEnabled"] != null) &&
                    (AdaptedControl.Attributes["AdapterEnabled"].Length > 0) &&
                    (AdaptedControl.Attributes["AdapterEnabled"].IndexOf("false", StringComparison.OrdinalIgnoreCase) == 0))
                {
                    bReturn = false;
                }

                return bReturn;
            }
        }

        public WebControlAdapterExtender(WebControl adaptedControl)
        {
            _adaptedControl = adaptedControl;
        }

        public void RegisterScripts()
        {
            string folderPath = (WebConfigurationManager.AppSettings.Get("CSSFriendly-JavaScript-Path") != null) ? WebConfigurationManager.AppSettings.Get("CSSFriendly-JavaScript-Path") : "~/Js";
            string filePath = folderPath.EndsWith("/") ? folderPath + "AdapterUtils.js" : folderPath + "/AdapterUtils.js";
            AdaptedControl.Page.ClientScript.RegisterClientScriptInclude(GetType(), GetType().ToString(), AdaptedControl.Page.ResolveUrl(filePath));
        }

        public void RaiseAdaptedEvent(string eventName, EventArgs e)
        {
            string attr = "OnAdapted" + eventName;
            if ((AdaptedControl != null) &&
                (AdaptedControl.Attributes[attr] != null) &&
                (AdaptedControl.Attributes[attr].Length > 0))
            {
                string delegateName = AdaptedControl.Attributes[attr];
                MethodInfo method = AdaptedControl.Page.GetType().GetMethod(delegateName);
                if (method != null)
                {
                    object[] args = new object[2];
                    args[0] = AdaptedControl;
                    args[1] = e;
                    method.Invoke(AdaptedControl.Page, args);
                }
            }
        }

        public void RenderBeginTag(HtmlTextWriter writer, string cssClass)
        {
            if ((AdaptedControl.Attributes["CssSelectorClass"] != null) && (AdaptedControl.Attributes["CssSelectorClass"].Length > 0))
            {
                WriteBeginDiv(writer, AdaptedControl.Attributes["CssSelectorClass"]);
            }

            WriteBeginDiv(writer, cssClass);
        }

        public void RenderEndTag(HtmlTextWriter writer)
        {
            WriteEndDiv(writer);

            if ((AdaptedControl.Attributes["CssSelectorClass"] != null) && (AdaptedControl.Attributes["CssSelectorClass"].Length > 0))
            {
                WriteEndDiv(writer);
            }
        }

        static public void RemoveProblemChildren(Control ctrl, List<ControlRestorationInfo> stashedControls)
        {
            RemoveProblemTypes(ctrl.Controls, stashedControls);
        }

        static public void RemoveProblemTypes(ControlCollection coll, List<ControlRestorationInfo> stashedControls)
        {
            foreach (Control ctrl in coll)
            {
                if (typeof(RequiredFieldValidator).IsAssignableFrom(ctrl.GetType()) ||
                    typeof(CompareValidator).IsAssignableFrom(ctrl.GetType()) ||
                    typeof(RegularExpressionValidator).IsAssignableFrom(ctrl.GetType()) ||
                    typeof(ValidationSummary).IsAssignableFrom(ctrl.GetType()))
                {
                    ControlRestorationInfo cri = new ControlRestorationInfo(ctrl, coll);
                    stashedControls.Add(cri);
                    coll.Remove(ctrl);
                    continue;
                }

                if (ctrl.HasControls())
                {
                    RemoveProblemTypes(ctrl.Controls, stashedControls);
                }
            }
        }

        static public void RestoreProblemChildren(List<ControlRestorationInfo> stashedControls)
        {
            foreach (ControlRestorationInfo cri in stashedControls)
            {
                cri.Restore();
            }
        }

        public string MakeChildId(string postfix)
        {
            return AdaptedControl.ClientID + "_" + postfix;
        }

        static public string MakeNameFromId(string id)
        {
            string name = "";
            for (int i=0; i<id.Length; i++)
            {
                char thisChar = id[i];
                char prevChar = ((i - 1) > -1) ? id[i - 1] : ' ';
                char nextChar = ((i + 1) < id.Length) ? id[i + 1] : ' ';
                if (thisChar == '_')
                {
                    if (prevChar == '_')
                    {
                        name += "_";
                    }
                    else if (nextChar == '_')
                    {
                        name += "$_";
                    }
                    else
                    {
                        name += "$";
                    }
                }
                else
                {
                    name += thisChar;
                }
            }
            return name;
        }

        static public string MakeIdWithButtonType(string id, ButtonType type)
        {
            string idWithType = id;
            switch (type)
            {
                case ButtonType.Button:
                    idWithType += "Button";
                    break;
                case ButtonType.Image:
                    idWithType += "ImageButton";
                    break;
                case ButtonType.Link:
                    idWithType += "LinkButton";
                    break;
            }
            return idWithType;
        }

        public string MakeChildName(string postfix)
        {
            return MakeNameFromId(MakeChildId(postfix));
        }

        static public void WriteBeginDiv(HtmlTextWriter writer, string className)
        {
            writer.WriteLine();
            writer.WriteBeginTag("div");
            if (className.Length > 0)
            {
                writer.WriteAttribute("class", className);
            }
            writer.Write(HtmlTextWriter.TagRightChar);
            writer.Indent++;
        }

        static public void WriteEndDiv(HtmlTextWriter writer)
        {
            writer.Indent--;
            writer.WriteLine();
            writer.WriteEndTag("div");
        }

        static public void WriteSpan(HtmlTextWriter writer, string className, string content)
        {
            if (content.Length > 0)
            {
                writer.WriteLine();
                writer.WriteBeginTag("span");
                if (className.Length > 0)
                {
                    writer.WriteAttribute("class", className);
                }
                writer.Write(HtmlTextWriter.TagRightChar);
                writer.Write(content);
                writer.WriteEndTag("span");
            }
        }

        static public void WriteImage(HtmlTextWriter writer, string url, string alt)
        {
            if (url.Length > 0)
            {
                writer.WriteLine();
                writer.WriteBeginTag("img");
                writer.WriteAttribute("src", url);
                writer.WriteAttribute("alt", alt);
                writer.Write(HtmlTextWriter.SelfClosingTagEnd);
            }
        }

        static public void WriteLink(HtmlTextWriter writer, string className, string url, string title, string content)
        {
            if ((url.Length > 0) && (content.Length > 0))
            {
                writer.WriteLine();
                writer.WriteBeginTag("a");
                if (className.Length > 0)
                {
                    writer.WriteAttribute("class", className);
                }
                writer.WriteAttribute("href", url);
                writer.WriteAttribute("title", title);
                writer.Write(HtmlTextWriter.TagRightChar);
                writer.Write(content);
                writer.WriteEndTag("a");
            }
        }

        //  Can't be static because it uses MakeChildId
        public void WriteLabel(HtmlTextWriter writer, string className, string text, string forId)
        {
            if (text.Length > 0)
            {
                writer.WriteLine();
                writer.WriteBeginTag("label");
                writer.WriteAttribute("for", MakeChildId(forId));
                if (className.Length > 0)
                {
                    writer.WriteAttribute("class", className);
                }
                writer.Write(HtmlTextWriter.TagRightChar);
                writer.Write(text);
                writer.WriteEndTag("label");
            }
        }

        //  Can't be static because it uses MakeChildId
        public void WriteTextBox(HtmlTextWriter writer, bool isPassword, string labelClassName, string labelText, string inputClassName, string id, string value)
        {
            WriteLabel(writer, labelClassName, labelText, id);

            writer.WriteLine();
            writer.WriteBeginTag("input");
            writer.WriteAttribute("type", isPassword ? "password" : "text");
            if (inputClassName.Length > 0)
            {
                writer.WriteAttribute("class", inputClassName);
            }
            writer.WriteAttribute("id", MakeChildId(id));
            writer.WriteAttribute("name", MakeChildName(id));
            writer.WriteAttribute("value", value);
            writer.Write(HtmlTextWriter.SelfClosingTagEnd);
        }

        //  Can't be static because it uses MakeChildId
        public void WriteReadOnlyTextBox(HtmlTextWriter writer,  string labelClassName, string labelText, string inputClassName, string value)
        {
            WriteLabel(writer, labelClassName, labelText, "");

            writer.WriteLine();
            writer.WriteBeginTag("input");
            writer.WriteAttribute("readonly", "true");
            if (inputClassName.Length > 0)
            {
                writer.WriteAttribute("class", inputClassName);
            }
            writer.WriteAttribute("value", value);
            writer.Write(HtmlTextWriter.SelfClosingTagEnd);
        }

        //  Can't be static because it uses MakeChildId
        public void WriteCheckBox(HtmlTextWriter writer, string labelClassName, string labelText, string inputClassName, string id, bool isChecked)
        {
            writer.WriteLine();
            writer.WriteBeginTag("input");
            writer.WriteAttribute("type", "checkbox");
            if (inputClassName.Length > 0)
            {
                writer.WriteAttribute("class", inputClassName);
            }
            writer.WriteAttribute("id", MakeChildId(id));
            writer.WriteAttribute("name", MakeChildName(id));
            if (isChecked)
            {
                writer.WriteAttribute("checked", "checked");
            }
            writer.Write(HtmlTextWriter.SelfClosingTagEnd);

            WriteLabel(writer, labelClassName, labelText, id);
        }

        //  Can't be static because it uses MakeChildId
        public void WriteSubmit(HtmlTextWriter writer, ButtonType buttonType, string className, string id, string imageUrl, string javascript, string text)
        {
            writer.WriteLine();

            string idWithType = id;

            switch (buttonType)
            {
                case ButtonType.Button:
                    writer.WriteBeginTag("input");
                    writer.WriteAttribute("type", "submit");
                    writer.WriteAttribute("value", text);
                    idWithType += "Button";
                    break;
                case ButtonType.Image:
                    writer.WriteBeginTag("input");
                    writer.WriteAttribute("type", "image");
                    writer.WriteAttribute("src", imageUrl);
                    idWithType += "ImageButton";
                    break;
                case ButtonType.Link:
                    writer.WriteBeginTag("a");
                    idWithType += "LinkButton";
                    break;
            }

            if (className.Length > 0)
            {
                writer.WriteAttribute("class", className);
            }
            writer.WriteAttribute("id", MakeChildId(idWithType));
            writer.WriteAttribute("name", MakeChildName(idWithType));

            if (javascript.Length > 0)
            {
                switch (buttonType)
                {
                    case ButtonType.Button:
                        writer.WriteAttribute("onclick", javascript);
                        break;
                    case ButtonType.Image:
                        writer.WriteAttribute("onclick", javascript);
                        break;
                    case ButtonType.Link:
                        writer.WriteAttribute("href", javascript);
                        break;
                }
            }

            if (buttonType == ButtonType.Link)
            {
                writer.Write(HtmlTextWriter.TagRightChar);
                writer.Write(text);
                writer.WriteEndTag("a");
            }
            else
            {
                writer.Write(HtmlTextWriter.SelfClosingTagEnd);
            }
        }

        static public void WriteRequiredFieldValidator(HtmlTextWriter writer, RequiredFieldValidator rfv, string className, string controlToValidate, string msg)
        {
            if (rfv != null)
            {
                rfv.CssClass = className;
                rfv.ControlToValidate = controlToValidate;
                rfv.ErrorMessage = msg;
                rfv.RenderControl(writer);
            }
        }

        static public void WriteRegularExpressionValidator(HtmlTextWriter writer, RegularExpressionValidator rev, string className, string controlToValidate, string msg, string expression)
        {
            if (rev != null)
            {
                rev.CssClass = className;
                rev.ControlToValidate = controlToValidate;
                rev.ErrorMessage = msg;
                rev.ValidationExpression = expression;
                rev.RenderControl(writer);
            }
        }

        static public void WriteCompareValidator(HtmlTextWriter writer, CompareValidator cv, string className, string controlToValidate, string msg, string controlToCompare)
        {
            if (cv != null)
            {
                cv.CssClass = className;
                cv.ControlToValidate = controlToValidate;
                cv.ErrorMessage = msg;
                cv.ControlToCompare = controlToCompare;
                cv.RenderControl(writer);
            }
        }
    }

    public class ControlRestorationInfo
    {
        private Control _ctrl = null;
        public Control Control
        {
            get { return _ctrl; }
        }

        private ControlCollection _coll = null;
        public ControlCollection Collection
        {
            get { return _coll; }
        }

        public bool IsValid
        {
            get { return (Control != null) && (Collection != null); }
        }

        public ControlRestorationInfo(Control ctrl, ControlCollection coll)
        {
            _ctrl = ctrl;
            _coll = coll;
        }

        public void Restore()
        {
            if (IsValid)
            {
                _coll.Add(_ctrl);
            }
        }
    }
}
