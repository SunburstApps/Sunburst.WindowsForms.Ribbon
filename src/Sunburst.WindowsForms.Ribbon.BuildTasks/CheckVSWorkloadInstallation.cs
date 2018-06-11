using System;
using System.Diagnostics;
using System.IO;
using System.Text;
using Microsoft.Build.Framework;
using Microsoft.Build.Utilities;

namespace Sunburst.WindowsForms.BuildTasks
{
    public sealed class CheckVSWorkloadInstallation : Task
    {
        [Required]
        public string[] RequiredWorkloads { get; set; }

        [Output]
        public bool Present { get; set; }

        private bool CheckWorkloads()
        {
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
            if (proc.ExitCode != 0) return false;

            string allOutput = proc.StandardOutput.ReadToEnd();
            string[] lines = allOutput.Split(new[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries);

            string installRoot = null;
            foreach (string line in lines)
            {
                if (line.StartsWith("installationPath:"))
                {
                    installRoot = line.Substring("installationPath:".Length).TrimStart();
                }
            }

            return installRoot != null;
        }

        public override bool Execute()
        {
            Present = CheckWorkloads();
            return true;
        }
    }
}
