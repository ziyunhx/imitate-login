using ImitateLogin;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Thrift.Collections;

namespace PluginConfigBuild
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Build the plugin config, please make sure you had change the Config object.");
            Config pluginConfig = new Config();
            pluginConfig.Extensions = new List<Extension>();

            //Mail Notication for Wechat login.
            Extension mailNotication = new Extension();
            mailNotication.ExtendType = PluginType.REST;
            mailNotication.HttpMethod = "GET";
            mailNotication.SupportSite = new THashSet<string>();
			mailNotication.SupportSite.Add(LoginSite.WeChat.ToString());
            mailNotication.UrlFormat = "http://localhost:2920/Mail/SendMail?loginSite={0}&imageUrl={1}";
            pluginConfig.Extensions.Add(mailNotication);

            //Recog CAPTCHA for Baidu & Weibo login.
            Extension recogCaptcha = new Extension();
            recogCaptcha.ExtendType = PluginType.MEF;
            recogCaptcha.Path = "Extensions";
            recogCaptcha.SupportSite = new THashSet<string>();
			recogCaptcha.SupportSite.Add(LoginSite.Baidu.ToString());
			recogCaptcha.SupportSite.Add(LoginSite.Weibo.ToString());
            pluginConfig.Extensions.Add(recogCaptcha);

            //Code Platform for WeiboWap login.
            Extension codePlatform = new Extension();
            codePlatform.ExtendType = PluginType.Thrift;
            codePlatform.Host = "127.0.0.1";
            codePlatform.Port = 7801;
            codePlatform.SupportSite = new THashSet<string>();
			codePlatform.SupportSite.Add(LoginSite.WeiboWap.ToString());
            pluginConfig.Extensions.Add(codePlatform);

            Console.WriteLine("Write the plugin config to file.");
            string configStr = JsonConvert.SerializeObject(pluginConfig);
            File.WriteAllText(AppDomain.CurrentDomain.BaseDirectory + "extension.conf", ConvertJsonString(configStr));

            Console.WriteLine("Finish, please copy the plugin config ({0}) to application path.", AppDomain.CurrentDomain.BaseDirectory + "extension.conf");
            Console.WriteLine("Press any key to exit!");
            Console.ReadKey();
        }

        private static string ConvertJsonString(string str)
        {
            //格式化json字符串
            JsonSerializer serializer = new JsonSerializer();
            TextReader tr = new StringReader(str);
            JsonTextReader jtr = new JsonTextReader(tr);
            object obj = serializer.Deserialize(jtr);
            if (obj != null)
            {
                StringWriter textWriter = new StringWriter();
                JsonTextWriter jsonWriter = new JsonTextWriter(textWriter)
                {
                    Formatting = Formatting.Indented,
                    Indentation = 4,
                    IndentChar = ' '
                };
                serializer.Serialize(jsonWriter, obj);
                return textWriter.ToString();
            }
            else
            {
                return str;
            }
        }
    }
}
