using log4net;
using Newtonsoft.Json;
using System;
using System.Drawing;
using System.IO;
using System.Linq;

namespace ImitateLogin
{
    /// <summary>
    /// Plugin helper.
    /// </summary>
    public class PluginHelper
    {
		private static string configPath = AppDomain.CurrentDomain.BaseDirectory + "extension.conf";
		private static object readLock = new object ();

		private static Config _pluginConfig = null;

		/// <summary>
		/// Gets the plugin config.
		/// </summary>
		/// <value>The plugin config.</value>
		public static Config pluginConfig
		{
			get{ 
				if (_pluginConfig == null)
					lock (readLock) {
						if (_pluginConfig == null) {
							if (File.Exists(configPath)) {
								try {
									_pluginConfig = JsonConvert.DeserializeObject<Config>(File.ReadAllText (configPath));
								} catch (Exception ex) {
									ILog logger = LogManager.GetLogger(typeof(PluginHelper));
									logger.Error("Plugin config is error!" + Environment.NewLine + ex.ToString());
								}
							}
							else {
								ILog logger = LogManager.GetLogger(typeof(PluginHelper));
								logger.Error("Plugin config is not exist!" );
							}
						}
					}

				return _pluginConfig;
			}
		}

		/// <summary>
		/// Operation the specified loginSite, imageUrl and image.
		/// </summary>
		/// <param name="loginSite">Login site.</param>
		/// <param name="imageUrl">Image URL.</param>
		/// <param name="image">Image.</param>
		public static string Operation(string loginSite, string imageUrl = "", Image image = null)
		{
			if (pluginConfig != null) {
				var extensions = pluginConfig.Extensions.Where (f => f.SupportSite.Contains (loginSite));
				if (extensions != null) {
					foreach (Extension extension in extensions) {
						string result = "";
						switch (extension.ExtendType) {
						case PluginType.MEF:
							MEFHelper mefHelper = new MEFHelper (extension.Path);
							result = mefHelper.Operation (loginSite, imageUrl, image);
							break;
						case PluginType.REST:
							RESTHelper restHelper = new RESTHelper (extension.UrlFormat, extension.HttpMethod);
							result = restHelper.Operation (loginSite, imageUrl, image);
							break;
						case PluginType.Thrift:
							ThriftHelper thriftHelper = new ThriftHelper (extension.Host, extension.Port);
							result = thriftHelper.Operation (loginSite, imageUrl, image);
							break;
						}
						if (!string.IsNullOrEmpty (result))
							return result;
					}
				} else {
					ILog logger = LogManager.GetLogger (typeof(PluginHelper));
					logger.Error ("Extension Not Found!");
				}
			} else {
				ILog logger = LogManager.GetLogger (typeof(PluginHelper));
				logger.Error ("Extension Config File Not Found!");
			}
			return "";
		}
    }
}
