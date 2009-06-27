using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using NUnit.Framework;
using System.Diagnostics;
using System.IO;

namespace N2.Tests
{
	[TestFixture]
	public class MSBuildTests
	{
		string pattern = "<ProjectReference.*?<Name>(?<Name>[^<]*).*?</ProjectReference>";
		string replacement = @"<Reference Include=""${Name}""><SpecificVersion>False</SpecificVersion><HintPath>bin\${Name}.dll</HintPath></Reference>";
		
		string projectXml = @"<Project ToolsVersion=""3.5"" DefaultTargets=""Build"" xmlns=""http://schemas.microsoft.com/developer/msbuild/2003"">
  <ItemGroup>
    <ProjectReference Include=""..\N2.Extensions\N2.Extensions.csproj"">
      <Project>{E1A4B329-2EA0-445E-B22F-08DBAD0DF497}</Project>
      <Name>N2.Extensions</Name>
    </ProjectReference>
  </ItemGroup>
  <ItemGroup>
    <Reference Include=""Castle.Core, Version=1.0.3.0, Culture=neutral, PublicKeyToken=407dd0808d44fbdc, processorArchitecture=MSIL"">
      <SpecificVersion>False</SpecificVersion>
      <HintPath>$(LibPath)\Castle.Core.dll</HintPath>
    </Reference>
  </ItemGroup>
</Project>
";
		string expectedXml = @"<Project ToolsVersion=""3.5"" DefaultTargets=""Build"" xmlns=""http://schemas.microsoft.com/developer/msbuild/2003"">
  <ItemGroup>
    <Reference Include=""N2.Extensions""><SpecificVersion>False</SpecificVersion><HintPath>bin\N2.Extensions.dll</HintPath></Reference>
  </ItemGroup>
  <ItemGroup>
    <Reference Include=""Castle.Core, Version=1.0.3.0, Culture=neutral, PublicKeyToken=407dd0808d44fbdc, processorArchitecture=MSIL"">
      <SpecificVersion>False</SpecificVersion>
      <HintPath>$(LibPath)\Castle.Core.dll</HintPath>
    </Reference>
  </ItemGroup>
</Project>
";

		[Test]
		public void CanMatch_ProjectReferences()
		{
			var match = Regex.Match(projectXml, pattern, RegexOptions.Singleline);

			Assert.That(match.Success, Is.True);
			Assert.That(match.Groups["Name"].Value, Is.EqualTo("N2.Extensions"));
			Assert.That(match.Groups["Name"].Captures[0].Value, Is.EqualTo("N2.Extensions"));
		}

		[Test]
		public void CanReplace_ProjectReferences()
		{
			string result = Regex.Replace(projectXml, pattern, replacement, RegexOptions.Singleline);

			Assert.That(result, Is.EqualTo(expectedXml));
		}

		[Test]
		public void CanOperate_OnProect()
		{
			string input = File.ReadAllText(@"..\N2.Tests.csproj");

			string result = Regex.Replace(input, pattern, replacement, RegexOptions.Singleline);

			Assert.That(input.Contains("ProjectReference"), Is.True);
			Assert.That(result.Contains("ProjectReference"), Is.False);
		}
	}
}
