using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using Microsoft.Build.Framework;
using Microsoft.Build.Utilities;

namespace Sunburst.WindowsForms.BuildTasks
{
    public abstract class VisualStudioToolTask : ToolTask
    {
        public abstract string[] RequiredWorkloads { get; }
        public abstract string InstallRootRelativeToolPath { get; }

        private string _CachedVisualStudioInstallRoot = null;
        public string VisualStudioInstallRoot
        {
            get
            {
                if (_CachedVisualStudioInstallRoot != null) return _CachedVisualStudioInstallRoot;

                string vswhereExe = @"C:\Program Files (x86)\Microsoft Visual Studio\Installer\vswhere.exe";
                if (!File.Exists(vswhereExe)) throw new FileNotFoundException("vswhere.exe not found - cannot continue", vswhereExe);

                ProcessStartInfo startInfo = new ProcessStartInfo(vswhereExe);
                startInfo.RedirectStandardOutput = true;
                startInfo.RedirectStandardError = true;
                startInfo.UseShellExecute = false;
                startInfo.Arguments = "-latest";

                if (RequiredWorkloads != null && RequiredWorkloads.Length > 0)
                {
                    startInfo.Arguments += " -requires " + string.Join(" ", RequiredWorkloads);
                }

                Process proc = Process.Start(startInfo);
                proc.WaitForExit();
                if (proc.ExitCode != 0) throw new InvalidOperationException($"vswhere.exe exited with code {proc.ExitCode}");

                string allOutput = proc.StandardOutput.ReadToEnd();
                string[] lines = allOutput.Split(new[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries);

                foreach (string line in lines)
                {
                    if (line.StartsWith("installationPath:"))
                    {
                        _CachedVisualStudioInstallRoot = line.Substring("installationPath:".Length).TrimStart();
                    }
                }

                if (_CachedVisualStudioInstallRoot == null) throw new InvalidOperationException("Could not determine Visual Studio install root");
                return _CachedVisualStudioInstallRoot;
            }
        }

        protected override string GenerateFullPathToTool()
        {
            return Path.Combine(VisualStudioInstallRoot, InstallRootRelativeToolPath);
        }

        protected override string ToolName => Path.GetFileName(GenerateFullPathToTool());
    }
}
