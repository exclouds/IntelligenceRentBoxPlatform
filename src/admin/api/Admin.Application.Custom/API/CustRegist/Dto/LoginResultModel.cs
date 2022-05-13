using System;
using System.Collections.Generic;
using System.Text;

namespace Admin.Application.Custom.API.CustRegist.Dto
{
    public class LoginResultModel
    {
        public string AccessToken { get; set; }       

        public long UserId { get; set; }
        //public long RoleId { get; set; }

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
        /// 用户性质（0个人租客，1：个人箱东；2：公司租客，3：公司箱东）
        /// </summary>
        public int CompanyType { get; set; }
        public int? TenantId { get; set; }
}
}
