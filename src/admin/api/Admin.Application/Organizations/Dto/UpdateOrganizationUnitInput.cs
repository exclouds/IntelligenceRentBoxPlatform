using System.ComponentModel.DataAnnotations;
using Abp.Organizations;

namespace Magicodes.Admin.Organizations.Dto
{
    public class UpdateOrganizationUnitInput
    {
        [Range(1, long.MaxValue)]
        public long Id { get; set; }
        /// <summary>
        /// ��֯��������
        /// </summary>
        [Required]
        public string Code { get; set; }
        /// <summary>
        /// �ϼ���˾����
        /// </summary>
        public string ParentCode { get; set; }
        /// <summary>
        /// ��˾����
        /// </summary>
        [Required]
        [StringLength(OrganizationUnit.MaxDisplayNameLength)]
        public string DisplayName { get; set; }
        
        /// <summary>
        /// ��˾���
        /// </summary>
        public string ShortName { get; set; }
        
        
    }
}