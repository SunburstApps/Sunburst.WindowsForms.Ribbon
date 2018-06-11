using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Microsoft.Build.Framework;
using Microsoft.Build.Utilities;

namespace Sunburst.WindowsForms.BuildTasks
{
    public sealed class LinkRibbonResource : VisualStudioToolTask
    {
        private string GetMSVCToolsVersion()
        {
            string filePath = Path.Combine(VisualStudioInstallRoot, @"VC\Auxiliary\Build\Microsoft.VCToolsVersion.default.txt");
            return File.ReadAllText(filePath).Trim();
        }

        [Required]
        public ITaskItem[] Objects { get; set; }

        [Required]
        public ITaskItem OutputFile { get; set; }

        [Required]
        public string Architecture { get; set; }

        [Required]
        public string WindowsSDKBuildVersion { get; set; }

        public override string[] RequiredWorkloads => new[] { $"Microsoft.VisualCpp.Tools.HostX86.Target{Architecture.ToUpperInvariant()}" };

        public override string InstallRootRelativeToolPath => $@"VC\Tools\MSVC\{GetMSVCToolsVersion()}\bin\Host{Architecture}\{Architecture}\link.exe";

        protected override string GenerateCommandLineCommands()
        {
            List<string> argv = new List<string>();

            argv.Add("/nologo");
            argv.Add("/dll");
            argv.Add("/noentry");
            argv.Add("/out:" + OutputFile.GetMetadata("FullPath"));
            argv.Add("/machine:" + Architecture);
            argv.AddRange(Objects.Select(obj => obj.GetMetadata("FullPath")));

            return string.Join(" ", argv.Select(x => $"\"{x}\""));
        }

        public override bool Execute()
        {
            string[] pathAdditions = new[]
            {
                $@"C:\Program Files (x86)\Windows Kits\10\bin\10.0.{WindowsSDKBuildVersion}.0\{Architecture}",
                Path.GetDirectoryName(GenerateFullPathToTool())
            };

            EnvironmentVariables = new[]
            {
                "PATH=" + string.Join(";", pathAdditions) + ";" + Environment.GetEnvironmentVariable("PATH")
            };

            string outputDirectory = Path.GetDirectoryName(OutputFile.GetMetadata("FullPath"));
            if (!Directory.Exists(outputDirectory))
                Directory.CreateDirectory(outputDirectory);

            return base.Execute();
        }
    }
}
