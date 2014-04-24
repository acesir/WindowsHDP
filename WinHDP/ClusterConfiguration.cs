using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace WinHDP
{
    static class ClusterConfiguration
    {
        static ClusterConfiguration()
        { }

        public static IDictionary<string, string> Get()
        {
            return Read();
        }

        private static IDictionary<string, string> Read()
        {
            Dictionary<string, string> hostServices = new Dictionary<string, string>();
            try
            {
                StreamReader sr = new StreamReader(Configuration.ClusterProperties);
                List<string> hosts = new List<string>();

                hostServices.Add("HDP_LOG_DIR", "");
                hostServices.Add("HDP_DATA_DIR", "");
                hostServices.Add("NAMENODE_HOST", "");
                hostServices.Add("SECONDARY_NAMENODE_HOST", "");
                hostServices.Add("HIVE_SERVER_HOST", "");
                hostServices.Add("OOZIE_SERVER_HOST", "");
                hostServices.Add("WEBHCAT_HOST", "");
                hostServices.Add("KNOX_HOST", "");
                hostServices.Add("SLAVE_HOSTS", "");
                hostServices.Add("HOSTS", "");

                while (!sr.EndOfStream)
                {
                    string service = sr.ReadLine().ToString();

                    if (hostServices.Keys.Any(s => service.StartsWith(s)))
                    {
                        string key = service.Substring(0, service.IndexOf('='));
                        string values = service.Substring(service.LastIndexOf('=') + 1);

                        hostServices[key] = values;
                        if (key != "HDP_LOG_DIR" && key != "HDP_DATA_DIR")
                            hosts.AddRange(values.Split(','));
                    }
                }
                sr.Close();
                hostServices["HOSTS"] = String.Join(",", hosts.Distinct());
                var test = hostServices.Values.Distinct().ToList();
            }
            catch (Exception ex)
            {
                Global.Log.Error(ex.ToString());
            }
            return hostServices;
        }
    }
}
