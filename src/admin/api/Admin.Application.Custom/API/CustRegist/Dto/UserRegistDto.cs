using System;
using System.Collections.Generic;
using System.Text;

namespace Admin.Application.Custom.API.CustRegist.Dto
{
    /// <summary>
    /// 注册
    /// </summary>
    public class UserRegistDto
    {
        //public string Id { get; set; }
        ///// <summary>
        ///// 姓名
        ///// </summary>
        //public string Name { get; set; }
        ///// <summary>
        ///// 登陆名
        ///// </summary>
        //public string UserName { get; set; }

        //public string Password { get; set; }
        ///// <summary>
        ///// 公司code：公司新增用户时写入
        ///// </summary>
        //public string OrganizationCode { get; set; }
        ///// <summary>
        ///// 联系电话(SignInToken)
        ///// </summary>
        //public string TelNumber { get; set; }
        ///// <summary>
        ///// 手机号码
        ///// </summary>
        //public string PhoneNumber { get; set; }

        ///// <summary>
        ///// 邮箱
        ///// </summary>
        //public string EmailAddress { get; set; }

        //public bool ShouldChangePasswordOnNextLogin { get; set; }       
        public string Id { get; set; }
        /// <summary>
        /// 公司名称
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// 公司code：公司新增用户时写入
        /// </summary>
        public string OrganizationCode { get; set; }
        /// <summary>
        /// 登陆名
        /// </summary>
        public string UserName { get; set; }

        public string Password { get; set; }

        /// <summary>
        /// 手机号码
        /// </summary>
        public string PhoneNumber { get; set; }
        /// <summary>
        /// 座机
        /// </summary>
        public string TelNumber { get; set; }

        /// <summary>
        /// 邮箱
        /// </summary>
        public string EmailAddress { get; set; }

        /// <summary>
        /// 父级公司编码
        /// </summary>    
        public string ParentCode { get; set; }

        /// <summary>
        /// 简称
        /// </summary>    
        public string ShortName { get; set; }

        /// <summary>
        /// 营业执照存储路径
        /// </summary>
        public string BusinessLicense { get; set; }

        /// <summary>
        /// 用户性质（0：租客客户，1：箱东客户；2：平台管理员）
        /// </summary>
        public int CompanyType { get; set; }

        /// <summary>
        /// 性别(0男1女)
        /// </summary>
        public int? Sex { get; set; }

        /// <summary>
        /// 是否审核通过（IsLockoutEnabled）:对应区分个人注册和公司新增个人用户
        /// </summary>
        public bool IsActive { get; set; }

    }
}
