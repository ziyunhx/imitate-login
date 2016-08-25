/*
 * Licensed to the Apache Software Foundation (ASF) under one
 * or more contributor license agreements. See the NOTICE file
 * distributed with this work for additional information
 * regarding copyright ownership. The ASF licenses this file
 * to you under the Apache License, Version 2.0 (the
 * "License"); you may not use this file except in compliance
 * with the License. You may obtain a copy of the License at
 *
 *   http:  //www.apache.org/licenses/LICENSE-2.0
 *
 * Unless required by applicable law or agreed to in writing,
 * software distributed under the License is distributed on an
 * "AS IS" BASIS, WITHOUT WARRANTIES OR CONDITIONS OF ANY
 * KIND, either express or implied. See the License for the
 * specific language governing permissions and limitations
 * under the License.
 *
 * Contains some contributions under the Thrift Software License.
 * Please see doc/old-thrift-license.txt in the Thrift distribution for
 * details.
 */

namespace csharp ImitateLogin
namespace java com.tnidea.ImitateLogin
namespace py com.tnidea.ImitateLogin

enum ResultType {
  Success = 200,
  Failed = 400,
  NeedCaptcha = 401,
  UserNameWrong = 402,
  PasswordWrong = 403,
  IpLimit = 404,
  AccounntLimit = 405,
  Timeout = 408,
  ServiceError = 500
}

enum LoginSite {
  //Weibo
  Weibo = 1,
  //Weibo Wap
  WeiboWap = 2,
  //Taobao
  Taobao = 3,
  //Tencent QQ
  QQ = 4,
  //Baidu
  Baidu = 5,
  //WeChat
  WeChat = 6,
  //Facebook
  Facebook = 21,
  //Twitter
  Twitter = 22,
  //Google
  Google = 23,
  //Universal
	Universal = 99,
}

struct LoginResult {
  1: required ResultType Result;
  2: optional string Msg = "";
  3: optional map<string,string> Cookies;
  4: optional string Referer;
  5: optional string UserAgent;
}

service Login {
  LoginResult Login(1: string userName, 2: string password, 3: LoginSite loginSite);
  LoginResult DoLogin(1: string userName, 2: string password, 3: string loginSite);
}

## Plugins support start.
enum PluginType {
  //Managed Extensibility Framework
  MEF = 1,
  //Http RESTful
  REST = 2,
  //Thrift
  Thrift = 3
}

struct Extension {
  1: required PluginType ExtendType;
  2: required set<string> SupportSite;
  3: optional string Path;
  4: optional string Host;
  5: optional i32 Port;
  6: optional string UrlFormat;
  7: optional string HttpMethod;
}

struct Config {
  1: required list<Extension> Extensions;
}

struct OperationObj {
  1: required string loginSite;
  2: optional string imageUrl;
  3: optional binary image;
}

service ThriftOperation {
  string Operation(1: OperationObj operationObj);
}
## Plugins support end.