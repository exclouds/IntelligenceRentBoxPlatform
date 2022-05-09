using System.Collections.Generic;
using System.Threading.Tasks;
using Abp.Application.Services;
using Abp.Application.Services.Dto;
using Magicodes.Admin.Authorization.Users.Dto;
using Magicodes.Admin.Dto;

namespace Magicodes.Admin.Authorization.Users
{
	public interface IUserAppService : IApplicationService
	{
		Task<PagedResultDto<UserListDto>> GetUsers(GetUsersInput input);

		Task<FileDto> GetUsersToExcel();

		Task<GetUserForEditOutput> GetUserForEdit(NullableIdDto<long> input);

		Task<GetUserPermissionsForEditOutput> GetUserPermissionsForEdit(EntityDto<long> input);

		Task ResetUserSpecificPermissions(EntityDto<long> input);

		Task UpdateUserPermissions(UpdateUserPermissionsInput input);

		Task CreateOrUpdateUser(CreateOrUpdateUserInput input);

		Task DeleteUser(EntityDto<long> input);

		Task UnlockUser(EntityDto<long> input);

		/// <summary>
		/// IsActive开关服务
		/// </summary>
		/// <param name="input">开关输入参数</param>
		/// <returns></returns>
		Task UpdateIsActiveSwitchAsync(SwitchEntityInputDto input);

		/// <summary>
		/// IsActive开关服务
		/// </summary>
		/// <param name="input">开关输入参数</param>
		/// <returns></returns>
		Task UpdateIsEmailConfirmedSwitchAsync(SwitchEntityInputDto input);

		/// <summary>
		/// 批量删除
		/// </summary>
		/// <param name="input">要删除的集合</param>
		/// <returns></returns>
		Task BatchDelete(List<string> input);

		/// <summary>
		/// 取所有人员列表
		/// </summary>
		/// <param name="input"></param>
		/// <returns></returns>
		Task<PagedResultDto<UserListExtionDto>> GetAllUsers(GetUsersInput input);

		/// <summary>
		/// 创建或修改用户
		/// </summary>
		/// <param name="input"></param>
		/// <returns></returns>
		Task NewCreateOrUpdateUser(NewCreateOrUpdateUserInput input);

		/// <summary>
		/// 根据Id取用户信息   
		/// </summary>
		/// <param name="input"></param>
		/// <returns></returns>
		Task<NewGetUserForEditOutput> NewGetUserForEdit(NullableIdDto<long> input);

		/// <summary>
		///锁定用户
		/// </summary>
		/// <param name="input">input parameter</param>
		/// <returns></returns>
		Task UpdateIsLockedAsync(SwitchEntityInputDto input);
        
	}
}