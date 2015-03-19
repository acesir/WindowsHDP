using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Management;
using System.IO;
using System.Configuration;
using System.Collections.Specialized;
using System.Threading;
using System.Reflection;
using log4net;

namespace WinHDP
{
    class WinHDP
    {
        static void Main(string[] args)
        {
            log4net.GlobalContext.Properties["LogFileName"] = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) 
                + @"\WinHDP.log.txt";
            log4net.Config.XmlConfigurator.Configure();
            MainOptions();
        }

        private static void MainOptions()
        {
            try
            {
                Console.Clear();
                Console.WriteLine();
                Console.WriteLine("1. HDP Install.");
                Console.WriteLine("2. HDP Uninstall.");
                Console.WriteLine("3. Add Node.");
                Console.WriteLine();
                Console.WriteLine("Pease select one of the above [1] [2] [3]");
                ConsoleKeyInfo setup = Console.ReadKey(true);
                Console.WriteLine();
                Global.ClusterProperties = ClusterConfiguration.Get();
                Global.Hostnames = ClusterConfiguration.Get()["HOSTS"].Split(',');
                if (setup.KeyChar.ToString() == "1")
                {
                    Console.WriteLine("1. Firewall is turned off on all host nodes.");
                    Console.WriteLine("2. You are running this with an account that has Administrative rights on all host nodes.");
                    Console.WriteLine("3. All hosts in the cluster have .NET Framework 4.0 and Powershell 3.0 installed.");
                    Console.WriteLine();
                    Console.WriteLine("Are all of the above true? [Y] [N]");
                    ConsoleKeyInfo cki = Console.ReadKey(true);
                    if (cki.KeyChar.ToString().ToLower() != "y")
                    {
                        Console.WriteLine();
                        Console.WriteLine("Installation aborted! Press any key to exit.");
                        Console.ReadKey(true);
                        return;
                    }
                    else
                    {
                        Global.Log.Info("HDP Uninstall starting...");
                        HDPInstall();
                    }
                }
                else if (setup.KeyChar.ToString() == "2")
                {
                    HostPreparation.SetTrustedHosts(new PowershellCommand(Global.UserName, Global.Password), true);
                    UninstallHadoop(new PowershellCommand(Global.UserName, Global.Password));
                    ConsoleText.Green("Done");
                }
                else if (setup.KeyChar.ToString() == "3")
                {
                    Console.WriteLine("1. To add nodes enter them in the format like so $serviceName=nodename1,nodename2|$serviceName=nodename2...");
                    Console.WriteLine("2. For $serviceName use the services names from the clusterproperties.txt file...");
                    Console.WriteLine("3. Example: $SLAVE_HOSTS=nodename1,nodename2|$HBASE_REGIONSERVERS=nodename2");
                    Console.WriteLine();
                    Console.Write("Enter the nodes to add below and hit enter:");
                    Console.WriteLine();
                    AddNodes.Install(Console.ReadLine().Split('|'));
                }

                Console.WriteLine();
                Console.ReadKey();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
        }

        public static void HDPInstall()
        {
            PowershellCommand ps = new PowershellCommand(Global.UserName, Global.Password);
            try
            {
                Global.Log.Info("HDP Install starting...");
                HostPreparation.SetTrustedHosts(ps, true);
                HostPreparation.EnablePSRemoting();
                HostPreparation.SetTrustedHosts(ps);
                HostPreparation.TestHosts();
                HostPreparation.ConfigurePorts(ps, true);
                CopyFiles();
                HostPreparation.DisableIPv6(ps);
                Console.WriteLine("");
                Console.WriteLine("This pause is added if any manual configurations need to be performed on the cluster nodes " +
                    "prior to installation of HDP. Once finished hit any key to continue with the installation..");
                Console.ReadKey();
                Console.WriteLine("");
                ApplicationInstallation(ps);
                HostPreparation.EnableFirewall(ps);
                Console.WriteLine("");
                Console.WriteLine("This pause is added if any manual jars need to be added to the hadoop libraries. An example is adding SQL Server jdbc for SQL Server metastores..");
                Console.ReadKey();
                Console.WriteLine("");
                HostPreparation.StartServices(ps);
                HostPreparation.RunSmokeTests(ps);
            }
            catch (Exception ex)
            {
                Global.Log.Error("Installation Failed! " + ex.ToString());
                Console.ReadKey();
                return;
            }
        }

        private static void ApplicationInstallation(PowershellCommand ps)
        {
            Console.CursorVisible = false;
            Console.WriteLine();
            ConsoleText.Yellow("Required application installation...");
            ManualResetEvent[] doneEvents = new ManualResetEvent[Global.Hostnames.Count()];
            for (int i = 0; i < Global.Hostnames.Count(); i++)
            {
                doneEvents[i] = new ManualResetEvent(false);
                AppThreadPool app = new AppThreadPool(doneEvents[i], ps, Global.Hostnames[i]);
                ThreadPool.QueueUserWorkItem(app.Install, i);
            }
            WaitHandle.WaitAll(doneEvents);
            Console.WriteLine();
            Console.CursorVisible = true;
        }

        private static void UninstallHadoop(PowershellCommand ps)
        {
            ManualResetEvent[] doneEvents = new ManualResetEvent[Global.Hostnames.Count()];
            ConsoleText.Yellow("Uninstall started...");
            for (int i = 0; i < Global.Hostnames.Count(); i++)
            {
                doneEvents[i] = new ManualResetEvent(false);
                AppThreadPool app = new AppThreadPool(doneEvents[i], ps, Global.Hostnames[i]);
                ThreadPool.QueueUserWorkItem(app.Uninstall, i);
            }
            WaitHandle.WaitAll(doneEvents);
            Console.CursorVisible = true;
        }

        private static void CopyFiles()
        {
            ManualResetEvent[] doneEvents = new ManualResetEvent[Global.Hostnames.Count()];
            ConsoleText.Yellow("Copying required installation files...");
            for (int i = 0; i < Global.Hostnames.Count(); i++)
            {
                doneEvents[i] = new ManualResetEvent(false);
                AppThreadPool app = new AppThreadPool(doneEvents[i], Global.Hostnames[i]);
                ThreadPool.QueueUserWorkItem(app.CopyInstallationFiles, i);
            }
            WaitHandle.WaitAll(doneEvents);
            ConsoleText.Green("Done");
            Console.CursorVisible = true;
        }
    }
}
