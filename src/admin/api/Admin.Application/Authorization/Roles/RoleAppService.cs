using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Abp.Application.Services.Dto;
using Abp.Authorization;
using Abp.Linq.Extensions;
using Abp.Extensions;
using Microsoft.EntityFrameworkCore;
using Magicodes.Admin.Authorization.Permissions;
using Magicodes.Admin.Authorization.Permissions.Dto;
using Magicodes.Admin.Authorization.Roles.Dto;
using System.Linq.Dynamic.Core;
using Abp.UI;

namespace Magicodes.Admin.Authorization.Roles
{
    /// <summary>
    /// Application service that is used by 'role management' page.
    /// </summary>
    [AbpAuthorize(AppPermissions.Pages_Administration_Roles)]
	public class RoleAppService : AdminAppServiceBase, IRoleAppService
	{
		private readonly RoleManager _roleManager;

		public RoleAppService(RoleManager roleManager)
		{
			_roleManager = roleManager;
		}

        /// <summary>
        /// 角色列表
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
		public async Task<PagedResultDto<RoleListDto>> GetRoles(GetRolesInput input)
		{
            //只显示当前登录人增加的角色信息
            var query = _roleManager.Roles
                .WhereIf(
                    !input.FilterText.IsNullOrWhiteSpace(),
                    r =>
                        r.Name.Contains(input.FilterText) ||
                        r.DisplayName.Contains(input.FilterText)
                )
                .WhereIf(
                    input.PermissionNames != null && input.PermissionNames.Count > 0,
                    r => r.Permissions.Any(rp => input.PermissionNames.Contains(rp.Name) && rp.IsGranted)
                )
                .WhereIf(AbpSession.UserId != 2, r => r.CreatorUserId == AbpSession.UserId);
                //当前管理员增加角色
                //.Where(r=>r.CreatorUserId==AbpSession.UserId );

			var count = await query.CountAsync();

			var roles = await query
				.OrderBy(input.Sorting)
				.PageBy(input)
				.ToListAsync();

			return new PagedResultDto<RoleListDto>(count, ObjectMapper.Map<List<RoleListDto>>(roles));
		}

		//[AbpAuthorize(AppPermissions.Pages_Administration_Roles_Create, AppPermissions.Pages_Administration_Roles_Edit)]
		public async Task<GetRoleForEditOutput> GetRoleForEdit(NullableIdDto input)
		{
			var permissions = PermissionManager.GetAllPermissions();
			var grantedPermissions = new Permission[0];
			RoleEditDto roleEditDto;

			if (input.Id.HasValue) //Editing existing role?
			{
				var role = await _roleManager.GetRoleByIdAsync(input.Id.Value);
				grantedPermissions = (await _roleManager.GetGrantedPermissionsAsync(role)).ToArray();
				roleEditDto = ObjectMapper.Map<RoleEditDto>(role);
			}
			else
			{
				roleEditDto = new RoleEditDto();
			}

			return new GetRoleForEditOutput
			{
				Role = roleEditDto,
				Permissions = ObjectMapper.Map<List<FlatPermissionDto>>(permissions).OrderBy(p => p.DisplayName).ToList(),
				GrantedPermissionNames = grantedPermissions.Select(p => p.Name).ToList()
			};
		}
        /// <summary>
        /// 新增或修改角色
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
		public async Task CreateOrUpdateRole(CreateOrUpdateRoleInput input)
		{
			if (input.Role.Id.HasValue)
			{
				await UpdateRoleAsync(input);
			}
			else
			{
				await CreateRoleAsync(input);
			}
		}

		[AbpAuthorize(AppPermissions.Pages_Administration_Roles_Delete)]
		public async Task DeleteRole(EntityDto input)
		{
			var role = await _roleManager.GetRoleByIdAsync(input.Id);

			var users = await UserManager.GetUsersInRoleAsync(role.Name);
			if (users.Count > 0)
			{
				throw new UserFriendlyException(3000, "该角色下存在用户，无法删除");
			}
			foreach (var user in users)
			{
				CheckErrors(await UserManager.RemoveFromRoleAsync(user, role.Name));
			}

			CheckErrors(await _roleManager.DeleteAsync(role));
		}

		/// <summary>
		/// 获取所有角色列表
		/// </summary>
		/// <returns></returns>
		public async Task<ListResultDto<RoleListDto>> GetAllRoles()
		{
			var query = await _roleManager.Roles.ToListAsync();
			return new ListResultDto<RoleListDto>(ObjectMapper.Map<List<RoleListDto>>(query));
		}

		/// <summary>
		/// 批量删除
		/// </summary>
		/// <param name="input">要删除的集合</param>
		/// <returns></returns>
		public async Task BatchDelete(List<EntityDto> input)
		{
			foreach (var entity in input)
			{
				await DeleteRole(entity);
			}
		}

		//[AbpAuthorize(AppPermissions.Pages_Administration_Roles_Edit)]
		protected virtual async Task UpdateRoleAsync(CreateOrUpdateRoleInput input)
		{
			Debug.Assert(input.Role.Id != null, "input.Role.Id should be set.");

			var role = await _roleManager.GetRoleByIdAsync(input.Role.Id.Value);
			role.DisplayName = input.Role.DisplayName;
			role.IsDefault = input.Role.IsDefault;

			await UpdateGrantedPermissionsAsync(role, input.GrantedPermissionNames);
		}

		//[AbpAuthorize(AppPermissions.Pages_Administration_Roles_Create)]
		protected virtual async Task CreateRoleAsync(CreateOrUpdateRoleInput input)
		{
			var role = new Role(AbpSession.TenantId, input.Role.DisplayName) { IsDefault = input.Role.IsDefault };
			CheckErrors(await _roleManager.CreateAsync(role));
			await CurrentUnitOfWork.SaveChangesAsync(); //It's done to get Id of the role.
			await UpdateGrantedPermissionsAsync(role, input.GrantedPermissionNames);
		}

		private async Task UpdateGrantedPermissionsAsync(Role role, List<string> grantedPermissionNames)
		{
			var grantedPermissions = PermissionManager.GetPermissionsFromNamesByValidating(grantedPermissionNames);
			await _roleManager.SetGrantedPermissionsAsync(role, grantedPermissions);
		}

		#region 新前端方法

		/// <summary>
		/// 修改角色名称
		/// </summary>
		/// <param name="input"></param>
		/// <returns></returns>
		//[AbpAuthorize(AppPermissions.Pages_Administration_Roles_Edit)]
		public async Task UpdateRoleNameAsync(RoleNameEditDto input)
		{
			Debug.Assert(input.Id != null, "input.Role.Id should be set.");

			var role = await _roleManager.GetRoleByIdAsync(input.Id.Value);
			//role.Name = input.DisplayName;
			role.DisplayName = input.DisplayName;
		}
		/// <summary>
		/// 修改权限
		/// </summary>
		/// <param name="input"></param>
		/// <returns></returns>
		//[AbpAuthorize(AppPermissions.Pages_Administration_Roles_Edit)]
		public async Task UpdateRolePermissionsAsync(GrantedPermissionNamesDto input)
		{
			var role = await _roleManager.GetRoleByIdAsync(input.Id);
			await UpdateGrantedPermissionsAsync(role, input.GrantedPermissionNames);
		}
		/// <summary>
		/// 根据角色id获取权限
		/// </summary>
		/// <param name="input"></param>
		/// <returns></returns>
		public async Task<GetRoleForEditOutput> GetPermissionsbyRoleId(NullableIdDto input)
		{
			var grantedPermissions = new Permission[0];
			RoleEditDto roleEditDto;

			if (input.Id.HasValue) //Editing existing role?
			{
				var role = await _roleManager.GetRoleByIdAsync(input.Id.Value);
				grantedPermissions = (await _roleManager.GetGrantedPermissionsAsync(role)).ToArray();
				roleEditDto = ObjectMapper.Map<RoleEditDto>(role);
			}
			else
			{
				roleEditDto = new RoleEditDto();
			}

			return new GetRoleForEditOutput
			{
				//Role = roleEditDto,
				GrantedPermissionNames = grantedPermissions.Select(p => p.Name).ToList()
			};
		}
		#endregion

	}
}
