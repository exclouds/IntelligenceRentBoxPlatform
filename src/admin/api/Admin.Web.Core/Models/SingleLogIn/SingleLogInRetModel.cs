using System;
using System.Collections.Generic;
using System.Text;

namespace Magicodes.Admin.Web.Models.SingleLogIn
{
	public class SingleLogInRetModel
	{
		/// <summary>
		/// token
		/// </summary>
		public string AccessToken { get; set; }
		/// <summary>
		/// 加密后的token
		/// </summary>
		public string EncryptedAccessToken { get; set; }

		/// <summary>
		/// 过期时间
		/// </summary>
		public int ExpireInSeconds { get; set; }

		/// <summary>
		/// 跳转Url
		/// </summary>
		public string ReturnUrl { get; set; }

		/// <summary>
		/// 用户Id
		/// </summary>
		public long UserId { get; set; }
		/// <summary>
		/// 用户真实姓名
		/// </summary>
		public string UserRealName { get; set; }
		/// <summary>
		/// 性别
		/// </summary>
		public int Sex { get; set; }
		/// <summary>
		/// 登陆账户名
		/// </summary>
		public string LogName { get; set; }
		
		/// <summary>
		/// 手机号
		/// </summary>
		public string MobilePhone { get; set; }
		/// <summary>
		/// 座机号码
		/// </summary>
		public string Tel { get; set; }
		/// <summary>
		/// 角色名称
		/// </summary>
		public string RoleName { get; set; }
		/// <summary>
		/// 部门名称
		/// </summary>
		public string DeptName { get; set; }
	}
}
