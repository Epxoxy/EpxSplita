using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EpxSplita.Model
{
    public class LoginModel
    {
        public string UserName { get; set; }
        public string SiteName { get; set; }
        public bool IsLogined { get; set; }
        public bool IsStoredUser { get; set; }
        public bool RememberUser { get; set; }
        public string LoginWebSite { get; set; }
        public List<string> UserNameList { get; set; }

        public LoginModel()
        {

        }
    }
}
