using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Management.Automation.Runspaces;
using System.Collections.ObjectModel;
using System.Management.Automation;
using System.Diagnostics;
using System.Security;
using System.IO;
using System.Threading;

namespace WinHDP
{
    public class PowershellCommand
    {
        string _username;
        string _password;

        public PowershellCommand(string username, string password)
        {
            _username = username;
            _password = password;
        }

        public StringBuilder ExecuteRemote(string host, string command)
        {
            try
            {
                Runspace runspace = RunspaceFactory.CreateRunspace();
                runspace.Open();
                Pipeline pipeline = runspace.CreatePipeline();
                pipeline.Commands.AddScript("invoke-command -computername " + host + " -scriptblock {" + command + "}");
                Global.Log.Info("Running remote command: " + pipeline.Commands[0].CommandText);
                pipeline.Commands.Add("Out-String");
                Collection<PSObject> results = pipeline.Invoke();

                runspace.Close();

                StringBuilder stringBuilder = new StringBuilder();
                foreach (PSObject obj in results)
                {
                    stringBuilder.Append(obj.ToString());
                }

                Global.Log.Info(stringBuilder);
                return stringBuilder;
            }
            catch (Exception ex)
            {
                Global.Log.Error(ex.ToString());
                throw;
            }
        }

        public void ExecuteRemoteAsync(string command)
        {
            try
            {
                ManualResetEvent wait = new ManualResetEvent(false);

                var rs = RunspaceFactory.CreateRunspace();
                rs.Open();

                var pipe = rs.CreatePipeline();

                pipe.Output.DataReady += (s, e) =>
                {
                    var reader = (PipelineReader<PSObject>)s;

                    foreach (var item in reader.Read(reader.Count))
                    {
                        Console.WriteLine(item.ToString());
                    }
                };

                pipe.StateChanged += (s, e) =>
                {
                    var state = e.PipelineStateInfo.State;

                    if (state == PipelineState.Completed ||
                       state == PipelineState.Failed)
                    {
                        wait.Set();
                    }
                };

                pipe.Commands.AddScript(command);
                pipe.Input.Close();
                pipe.InvokeAsync();

                while (pipe.PipelineStateInfo.State == PipelineState.Running)
                {
                    wait.WaitOne(60 * 500);
                }

                rs.Close();
            }
            catch (Exception ex)
            {
                Global.Log.Error(ex.ToString());
                throw;
            }
        }

        public string LoadScript(string fileName)
        { 
            try 
            { 
                using (StreamReader sr = new StreamReader(fileName)) 
                {
                    StringBuilder fileContents = new StringBuilder();
                    string curLine;

                    while ((curLine = sr.ReadLine()) != null) 
                    { 
                        fileContents.Append(curLine + "\n"); 
                    }

                    return fileContents.ToString(); 
                } 
            } 
            catch (Exception ex) 
            {
                Global.Log.Error(ex.ToString());
                throw;
            }
        }
    }
}
