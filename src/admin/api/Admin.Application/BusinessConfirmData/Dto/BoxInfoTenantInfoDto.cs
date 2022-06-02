
using Magicodes.Admin.Core.Custom.Business;
using System;
using System.Collections.Generic;
using System.Text;

namespace Magicodes.Admin.BusinessConfirmData.Dto
{
    public class BoxInfoTenantInfoDto
    {
        public string Msg { get; set; }
        public string CreateName { get; set; }
        public string Company { get; set; }
        /// <summary>
        /// 手机
        /// </summary>
        public string PhoneNumber { get; set; }
        /// <summary>
        /// 电话
        /// </summary>
        public string TelNumber { get; set; }
        public BoxInfo BoxInfo { get; set; }
        public TenantInfo TenantInfo { get; set; }
    }
}
