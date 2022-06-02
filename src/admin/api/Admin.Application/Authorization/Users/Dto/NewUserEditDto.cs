using System;
using System.Collections.Generic;
using System.Text;

namespace Magicodes.Admin.Authorization.Users.Dto
{
    public class NewUserEditDto
    {
        public string Id { get; set; }
        public string Password { get; set; }
        public bool ShouldChangePasswordOnNextLogin { get; set; }
        /// <summary>
        /// 真实姓名
        /// </summary>
        public string Name { get; set; }


        /// <summary>
        /// 登陆名
        /// </summary>
        public string UserName { get; set; }
        /// <summary>
        /// 公司code
        /// </summary>
        public string OrganizationCode { get; set; }
        /// <summary>
        /// 部门code
        /// </summary>
        public string DeptCode { get; set; }

        /// <summary>
        /// 邮箱
        /// </summary>
        public string EmailAddress { get; set; }

        /// <summary>
        /// 手机号码
        /// </summary>
        public string PhoneNumber { get; set; }


        /// <summary>
        /// 办公电话(SignInToken)
        /// </summary>
        public string TelNumber { get; set; }

        /// <summary>
        /// 性别(0男1女)
        /// </summary>
        public string Sex { get; set; }
        /// <summary>
        /// bu权限
        /// </summary>
        public string BUAuthority { get; set; }


        /// <summary>
        /// 上一次登陆时间
        /// </summary>
        public DateTime? LastLoginTime { get; set; }


        /// <summary>
        /// 是否激活（IsLockoutEnabled）
        /// </summary>
        public bool IsActive { get; set; }

        /// <summary>
        /// 是否审核
        /// </summary>
        public bool IsVerify { get; set; }

        /// <summary>
        /// 审核评语
        /// </summary>
        public string VerifyRem { get; set; }

        /// <summary>
        /// 添加时间
        /// </summary>
        public DateTime CreationTime { get; set; }
       
    }
}
