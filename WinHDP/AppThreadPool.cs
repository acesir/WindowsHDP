using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace WinHDP
{
    public class AppThreadPool
    {
        private PowershellCommand _ps;
        private ManualResetEvent _doneEvent;
        private string _host;

        public AppThreadPool(ManualResetEvent doneEvent, PowershellCommand ps, string host)
        {
            _ps = ps;
            _doneEvent = doneEvent;
            _host = host;
        }

        public AppThreadPool(ManualResetEvent doneEvent, string host)
        {
            _doneEvent = doneEvent;
            _host = host;
        }

        public void Install(Object threadContext)
        {
            Global.Log.Info(_host + " install started with thread: " + threadContext.ToString());
            AppInstall.Instantiate().HadoopRequirements(_ps, _host, (int)threadContext);
            _doneEvent.Set();
            Global.Log.Info(_host + " install ended with thread: " + threadContext.ToString());
        }

        public void Uninstall(Object threadContext)
        {
            Global.Log.Info(_host + " uninstall started with thread: " + threadContext.ToString());
            AppUninstall.Hadoop(_ps, _host);
            _doneEvent.Set();
            Console.WriteLine(_host + " completed!");
            Global.Log.Info(_host + " uninstall ended with thread: " + threadContext.ToString());
        }

        public void CopyInstallationFiles(Object threadContext)
        {
            Global.Log.Info(_host + " file copy started with thread: " + threadContext.ToString());
            CopyFiles.CopyInstallationFiles(_host);
            _doneEvent.Set();
            Console.WriteLine(_host + " completed");
            Global.Log.Info(_host + " uninstall ended with thread: " + threadContext.ToString());
        }
    }
}
