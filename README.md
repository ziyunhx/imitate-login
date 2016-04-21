Imitate Login
============

Overview
-------

This project is used fo imitate login the social network. Such as weibo, qq, facebook or twitter.

#### Finished part

 - Weibo
 - WeiboWap
 - Baidu
 - ~~WeChat~~

#### Plan to support

 - Taobao
 - QQ
 - Facebook
 - Twitter
 - Google
 
How to use it?
-------

 - This project is code by csharp, you can use .Net Framework or Mono to run it.
 - You can test it through the LoginTestTool project which depend on Gtk+ or WPF.
 - You can also call this lib on multri language through Apache thrift.
 - The only method at this project is  `LoginResult Login(1: string userName, 2: string password, 3: LoginSite loginSite);`  on LoginHelper.cs.
 - You will get the result contain result type, descript message, cookies dictionary.

Extensions
-------

There are some demo at plugins dir in order to tell you how to develop an extend plugin for imitate login.
The extend plugins will be used for solve some problems such as CAPTCHA, second verify, QR code and so on.

#### Support Mode

 - Thrift RPC (Apache Thrift)
 - HTTP RESTful (POST/GET)
 - MEF (Managed Extensibility Framework)

#### Config

In order to use the plugins, you must add their config at extension.conf under runtime dir. Then the imitate login will be known how and when call the plugin. If you forgot add this, the plugin will be ingored.

This is the format what config file support followed behind:

    {
        "Extensions": [{
            "ExtendType": 2,
            "SupportSite": [
            6],
            "Path": null,
            "Host": null,
            "Port": 0,
            "UrlFormat": "http://localhost:2920/Mail/SendMail?loginSite={0}&imageUrl={1}",
            "HttpMethod": "GET"
        }, {
            "ExtendType": 1,
            "SupportSite": [
            5, 1],
            "Path": "Extensions",
            "Host": null,
            "Port": 0,
            "UrlFormat": null,
            "HttpMethod": null
        }, {
            "ExtendType": 3,
            "SupportSite": [
            2],
            "Path": null,
            "Host": "127.0.0.1",
            "Port": 7801,
            "UrlFormat": null,
            "HttpMethod": null
        }]
    }

#### Demo & Tools

 - [Mail Nocation](https://github.com/ziyunhx/imitate-login/tree/master/Extensions/MailNotication) (Use the http restful)
 - [RecogCAPTCHA](https://github.com/ziyunhx/imitate-login/tree/master/Extensions/RecogCAPTCHA) (Apache Thrift)
 - [CodePlatform](https://github.com/ziyunhx/imitate-login/tree/master/Extensions/CodePlatform) (MEF)
 - [PluginConfigBuild](https://github.com/ziyunhx/imitate-login/tree/master/Tools/PluginConfigBuild) (A tool for build plugin config file.)

License
-------

The Apache License 2.0 applies to all samples in this repository.

   Licensed under the Apache License, Version 2.0 (the "License");
   you may not use this file except in compliance with the License.
   You may obtain a copy of the License at

       http://www.apache.org/licenses/LICENSE-2.0

   Unless required by applicable law or agreed to in writing, software
   distributed under the License is distributed on an "AS IS" BASIS,
   WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
   See the License for the specific language governing permissions and
   limitations under the License.
