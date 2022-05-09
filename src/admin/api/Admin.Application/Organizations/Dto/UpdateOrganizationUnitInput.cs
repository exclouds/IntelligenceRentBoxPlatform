using System.ComponentModel.DataAnnotations;
using Abp.Organizations;

namespace Magicodes.Admin.Organizations.Dto
{
    public class UpdateOrganizationUnitInput
    {
        [Range(1, long.MaxValue)]
        public long Id { get; set; }
        /// <summary>
        /// 组织机构代码
        /// </summary>
        [Required]
        public string Code { get; set; }
        /// <summary>
        /// 上级公司代码
        /// </summary>
        public string ParentCode { get; set; }
        /// <summary>
        /// 公司名称
        /// </summary>
        [Required]
        [StringLength(OrganizationUnit.MaxDisplayNameLength)]
        public string DisplayName { get; set; }
        
        /// <summary>
        /// 公司简称
        /// </summary>
        public string ShortName { get; set; }
        
        
    }
}