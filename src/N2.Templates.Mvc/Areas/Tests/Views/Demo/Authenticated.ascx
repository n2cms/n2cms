<%@ Import Namespace="N2.Templates.Mvc.Areas.Tests.Models" %>
<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<DemoViewModel>" %>
<div id="demo" class="uc">
	<h4>Demo panel</h4>
	<div style="display:none" class="box"><div class="inner">
		<p><strong>On your first visit?</strong> These are a few of my favorite things.</p>
		<ul style="margin-left:20px;">
			<li><a href="javascript:void(0);" onclick="$('.sc .open').click();">Open</a> the control panel <span style="border:solid 1px silver; background-color:#eee;">&raquo;</span></li>
			<li><a href="/n2">Try</a> the management UI (<img src="/N2/Resources/icons/application_side_expand.png" style="vertical-align:middle" />)</li>
			<li><a href="<%=Html.CurrentPage().Url %>?edit=drag">Organize parts</a> on this page (<img src="/N2/Resources/icons/layout_edit.png" style="vertical-align:middle" />)</li>
			<li><a href="http://n2cms.codeplex.com/releases" target="_top">Start</a> developing with N2 today!</li>
		</ul>
	</div></div>
	<div style="display:none" class="box"><div class="inner">
		<% using(Html.BeginForm("Logout", null)){ %>
			<p><strong>Done testing?</strong> What did you think?</p>
			<p>
				<label><input type="radio" name="vote" value="Good" />Pretty good</label><br />
				<label><input type="radio" name="vote" value="Bad" />Could be better</label><br />
				<textarea name="feedback" style="width:100%" onfocus="if(this.value == '<%= DemoViewModel.FeedbackInstruction %>')this.value = ''"><%= DemoViewModel.FeedbackInstruction %></textarea>
			</p>
			<input type="submit" value="Send (It's appreciated)" />
		<%} %>
	</div></div>
</div>
<script type="text/javascript">	//<![CDATA[
	$(document).ready(function() {
		$("#demo .box").fadeIn("slow");
	});
//]]></script>
<style>
	.bubble { display:none; }
</style>