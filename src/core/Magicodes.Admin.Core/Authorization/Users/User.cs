using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Abp.Authorization.Users;
using Abp.Extensions;
using Abp.Timing;

namespace Magicodes.Admin.Authorization.Users
{
	/// <summary>
	/// Represents a user in the system.
	/// </summary>
	public class User : AbpUser<User>
	{
        /// <summary>
        /// 对公司代码
        /// </summary>
        [Display(Name = "对公司代码")]
        [MaxLength(100)]
        public string OrganizationCode { get; set; }

        /// <summary>
        /// 对应组织机构代码
        /// </summary>
        [Display(Name = "对应部门代码")]
        [MaxLength(50)]
        public string DeptCode { get; set; }

       
        /// <summary>
        /// 性别
        /// </summary>
        [Display(Name = "性别")]
        public int Sex { get; set; }
        /// <summary>
        /// 下次登录修改密码
        /// </summary>
        public bool ShouldChangePasswordOnNextLogin { get; set; }

        public const string DefaultPassword = "admin123456";
        /// <summary>
        /// 过期时间
        /// </summary>
        public DateTime? SignInTokenExpireTimeUtc { get; set; }
		[MaxLength(500)]
		public string SignInToken { get; set; }

        [Display(Name = "电话")]
        [MaxLength(20)]
        public string TelNumber { get; set; }
        
        [Display(Name = "用户性质（0：租客客户，1：箱东客户；2：平台管理员）")]
        public int UserNature { get; set; } 
        [Display(Name = "是否管理员（客户端企业用户）")]
        public bool IsAdmin { get; set; } = false;

        /// <summary>
        /// 是否审核
        /// </summary>
        [Display(Name = "是否审核")]
        public bool IsVerify { get; set; }

        /// <summary>
        /// 审核评语
        /// </summary>
        [Display(Name = "审核评语")]
        [MaxLength(500)]
        public string VerifyRem { get; set; }

        public User()
		{
			IsLockoutEnabled = true;
			IsTwoFactorEnabled = true;
		}

		/// <summary>
		/// Creates admin <see cref="User"/> for a tenant.
		/// </summary>
		/// <param name="tenantId">Tenant Id</param>
		/// <param name="emailAddress">Email address</param>
		/// <returns>Created <see cref="User"/> object</returns>
		public static User CreateTenantAdminUser(int tenantId, string emailAddress)
		{
			var user = new User
			{
				TenantId = tenantId,
				UserName = AdminUserName,
				Name = AdminUserName,
				EmailAddress = emailAddress,
                IsLockoutEnabled = true,
                IsTwoFactorEnabled = true,
                Roles = new List<UserRole>()
            };

			user.SetNormalizedNames();
			return user;
		}

		public static string CreateRandomPassword()
		{
			return Guid.NewGuid().ToString("N").Truncate(16);
		}

		public override void SetNewPasswordResetCode()
		{
			/* This reset code is intentionally kept short.
             * It should be short and easy to enter in a mobile application, where user can not click a link.
             */
			PasswordResetCode = Guid.NewGuid().ToString("N").Truncate(10).ToUpperInvariant();
		}

		public void Unlock()
		{
			AccessFailedCount = 0;
			LockoutEndDateUtc = null;
		}

		public void SetSignInToken()
		{
			SignInToken = Guid.NewGuid().ToString();
			SignInTokenExpireTimeUtc = Clock.Now.AddDays(1).ToUniversalTime();
		}
	}
}