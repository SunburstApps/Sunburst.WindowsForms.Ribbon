using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Microsoft.Build.Framework;
using Microsoft.Build.Utilities;

namespace Sunburst.WindowsForms.BuildTasks
{
    public class RibbonCompileRC : VisualStudioToolTask
    {
        private string GetMSVCToolsVersion()
        {
            string filePath = Path.Combine(VisualStudioInstallRoot, @"VC\Auxiliary\Build\Microsoft.VCToolsVersion.default.txt");
            return File.ReadAllText(filePath).Trim();
        }

        [Required]
        public ITaskItem ResourceScript { get; set; }

        [Required]
        public ITaskItem OutputDirectory { get; set; }

        [Required]
        public string Architecture { get; set; }

        [Required]
        public string WindowsSDKBuildVersion { get; set; }

        public override string[] RequiredWorkloads => new[] { $"Microsoft.VisualCpp.Tools.HostX86.Target{Architecture.ToUpperInvariant()}" };

        public override string InstallRootRelativeToolPath => throw new NotImplementedException();

        protected override string GenerateFullPathToTool()
        {
            return $@"C:\Program Files (x86)\Windows Kits\10\bin\10.0.{WindowsSDKBuildVersion}.0\{Architecture}\rc.exe";
        }

        protected override string GetWorkingDirectory()
        {
            return OutputDirectory.GetMetadata("FullPath");
        }

        protected override string GenerateCommandLineCommands()
        {
            List<string> argv = new List<string>();
            argv.Add("/nologo");
            argv.Add("/Fo");
            argv.Add(Path.Combine(OutputDirectory.GetMetadata("FullPath"), Path.ChangeExtension(Path.GetFileName(ResourceScript.GetMetadata("FullPath")), "res")));

            Log.LogWarning(ResourceScript.GetMetadata("SourceFilePath"));
            string sourceFileDirectory = Path.GetDirectoryName(ResourceScript.GetMetadata("SourceFilePath"));
            if (sourceFileDirectory.EndsWith("\\")) sourceFileDirectory = sourceFileDirectory.Substring(0, sourceFileDirectory.Length - 1);

            string[] includePaths = new[]
            {
                sourceFileDirectory,
                @"C:\Program Files (x86)\Windows Kits\NETFXSDK\4.6.1\include\um",
                $@"C:\Program Files (x86)\Windows Kits\10\include\10.0.{WindowsSDKBuildVersion}.0\ucrt",
                $@"C:\Program Files (x86)\Windows Kits\10\include\10.0.{WindowsSDKBuildVersion}.0\shared",
                $@"C:\Program Files (x86)\Windows Kits\10\include\10.0.{WindowsSDKBuildVersion}.0\um",
                $@"C:\Program Files (x86)\Windows Kits\10\include\10.0.{WindowsSDKBuildVersion}.0\winrt",
                $@"{VisualStudioInstallRoot}\VC\Tools\MSVC\{GetMSVCToolsVersion()}\ATLMFC\include",
                $@"{VisualStudioInstallRoot}\VC\Tools\MSVC\{GetMSVCToolsVersion()}\include"
            };

            argv.AddRange(includePaths.Select(path => "/i" + path));
            argv.Add(ResourceScript.GetMetadata("FullPath"));

            return string.Join(" ", argv.Select(x => $"\"{x}\""));
        }
    }
}
