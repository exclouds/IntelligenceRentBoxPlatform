using System;
using System.Collections.Generic;
using System.Text;

namespace Magicodes.Admin.Web.Models.SingleLogIn
{
	public class SingleLogInGetTenantModel
	{
		/// <summary>
		/// 租户Id
		/// </summary>
		public int tenantId { get; set; }
		/// <summary>
		/// 租户名
		/// </summary>
		public string tenantName { get; set; }
		/// <summary>
		/// 是否默认租户
		/// </summary>
		public bool IsDefault { get; set; }

		/// <summary>
		/// 陆海客户代码
		/// </summary>
		public string ComCode { get; set; }
	}
}
