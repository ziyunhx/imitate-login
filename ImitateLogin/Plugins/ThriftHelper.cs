using log4net;
using System;
using System.Drawing;
using Thrift.Protocol;
using Thrift.Transport;

namespace ImitateLogin
{
    /// <summary>
    /// Thrift helper.
    /// </summary>
    public class ThriftHelper
    {
		private string _host = "";
		private int _port = 0;

		/// <summary>
		/// Initializes a new instance of the <see cref="ImitateLogin.ThriftHelper"/> class.
		/// </summary>
		/// <param name="host">Host.</param>
		/// <param name="port">Port.</param>
		public ThriftHelper(string host, int port)
		{
			_host = host;
			_port = port;
		}

		/// <summary>
		/// Operation the specified loginSite, imageUrl and image.
		/// </summary>
		/// <param name="loginSite">Login site.</param>
		/// <param name="imageUrl">Image URL.</param>
		/// <param name="image">Image.</param>
		public string Operation(string loginSite, string imageUrl = "", Image image = null)
		{
			string result = "";
			TTransport transport = new TSocket(_host, _port); 
			try
			{
				TProtocol protocol = new TBinaryProtocol(transport); 
				ThriftOperation.Client client = new ThriftOperation.Client(protocol); 
				transport.Open();
				OperationObj operationObj = new OperationObj()
					{
						LoginSite = loginSite,
						ImageUrl = imageUrl,
						//Image = ImageHelper.GetBytesByImage(image)
					};

				result = client.Operation(operationObj);
			}
			catch(Exception ex) 
			{
				ILog logger = LogManager.GetLogger(typeof(ThriftHelper));
				logger.Error("Operation Not Found!" +Environment.NewLine + ex.ToString());
			}
			finally {
				transport.Close();
			}
			return result;
		}
    }
}
