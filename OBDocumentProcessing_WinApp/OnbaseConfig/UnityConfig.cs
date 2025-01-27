using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OBDocumentProcessing_WinApp.OnbaseConfig
{
    public class UnityConfig
    {
        public string DataSource
        {
            get { return ConfigurationSettings.AppSettings["www_DataSource"]; }
        }
        public string UserName
        {
            get { return ConfigurationSettings.AppSettings["www_username"]; }
        }

        public string Password
        {
            get { return ConfigurationSettings.AppSettings["www_password"]; }
        }

        public string Service
        {
            get { return ConfigurationSettings.AppSettings["www_Service"]; }
        }
    }
}
