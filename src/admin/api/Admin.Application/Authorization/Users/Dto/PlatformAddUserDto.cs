using System;
using System.Collections.Generic;
using System.Text;

namespace Magicodes.Admin.Authorization.Users.Dto
{
	public class PlatformAddUserDto
	{
		/// <summary>
		/// 客户类型
		/// </summary>
		public string type { get; set; }

		/// <summary>
		/// 主数据客户平台代码
		/// </summary>
		public string comCode { get; set; }

		/// <summary>
		/// 登陆账户
		/// </summary>
		public string account { get; set; }

		/// <summary>
		/// 真实姓名
		/// </summary>
		public string name { get; set; }

		/// <summary>
		/// 手机号码
		/// </summary>
		public string mobilePhone { get; set; }

		/// <summary>
		/// 租户Id
		/// </summary>
		public string tenantId { get; set; }

		/// <summary>
		/// 角色id
		/// </summary>
		public string roleId { get; set; }

		/// <summary>
		/// 部门id
		/// </summary>
		public string deptId { get; set; }
	}
}
