using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Magicodes.Admin.Web.Models.SingleLogIn
{
	public class SingleLogInModel
	{
		/// <summary>
		/// 登录名
		/// </summary>
		[Required]
		public string UserName { get; set; }

		/// <summary>
		/// 租户Id
		/// </summary>
		[Required]
		public int TenantId { get; set; }

		/// <summary>
		/// 跳转url
		/// </summary>
		public string ReturnUrl { get; set; }
	}
}
