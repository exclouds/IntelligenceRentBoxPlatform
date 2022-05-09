using System;
using System.Collections.Generic;
using System.Text;

namespace Magicodes.Admin.Web.Models.SingleLogIn
{
	public class SecondRet
	{
		public static long serialVersionUID = 8098354063458373513L;
		// root 对象中必须要有一个主键，Spring Security 的 FixedPrincipalExtractor.java 限定了这几个："user", "username","userid", "user_id", "login", "id", "name"
		// 也因为这个场景，所以这里冗余了其他几个属性
		// 客户端需要用到哪个属性作为主键就用哪个，没必要全部搬过去
		/// <summary>
		/// 用户名
		/// </summary>
		public string username { get; set; }
		/// <summary>
		/// 姓名
		/// </summary>
		public string name { get; set; }
		/// <summary>
		/// id
		/// </summary>
		public string id { get; set; }
		/// <summary>
		/// 工号
		/// </summary>
		public string com_code { get; set; }
		/// <summary>
		/// 电话
		/// </summary>
		public string mobile_phone { get; set; }
	}
}
