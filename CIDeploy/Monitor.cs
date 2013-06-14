using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.ServiceProcess;
using System.Text;

namespace CIDeploy
{
    public partial class Monitor : ServiceBase
    {
        private FileSystemWatcher watcher = new FileSystemWatcher();
        public Monitor()
        {
            InitializeComponent();
        }

        public void InteractiveStart()
        {
            OnStart(null);
        }

        protected override void OnStart(string[] args)
        {
            watcher.Path = Properties.Settings.Default.NugetFolder;
            watcher.IncludeSubdirectories = false;
            watcher.Created += Watcher_Created;
            watcher.EnableRaisingEvents = true;
        }

        protected override void OnStop()
        {
            watcher.EnableRaisingEvents = false;
            watcher.Dispose();
        }

        #region File Creation
        void Watcher_Created(object sender, FileSystemEventArgs e)
        {
            var projectName = GetProjectName(e.Name);

            CreateCMDProcess(projectName);
        }

        private void CreateCMDProcess(string projectName)
        {
            var process = new Process();

            var arguments = !string.IsNullOrEmpty(Properties.Settings.Default.DefaultDeployEnviroment)
                            ? "/C octo create-release --project=\"" + projectName + "\" --deployto=" +
                              Properties.Settings.Default.DefaultDeployEnviroment + " --server=" +
                              Properties.Settings.Default.OctopusServerUrl + "/api --apiKey=" +
                              Properties.Settings.Default.OctopusAPIKey
                            : "/C octo create-release --project=\"" + projectName + "\" --server=" +
                              Properties.Settings.Default.OctopusServerUrl + "/api --apiKey=" +
                              Properties.Settings.Default.OctopusAPIKey;

            var startInfo = new ProcessStartInfo
            {
                WindowStyle = ProcessWindowStyle.Maximized,
                FileName = "cmd.exe",
                WorkingDirectory = "C:/Octopus/",
                Arguments = arguments
            };

            process.StartInfo = startInfo;
            process.Start();
        }

        private string GetProjectName(string name)
        {
            var ch = '.';
            var n = 2;

            var result = name
              .Select((c, i) => new { c, i })
              .Where(x => x.c == ch)
              .Skip(n - 1)
              .FirstOrDefault();

            var projectName = name.Remove(result.i, (name.Length - result.i));

            return projectName.Replace(".", " ");

        }
        #endregion

    }
}
