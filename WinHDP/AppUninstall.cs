using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WinHDP
{
    public static class AppUninstall
    {
        public static void Hadoop(PowershellCommand ps, string host)
        {
            try
            {
                ps.ExecuteRemote(host, PowershellCommandString.HDPPorts(false));
                ps.ExecuteRemote(host, PowershellCommandString.HDPUninstall);
                ps.ExecuteRemote(host, PowershellCommandString.RemoveJavaEnVar);
                ps.ExecuteRemote(host, PowershellCommandString.RemovePythonEnVar);
                ps.ExecuteRemote(host, PowershellCommandString.DeleteDirectory(Configuration.HDPDir));
                ps.ExecuteRemote(host, PowershellCommandString.DeleteDirectory(Global.ClusterProperties["HDP_LOG_DIR"]));
                ps.ExecuteRemote(host, PowershellCommandString.DeleteDirectory(Global.ClusterProperties["HDP_DATA_DIR"]));
                ps.ExecuteRemote(host, PowershellCommandString.RequiredApplicationUninstall);
            }
            catch (Exception ex)
            {
                Global.Log.Error(ex.ToString());
                throw;
            }
        }
    }
}
