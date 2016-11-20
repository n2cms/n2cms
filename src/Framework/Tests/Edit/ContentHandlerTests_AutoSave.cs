using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Shouldly;
using System.Web.Script.Serialization;
using N2.Edit.Versioning;

namespace N2.Tests.Edit
{
	[TestFixture]
	public class ContentHandlerTests_AutoSave : ContentHandlerTests
	{
		//startPage = new ContentHandlerTestsPage { Title = "Start page" };
		//page = new ContentHandlerTestsPage { Title = "Page in question" };

		[Test]
		public void ExistingPage_CreatesDraft()
		{
			context.httpContext.request.CreatePost("/N2/Api/Content.ashx/autosave?n2item=" + page.ID, "application/json", "{ Title: 'New title' }", pathInfo: "/autosave");

			handler.ProcessRequest(context.HttpContext);

			var output = context.httpContext.response.output.ToString();
			var serializer = new JavaScriptSerializer();
			var response = serializer.Deserialize<Dictionary<string, object>>(output);
			response["ID"].ShouldBe(page.ID);
			((int)response["VersionIndex"]).ShouldBeGreaterThan(page.VersionIndex);

			var versions = versionManager.GetVersionsOf(page);
			versions.Count().ShouldBe(2);
			versions.Single(v => v.State != ContentState.Draft).Title.ShouldBe("Page in question");
			versions.Single(v => v.State == ContentState.Draft).Title.ShouldBe("New title");
		}

		[Test]
		public void NewPartOnExistingPage_CreatesNewDraftOfPage()
		{
			context.httpContext.request.CreatePost("/N2/Api/Content.ashx/autosave?discriminator=ContentHandlerTestsPart&n2item=" + page.ID, "application/json", "{ Title: 'New child' }", pathInfo: "/autosave");

			handler.ProcessRequest(context.HttpContext);

			var output = context.httpContext.response.output.ToString();
			var serializer = new JavaScriptSerializer();
			var response = serializer.Deserialize<Dictionary<string, object>>(output);
			response["ID"].ShouldBe(0);

			var versions = versionManager.GetVersionsOf(page);
			versions.Count().ShouldBe(2);
			versions.Single(v => v.State == ContentState.Draft).Content.Children.Single().Title.ShouldBe("New child");
		}

		[Test]
		public void NewPartOnExistingDraftOfPage_CreatesNewDraftOfPage()
		{
			var draft = versionManager.AddVersion(page, asPreviousVersion: false);
			context.httpContext.request.CreatePost("/N2/Api/Content.ashx/autosave?discriminator=ContentHandlerTestsPart&n2item=" + page.ID + "&n2versionIndex=" + draft.VersionIndex, "application/json", "{ Title: 'New child' }", pathInfo: "/autosave");

			handler.ProcessRequest(context.HttpContext);

			var output = context.httpContext.response.output.ToString();
			var serializer = new JavaScriptSerializer();
			var response = serializer.Deserialize<Dictionary<string, object>>(output);
			response["ID"].ShouldBe(0);

			var versions = versionManager.GetVersionsOf(page);
			versions.Count().ShouldBe(2);
			versions.Single(v => v.State == ContentState.Draft).Content.Children.Single().Title.ShouldBe("New child");
		}

		[Test]
		public void ExistingPageDraft_UpdatesDraft()
		{
			var draft = versionManager.AddVersion(page, asPreviousVersion: false);
			context.httpContext.request.CreatePost("/N2/Api/Content.ashx/autosave?n2item=" + page.ID + "&n2versionIndex=" + draft.VersionIndex, "application/json", "{ Title: 'New title' }", pathInfo: "/autosave");

			handler.ProcessRequest(context.HttpContext);

			var response = new JavaScriptSerializer().Deserialize<Dictionary<string, object>>(context.httpContext.response.output.ToString());
			response["ID"].ShouldBe(page.ID);
			((int)response["VersionIndex"]).ShouldBeGreaterThan(page.VersionIndex);

			var versions = versionManager.GetVersionsOf(page);
			versions.Count().ShouldBe(2);
			versions.Single(v => v.State != ContentState.Draft).Title.ShouldBe("Page in question");
			versions.Single(v => v.State == ContentState.Draft).Title.ShouldBe("New title");
		}

		[Test]
		public void ExistingPartOnPageDraft_UpdatesDraft()
		{
			var draft = versionManager.AddVersion(page, asPreviousVersion: false);
			var partDraft = engine.Definitions.CreateInstance<ContentHandlerTestsPart>(draft);
			partDraft.Title = "New title";
			partDraft.AddTo(draft);
			versionManager.UpdateVersion(draft);

			context.httpContext.request.CreatePost("/N2/Api/Content.ashx/autosave?n2item=" + page.ID + "&n2versionIndex=" + draft.VersionIndex + "&n2versionKey=" + partDraft.GetVersionKey(), "application/json", "{ Title: 'Updated child' }", pathInfo: "/autosave");
			handler.ProcessRequest(context.HttpContext);

			var response = new JavaScriptSerializer().Deserialize<Dictionary<string, object>>(context.httpContext.response.output.ToString());
			((int)response["VersionIndex"]).ShouldBe(draft.VersionIndex);

			var versions = versionManager.GetVersionsOf(page);
			versions.Count().ShouldBe(2);
			versions.Single(v => v.State == ContentState.Draft).Content.Children.Single().Title.ShouldBe("Updated child");
		}

		[Test]
		public void ExistingPartOnPage_CreatesNewDraftOfPage()
		{
			var part = engine.Definitions.CreateInstance<ContentHandlerTestsPart>(page);
			part.Title = "New title";
			part.AddTo(page);
			engine.Persister.Save(part);

			context.httpContext.request.CreatePost("/N2/Api/Content.ashx/autosave?n2item=" + part.ID, "application/json", "{ Title: 'Updated child' }", pathInfo: "/autosave");
			handler.ProcessRequest(context.HttpContext);

			var response = new JavaScriptSerializer().Deserialize<Dictionary<string, object>>(context.httpContext.response.output.ToString());

			var versions = versionManager.GetVersionsOf(page);
			versions.Count().ShouldBe(2);
			versions.Single(v => v.State == ContentState.Draft).Content.Children.Single().Title.ShouldBe("Updated child");
		}

		[Test]
		public void EditingExistingPageDraft_UpdatesDraft()
		{
			var draft = versionManager.AddVersion(page, asPreviousVersion: false);
			context.httpContext.request.CreatePost("/N2/Api/Content.ashx/autosave?n2item=" + page.ID + "&n2versionIndex=" + draft.VersionIndex, "application/json", "{ Title: 'New title' }", pathInfo: "/autosave");

			handler.ProcessRequest(context.HttpContext);

			var response = new JavaScriptSerializer().Deserialize<Dictionary<string, object>>(context.httpContext.response.output.ToString());
			response["ID"].ShouldBe(page.ID);
			((int)response["VersionIndex"]).ShouldBeGreaterThan(page.VersionIndex);

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

			var response = new JavaScriptSerializer().Deserialize<Dictionary<string, object>>(context.httpContext.response.output.ToString());
			var id = (int)response["ID"];

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

			var response = new JavaScriptSerializer().Deserialize<Dictionary<string, object>>(context.httpContext.response.output.ToString());
			var id = (int)response["ID"];

			context.httpContext.request.CreatePost("/N2/Api/Content.ashx/autosave?n2item=" + startPage.ID + "&discriminator=ContentHandlerTestsPage", "application/json", "{ ID: " + id + ", Title: 'Even newer title' }", pathInfo: "/autosave");
			context.httpContext.items.Clear();
			handler.ProcessRequest(context.HttpContext);

			var draft = this.engine.Persister.Get(id);
			draft.Title.ShouldBe("Even newer title");
		}
	}
}
