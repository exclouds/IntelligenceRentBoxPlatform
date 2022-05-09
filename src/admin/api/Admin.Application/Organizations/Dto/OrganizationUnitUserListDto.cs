using System;
using Abp.Application.Services.Dto;

namespace Magicodes.Admin.Organizations.Dto
{
    public class OrganizationUnitDepListDto : EntityDto<long>
    {
        public string DepCode { get; set; }

        public string DepName { get; set; }

        public string DepNCCName { get; set; }

        public string DepNCCCode { get; set; }
        

        public DateTime AddedTime { get; set; }
    }

    public class OrganizationUnitListDto : EntityDto<long>
    {
        public string DisplayName { get; set; }
        /// <summary>
        /// 组织机构代码
        /// </summary>
        public string Code { get; set; }
        /// <summary>
        /// 简称
        /// </summary>
        public string ShortName { get; set; }
        

    }
}