using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace WinHDP
{
    public static class CopyFiles
    {
        public static void CopyInstallationFiles(string host)
        {
            try
            {
                string hostDirectory = @"\\" + host + @"\" + Configuration.HDPDir.Replace(@":\", @"$\");
                string hostFileDirectory = Path.Combine(hostDirectory, Configuration.HdpInstallFiles);

                if (!Directory.Exists(hostDirectory))
                {
                    try
                    {
                        Directory.CreateDirectory(hostDirectory);
                        Directory.CreateDirectory(Path.Combine(hostDirectory, "WinHDP"));
                        Directory.CreateDirectory(hostFileDirectory);
                        Directory.CreateDirectory(hostDirectory + @"\" + Configuration.HdpInstallLogs);
                    }
                    catch (Exception ex)
                    {
                        Global.Log.Error(ex.ToString());
                        throw;
                    }
                }

                File.Copy(Configuration.ClusterProperties, Path.Combine(hostFileDirectory, Path.GetFileName(Configuration.ClusterProperties)), true);
                File.Copy(Configuration.DotNetFramework, Path.Combine(hostFileDirectory, Path.GetFileName(Configuration.DotNetFramework)), true);
                File.Copy(Configuration.Python, Path.Combine(hostFileDirectory, Path.GetFileName(Configuration.Python)), true);
                File.Copy(Configuration.Java, Path.Combine(hostFileDirectory, Path.GetFileName(Configuration.Java)), true);
                File.Copy(Configuration.VisualC, Path.Combine(hostFileDirectory, Path.GetFileName(Configuration.VisualC)), true);
                File.Copy(Configuration.HDP, Path.Combine(hostFileDirectory, Path.GetFileName(Configuration.HDP)), true);
                File.Copy(Configuration.Powershell3, Path.Combine(hostFileDirectory, Path.GetFileName(Configuration.Powershell3)), true);
            }
            catch (Exception ex)
            {
                Global.Log.Error(ex.ToString());
                throw;
            }
        }
    }
}
