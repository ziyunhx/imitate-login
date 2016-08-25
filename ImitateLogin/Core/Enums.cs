using System;
using System.ComponentModel;
using System.Reflection;
using System.Collections.Generic;

namespace ImitateLogin
{
	/// <summary>
	/// Login site.
	/// </summary>
	public enum LoginSite
	{
		[Description("Weibo")]
		Weibo = 1,
		[Description("Weibo Wap")]
		WeiboWap = 2,
		[Description("*Taobao")]
		Taobao = 3,
		[Description("*Tencent QQ")]
		QQ = 4,
		[Description("Baidu")]
		Baidu = 5,
        [Description("WeChat")]
        WeChat = 6,
		[Description("*Facebook")]
		Facebook = 21,
		[Description("*Twitter")]
		Twitter = 22,
		[Description("*Google")]
		Google = 23,
		[Description("*Universal")]
		Universal = 99,
	}

	/// <summary>
	/// Login Result type.
	/// </summary>
	public enum ResultType
	{
		Success = 200,
		Failed = 400,
		NeedCaptcha = 401,
		UserNameWrong = 402,
		PasswordWrong = 403,
		IpLimit = 404,
		AccounntLimit = 405,
		Timeout = 408,
		ServiceError = 500,
	}

    /// <summary>
    /// Plugin type
    /// </summary>
    public enum PluginType
    {
        MEF = 1,
        REST = 2,
        Thrift = 3,
    }

    public class Enums
	{
		/// <summary>
		/// Gets the description.
		/// </summary>
		/// <returns>The description.</returns>
		/// <param name="enumName">Enum name.</param>
		public static string GetDescription(Enum enumName)
		{
			string _description = string.Empty;
			FieldInfo _fieldInfo = enumName.GetType().GetField(enumName.ToString());
			DescriptionAttribute[] _attributes = GetDescriptAttr(_fieldInfo);
			if (_attributes != null && _attributes.Length > 0)
				_description = _attributes[0].Description;
			else
				_description = enumName.ToString();
			return _description;
		}

		public static List<string> GetDescriptions<T>()
		{
			List<string> descriptions = new List<string> ();
			Type _type = typeof(T);
			foreach (FieldInfo field in _type.GetFields())
			{
				DescriptionAttribute[] _curDesc = GetDescriptAttr(field);
				if (_curDesc != null && _curDesc.Length > 0)
					descriptions.Add(_curDesc[0].Description);
			}
			return descriptions;
		}

		/// <summary>
		/// Gets the descript attr.
		/// </summary>
		/// <returns>The descript attr.</returns>
		/// <param name="fieldInfo">Field info.</param>
		public static DescriptionAttribute[] GetDescriptAttr(FieldInfo fieldInfo)
		{
			if (fieldInfo != null)
			{
				return (DescriptionAttribute[])fieldInfo.GetCustomAttributes(typeof(DescriptionAttribute), false);
			}
			return null;
		}

		/// <summary>
		/// Gets the name of the enum.
		/// </summary>
		/// <returns>The enum name.</returns>
		/// <param name="description">Description.</param>
		public static T GetEnumName<T>(string description)
		{
			Type _type = typeof(T);
			foreach (FieldInfo field in _type.GetFields())
			{
				DescriptionAttribute[] _curDesc = GetDescriptAttr(field);
				if (_curDesc != null && _curDesc.Length > 0)
				{
					if (_curDesc[0].Description == description)
						return (T)field.GetValue(null);
				}
				else
				{
					if (field.Name == description)
						return (T)field.GetValue(null);
				}
			}
			throw new ArgumentException(string.Format("{0} can't find the enum.", description), "Description");
		}
	}
}

