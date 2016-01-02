using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ImitateLogin;

namespace RecogCAPTCHA
{
    [Export(typeof(IMEFOperation))]
    [ExportMetadata("loginSite", LoginSite.Baidu)]
    public class demo : IMEFOperation
    {
        public string Operate(string imageUrl = "", Image image = null, params string[] param)
        {
            return "1234";
        }
    }
}
