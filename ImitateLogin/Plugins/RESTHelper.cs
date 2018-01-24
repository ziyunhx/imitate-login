using log4net;
using Newtonsoft.Json;
using System;
using System.Drawing;
using System.Web;
using Thrinax.Http;

namespace ImitateLogin
{
    /// <summary>
    /// REST helper. Because of the image, only support POST method.
    /// If you choose GET, only the imageUrl can you get.
    /// </summary>
    public class RESTHelper
    {
		private string _urlFormat = "";
		private string _method = "POST";

		/// <summary>
		/// Initializes a new instance of the <see cref="ImitateLogin.RESTHelper"/> class.
		/// </summary>
		/// <param name="urlFormat">URL format.</param>
		/// <param name="method">Method.</param>
		public RESTHelper(string urlFormat, string method = "POST")
		{
			_urlFormat = urlFormat;
			_method = method;
		}

		/// <summary>
		/// Operation the specified loginSite, imageUrl and image.
		/// </summary>
		/// <param name="loginSite">Login site.</param>
		/// <param name="imageUrl">Image URL.</param>
		/// <param name="image">Image.</param>
		public string Operation(string loginSite, string imageUrl = "", Image image = null)
		{
			try
			{
				if(_method.ToUpper() == "GET")
				{
					return HttpHelper.GetHttpContent(string.Format(_urlFormat, loginSite, HttpUtility.UrlEncode(imageUrl)));
				}
				else if(_method.ToUpper() == "POST")
				{
					string postData = JsonConvert.SerializeObject(
						new OperationObj(){
							LoginSite=loginSite, ImageUrl = imageUrl//, Image = ImageHelper.GetBytesByImage(image)
						});
					return HttpHelper.GetHttpContent(_urlFormat,postData);
				}

				ILog logger = LogManager.GetLogger(typeof(RESTHelper));
				logger.Error("Operation Method Not Found!");
			}
			catch(Exception ex) 
			{
				ILog logger = LogManager.GetLogger(typeof(RESTHelper));
				logger.Error("Operation Not Found!" +Environment.NewLine + ex.ToString());
			}
			return "";
		}
    }
}
