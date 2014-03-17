using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net.NetworkInformation;
using System.Reflection;
using System.Security.Principal;
using System.Text;
using System.Threading.Tasks;

namespace WinHDP
{
    public static class HostPreparation
    {
        private static readonly string[] _hostnames;

        static HostPreparation()
        {
            _hostnames = Global.Hostnames;
        }

        public static void TestHosts()
        {
            Ping ping = new Ping();
            int failedHostCount = 0;
            ConsoleText.Yellow("Checking accesibility on all Hosts...");
            foreach (string host in _hostnames)
            {
                Console.WriteLine(host);
                try
                {
                    PingReply pingReply = ping.Send(host, 10000);

                    if (pingReply.Status == IPStatus.Success)
                    { }
                    else
                    { 
                        throw new Exception("Connection to host " + host + " failed! Exiting...");
                    }
                }
                catch (PingException ex)
                {
                    Global.Log.Error(ex.ToString());
                    failedHostCount++;
                }

                try
                {
                    string directory = @"\\" + host + @"\C$\TestDirectory";
                    if (Directory.Exists(directory))
                        Directory.Delete(directory);

                    Directory.CreateDirectory(@"\\" + host + @"\C$\TestDirectory");
                }
                catch (IOException ex)
                {
                    Global.Log.Error(ex.ToString());
                    throw;
                }
            }
            ConsoleText.Green("Done...");
            if (failedHostCount > 0)
            {
                throw new Exception("Some hosts were not reachable, please correct this and try again.");
            }
        }

        public static void EnablePSRemoting()
        {
            try
            {
                ConsoleText.Yellow("Enabling Powershell Remoting...");
                RunRemoteCommand("-h powershell.exe \"enable-psremoting -force\"");
                ConsoleText.Green("Done");
            }
            catch (Exception ex)
            {
                Global.Log.Error(ex.ToString());
                throw;
            }
        }

        public static void Server2008PreReqs()
        {
            ConsoleText.Yellow("Installing .NET Framework...");
            RunRemoteCommand(@"-h cmd.exe start /wait \\winnode2\c$\WinHDP\Files\dotNetFx40_Full_setup.exe /norestart /quiet");
            ConsoleText.Green("Done");

            //ConsoleText.Yellow("Installing Powershell 3.0 if equired...");
            //RunRemoteCommand("-h cmd.exe start /wait " + Configuration.GetRemoteConfiguration(Application.Powershell3)
            //    + @" /norestart /quiet");
            //ConsoleText.Green("Done");
        }

        public static void EnableFirewall(PowershellCommand ps)
        {
            if (Configuration.EnableFirewall)
            {
                ConsoleText.Yellow("Enabling all Firewall Profiles...");
                try
                {
                    foreach (string host in _hostnames)
                    {
                        Console.WriteLine(host);
                        StringBuilder sb = ps.ExecuteRemote(host, "netsh advfirewall set allprofiles state on");
                    }
                }
                catch (Exception ex)
                {
                    throw;
                }
                ConsoleText.Green("Done");
            }
        }

        private static void RunRemoteCommand(string command)
        {
            Process cmd = new Process();
            string output = null;
            string error = null;

            try
            {
                foreach (string host in _hostnames)
                {
                    Console.WriteLine(host);
                    cmd.StartInfo.FileName = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) + @"\psexec.exe";
                    cmd.StartInfo.Arguments = @"\\" + host + " " +  command;
                    cmd.StartInfo.RedirectStandardOutput = true;
                    cmd.StartInfo.RedirectStandardError = true;
                    cmd.StartInfo.CreateNoWindow = true;
                    cmd.StartInfo.UseShellExecute = false;
                    cmd.Start();

                    output = cmd.StandardError.ReadToEnd();
                    error = cmd.StandardOutput.ReadToEnd();
                    cmd.WaitForExit();
                }
            }
            catch (Exception ex)
            {
                Global.Log.Error(ex.ToString() + " " + output + " " + error );
                throw;
            }
        }

        public static void SetTrustedHosts(PowershellCommand ps)
        {
            ConsoleText.Yellow("Setting Trusted Hosts on all nodes...");
            try
            {
                foreach (string host in _hostnames)
                {
                    Console.WriteLine(host);
                    string hosts = String.Join(",", Global.Hostnames.ToArray());
                    StringBuilder s = ps.ExecuteRemote(host, @"Set-item wsman:localhost\client\trustedhosts -value """ 
                        + hosts +
                        @""" -force;" +
                        @"Get-item wsman:localhost\client\trustedhosts");
                }
                ConsoleText.Green("Done");
            }
            catch (Exception ex)
            {
                Global.Log.Error(ex.ToString());
                throw;
            }
        }

        public static void SetTrustedHosts(PowershellCommand ps, bool isLocal)
        {
            try
            {
                string hosts = String.Join(",", Global.Hostnames.ToArray());
                StringBuilder s = ps.ExecuteRemote(Environment.MachineName, @"Set-item wsman:localhost\client\trustedhosts -value """ + hosts +
                    @""" -force;" +
                    @"Get-item wsman:localhost\client\trustedhosts");
            }
            catch (Exception ex)
            {
                Global.Log.Error(ex.ToString());
                throw;
            }
        }

        public static void ConfigurePorts(PowershellCommand ps, bool enable)
        {
            ConsoleText.Yellow("Configuring HDP ports...");
            try
            {
                foreach (string host in Global.Hostnames)
                {
                    Console.WriteLine(host);
                    string hosts = String.Join(",", Global.Hostnames.ToArray());
                    StringBuilder s = ps.ExecuteRemote(host, PowershellCommandString.HDPPorts(true));
                }
            }
            catch (Exception ex)
            {
                Global.Log.Error(ex.ToString());
                throw;
            }
            ConsoleText.Green("Done");
        }

        public static void DisableIPv6(PowershellCommand ps)
        {
            try
            {
                ConsoleText.Yellow("Disabling IPv6...");
                foreach (string host in _hostnames)
                {
                    Console.WriteLine(host);
                    StringBuilder s = ps.ExecuteRemote(host, @"New-ItemProperty -Path HKLM:\SYSTEM\CurrentControlSet\services\TCPIP6\Parameters -Name DisabledComponents -PropertyType DWord -Value 0xffffffff");
                }
                if (Configuration.RestartForIPV6)
                {
                    Console.WriteLine("Restarting all Hosts for IPv4 resolution to take effect...");
                    ps.ExecuteRemoteAsync("Restart-Computer -ComputerName "
                        + String.Join(",", Global.Hostnames.ToArray()) + " -Force -Wait -For WinRM -Timeout 180");
                }
                ConsoleText.Green("Done");
            }
            catch (Exception ex)
            {
                Global.Log.Error(ex.ToString());
                throw;
            }
        }

        public static void StartServices(PowershellCommand ps)
        {
            if (Configuration.StartServices)
            {
                try
                {
                    foreach (string host in _hostnames)
                    {
                        ConsoleText.Yellow("Starting services on " + host);
                        ps.ExecuteRemoteAsync("invoke-command -computername " + host
                            + @" -scriptblock {C:\HDP\hadoop\start_local_hdp_services}");
                    }
                }
                catch (Exception ex)
                {
                    Global.Log.Error(ex.ToString());
                    throw;
                }
            }
        }

        public static void RunSmokeTests(PowershellCommand ps)
        {
            if (Configuration.RunSmokeTests)
            {
                try
                {
                    ConsoleText.Yellow("Running Smoke Tests...");
                    ps.ExecuteRemoteAsync("invoke-command -computername " + Global.ClusterProperties["NAMENODE_HOST"]
                            + @" -scriptblock {" + Configuration.HDPDir + @"\hadoop\Run-SmokeTests.cmd}");
                    ConsoleText.Green("HDP Installation Complete. Press any key to exit...");
                    Console.ReadKey();
                }
                catch (Exception ex)
                {
                    Global.Log.Error(ex.ToString());
                    throw;
                }
            }
        }
    }
}
