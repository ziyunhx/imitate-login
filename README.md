Imitate Login
============

Overview
-------

This project is used fo imitate login the social network. Such as weibo, qq, facebook or twitter.

#### Finished part

 - Weibo
 - WeiboWap

#### Plan to support

 - Taobao
 - QQ
 - Facebook
 - Twitter
 - Google
 
How to use it?
-------

This project is code by csharp, you can use .Net Framework or Mono to run it. You can test it through the LoginTestTool project which depend on Gtk+. You can also call this lib on multri language through Apache thrift.

The only method at this project is `LoginResult Login(1: string userName, 2: string password, 3: LoginSite loginSite);` on LoginHelper.cs. You will get the result contain result type, descript message, cookies dictionary.

License
-------

The Apache License 2.0 applies to all samples in this repository.

   Copyright 2011 Xamarin Inc

   Licensed under the Apache License, Version 2.0 (the "License");
   you may not use this file except in compliance with the License.
   You may obtain a copy of the License at

       http://www.apache.org/licenses/LICENSE-2.0

   Unless required by applicable law or agreed to in writing, software
   distributed under the License is distributed on an "AS IS" BASIS,
   WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
   See the License for the specific language governing permissions and
   limitations under the License.