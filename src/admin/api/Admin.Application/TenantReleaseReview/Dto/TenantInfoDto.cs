using Abp.Application.Services.Dto;
using Magicodes.Admin.Core.Custom.Business;
using System;
using System.Collections.Generic;
using System.Text;

namespace Magicodes.Admin.TenantReleaseReview.Dto
{
    public class TenantInfoDto : AuditedEntityDto<int>
    {
        public TenantInfo tenant { get; set; }
        /// <summary>
        /// 制单人
        /// </summary>
        public string CreateName { get; set; }
        /// <summary>
        /// 公司
        /// </summary>
        public string Company { get; set; }
        /// <summary>
        /// 手机
        /// </summary>
        public string PhoneNumber { get; set; }
        /// <summary>
        /// 电话
        /// </summary>
        public string TelNumber { get; set; }
    }
}
