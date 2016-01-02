using ImitateLogin;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodePlatform
{
    class demo : ThriftOperation.Iface
    {
        public string Operation(OperationObj operationObj)
        {
            return "1234";
        }
    }
}
