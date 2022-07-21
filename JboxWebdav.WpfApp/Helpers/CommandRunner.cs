using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JboxWebdav.WpfApp.Helpers
{
    public class CommandRunner
    {
        public string ExecutablePath { get; }
        public string WorkingDirectory { get; }
        public Process Process { get; set; }

        public CommandRunner(string executablePath, string? workingDirectory = null)
        {
            ExecutablePath = executablePath ?? throw new ArgumentNullException(nameof(executablePath));
            WorkingDirectory = workingDirectory ?? Path.GetDirectoryName(executablePath);
        }

        public void Run(string arguments)
        {
            var info = new ProcessStartInfo(ExecutablePath, arguments)
            {
                CreateNoWindow = true,
                RedirectStandardOutput = true,
                UseShellExecute = false,
                WorkingDirectory = WorkingDirectory,
                RedirectStandardInput = true,
            };
            Process = new Process
            {
                StartInfo = info,
            };
            Process.Start();
        }

        public void WriteLine(string str)
        {
            Process.StandardInput.WriteLine(str);
        }

        public string GetOutput()
        {
            return Process.StandardOutput.ReadToEnd();
        }
    }
}
