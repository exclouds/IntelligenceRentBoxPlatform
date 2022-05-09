using System;
using System.Collections.Generic;
using System.Text;

namespace Magicodes.Admin.Authorization.Users.Dto
{
	public class UserListExtionDto
	{
        public long Id { get; set; }
		/// <summary>
		/// 真实姓名
		/// </summary>
		public string Name { get; set; }
        
		/// <summary>
		/// 登陆名
		/// </summary>
		public string UserName { get; set; }

		/// <summary>
		/// 邮箱
		/// </summary>
		public string EmailAddress { get; set; }

		/// <summary>
		/// 手机号码
		/// </summary>
		public string PhoneNumber { get; set; }


		/// <summary>
		/// 办公电话SignInToken
		/// </summary>
		public string TelNumber { get; set; }

		/// <summary>
		/// 性别Balance(0男1女)
		/// </summary>
		public int? Sex { get; set; }
        /// <summary>
		/// 所属公司code
		/// </summary>
		public string companyWorkNo { get; set; }
        /// <summary>
		/// 所属公司名称
		/// </summary>
		public string surname { get; set; }

        /// <summary>
        /// 部门
        /// </summary>
        public string Dpts { get; set; }

		/// <summary>
		/// 角色
		/// </summary>
		public string Roles { get; set; }

		/// <summary>
		/// 上一次登陆时间
		/// </summary>
		public DateTime? LastLoginTime { get; set; }
		

		/// <summary>
		/// 是否审核通过
		/// </summary>
		public bool IsLockoutEnabled { get; set; }

		/// <summary>
		/// 添加时间
		/// </summary>
		public DateTime CreationTime { get; set; }
        /// <summary>
        /// 是否管理员
        /// </summary>
        public bool IsAdmin { get; set; }
    }
}
