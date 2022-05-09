using System.Collections.Generic;
using System.Threading.Tasks;
using Abp.Application.Services;
using Magicodes.Admin.Authorization.Roles.Dto;
using Magicodes.Admin.Common.Dto;
using Magicodes.Admin.Organizations.Dto;

namespace Magicodes.Admin.Common
{
    /// <summary>
    /// 通用服务
    /// </summary>
    public interface ICommonAppService : IApplicationService
    {
        /// <summary>
        /// 获取枚举值列表
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        List<GetEnumValuesListDto> GetEnumValuesList(GetEnumValuesListInput input);

        /// <summary>
        /// 获取角色下拉
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        Task<List<RoleListDto>> GetRolesList();
        /// <summary>
        /// 获取组织机构，树形显示,不统计人数
        /// </summary>
        /// <returns></returns>
        Task<List<NewOrganizationUnitDto>> GetNewOrganizationUnitNoUsers();
    }
}
