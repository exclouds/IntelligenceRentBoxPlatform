using System.Collections.Generic;
using System.Threading.Tasks;
using Abp.Application.Services;
using Abp.Application.Services.Dto;
using Magicodes.Admin.Organizations.Dto;

namespace Magicodes.Admin.Organizations
{
    public interface IOrganizationUnitAppService : IApplicationService
    {
        Task<ListResultDto<OrganizationUnitDto>> GetOrganizationUnits();

        Task<PagedResultDto<OrganizationUnitDepListDto>> GetOrganizationUnitUsers(GetOrganizationUnitUsersInput input);
        

        Task<OrganizationUnitDto> CreateOrganizationUnit(CreateOrganizationUnitInput input);

        Task<OrganizationUnitDto> UpdateOrganizationUnit(UpdateOrganizationUnitInput input);

        Task<PagedResultDto<OrganizationUnitListDto>> GetOrganizationUnitOrg(GetOrganizationUnitUsersInput input);

        //Task<OrganizationUnitDto> MoveOrganizationUnit(MoveOrganizationUnitInput input);

        Task DeleteOrganizationUnit(EntityDto<long> input);

        //Task RemoveUserFromOrganizationUnit(UserToOrganizationUnitInput input);

        //Task AddUsersToOrganizationUnit(UsersToOrganizationUnitInput input);

        //Task<PagedResultDto<NameValueDto>> FindUsers(FindOrganizationUnitUsersInput input);

        /// <summary>
        ///     批量从组织中移除用户
        /// </summary>
        /// <param name="userIds">用户Id列表</param>
        /// <param name="organizationUnitId">组织机构Id</param>
        /// <returns></returns>
        //Task BatchRemoveUserFromOrganizationUnit(List<long> userIds, long organizationUnitId);

        Task<List<NewOrganizationUnitDto>> GetNewOrganizationUnits();


        //Task<PagedResultDto<FindUsersDto>> FindAllUsers(NewFindOrganizationUnitUsersInput input);
        //OrganizationUnitForEditDto GetOrganizationUnitForEdit(NullableIdDto<long> input);

        Task<CreateDept> CreateDept(CreateDept input);
        Task UpdateDept(UpdateDept input);

        Task DeleteDept(EntityDto<int> input);
    }
}
