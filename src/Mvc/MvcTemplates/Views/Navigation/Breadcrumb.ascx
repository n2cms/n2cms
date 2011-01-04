<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<IEnumerable<ContentItem>>" %>
<%@ Import Namespace="N2"%>

<div class="path">
	<%=String.Join(" / ", Model
		.Select(item => String.Format(@"<a href=""{0}""{1}>{2}</a>", item.Url, Model.Last() == item ? @" class=""current""" : "", item.Title))
		.ToArray()) %>
</div>