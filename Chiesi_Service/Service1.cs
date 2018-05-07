using Chiesi;
using Chiesi.Log;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;
using System.Threading;

namespace Chiesi_Service
{
    public partial class Service1 : ServiceBase
    {
        ErrorLog errorLog;
        
        public Service1()
        {
            try
            {
                errorLog = new ErrorLog();
                InitializeComponent();

            }
            catch (Exception e)
            {
                errorLog.writeLog("Service", "Service1", e.ToString(), DateTime.Now);
            }


        }

        protected override void OnStart(string[] args)
        {
            try
            {
                Logic.DontStopMeNOW = true;
                var myThread = new Thread(new ThreadStart(StartMain));
                myThread.IsBackground = true;
                myThread.Start();
            }
            catch (Exception e)
            {
                errorLog.writeLog("Service", "OnStart", e.ToString(), DateTime.Now);
                           
            }
           

        }
        protected override void OnStop()
        {
            Logic.DontStopMeNOW = false;
            errorLog.writeLog("Service", "OnStop", "Parando Serviço", DateTime.Now);
            errorLog.writeLog("Service", "OnStop", "Aguarde...", DateTime.Now);
            errorLog.writeLog("Service", "OnStop", "Serviço Encerrado ...", DateTime.Now);
            
        }

        private void StartMain()
        {
            Logic.MainLogic(new string[0]);

        }
    }
}
