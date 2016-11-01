using ImitateLogin;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Thrift.Server;
using Thrift.Transport;

namespace CodePlatform
{
    class Program
    {
        static void Main(string[] args)
        {
            int port = 7801;

            string str = ConfigurationManager.AppSettings["ServerPort"];
            if (!string.IsNullOrEmpty(str))
                int.TryParse(str, out port);

            if (args != null && args.Length == 1)
            {
                int.TryParse(args[0], out port);
            }

            Start(port);
        }

        public static void Start(int port)
        {
            TServerSocket serverTransport = new TServerSocket(port, 0, false);
            ThriftOperation.Processor processor = new ThriftOperation.Processor(new demo());
            TServer server = new TSimpleServer(processor, serverTransport);
            Console.WriteLine("Starting server on port {0} ...", port);
            server.Serve();
        }
    }
}
