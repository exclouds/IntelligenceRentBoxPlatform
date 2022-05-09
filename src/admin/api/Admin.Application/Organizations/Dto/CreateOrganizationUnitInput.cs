using System.ComponentModel.DataAnnotations;
using Abp.Organizations;

namespace Magicodes.Admin.Organizations.Dto
{
    public class CreateOrganizationUnitInput
    {
        /// <summary>
        /// 父节点
        /// </summary>
        public string ParentCode { get; set; }
        
        /// <summary>
        /// 组织机构名称
        /// </summary>
        [Required]
        [StringLength(OrganizationUnit.MaxDisplayNameLength)]
        public string DisplayName { get; set; } 
        /// <summary>
        /// 组织机构代码
        /// </summary>
        public string Code { get; set; }
        /// <summary>
        /// 公司简称
        /// </summary>
        public string ShortName { get; set; }
        
    }

    public class CreateDept
    {
        /// <summary>
        /// 公司编码
        /// </summary>
        public string Code { get; set; }

        /// <summary>
        /// 经营报表部门编码
        /// </summary>
        public string DepCode { get; set; }

        /// <summary>
        /// 部门归属
        /// </summary>
        public string DepName { get; set; }
        
        
        
    }
}