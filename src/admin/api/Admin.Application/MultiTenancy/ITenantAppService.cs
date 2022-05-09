using System.Collections.Generic;
using System.Threading.Tasks;
using Abp.Application.Services;
using Abp.Application.Services.Dto;
using Magicodes.Admin.Dto;
using Magicodes.Admin.MultiTenancy.Dto;

namespace Magicodes.Admin.MultiTenancy
{
    public interface ITenantAppService : IApplicationService
    {
        Task<PagedResultDto<TenantListDto>> GetTenants(GetTenantsInput input);

        Task CreateTenant(CreateTenantInput input);

        Task<TenantEditDto> GetTenantForEdit(EntityDto input);

        Task UpdateTenant(TenantEditDto input);

        Task DeleteTenant(EntityDto input);

        Task<GetTenantFeaturesEditOutput> GetTenantFeaturesForEdit(EntityDto input);

        Task UpdateTenantFeatures(UpdateTenantFeaturesInput input);

        Task ResetTenantSpecificFeatures(EntityDto input);

        Task UnlockTenantAdmin(EntityDto input);

        /// <summary>
        /// IsActive开关服务
        /// </summary>
        /// <param name="input">开关输入参数</param>
        /// <returns></returns>
        Task UpdateIsActiveSwitchAsync(SwitchEntityInputDto input);

        /// <summary>
        /// 批量删除
        /// </summary>
        /// <param name="input">要删除的集合</param>
        /// <returns></returns>
        Task BatchDelete(List<EntityDto> input);
    }
}
