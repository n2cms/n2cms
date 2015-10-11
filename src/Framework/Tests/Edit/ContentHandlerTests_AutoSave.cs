using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Shouldly;
using System.Web.Script.Serialization;

namespace N2.Tests.Edit
{
	[TestFixture]
	public class ContentHandlerTests_AutoSave : ContentHandlerTests
	{
		[Test]
		public void ExistingItem_CreatesDraft()
		{
			context.httpContext.request.CreatePost("/N2/Api/Content.ashx/autosave?n2item=" + page.ID, "application/json", "{ Title: 'New title' }", pathInfo: "/autosave");

			handler.ProcessRequest(context.HttpContext);

			var response = new JavaScriptSerializer().Deserialize<Dictionary<string, string>>(context.httpContext.response.output.ToString());
			response["ID"].ShouldBe(page.ID.ToString());
			int.Parse(response["VersionIndex"]).ShouldBeGreaterThan(page.VersionIndex);

			var versions = versionManager.GetVersionsOf(page);
			versions.Count().ShouldBe(2);
			versions.Single(v => v.State != ContentState.Draft).Title.ShouldBe("Page in question");
			versions.Single(v => v.State == ContentState.Draft).Title.ShouldBe("New title");
		}

		[Test]
		public void ExistingDraft_UpdatesDraft()
		{
			var draft = versionManager.AddVersion(page, asPreviousVersion: false);
			context.httpContext.request.CreatePost("/N2/Api/Content.ashx/autosave?n2item=" + page.ID + "&n2versionIndex=" + draft.VersionIndex, "application/json", "{ Title: 'New title' }", pathInfo: "/autosave");

			handler.ProcessRequest(context.HttpContext);

			var response = new JavaScriptSerializer().Deserialize<Dictionary<string, string>>(context.httpContext.response.output.ToString());
			response["ID"].ShouldBe(page.ID.ToString());
			int.Parse(response["VersionIndex"]).ShouldBeGreaterThan(page.VersionIndex);

			var versions = versionManager.GetVersionsOf(page);
			versions.Count().ShouldBe(2);
			versions.Single(v => v.State != ContentState.Draft).Title.ShouldBe("Page in question");
			versions.Single(v => v.State == ContentState.Draft).Title.ShouldBe("New title");
		}

		[Test]
		public void EditingExistingDraft_UpdatesDraft()
		{
			var draft = versionManager.AddVersion(page, asPreviousVersion: false);
			context.httpContext.request.CreatePost("/N2/Api/Content.ashx/autosave?n2item=" + page.ID + "&n2versionIndex=" + draft.VersionIndex, "application/json", "{ Title: 'New title' }", pathInfo: "/autosave");

			handler.ProcessRequest(context.HttpContext);

			var response = new JavaScriptSerializer().Deserialize<Dictionary<string, string>>(context.httpContext.response.output.ToString());
			response["ID"].ShouldBe(page.ID.ToString());
			int.Parse(response["VersionIndex"]).ShouldBeGreaterThan(page.VersionIndex);

			var versions = versionManager.GetVersionsOf(page);
			versions.Count().ShouldBe(2);
			versions.Single(v => v.State != ContentState.Draft).Title.ShouldBe("Page in question");
			versions.Single(v => v.State == ContentState.Draft).Title.ShouldBe("New title");
		}

		[Test]
		public void CreatingPage_CreatesDraft()
		{
			context.httpContext.request.CreatePost("/N2/Api/Content.ashx/autosave?n2item=" + startPage.ID + "&discriminator=ContentHandlerTestsPage", "application/json", "{ Title: 'New title' }", pathInfo: "/autosave");

			handler.ProcessRequest(context.HttpContext);

			var response = new JavaScriptSerializer().Deserialize<Dictionary<string, string>>(context.httpContext.response.output.ToString());
			var id = int.Parse(response["ID"]);
			
			id.ShouldBeGreaterThan(page.ID);
			var draft = this.engine.Persister.Get(id);
			draft.Title.ShouldBe("New title");
			draft.State.ShouldBe(ContentState.Draft);
			draft.Parent.ShouldBe(startPage);
		}

		[Test]
		public void CreatingPageWithPreviouslySaveDraft_UpdatesExistingDraft()
		{
			context.httpContext.request.CreatePost("/N2/Api/Content.ashx/autosave?n2item=" + startPage.ID + "&discriminator=ContentHandlerTestsPage", "application/json", "{ Title: 'New title' }", pathInfo: "/autosave");

			handler.ProcessRequest(context.HttpContext);

			var response = new JavaScriptSerializer().Deserialize<Dictionary<string, string>>(context.httpContext.response.output.ToString());
			var id = int.Parse(response["ID"]);

			context.httpContext.request.CreatePost("/N2/Api/Content.ashx/autosave?n2item=" + startPage.ID + "&discriminator=ContentHandlerTestsPage", "application/json", "{ ID: " + id + ", Title: 'Even newer title' }", pathInfo: "/autosave");
			context.httpContext.items.Clear();
			handler.ProcessRequest(context.HttpContext);

			var draft = this.engine.Persister.Get(id);
			draft.Title.ShouldBe("Even newer title");
		}
	}
}
