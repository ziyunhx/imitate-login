using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.IO;
using Thrift;
using Thrift.Collections;
using Thrift.Protocol;
using Thrift.Transport;

namespace ImitateLogin
{
	public class LoginHelper : Login.Iface
	{
		/// <summary>
		/// Login the specified userName, password and loginSite.
		/// </summary>
		/// <param name="userName">User name.</param>
		/// <param name="password">Password.</param>
		/// <param name="loginSite">Login site.</param>
		/// <returns>cookies string</returns>
		public string Login(string userName, string password, LoginSite loginSite)
		{
			if(string.IsNullOrEmpty(userName) || string.IsNullOrEmpty(password))
				return "error, username or password can't be null.";

			ILogin LoginClass = null;

			switch (loginSite) {
			case LoginSite.Weibo:
				LoginClass = new WeiboLogin ();
				break;
			case LoginSite.WeiboWap:
				LoginClass = new WeiboWapLogin ();
				break;
			case LoginSite.SinaWap:
				LoginClass = new SinaWapLogin ();
				break;
			}

			if(LoginClass == null)
				return "error, can't find the login class.";

			return LoginClass.DoLogin (userName, password);
		}
	}

	public class Login {
		
		public interface Iface {
			string Login(string userName, string password, LoginSite loginSite);
		}

		public class Client : Iface {
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


			public string Login(string userName, string password, LoginSite loginSite)
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

			public string recv_Login()
			{
				TMessage msg = iprot_.ReadMessageBegin();
				if (msg.Type == TMessageType.Exception) {
					TApplicationException x = TApplicationException.Read(iprot_);
					iprot_.ReadMessageEnd();
					throw x;
				}
				Login_result result = new Login_result();
				result.Read(iprot_);
				iprot_.ReadMessageEnd();
				if (result.__isset.success) {
					return result.Success;
				}
				throw new TApplicationException(TApplicationException.ExceptionType.MissingResult, "Login failed: unknown result");
			}

		}
		public class Processor : TProcessor {
			public Processor(Iface iface)
			{
				iface_ = iface;
				processMap_["Login"] = Login_Process;
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
					if (fn == null) {
						TProtocolUtil.Skip(iprot, TType.Struct);
						iprot.ReadMessageEnd();
						TApplicationException x = new TApplicationException (TApplicationException.ExceptionType.UnknownMethod, "Invalid method name: '" + msg.Name + "'");
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
			public struct Isset {
				public bool userName;
				public bool password;
				public bool loginSite;
			}

			public Login_args() {
			}

			public void Read (TProtocol iprot)
			{
				TField field;
				iprot.ReadStructBegin();
				while (true)
				{
					field = iprot.ReadFieldBegin();
					if (field.Type == TType.Stop) { 
						break;
					}
					switch (field.ID)
					{
					case 1:
						if (field.Type == TType.String) {
							UserName = iprot.ReadString();
						} else { 
							TProtocolUtil.Skip(iprot, field.Type);
						}
						break;
					case 2:
						if (field.Type == TType.String) {
							Password = iprot.ReadString();
						} else { 
							TProtocolUtil.Skip(iprot, field.Type);
						}
						break;
					case 3:
						if (field.Type == TType.I32) {
							LoginSite = (LoginSite)iprot.ReadI32();
						} else { 
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

			public void Write(TProtocol oprot) {
				TStruct struc = new TStruct("Login_args");
				oprot.WriteStructBegin(struc);
				TField field = new TField();
				if (UserName != null && __isset.userName) {
					field.Name = "userName";
					field.Type = TType.String;
					field.ID = 1;
					oprot.WriteFieldBegin(field);
					oprot.WriteString(UserName);
					oprot.WriteFieldEnd();
				}
				if (Password != null && __isset.password) {
					field.Name = "password";
					field.Type = TType.String;
					field.ID = 2;
					oprot.WriteFieldBegin(field);
					oprot.WriteString(Password);
					oprot.WriteFieldEnd();
				}
				if (__isset.loginSite) {
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

			public override string ToString() {
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
			private string _success;

			public string Success
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
			public struct Isset {
				public bool success;
			}

			public Login_result() {
			}

			public void Read (TProtocol iprot)
			{
				TField field;
				iprot.ReadStructBegin();
				while (true)
				{
					field = iprot.ReadFieldBegin();
					if (field.Type == TType.Stop) { 
						break;
					}
					switch (field.ID)
					{
					case 0:
						if (field.Type == TType.String) {
							Success = iprot.ReadString();
						} else { 
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

			public void Write(TProtocol oprot) {
				TStruct struc = new TStruct("Login_result");
				oprot.WriteStructBegin(struc);
				TField field = new TField();

				if (this.__isset.success) {
					if (Success != null) {
						field.Name = "Success";
						field.Type = TType.String;
						field.ID = 0;
						oprot.WriteFieldBegin(field);
						oprot.WriteString(Success);
						oprot.WriteFieldEnd();
					}
				}
				oprot.WriteFieldStop();
				oprot.WriteStructEnd();
			}

			public override string ToString() {
				StringBuilder sb = new StringBuilder("Login_result(");
				sb.Append("Success: ");
				sb.Append(Success);
				sb.Append(")");
				return sb.ToString();
			}

		}

	}
}

