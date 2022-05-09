using Abp;
using Abp.Application.Features;
using Abp.Application.Services.Dto;
using Abp.Authorization;
using Abp.Authorization.Users;
using Abp.Extensions;
using Abp.Linq.Extensions;
using Abp.Runtime.Security;
using Magicodes.Admin.Authorization;
using Magicodes.Admin.Dto;
using Magicodes.Admin.Editions.Dto;
using Magicodes.Admin.MultiTenancy.Dto;
using Magicodes.Admin.Url;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Threading.Tasks;

namespace Magicodes.Admin.MultiTenancy
{
	[AbpAllowAnonymous]
	public class TenantAppService : AdminAppServiceBase, ITenantAppService
	{
		public IAppUrlService AppUrlService { get; set; }

		public TenantAppService()
		{
			AppUrlService = NullAppUrlService.Instance;
		}

		/// <summary>
		/// 查询租户
		/// </summary>
		/// <param name="input"></param>
		/// <returns></returns>
		public async Task<PagedResultDto<TenantListDto>> GetTenants(GetTenantsInput input)
		{
			var query = TenantManager.Tenants
				.Include(t => t.Edition)
				.WhereIf(!input.Filter.IsNullOrWhiteSpace(), t => t.Name.Contains(input.Filter) || t.TenancyName.Contains(input.Filter));

			var tenantCount = await query.CountAsync();
			var tenants = await query.OrderBy(input.Sorting).PageBy(input).ToListAsync();

			List<TenantListDto> list = ObjectMapper.Map<List<TenantListDto>>(tenants);
			foreach (var item in list)
			{
				item.ConnectionString = SimpleStringCipher.Instance.Decrypt(item.ConnectionString);
			}

			return new PagedResultDto<TenantListDto>(
				tenantCount,
				list
				);
		}

		/// <summary>
		/// 租户创建
		/// </summary>
		/// <param name="input"></param>
		/// <returns></returns>
		//[AbpAuthorize(AppPermissions.Pages_Tenants_Create)]
		//[UnitOfWork(IsDisabled = true)]
		public async Task CreateTenant(CreateTenantInput input)
		{
			await TenantManager.CreateWithAdminUserAsync(input.TenancyName,
				input.Name,
				input.AdminPassword,
				input.AdminEmailAddress,
				input.ConnectionString,
				input.IsActive,
				input.EditionId,
				input.ShouldChangePasswordOnNextLogin,
				input.SendActivationEmail,
				input.SubscriptionEndDateUtc?.ToUniversalTime(),
				input.IsInTrialPeriod,
				AppUrlService.CreateEmailActivationUrlFormat(input.TenancyName)
			);
		}

		[AbpAuthorize(AppPermissions.Pages_Tenants_Edit)]
		public async Task<TenantEditDto> GetTenantForEdit(EntityDto input)
		{
			var tenantEditDto = ObjectMapper.Map<TenantEditDto>(await TenantManager.GetByIdAsync(input.Id));
			tenantEditDto.ConnectionString = SimpleStringCipher.Instance.Decrypt(tenantEditDto.ConnectionString);
			return tenantEditDto;
		}

		//[AbpAuthorize(AppPermissions.Pages_Tenants_Edit)]
		public async Task UpdateTenant(TenantEditDto input)
		{
			await TenantManager.CheckEditionAsync(input.EditionId, input.IsInTrialPeriod);

			input.ConnectionString = SimpleStringCipher.Instance.Encrypt(input.ConnectionString);
			var tenant = await TenantManager.GetByIdAsync(input.Id);
			ObjectMapper.Map(input, tenant);
			tenant.SubscriptionEndDateUtc = tenant.SubscriptionEndDateUtc?.ToUniversalTime();

			await TenantManager.UpdateAsync(tenant);
		}

		//[AbpAuthorize(AppPermissions.Pages_Tenants_Delete)]
		public async Task DeleteTenant(EntityDto input)
		{
			var tenant = await TenantManager.GetByIdAsync(input.Id);
			await TenantManager.DeleteAsync(tenant);
		}

		[AbpAuthorize(AppPermissions.Pages_Tenants_ChangeFeatures)]
		public async Task<GetTenantFeaturesEditOutput> GetTenantFeaturesForEdit(EntityDto input)
		{
			var features = FeatureManager.GetAll()
				.Where(f => f.Scope.HasFlag(FeatureScopes.Tenant));
			var featureValues = await TenantManager.GetFeatureValuesAsync(input.Id);

			return new GetTenantFeaturesEditOutput
			{
				Features = ObjectMapper.Map<List<FlatFeatureDto>>(features).OrderBy(f => f.DisplayName).ToList(),
				FeatureValues = featureValues.Select(fv => new NameValueDto(fv)).ToList()
			};
		}

		[AbpAuthorize(AppPermissions.Pages_Tenants_ChangeFeatures)]
		public async Task UpdateTenantFeatures(UpdateTenantFeaturesInput input)
		{
			await TenantManager.SetFeatureValuesAsync(input.Id, input.FeatureValues.Select(fv => new NameValue(fv.Name, fv.Value)).ToArray());
		}

		[AbpAuthorize(AppPermissions.Pages_Tenants_ChangeFeatures)]
		public async Task ResetTenantSpecificFeatures(EntityDto input)
		{
			await TenantManager.ResetAllFeaturesAsync(input.Id);
		}

		public async Task UnlockTenantAdmin(EntityDto input)
		{
			using (CurrentUnitOfWork.SetTenantId(input.Id))
			{
				var tenantAdmin = await UserManager.FindByNameAsync(AbpUserBase.AdminUserName);
				if (tenantAdmin != null)
				{
					tenantAdmin.Unlock();
				}
			}
		}

		/// <summary>
		/// IsActive开关服务
		/// </summary>
		/// <param name="input">开关输入参数</param>
		/// <returns></returns>
		[AbpAuthorize(AppPermissions.Pages_Tenants_Edit)]
		public async Task UpdateIsActiveSwitchAsync(SwitchEntityInputDto input)
		{
			var tenant = await TenantManager.GetByIdAsync(input.Id);
			tenant.IsActive = input.SwitchValue;
		}

		/// <summary>
		/// 批量删除
		/// </summary>
		/// <param name="input">要删除的集合</param>
		/// <returns></returns>
		[AbpAuthorize(AppPermissions.Pages_Tenants_BatchDelete)]
		public async Task BatchDelete(List<EntityDto> input)
		{
			foreach (var entity in input)
			{
				var tenant = await TenantManager.GetByIdAsync(entity.Id);
				await TenantManager.DeleteAsync(tenant);
			}

		}

		/// <summary>
		/// 获取租户列表
		/// </summary>
		/// <returns></returns>
		[AbpAllowAnonymous]
		public async Task<List<GetDataComboItemDto<string>>> getTenantsComboItemDtoList()
		{
			var list = await TenantManager.Tenants
		   .OrderByDescending(p => p.Id)
		   .Select(p => new { p.Id, p.TenancyName }).ToListAsync();
			return list.Select(p => new GetDataComboItemDto<string>()
			{
				DisplayName = p.TenancyName,
				Value = p.Id.ToString()
			}).ToList();
		}

	}
}