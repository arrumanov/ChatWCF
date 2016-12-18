using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Threading.Tasks;
using System.ServiceModel;
using ChatServiceLibrary;

namespace WindowsServiceHost
{
    public partial class Service1 : ServiceBase
    {
        private ServiceHost myHost;

        public Service1()
        {
            InitializeComponent();
        }

        protected override void OnStart(string[] args)
        {
            //проверка для подстраховки
            if(myHost != null)
            {
                myHost.Close();
                myHost = null;
            }

            //создать хост
            myHost = new ServiceHost(typeof(Service));
            //открыть хост
            myHost.Open();
        }

        protected override void OnStop()
        {
            //остановить хост
            if(myHost != null)
            {
                myHost.Close();
            }
        }
    }
}
