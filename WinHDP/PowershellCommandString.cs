using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WinHDP
{
    public enum CommandName
    {
        Unknown = 0,
        PythonInstall = 1,
        VisualC_Install = 2,
        DotNetFrameworkInstal = 3,
        JavaInstall = 4,
        HDP_Install = 5,
        Powershell3 = 6,
        HDP_Uninstall = 7,
        RemoveJavaEnv = 8
    }

    public static class PowershellCommandString
    {
        static string _installFile = Path.Combine(Configuration.HDPDir, Configuration.HdpInstallFiles);
        static string _installLog = Path.Combine(Configuration.HDPDir, Configuration.HdpInstallLogs);

        public static string PythonInstall
        {
            get { 
                    return
                        @"msiexec /qn /norestart /log " + _installLog + @"\python.log /i "
                        + Configuration.GetRemoteConfiguration(CommandName.PythonInstall)
                        + @" INSTALLLOCATION=""C:\Python27"" | Wait-Process; $env:Path += "";C:\Python27""; [Environment]::SetEnvironmentVariable(""Path"", $env:Path, [System.EnvironmentVariableTarget]::Machine)";
                }
        }

        public static string VisualCInstall
        {
            get
            {
                return
                    Configuration.GetRemoteConfiguration(CommandName.VisualC_Install)
                    + @" /q /norestart /log "
                    + _installLog + @"\vcredist.log | Wait-Process";
            }
        }

        public static string JavaInstall
        {
            get
            {
                return
                    Configuration.GetRemoteConfiguration(CommandName.JavaInstall)
                    + @" /qn /norestart /log " + _installLog
                    + @"\java-jdk.log  INSTALLDIR=C:\java\jdk1.6.0_31 | Wait-Process; $env:JAVA_HOME += ""C:\java\jdk1.6.0_31""; [Environment]::SetEnvironmentVariable(""JAVA_HOME"", $env:JAVA_HOME, [System.EnvironmentVariableTarget]::Machine)";
            }
        }

        public static string HDPInstall
        {
            get
            {
                return
                    @"msiexec /i """
                    + Configuration.GetRemoteConfiguration(CommandName.HDP_Install)
                    + @""" /lv " + _installLog + @"""\HDPinstall.log"" HDP_LAYOUT="""
                    + Path.Combine(_installFile, Path.GetFileName(Configuration.ClusterProperties)) + @""" HDP_DIR="""
                    + Configuration.HDPDir + @"\hadoop"" DESTROY_DATA=""yes"" HDP_USER_PASSWORD=""" + Configuration.HadoopPassword
                    + (String.IsNullOrEmpty(ClusterConfiguration.Get()["KNOX_HOST"])
                        ? "KNOX=" + @"""NO"""
                        : @""" KNOX_MASTER_SECRET=""" + Configuration.KnoxMasterKey) + @""" "
                    + (String.IsNullOrEmpty(ClusterConfiguration.Get()["RANGER"])
                        ? "RANGER=" + @"""NO"""
                        : @"""RANGER=""" + Configuration.KnoxMasterKey)
                    + @""
                    + " | Wait-Process";
            }
        }

        public static string HDPUninstall
        {
            get
            {
                return
                    @"msiexec /x """ + Configuration.GetRemoteConfiguration(CommandName.HDP_Install)
                    + @""" DESTROY_DATA=""yes"" /qn | Wait-Process";
            }
        }

        public static string RemoveJavaEnVar
        {
            get
            {
                return
                    @"[Environment]::SetEnvironmentVariable(""JAVA_HOME"",$null, [System.EnvironmentVariableTarget]::Machine)";
            }
        }

        public static string RemovePythonEnVar
        {
            get
            {
                return
                    @"$path = [Environment]::GetEnvironmentVariable(""Path"",""Machine"");" +
                    @"[Environment]::SetEnvironmentVariable(""Path"",$a.Replace("";C:\Python27"",""""),[System.EnvironmentVariableTarget]::Machine)";
            }
        }

        public static string RequiredApplicationUninstall
        {
            get
            {
                return
                    @"$applications = Get-WmiObject -Query ""SELECT * FROM Win32_Product WHERE Name LIKE '%Visual C++%'" +
                    @"OR Name LIKE '%Python%' OR Name LIKE '%Java%'""; foreach ($application in $applications) {$application.Uninstall()}";
            }
        }

        public static string HDPPorts(bool enable)
        {
            string ports =
            @"netsh advfirewall firewall add rule name=HDP_NameNodeWebUI_http dir=in action=allow protocol=TCP localport=50070;
            netsh advfirewall firewall add rule name=HDP_NameNodeWebUI_https dir=in action=allow protocol=TCP localport=50470;
            netsh advfirewall firewall add rule name=HDP_NameNodeMetadataService dir=in action=allow protocol=TCP localport=8020-9000;
            netsh advfirewall firewall add rule name=HDP_DataNodeWebUI dir=in action=allow protocol=TCP localport=50075;
            netsh advfirewall firewall add rule name=HDP_DataNodeSecureHTTP dir=in action=allow protocol=TCP localport=50475;
            netsh advfirewall firewall add rule name=HDP_DataNodeDataTransfer dir=in action=allow protocol=TCP localport=50010;
            netsh advfirewall firewall add rule name=HDP_DataNodeMetadataOps dir=in action=allow protocol=TCP localport=50020;
            netsh advfirewall firewall add rule name=HDP_SecondaryNameNode dir=in action=allow protocol=TCP localport=50090;
            netsh advfirewall firewall add rule name=HDP_ResourceManagerWebUI dir=in action=allow protocol=TCP localport=8088;
            netsh advfirewall firewall add rule name=HDP_ResourceManager dir=in action=allow protocol=TCP localport=8032;
            netsh advfirewall firewall add rule name=HDP_NodeManagerWebUI dir=in action=allow protocol=TCP localport=50060;
            netsh advfirewall firewall add rule name=HDP_JobHistoryWebUI dir=in action=allow protocol=TCP localport=51111;
            netsh advfirewall firewall add rule name=HDP_HIveServer2 dir=in action=allow protocol=TCP localport=10001;
            netsh advfirewall firewall add rule name=HDP_HiveServer dir=in action=allow protocol=TCP localport=10000;
            netsh advfirewall firewall add rule name=HDP_HiveMetastore dir=in action=allow protocol=TCP localport=9083;
            netsh advfirewall firewall add rule name=HDP_WebHCatServer dir=in action=allow protocol=TCP localport=50111;
            netsh advfirewall firewall add rule name=HDP_HMaster dir=in action=allow protocol=TCP localport=60000;
            netsh advfirewall firewall add rule name=HDP_HMasterWebUI dir=in action=allow protocol=TCP localport=60010;
            netsh advfirewall firewall add rule name=HDP_RegionServer dir=in action=allow protocol=TCP localport=60020;
            netsh advfirewall firewall add rule name=HDP_RegionServer_http dir=in action=allow protocol=TCP localport=60030;
            netsh advfirewall firewall add rule name=HDP_Zookeper_PeerPort dir=in action=allow protocol=TCP localport=2888;
            netsh advfirewall firewall add rule name=HDP_Zookeper_LeaderPort dir=in action=allow protocol=TCP localport=3888;
            netsh advfirewall firewall add rule name=HDP_Zookeper_ClientPort dir=in action=allow protocol=TCP localport=2181;";

            return enable ? ports : ports.Replace("add", "delete").Replace("action=allow", "");
        }

        public static string DeleteDirectory(string directory)
        {
            return "if(Test-Path "+ directory +"){Remove-Item -Recurse -Force " + directory + "}";
        }
    }
}
