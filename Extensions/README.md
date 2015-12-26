Imitate Login Extend
============

Overview
-------

This file in order to tell you how to develop an extend plugin for imitate login. There are some demo at plugins dir, and some subsidiary projects are placed at subsidiary.

The extend plugins will be used for solve some problems such as CAPTCHA, second verify, QR code and so on.

Support Mode
-------

#### Thrift RPC 

#### HTTP RESRful

#### IPC (File Listenter)

#### Process Call


Config
-------
 
The plugin must add their config at plugins/config.json under imitate login dir, Then the imitate login will be known how and when call the plugin. If you forgot add this, the plugin will be ingored.

This is the format what config file support followed behind:



Demo
-------

#### Mail Nocation

#### WeChat Login

#### CAPTCHA (IPC)

#### CAPTCHA (Thrift)

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
