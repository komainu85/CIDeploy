using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceProcess;
using System.Text;

namespace CIDeploy
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        static void Main()
        {
            ServiceBase[] ServicesToRun;
            ServicesToRun = new ServiceBase[] 
            { 
                new Monitor() 
            };

            if (!Environment.UserInteractive)
            {
                ServiceBase.Run(ServicesToRun);
            }
            else
            {
                var monitor = new Monitor();
                monitor.InteractiveStart();
            }
        }
    }
}
