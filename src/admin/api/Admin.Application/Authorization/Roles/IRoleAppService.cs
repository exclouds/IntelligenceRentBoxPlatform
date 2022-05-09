using System.Collections.Generic;
using System.Threading.Tasks;
using Abp.Application.Services;
using Abp.Application.Services.Dto;
using Magicodes.Admin.Authorization.Roles.Dto;

namespace Magicodes.Admin.Authorization.Roles
{
    /// <summary>
    /// Application service that is used by 'role management' page.
    /// </summary>
    public interface IRoleAppService : IApplicationService
    {
        Task<PagedResultDto<RoleListDto>> GetRoles(GetRolesInput input);

        Task<GetRoleForEditOutput> GetRoleForEdit(NullableIdDto input);

        Task CreateOrUpdateRole(CreateOrUpdateRoleInput input);

        Task DeleteRole(EntityDto input);

        /// <summary>
        /// 获取所有角色列表
        /// </summary>
        /// <returns></returns>
        Task<ListResultDto<RoleListDto>> GetAllRoles();

        /// <summary>
        /// 批量删除
        /// </summary>
        /// <param name="input">要删除的集合</param>
        /// <returns></returns>
        Task BatchDelete(List<EntityDto> input);

        Task UpdateRoleNameAsync(RoleNameEditDto input);
        Task UpdateRolePermissionsAsync(GrantedPermissionNamesDto input);
    }
}