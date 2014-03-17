using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WinHDP
{
    public class AppInstall
    {
        private static AppInstall instance = null;
        
        public AppInstall() { }

        public static AppInstall Instantiate()
        {
            if (instance == null)
                instance = new AppInstall();
            
            return instance;
        }

        public void HadoopRequirements(PowershellCommand ps, string host, int threadId)
        {
            //Console.WriteLine(host + " " + (threadId - Configuration.Hostnames.Count()).ToString());
            Console.WriteLine("Starting installation on host " + host);
            Global.AppInstallCursorPosition.Add(host, Console.CursorTop - 1);
            System.Threading.Thread.Sleep(Global.Hostnames.Count() + 2000);
            
            Console.SetCursorPosition(0, Global.AppInstallCursorPosition[host]);
            Console.WriteLine(host + " - Installing Python and setting Environmental variables                      ");
            ps.ExecuteRemote(host, PowershellCommandString.PythonInstall);

            Console.SetCursorPosition(0, Global.AppInstallCursorPosition[host]);
            Console.WriteLine(host + " - Installing Visual C++ Redistributable                                      ");
            ps.ExecuteRemote(host, PowershellCommandString.VisualCInstall);

            Console.SetCursorPosition(0, Global.AppInstallCursorPosition[host]);
            Console.WriteLine(host + " - Installing Java and setting Environmental variables                        ");
            ps.ExecuteRemote(host, PowershellCommandString.JavaInstall);

            Console.SetCursorPosition(0, Global.AppInstallCursorPosition[host]);
            Console.WriteLine(host + " - Installing HDP. This may take a minute or two                              ");
            ps.ExecuteRemote(host, PowershellCommandString.HDPInstall);
            Console.SetCursorPosition(0, Global.AppInstallCursorPosition[host]);
            ConsoleText.Green(host + " - Installation completed!                                                    ");
        }
    }
}
