using Abp.Application.Services.Dto;

namespace Magicodes.Admin.Organizations.Dto
{
    public class OrganizationUnitDto : AuditedEntityDto<long>
    {
        /// <summary>
        /// 父级code
        /// </summary>
        public string ParentCode { get; set; }
        /// <summary>
        /// code值
        /// </summary>
        public string Code { get; set; }
        /// <summary>
        /// 公司名称
        /// </summary>
        public string DisplayName { get; set; }
        /// <summary>
        /// 公司简称
        /// </summary>
        public string ShortName { get; set; }
    }
}