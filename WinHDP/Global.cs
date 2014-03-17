using log4net;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WinHDP
{
    public class Global
    {
        public static string UserName;
        public static string Password;
        public static Dictionary<string, int> AppInstallCursorPosition = new Dictionary<string, int>();
        public static string[] Hostnames;
        public static readonly ILog Log = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        public static IDictionary<string, string> ClusterProperties = new Dictionary<string, string>();
    }
}