using System;
using System.Configuration;
using Thrift.Server;
using Thrift.Transport;

[assembly: log4net.Config.XmlConfigurator(Watch = true)]

namespace ImitateLogin.Service
{
    class Program
    {
        static void Main(string[] args)
        {
            int port = 7901;

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
            Login.Processor processor = new Login.Processor(new LoginHelper());
            TServer server = new TSimpleServer(processor, serverTransport);
            Console.WriteLine("Starting server on port {0} ...", port);
            server.Serve();
        }
    }
}
