using System.IO;
using System.Text.RegularExpressions;
using NUnit.Framework;

namespace N2.Tests
{
    [TestFixture]
    public class MSBuildTests
    {
        string pattern = "<ProjectReference.*?<Name>(?<Name>[^<]*).*?</ProjectReference>";
        string replacement = @"<Reference Include=""${Name}""><SpecificVersion>False</SpecificVersion><HintPath>bin\${Name}.dll</HintPath></Reference>";

        #region Xmls

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
        string pattern2 = "<Reference Include=\"(?<Name>[^,\"]*)(,[^\"]*)?\">(?<Contents>.*?)</Reference>";

        #region 2a
        string projectXml2 = @"<Project ToolsVersion=""3.5"" DefaultTargets=""Build"" xmlns=""http://schemas.microsoft.com/developer/msbuild/2003"">
  <ItemGroup>
    <Reference Include=""Castle.Core, Version=1.0.3.0, Culture=neutral, PublicKeyToken=407dd0808d44fbdc, processorArchitecture=MSIL"">
      <SpecificVersion>False</SpecificVersion>
      <HintPath>..\..\lib\Castle.Core.dll</HintPath>
    </Reference>
    <Reference Include=""System"" />
    <Reference Include=""System.Core"">
      <RequiredTargetFramework>3.5</RequiredTargetFramework>
    </Reference>
  </ItemGroup>
</Project>
";
        string expectedXml2 = @"<Project ToolsVersion=""3.5"" DefaultTargets=""Build"" xmlns=""http://schemas.microsoft.com/developer/msbuild/2003"">
  <ItemGroup>
    <Reference Include=""Castle.Core""><SpecificVersion>False</SpecificVersion><HintPath>bin\Castle.Core.dll</HintPath></Reference>
    <Reference Include=""System"" />
    <Reference Include=""System.Core"">
      <RequiredTargetFramework>3.5</RequiredTargetFramework>
    </Reference>
  </ItemGroup>
</Project>
";
        #endregion

        string projectXml2b = @"<Project ToolsVersion=""3.5"" DefaultTargets=""Build"" xmlns=""http://schemas.microsoft.com/developer/msbuild/2003"">
  <ItemGroup>
    <Reference Include=""NHibernate, Version=2.1.2.4000, Culture=neutral, PublicKeyToken=aa95f207798dfdb4, processorArchitecture=MSIL"">
      <SpecificVersion>False</SpecificVersion>
      <HintPath>..\packages\NHibernate.3.3.2.4000\lib\Net35\NHibernate.dll</HintPath>
    </Reference>
    <Reference Include=""System"" />
    <Reference Include=""System.ComponentModel.DataAnnotations"">
      <RequiredTargetFramework>3.5</RequiredTargetFramework>
    </Reference>
    <Reference Include=""System.Data"" />
    <Reference Include=""System.Core"">
      <RequiredTargetFramework>3.5</RequiredTargetFramework>
    </Reference>
    <Reference Include=""System.Web.Abstractions, Version=3.5.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35, processorArchitecture=MSIL"">
      <SpecificVersion>False</SpecificVersion>
      <HintPath>..\..\lib\System.Web.Abstractions.dll</HintPath>
      <RequiredTargetFramework>3.5</RequiredTargetFramework>
    </Reference>
  </ItemGroup>
</Project>
";
        string expectedXml2b = @"<Project ToolsVersion=""3.5"" DefaultTargets=""Build"" xmlns=""http://schemas.microsoft.com/developer/msbuild/2003"">
  <ItemGroup>
    <Reference Include=""NHibernate""><SpecificVersion>False</SpecificVersion><HintPath>bin\NHibernate.dll</HintPath></Reference>
    <Reference Include=""System"" />
    <Reference Include=""System.ComponentModel.DataAnnotations"">
      <RequiredTargetFramework>3.5</RequiredTargetFramework>
    </Reference>
    <Reference Include=""System.Data"" />
    <Reference Include=""System.Core"">
      <RequiredTargetFramework>3.5</RequiredTargetFramework>
    </Reference>
    <Reference Include=""System.Web.Abstractions""><SpecificVersion>False</SpecificVersion><HintPath>bin\System.Web.Abstractions.dll</HintPath></Reference>
  </ItemGroup>
</Project>
";

        public string Rehint(Match m)
        {
            if (!m.Groups["Contents"].Success || !m.Groups["Contents"].Value.Contains("<HintPath"))
                return m.Value;

            return @"<Reference Include=""${Name}""><SpecificVersion>False</SpecificVersion><HintPath>bin\${Name}.dll</HintPath></Reference>".Replace("${Name}", m.Groups["Name"].Value);
        }
        #endregion

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
        public void CanRelocate_LibraryReferences()
        {
            string result = Regex.Replace(projectXml2, pattern2, Rehint, RegexOptions.Singleline);

            Assert.That(result, Is.EqualTo(expectedXml2));
        }

        [Test]
        public void CanRelocate_LibraryReferencesB()
        {
            string result = Regex.Replace(projectXml2b, pattern2, Rehint, RegexOptions.Singleline);

            Assert.That(result, Is.EqualTo(expectedXml2b));
        }

        [Test]
        public void CanOperate_OnProject()
        {
            string input = File.ReadAllText(@"..\..\N2.Tests.csproj");

            string result = Regex.Replace(input, pattern, replacement, RegexOptions.Singleline);

            Assert.That(input.Contains("ProjectReference"), Is.True);
            Assert.That(result.Contains("ProjectReference"), Is.False);
        }
    }
}
