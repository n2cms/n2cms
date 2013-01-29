/*************************************************************************************************

Slideshow: Generic Model and MVC Adapter
Licensed to users of N2CMS under the terms of The MIT License (MIT)

Copyright (c) 2013 Benjamin Herila <mailto:ben@herila.net>

Permission is hereby granted, free of charge, to any person obtaining a copy of this software and 
associated documentation files (the "Software"), to deal in the Software without restriction, 
including without limitation the rights to use, copy, modify, merge, publish, distribute, 
sublicense, and/or sell copies of the Software, and to permit persons to whom the Software is 
furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in all copies or 
substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING 
BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND 
NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, 
DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, 
OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.

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
		HtmlParagraphs,
		UlLiImg,			  // <ul>{<li><img /></li>}*</ul>
		DivImg,
		Responsiveslidercss3, //http://csscience.com/responsiveslidercss3/
		CustomTemplate
	}

	[PartDefinition(Title = "Slideshow", IconUrl = "{IconsUrl}/photos.png")]
	[RestrictChildren(
		typeof(ISlideshowEntryProvider), 
		typeof(SlideshowDirectoryInclude), 
		typeof(SlideshowFileInclude))]
	[AvailableZone("Sources", "Sources")]
	public class Slideshow : ContentItem
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

		[EditableEnum("Slideshow Template", 100, typeof(SlideshowMode))]
		public SlideshowMode SlideshowTemplate
		{
			get { return GetDetail("SlideshowTemplate", SlideshowMode.HtmlParagraphs); }
			set { SetDetail("SlideshowTemplate", value, SlideshowMode.HtmlParagraphs); }
		} 

		[EditableText("Outer CSS Class", 400)]
		public string CssClass
		{
			get { return GetDetail("CssClass", ""); }
			set { SetDetail("CssClass", value, ""); }
		}
		
		[EditableText("Custom HTML Template", 500, ContainerName = "Advanced", Rows = 10, Columns = 50, TextMode = System.Web.UI.WebControls.TextBoxMode.MultiLine)]
		public string CustomTemplate
		{
			get { return GetDetail("CustomTemplate", ""); }
			set { SetDetail("CustomTemplate", value, ""); }
		} 


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

		[EditableCheckBox("Recurse", 200, CheckBoxText = "Include subdirectories")]
		public virtual bool Recurse
		{
			get { return GetDetail("Rec", false); }
			set { SetDetail("Rec", value, false); }
		}

		public IEnumerable<SlideshowImage> GetSlideshowImages()
		{
			return EnumerateImagesInDirectories(LocalPath);
		}

		public static IEnumerable<SlideshowImage> EnumerateImagesInDirectories(string relativeBasePath)
		{
			var baseDir = System.Web.Hosting.HostingEnvironment.MapPath(relativeBasePath);
			var toExplore = new List<string> { baseDir };
			Debug.Assert(baseDir != null, "baseDir != null");
			while (toExplore.Count > 0)
			{
				var files = System.IO.Directory.GetFiles(toExplore[0]);

				var filenames = new string[files.Length];
				for (var i = 0; i < filenames.Length; ++i)
					filenames[i] = System.IO.Path.GetFileName(files[i]);

				//var metadataIndex = -1;
				//for (var i = 0; i < filenames.Length; ++i)
				//	if (0 == String.Compare(filenames[i], "metadata.txt", StringComparison.OrdinalIgnoreCase))
				//	{
				//		metadataIndex = i;
				//		break;
				//	}

				//var descriptions = new Dictionary<string, SlideshowImage>();
				//if (metadataIndex >= 0)
				//{
				//	var mdfile = from line in System.IO.File.ReadAllLines(files[metadataIndex])
				//				 let lineParts = line.Split(new[] {':'}, 3)
				//				 select lineParts;

				//	foreach (var x in mdfile)
				//	{
				//		SlideshowImage si = new SlideshowImage(x[0]);
				//	}
				//		descriptions.Add(x[0]);

				//}

				for (var i = 0; i < files.Length; ++i)
				{
					var f = files[i];
					if (!(f.EndsWith("jpg") || f.EndsWith("gif") || f.EndsWith("png") || f.EndsWith("jpeg")))
						continue;
					yield return new SlideshowImage()
						             {
							             ImageHref = f.Substring(baseDir.Length),
							             Description = null,
							             Title = filenames[i]
						             };
				}

				toExplore.AddRange(System.IO.Directory.GetDirectories(toExplore[0]));
				toExplore.RemoveAt(0);
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

		[EditableCheckBox("Recurse", 200, CheckBoxText = "Include subdirectories")]
		public virtual bool Recurse
		{
			get { return GetDetail("Rec", false); }
			set { SetDetail("Rec", value, false); }
		}


		[EditableFreeTextArea("Description", 450, Rows = 8)]
		public string Description
		{
			get { return GetDetail("Description", ""); }
			set { SetDetail("Description", value, ""); }
		}

		public IEnumerable<SlideshowImage> GetSlideshowImages()
		{
			return (new List<SlideshowImage> { new SlideshowImage()
				                                   {
					                                   ImageHref = LocalPath,
													   Description = this.Description,
													   Title = this.Title
				                                   } }).AsReadOnly();
		}
	}


}
