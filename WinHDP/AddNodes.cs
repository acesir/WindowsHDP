using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace WinHDP
{
    public static class AddNodes
    {
        public static void Install(string[] addNode)
        {
            try
            {
                List<string> newHosts = new List<string>();
                foreach (string s in addNode)
                {
                    newHosts.AddRange(s.Substring(s.IndexOf('=') + 1).Split(','));
                }

                Global.Hostnames = newHosts.Distinct().ToArray();

                string[] clusterProperties = File.ReadLines(Configuration.ClusterProperties).ToArray();
                for (int i = 0; i <= clusterProperties.Length - 1; i++)
                {
                    if (!String.IsNullOrEmpty(clusterProperties[i]))
                    {
                        foreach (string sx in addNode)
                        {
                            if (clusterProperties[i].StartsWith(sx.Substring(1, sx.IndexOf('=')))
                                && !clusterProperties[i].Contains(sx.Substring(sx.IndexOf('=') + 1)))
                            {
                                clusterProperties[i] = clusterProperties[i] + "," + sx.Substring(sx.IndexOf('=') + 1);
                            }
                        }
                    }
                }

                File.WriteAllText(Configuration.ClusterProperties, String.Join(System.Environment.NewLine, clusterProperties));
                WinHDP.HDPInstall();
            }
            catch (Exception ex)
            {
                Global.Log.Error(ex.ToString());
                throw;
            }
        }
    }
}
