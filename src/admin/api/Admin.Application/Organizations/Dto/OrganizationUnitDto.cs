using Abp.Application.Services.Dto;

namespace Magicodes.Admin.Organizations.Dto
{
    public class OrganizationUnitDto : AuditedEntityDto<long>
    {
        /// <summary>
        /// ����code
        /// </summary>
        public string ParentCode { get; set; }
        /// <summary>
        /// codeֵ
        /// </summary>
        public string Code { get; set; }
        /// <summary>
        /// ��˾����
        /// </summary>
        public string DisplayName { get; set; }
        /// <summary>
        /// ��˾���
        /// </summary>
        public string ShortName { get; set; }
    }
}