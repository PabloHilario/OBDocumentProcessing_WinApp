using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OBDocumentProcessing_WinApp.Model;

namespace OBDocumentProcessing_WinApp.Model
{
    public class OnbaseUser
    {
        public string RealName { get; set; }
        public string Username { get; set; }
        public string Email { get; set; }
        public string SessionID { get; set; }
        public object UserGroupList { get; set; }
    }
}
