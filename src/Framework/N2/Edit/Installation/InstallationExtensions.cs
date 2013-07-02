using N2.Persistence;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace N2.Edit.Installation
{
	public static class InstallationExtensions
	{
		public static string GetFileVersion(this Assembly assembly)
		{
			return assembly.GetCustomAttributes(typeof(AssemblyFileVersionAttribute), true)
				.OfType<AssemblyFileVersionAttribute>()
				.Select(afva => afva.Version)
				.FirstOrDefault()
				?? assembly.GetName().Version.ToString();
		}

		public static string[] GetInstalledFeatures(this ContentItem root)
		{
			var features = root.GetDetailCollection(InstallationManager.installationFeatures, false);
			if (features == null)
				return new string[0];

			return features.OfType<string>().ToArray();
		}

		public static void RecordInstalledFeature(this ContentItem root, string feature)
		{
			var features = root.GetDetailCollection(InstallationManager.installationFeatures, true);
			if (features.Contains(feature))
				return;
			features.Add(feature);
		}

		public static void UnrecordInstalledFeature(this ContentItem root, string feature)
		{
			var features = root.GetDetailCollection(InstallationManager.installationFeatures, true);
			if (!features.Contains(feature))
				return;

			features.Remove(feature);
		}
	}
}
