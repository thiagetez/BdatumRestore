using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Windows;
using EQATEC.Analytics.Monitor;

namespace BdatumRestore
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        private IAnalyticsMonitor monitor;
        protected override void OnStartup(StartupEventArgs e)
        {
            // your existing startup logic
            monitor = AnalyticsMonitorFactory.CreateMonitor("38d3934e3fb1434b810369a4cfab0ceb");
            monitor.Start();
        }

        protected override void OnExit(ExitEventArgs e)
        {
            // stopping the monitor when the application exits
            if (monitor != null)
                monitor.Stop();
        }
    }
}
