using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WinHDP
{
    public static class Configuration
    {
        static readonly string _python;
        static readonly string _visualC;
        static readonly string _dotNetFramework;
        static readonly string _java;
        static readonly string _HDP;
        static readonly string _HDPDir;
        static readonly string _clusterProperties;
        static readonly string _hadoopPassword;
        static readonly bool _enableFirewall;
        static readonly string _hdpInstallFiles;
        static readonly string _hdpInstallLogs;
        static readonly string _powershell3;
        static readonly bool _restartForIPV6;
        static readonly bool _startServices;
        static readonly bool _runSmokeTests;

        static Configuration()
        {
            _python = ConfigurationManager.AppSettings["Python"];
            _visualC = ConfigurationManager.AppSettings["VisualC"];
            _dotNetFramework = ConfigurationManager.AppSettings["DotNetFramework"];
            _java = ConfigurationManager.AppSettings["Java"];
            _HDP = ConfigurationManager.AppSettings["HDP"];
            _HDPDir = ConfigurationManager.AppSettings["HDPDir"];
            _clusterProperties = ConfigurationManager.AppSettings["ClusterProperties"];
            _hadoopPassword = ConfigurationManager.AppSettings["HadoopPassword"];
            _enableFirewall = Convert.ToBoolean(ConfigurationManager.AppSettings["EnableFirewall"]);
            _powershell3 = ConfigurationManager.AppSettings["Powershell3"];
            _restartForIPV6 = Convert.ToBoolean(ConfigurationManager.AppSettings["RestartForIPV6"]);
            _startServices = Convert.ToBoolean(ConfigurationManager.AppSettings["StartServices"]);
            _runSmokeTests = Convert.ToBoolean(ConfigurationManager.AppSettings["RunSmokeTests"]);
            _hdpInstallFiles = @"WinHDP\Files";
            _hdpInstallLogs = @"WinHDP\Logs";
        }

        public static string Python
        {
            get { return _python; }
            //set { _python = value; }
        }

        public static string VisualC
        {
            get { return _visualC; }
            //set { _visualC = value; }
        }

        public static string DotNetFramework
        {
            get { return _dotNetFramework; }
            //set { _dotNetFramework = value; }
        }

        public static string Java
        {
            get { return _java; }
            //set { _java = value; }
        }

        public static string HDP
        {
            get { return _HDP; }
            //set { _HDP = value; }
        }

        public static string HDPDir
        {
            get { return _HDPDir; }
            //set { _HDP = value; }
        }

        public static string ClusterProperties
        {
            get { return _clusterProperties; }
            //set { _clusterProperties = value; }
        }

        public static string HadoopPassword
        {
            get { return _hadoopPassword; }
        }

        public static bool EnableFirewall
        {
            get { return _enableFirewall; }
        }

        public static bool RestartForIPV6
        {
            get { return _restartForIPV6; }
        }

        public static bool StartServices
        {
            get { return _startServices; }
        }

        public static bool RunSmokeTests
        {
            get { return _runSmokeTests; }
        }

        public static string Powershell3
        {
            get { return _powershell3; }
            //set { _clusterProperties = value; }
        }

        public static string HdpInstallFiles
        {
            get { return _hdpInstallFiles; }
        }

        public static string HdpInstallLogs
        {
            get { return _hdpInstallLogs; }
        }

        public static string GetRemoteConfiguration(CommandName app)
        {            
            switch (app)
            {
                case CommandName.PythonInstall:
                    return Path.Combine(HDPDir, HdpInstallFiles, Path.GetFileName(Python));
                case CommandName.VisualC_Install:
                    return Path.Combine(HDPDir, HdpInstallFiles, Path.GetFileName(VisualC));
                case CommandName.DotNetFrameworkInstal:
                    return Path.Combine(HDPDir, HdpInstallFiles, Path.GetFileName(DotNetFramework));
                case CommandName.JavaInstall:
                    return Path.Combine(HDPDir, HdpInstallFiles, Path.GetFileName(Java));
                case CommandName.HDP_Install:
                    return Path.Combine(HDPDir, HdpInstallFiles, Path.GetFileName(HDP));
                case CommandName.Powershell3:
                    return Path.Combine(HDPDir, HdpInstallFiles, Path.GetFileName(Powershell3));
                default:
                    return "Unknown application installation!!";
 
            }
        }
    }
}
