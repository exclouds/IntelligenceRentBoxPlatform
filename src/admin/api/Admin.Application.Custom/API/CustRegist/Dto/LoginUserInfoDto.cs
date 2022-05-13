using System;
using System.Collections.Generic;
using System.Text;

namespace Admin.Application.Custom.API.CustRegist.Dto
{
    public class LoginUserInfoDto
    {
        public string Id { get; set; }
        /// <summary>
        /// 公司用户名称
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

        public bool IsAdmin { get; set; } = false;
        /// <summary>
        /// 用户性质（0：租客客户，1：箱东客户；2：平台管理员）
        /// </summary>
        public int CompanyType { get; set; }    

        /// <summary>
        /// 是否审核通过（IsLockoutEnabled）:对应区分个人注册和公司新增个人用户
        /// </summary>
        public bool IsActive { get; set; }
        public int? TenantId { get; set; }
    }
}
