using log4net;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.ComponentModel.Composition.Hosting;
using System.IO;
using System.Text;
using Thrift;
using Thrift.Protocol;

[assembly: log4net.Config.XmlConfigurator(Watch = true)]

namespace ImitateLogin
{
    public class LoginHelper : Login.Iface
	{
        [ImportMany]
        IEnumerable<Lazy<ILogin, ILoginSiteData>> operations;

        private CompositionContainer _container;
        ILog logger = LogManager.GetLogger(typeof(LoginHelper));

        public LoginHelper(string path = "")
        {
            try
            {
                //An aggregate catalog that combines multiple catalogs
                var catalog = new AggregateCatalog();
                //Adds all the parts found in the same assembly as the Program class
                catalog.Catalogs.Add(new AssemblyCatalog(typeof(LoginHelper).Assembly));

                if (string.IsNullOrEmpty(path))
                    path = "Extensions";

                if (Directory.Exists(path))
                    catalog.Catalogs.Add(new DirectoryCatalog(path));
                else if (Directory.Exists(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, path)))
                    catalog.Catalogs.Add(new DirectoryCatalog(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, path)));
                else
                    logger.Warn("No MEF extensions path has configured.");

                //Create the CompositionContainer with the parts in the catalog
                _container = new CompositionContainer(catalog);

                //Fill the imports of this object
                this._container.ComposeParts(this);
            }
            catch (CompositionException compositionException)
            {
                logger.Error(compositionException.ToString());
            }
        }

        /// <summary>
        /// Login the specified userName, password and loginSite.
        /// </summary>
        /// <param name="userName">User name.</param>
        /// <param name="password">Password.</param>
        /// <param name="loginSite">Login site.</param>
        /// <returns>cookies string</returns>
        public LoginResult Login(string userName, string password, LoginSite loginSite)
		{
			return DoLogin(userName, password, loginSite.ToString());
		}

		public LoginResult DoLogin(string userName, string password, string loginSite)
		{
			LoginResult result = new LoginResult();

			if (string.IsNullOrEmpty(userName) || string.IsNullOrEmpty(password))
			{
				result.Result = ResultType.Failed;
				result.Msg = "error, username or password can't be null.";
			}
			else
			{
				bool hasOperation = false;

				foreach (Lazy<ILogin, ILoginSiteData> i in operations)
				{
					if (i.Metadata.loginSite.ToString().ToLower().Equals(loginSite.ToLower()))
					{
						hasOperation = true;
						result = i.Value.DoLogin(userName, password);
					}
				}

				if (!hasOperation)
				{
					result.Result = ResultType.Failed;
					result.Msg = "error, can't find the login class.";
				}
			}

			return result;
		}
	}

public class Login
	{
		public interface Iface
		{
			LoginResult Login(string userName, string password, LoginSite loginSite);
			LoginResult DoLogin(string userName, string password, string loginSite);
		}

		public class Client : Iface
		{
			public Client(TProtocol prot) : this(prot, prot)
			{
			}

			public Client(TProtocol iprot, TProtocol oprot)
			{
				iprot_ = iprot;
				oprot_ = oprot;
			}

			protected TProtocol iprot_;
			protected TProtocol oprot_;
			protected int seqid_;

			public TProtocol InputProtocol
			{
				get { return iprot_; }
			}
			public TProtocol OutputProtocol
			{
				get { return oprot_; }
			}


			public LoginResult Login(string userName, string password, LoginSite loginSite)
			{
				send_Login(userName, password, loginSite);
				return recv_Login();
			}

			public void send_Login(string userName, string password, LoginSite loginSite)
			{
				oprot_.WriteMessageBegin(new TMessage("Login", TMessageType.Call, seqid_));
				Login_args args = new Login_args();
				args.UserName = userName;
				args.Password = password;
				args.LoginSite = loginSite;
				args.Write(oprot_);
				oprot_.WriteMessageEnd();
				oprot_.Transport.Flush();
			}

			public LoginResult recv_Login()
			{
				TMessage msg = iprot_.ReadMessageBegin();
				if (msg.Type == TMessageType.Exception)
				{
					TApplicationException x = TApplicationException.Read(iprot_);
					iprot_.ReadMessageEnd();
					throw x;
				}
				Login_result result = new Login_result();
				result.Read(iprot_);
				iprot_.ReadMessageEnd();
				if (result.__isset.success)
				{
					return result.Success;
				}
				throw new TApplicationException(TApplicationException.ExceptionType.MissingResult, "Login failed: unknown result");
			}

			public LoginResult DoLogin(string userName, string password, string loginSite)
			{
				send_DoLogin(userName, password, loginSite);
				return recv_DoLogin();
			}

			public void send_DoLogin(string userName, string password, string loginSite)
			{
				oprot_.WriteMessageBegin(new TMessage("DoLogin", TMessageType.Call, seqid_));
				DoLogin_args args = new DoLogin_args();
				args.UserName = userName;
				args.Password = password;
				args.LoginSite = loginSite;
				args.Write(oprot_);
				oprot_.WriteMessageEnd();
				oprot_.Transport.Flush();
			}

			public LoginResult recv_DoLogin()
			{
				TMessage msg = iprot_.ReadMessageBegin();
				if (msg.Type == TMessageType.Exception)
				{
					TApplicationException x = TApplicationException.Read(iprot_);
					iprot_.ReadMessageEnd();
					throw x;
				}
				DoLogin_result result = new DoLogin_result();
				result.Read(iprot_);
				iprot_.ReadMessageEnd();
				if (result.__isset.success)
				{
					return result.Success;
				}
				throw new TApplicationException(TApplicationException.ExceptionType.MissingResult, "DoLogin failed: unknown result");
			}

		}
		public class Processor : TProcessor
		{
			public Processor(Iface iface)
			{
				iface_ = iface;
				processMap_["Login"] = Login_Process;
				processMap_["DoLogin"] = DoLogin_Process;
			}

			protected delegate void ProcessFunction(int seqid, TProtocol iprot, TProtocol oprot);
			private Iface iface_;
			protected Dictionary<string, ProcessFunction> processMap_ = new Dictionary<string, ProcessFunction>();

			public bool Process(TProtocol iprot, TProtocol oprot)
			{
				try
				{
					TMessage msg = iprot.ReadMessageBegin();
					ProcessFunction fn;
					processMap_.TryGetValue(msg.Name, out fn);
					if (fn == null)
					{
						TProtocolUtil.Skip(iprot, TType.Struct);
						iprot.ReadMessageEnd();
						TApplicationException x = new TApplicationException(TApplicationException.ExceptionType.UnknownMethod, "Invalid method name: '" + msg.Name + "'");
						oprot.WriteMessageBegin(new TMessage(msg.Name, TMessageType.Exception, msg.SeqID));
						x.Write(oprot);
						oprot.WriteMessageEnd();
						oprot.Transport.Flush();
						return true;
					}
					fn(msg.SeqID, iprot, oprot);
				}
				catch (IOException)
				{
					return false;
				}
				return true;
			}

			public void Login_Process(int seqid, TProtocol iprot, TProtocol oprot)
			{
				Login_args args = new Login_args();
				args.Read(iprot);
				iprot.ReadMessageEnd();
				Login_result result = new Login_result();
				result.Success = iface_.Login(args.UserName, args.Password, args.LoginSite);
				oprot.WriteMessageBegin(new TMessage("Login", TMessageType.Reply, seqid));
				result.Write(oprot);
				oprot.WriteMessageEnd();
				oprot.Transport.Flush();
			}

			public void DoLogin_Process(int seqid, TProtocol iprot, TProtocol oprot)
			{
				DoLogin_args args = new DoLogin_args();
				args.Read(iprot);
				iprot.ReadMessageEnd();
				DoLogin_result result = new DoLogin_result();
				result.Success = iface_.DoLogin(args.UserName, args.Password, args.LoginSite);
				oprot.WriteMessageBegin(new TMessage("DoLogin", TMessageType.Reply, seqid));
				result.Write(oprot);
				oprot.WriteMessageEnd();
				oprot.Transport.Flush();
			}

		}


		[Serializable]
		public partial class Login_args : TBase
		{
			private string _userName;
			private string _password;
			private LoginSite _loginSite;

			public string UserName
			{
				get
				{
					return _userName;
				}
				set
				{
					__isset.userName = true;
					this._userName = value;
				}
			}

			public string Password
			{
				get
				{
					return _password;
				}
				set
				{
					__isset.password = true;
					this._password = value;
				}
			}

			public LoginSite LoginSite
			{
				get
				{
					return _loginSite;
				}
				set
				{
					__isset.loginSite = true;
					this._loginSite = value;
				}
			}


			public Isset __isset;
			[Serializable]
			public struct Isset
			{
				public bool userName;
				public bool password;
				public bool loginSite;
			}

			public Login_args()
			{
			}

			public void Read(TProtocol iprot)
			{
				TField field;
				iprot.ReadStructBegin();
				while (true)
				{
					field = iprot.ReadFieldBegin();
					if (field.Type == TType.Stop)
					{
						break;
					}
					switch (field.ID)
					{
						case 1:
							if (field.Type == TType.String)
							{
								UserName = iprot.ReadString();
							}
							else
							{
								TProtocolUtil.Skip(iprot, field.Type);
							}
							break;
						case 2:
							if (field.Type == TType.String)
							{
								Password = iprot.ReadString();
							}
							else
							{
								TProtocolUtil.Skip(iprot, field.Type);
							}
							break;
						case 3:
							if (field.Type == TType.I32)
							{
								LoginSite = (LoginSite)iprot.ReadI32();
							}
							else
							{
								TProtocolUtil.Skip(iprot, field.Type);
							}
							break;
						default:
							TProtocolUtil.Skip(iprot, field.Type);
							break;
					}
					iprot.ReadFieldEnd();
				}
				iprot.ReadStructEnd();
			}

			public void Write(TProtocol oprot)
			{
				TStruct struc = new TStruct("Login_args");
				oprot.WriteStructBegin(struc);
				TField field = new TField();
				if (UserName != null && __isset.userName)
				{
					field.Name = "userName";
					field.Type = TType.String;
					field.ID = 1;
					oprot.WriteFieldBegin(field);
					oprot.WriteString(UserName);
					oprot.WriteFieldEnd();
				}
				if (Password != null && __isset.password)
				{
					field.Name = "password";
					field.Type = TType.String;
					field.ID = 2;
					oprot.WriteFieldBegin(field);
					oprot.WriteString(Password);
					oprot.WriteFieldEnd();
				}
				if (__isset.loginSite)
				{
					field.Name = "loginSite";
					field.Type = TType.I32;
					field.ID = 3;
					oprot.WriteFieldBegin(field);
					oprot.WriteI32((int)LoginSite);
					oprot.WriteFieldEnd();
				}
				oprot.WriteFieldStop();
				oprot.WriteStructEnd();
			}

			public override string ToString()
			{
				StringBuilder sb = new StringBuilder("Login_args(");
				sb.Append("UserName: ");
				sb.Append(UserName);
				sb.Append(",Password: ");
				sb.Append(Password);
				sb.Append(",LoginSite: ");
				sb.Append(LoginSite);
				sb.Append(")");
				return sb.ToString();
			}

		}


		[Serializable]
		public partial class Login_result : TBase
		{
			private LoginResult _success;

			public LoginResult Success
			{
				get
				{
					return _success;
				}
				set
				{
					__isset.success = true;
					this._success = value;
				}
			}


			public Isset __isset;
			[Serializable]
			public struct Isset
			{
				public bool success;
			}

			public Login_result()
			{
			}

			public void Read(TProtocol iprot)
			{
				TField field;
				iprot.ReadStructBegin();
				while (true)
				{
					field = iprot.ReadFieldBegin();
					if (field.Type == TType.Stop)
					{
						break;
					}
					switch (field.ID)
					{
						case 0:
							if (field.Type == TType.Struct)
							{
								Success = new LoginResult();
								Success.Read(iprot);
							}
							else
							{
								TProtocolUtil.Skip(iprot, field.Type);
							}
							break;
						default:
							TProtocolUtil.Skip(iprot, field.Type);
							break;
					}
					iprot.ReadFieldEnd();
				}
				iprot.ReadStructEnd();
			}

			public void Write(TProtocol oprot)
			{
				TStruct struc = new TStruct("Login_result");
				oprot.WriteStructBegin(struc);
				TField field = new TField();

				if (this.__isset.success)
				{
					if (Success != null)
					{
						field.Name = "Success";
						field.Type = TType.Struct;
						field.ID = 0;
						oprot.WriteFieldBegin(field);
						Success.Write(oprot);
						oprot.WriteFieldEnd();
					}
				}
				oprot.WriteFieldStop();
				oprot.WriteStructEnd();
			}

			public override string ToString()
			{
				StringBuilder sb = new StringBuilder("Login_result(");
				sb.Append("Success: ");
				sb.Append(Success == null ? "<null>" : Success.ToString());
				sb.Append(")");
				return sb.ToString();
			}

		}


		[Serializable]
		public partial class DoLogin_args : TBase
		{
			private string _userName;
			private string _password;
			private string _loginSite;

			public string UserName
			{
				get
				{
					return _userName;
				}
				set
				{
					__isset.userName = true;
					this._userName = value;
				}
			}

			public string Password
			{
				get
				{
					return _password;
				}
				set
				{
					__isset.password = true;
					this._password = value;
				}
			}

			public string LoginSite
			{
				get
				{
					return _loginSite;
				}
				set
				{
					__isset.loginSite = true;
					this._loginSite = value;
				}
			}


			public Isset __isset;
			[Serializable]
			public struct Isset
			{
				public bool userName;
				public bool password;
				public bool loginSite;
			}

			public DoLogin_args()
			{
			}

			public void Read(TProtocol iprot)
			{
				TField field;
				iprot.ReadStructBegin();
				while (true)
				{
					field = iprot.ReadFieldBegin();
					if (field.Type == TType.Stop)
					{
						break;
					}
					switch (field.ID)
					{
						case 1:
							if (field.Type == TType.String)
							{
								UserName = iprot.ReadString();
							}
							else
							{
								TProtocolUtil.Skip(iprot, field.Type);
							}
							break;
						case 2:
							if (field.Type == TType.String)
							{
								Password = iprot.ReadString();
							}
							else
							{
								TProtocolUtil.Skip(iprot, field.Type);
							}
							break;
						case 3:
							if (field.Type == TType.String)
							{
								LoginSite = iprot.ReadString();
							}
							else
							{
								TProtocolUtil.Skip(iprot, field.Type);
							}
							break;
						default:
							TProtocolUtil.Skip(iprot, field.Type);
							break;
					}
					iprot.ReadFieldEnd();
				}
				iprot.ReadStructEnd();
			}

			public void Write(TProtocol oprot)
			{
				TStruct struc = new TStruct("DoLogin_args");
				oprot.WriteStructBegin(struc);
				TField field = new TField();
				if (UserName != null && __isset.userName)
				{
					field.Name = "userName";
					field.Type = TType.String;
					field.ID = 1;
					oprot.WriteFieldBegin(field);
					oprot.WriteString(UserName);
					oprot.WriteFieldEnd();
				}
				if (Password != null && __isset.password)
				{
					field.Name = "password";
					field.Type = TType.String;
					field.ID = 2;
					oprot.WriteFieldBegin(field);
					oprot.WriteString(Password);
					oprot.WriteFieldEnd();
				}
				if (LoginSite != null && __isset.loginSite)
				{
					field.Name = "loginSite";
					field.Type = TType.String;
					field.ID = 3;
					oprot.WriteFieldBegin(field);
					oprot.WriteString(LoginSite);
					oprot.WriteFieldEnd();
				}
				oprot.WriteFieldStop();
				oprot.WriteStructEnd();
			}

			public override string ToString()
			{
				StringBuilder sb = new StringBuilder("DoLogin_args(");
				sb.Append("UserName: ");
				sb.Append(UserName);
				sb.Append(",Password: ");
				sb.Append(Password);
				sb.Append(",LoginSite: ");
				sb.Append(LoginSite);
				sb.Append(")");
				return sb.ToString();
			}

		}


		[Serializable]
		public partial class DoLogin_result : TBase
		{
			private LoginResult _success;

			public LoginResult Success
			{
				get
				{
					return _success;
				}
				set
				{
					__isset.success = true;
					this._success = value;
				}
			}


			public Isset __isset;
			[Serializable]
			public struct Isset
			{
				public bool success;
			}

			public DoLogin_result()
			{
			}

			public void Read(TProtocol iprot)
			{
				TField field;
				iprot.ReadStructBegin();
				while (true)
				{
					field = iprot.ReadFieldBegin();
					if (field.Type == TType.Stop)
					{
						break;
					}
					switch (field.ID)
					{
						case 0:
							if (field.Type == TType.Struct)
							{
								Success = new LoginResult();
								Success.Read(iprot);
							}
							else
							{
								TProtocolUtil.Skip(iprot, field.Type);
							}
							break;
						default:
							TProtocolUtil.Skip(iprot, field.Type);
							break;
					}
					iprot.ReadFieldEnd();
				}
				iprot.ReadStructEnd();
			}

			public void Write(TProtocol oprot)
			{
				TStruct struc = new TStruct("DoLogin_result");
				oprot.WriteStructBegin(struc);
				TField field = new TField();

				if (this.__isset.success)
				{
					if (Success != null)
					{
						field.Name = "Success";
						field.Type = TType.Struct;
						field.ID = 0;
						oprot.WriteFieldBegin(field);
						Success.Write(oprot);
						oprot.WriteFieldEnd();
					}
				}
				oprot.WriteFieldStop();
				oprot.WriteStructEnd();
			}

			public override string ToString()
			{
				StringBuilder sb = new StringBuilder("DoLogin_result(");
				sb.Append("Success: ");
				sb.Append(Success == null ? "<null>" : Success.ToString());
				sb.Append(")");
				return sb.ToString();
			}

		}

	}
}