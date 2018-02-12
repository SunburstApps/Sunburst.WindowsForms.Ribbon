using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Microsoft.Build.Framework;
using Microsoft.Build.Utilities;

namespace Sunburst.WindowsForms.BuildTasks
{
    public sealed class RibbonUICC : ToolTask
    {
        [Required]
        public ITaskItem RibbonDefinition { get; set; }

        [Required]
        public ITaskItem OutputDirectory { get; set; }

        [Required]
        public string WindowsSDKBuildVersion { get; set; }

        protected override string ToolName => "uicc.exe";

        protected override string GenerateFullPathToTool()
        {
            return $@"C:\Program Files (x86)\Windows Kits\10\bin\10.0.{WindowsSDKBuildVersion}.0\x86\uicc.exe";
        }

        protected override string GenerateCommandLineCommands()
        {
            List<string> argv = new List<string>();

            argv.Add(RibbonDefinition.GetMetadata("FullPath"));
            argv.Add(Path.Combine(OutputDirectory.GetMetadata("FullPath"), RibbonDefinition.GetMetadata("FileName") + ".bin"));
            argv.Add("/res:" + Path.Combine(OutputDirectory.GetMetadata("FullPath"), RibbonDefinition.GetMetadata("FileName") + ".rc"));

            return string.Join(" ", argv.Select(x => $"\"{x}\""));
        }

        protected override string GetWorkingDirectory()
        {
            return OutputDirectory.GetMetadata("FullPath");
        }

        public override bool Execute()
        {
            if (!Directory.Exists(OutputDirectory.GetMetadata("FullPath")))
                Directory.CreateDirectory(OutputDirectory.GetMetadata("FullPath"));

            return base.Execute();
        }
    }
}
