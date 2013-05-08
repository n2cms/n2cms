﻿/*************************************************************************************************

Slideshow: Generic Model and MVC Adapter
Licensed to users of N2CMS under the terms of the Boost Software License

Copyright (c) 2013 Benjamin Herila <mailto:ben@herila.net>

Boost Software License - Version 1.0 - August 17th, 2003

Permission is hereby granted, free of charge, to any person or organization obtaining a copy of the 
software and accompanying documentation covered by this license (the "Software") to use, reproduce,
display, distribute, execute, and transmit the Software, and to prepare derivative works of the
Software, and to permit third-parties to whom the Software is furnished to do so, all subject to 
the following:

The copyright notices in the Software and this entire statement, including the above license grant,
this restriction and the following disclaimer, must be included in all copies of the Software, in
whole or in part, and all derivative works of the Software, unless such copies or derivative works 
are solely in the form of machine-executable object code generated by a source language processor.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT 
NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE, TITLE AND NON-
INFRINGEMENT. IN NO EVENT SHALL THE COPYRIGHT HOLDERS OR ANYONE DISTRIBUTING THE SOFTWARE BE LIABLE
FOR ANY DAMAGES OR OTHER LIABILITY, WHETHER IN CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR
IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.

*************************************************************************************************/

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using N2.Integrity;
using N2.Web.Mvc;
using N2.Engine;
using N2.Details;

namespace N2.Web
{

	public enum SlideshowMode
	{
		Paragraphs,
		UlLiImg,
		DivImg,
		TripleDiv, //http://csscience.com/responsiveslidercss3/
		HtmlItemTemplate
	}

	[PartDefinition(Title = "Image Gallery", IconUrl = "{IconsUrl}/photos.png", TemplateUrl = "Slideshow")]
	[RestrictChildren(
		typeof(ISlideshowEntryProvider),
		typeof(SlideshowDirectoryInclude),
		typeof(SlideshowFileInclude))]
	[AvailableZone("Sources", "Sources")]
	[WithEditableTemplateSelection()]
	public class Slideshow : ContentItem, N2.Definitions.IPart
	{

		[EditableChildren("Image sources", "Sources", 100)]
		public virtual IList<ISlideshowEntryProvider> ImageContainers
		{
			get
			{
				try
				{
					var list = new List<ISlideshowEntryProvider>();
					var children = GetChildren();
					if (children != null)
						list.AddRange(children.Select(child => child as ISlideshowEntryProvider));
					return list;
				}
				catch
				{
					return new List<ISlideshowEntryProvider>();
				}
			}
		}

#if SlideshowUsesAdapter
		[EditableText("Outer CSS Class", 400)]
#endif
		public string CssClass
		{
			get { return GetDetail("CssClass", ""); }
			set { SetDetail("CssClass", value, ""); }
		}
		
#if SlideshowUsesAdapter
		[EditableEnum("Slideshow Template", 100, typeof(SlideshowMode))]
		public SlideshowMode SlideshowTemplate
		{
			get { return GetDetail("SlideshowTemplate", SlideshowMode.Paragraphs); }
			set { SetDetail("SlideshowTemplate", value, SlideshowMode.Paragraphs); }
		} 

		[EditableText("Custom HTML Template", 500, ContainerName = "Advanced", Rows = 10, Columns = 50, TextMode = System.Web.UI.WebControls.TextBoxMode.MultiLine, HelpText = "Put {0} where you want the Image URL and {1} where you want the Alt text.")]
		public string CustomTemplate
		{
			get { return GetDetail("CustomTemplate", ""); }
			set { SetDetail("CustomTemplate", value, ""); }
		}

		[EditableText("HTML Before", 510, ContainerName = "Advanced", Rows = 3, Columns = 50, TextMode = System.Web.UI.WebControls.TextBoxMode.MultiLine)]
		public string HTMLBefore
		{
			get { return GetDetail("HTMLBefore", ""); }
			set { SetDetail("HTMLBefore", value, ""); }
		}

		[EditableText("HTML After", 520, ContainerName = "Advanced", Rows = 3, Columns = 50, TextMode = System.Web.UI.WebControls.TextBoxMode.MultiLine)]
		public string HTMLAfter
		{
			get { return GetDetail("HTMLAfter", ""); }
			set { SetDetail("HTMLAfter", value, ""); }
		} 
#endif


		public List<SlideshowImage> GetSlideshowImages()
		{
			var slideshowImages = new List<SlideshowImage>();
			foreach (var imageSrc in ImageContainers)
				slideshowImages.AddRange(imageSrc.GetSlideshowImages());
			return slideshowImages;
		}

	}

	[Serializable]
	public class SlideshowImage
	{
		public string ImageHref { get; set; }
		public string Description { get; set; }
		public string Title { get; set; }
		public string LinkUrl { get; set; }
	}

	public interface ISlideshowEntryProvider
	{
		IEnumerable<SlideshowImage> GetSlideshowImages();
	}

	[PartDefinition("Images in Directory", IconUrl = "{IconsUrl}/link.png")]
	[RestrictParents(typeof(Slideshow))]
	public class SlideshowDirectoryInclude : ContentItem, ISlideshowEntryProvider
	{
		[EditableFolderSelection("Path", 100)]
		public virtual string LocalPath
		{
			get { return (string)GetDetail("LocalPath"); }
			set { SetDetail("LocalPath", value); }
		}

		[EditableText("File Include Filter", 125)]
		public string FileFilter
		{
			get { return GetDetail("FileFilter", "*.*"); }
			set { SetDetail("FileFilter", value, "*.*"); }
		}


		[EditableCheckBox("Recurse", 200, CheckBoxText = "Include subdirectories")]
		public virtual bool Recurse
		{
			get { return GetDetail("Rec", false); }
			set { SetDetail("Rec", value, false); }
		}

		public IEnumerable<SlideshowImage> GetSlideshowImages()
		{
			return EnumerateImagesInDirectories(LocalPath, FileFilter);
		}

		public static IEnumerable<SlideshowImage> EnumerateImagesInDirectories(string relativeBasePath, string filter)
		{
			// sanitize input just in case
			relativeBasePath = relativeBasePath.Replace('\\', '/'); 
			filter = filter ?? "*";
			if (relativeBasePath[0] == '/')
				relativeBasePath = '~' + relativeBasePath;

			foreach (var f in N2.Utility.ListFiles(System.Web.Hosting.HostingEnvironment.MapPath(relativeBasePath), filter))
			{
				if (!(f.EndsWith("jpg") || f.EndsWith("gif") || f.EndsWith("png") || f.EndsWith("jpeg")))
					continue;

				yield return new SlideshowImage() {
					ImageHref = N2.Utility.RemapVirtualPath(f),
					Description = null,
					Title = System.IO.Path.GetFileName(f)
				};
			}
		}
	}

	[PartDefinition("Individual Image", IconUrl = "{IconsUrl}/link.png")]
	[RestrictParents(typeof(Slideshow))]
	[WithEditableTitle()]
	public class SlideshowFileInclude : ContentItem, ISlideshowEntryProvider
	{
		[EditableFileUpload("Path", 100)]
		public virtual string LocalPath
		{
			get { return (string)GetDetail("LocalPath"); }
			set { SetDetail("LocalPath", value); }
		}


		[EditableFreeTextArea("Description", 450, Rows = 8)]
		public string Description
		{
			get { return GetDetail("Description", ""); }
			set { SetDetail("Description", value, ""); }
		}

		[EditableUrl("Link Destination", 200)]
		public virtual string LinkHref
		{
			get { return (string)GetDetail("LinkHref"); }
			set { SetDetail("LinkHref", value); }
		}

		public IEnumerable<SlideshowImage> GetSlideshowImages()
		{
			return (new List<SlideshowImage> { new SlideshowImage()
				                                   {
					                                   ImageHref = LocalPath,
													   Description = this.Description,
													   Title = this.Title,
													   LinkUrl = this.LinkHref
				                                   } }).AsReadOnly();
		}
	}


}
