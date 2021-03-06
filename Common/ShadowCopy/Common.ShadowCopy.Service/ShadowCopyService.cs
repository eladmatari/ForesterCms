using Common.ShadowCopy.Logic;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Threading.Tasks;

namespace Common.ShadowCopy.Service
{
    public partial class ShadowCopyService : ServiceBase
    {
        public ShadowCopyService()
        {
            InitializeComponent();
        }

        protected override void OnStart(string[] args)
        {
            SiteWatcher.Start();
        }

        protected override void OnStop()
        {
            SiteWatcher.Stop();
        }
    }
}
