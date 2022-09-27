using Abp.Application.Services.Dto;
using Abp.Authorization;
using Abp.Authorization.Users;
using Abp.AutoMapper;
using Abp.Dapper.Repositories;
using Abp.Domain.Repositories;
using Abp.Extensions;
using Abp.Linq.Extensions;
using Abp.UI;
using Magicodes.Admin.Core.Custom.Basis;
using Magicodes.Admin.SiteTab.Dto;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Threading.Tasks;
namespace Magicodes.Admin.SiteTab
{
    public class SiteTablesAppService : AdminAppServiceBase
    {
        private readonly IRepository<SiteTable, int> _siteTablesRepository;
        private readonly IDapperRepository<SiteTable, int> _siteTablesDapperRepository;

        public SiteTablesAppService(IRepository<SiteTable, int> siteTablesRepository, IDapperRepository<SiteTable, int> siteTablesDapperRepository)
        {
            _siteTablesRepository = siteTablesRepository;
            _siteTablesDapperRepository = siteTablesDapperRepository;
        }

        /// <summary>
        /// 查询站点信息列表
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<PagedResultDto<SiteTablesListDto>> GetAllSiteTablesInfo(GetSiteTablesInput input)
        {
            var query = from site in _siteTablesRepository.GetAll()
                        .WhereIf(!input.Code.IsNullOrEmpty(), s => s.Code == input.Code)
                        .WhereIf(!input.Filter.IsNullOrWhiteSpace(),
                           s => s.SiteName.Contains(input.Filter) || s.ENSiteName.Contains(input.Filter))
						select new SiteTablesListDto
                        {
                            Id = site.Id,
                            Code = site.Code,
                            SiteName = site.SiteName,
							ENSiteName = site.ENSiteName,
							CountryCode = site.CountryCode,
							IsEnable = site.IsEnable,
							Remarks = site.Remarks,
                            CreatorUserId = site.CreatorUserId,
                            CreationTime = site.CreationTime
                        };
            var totalCount = await query.CountAsync();
            var items = await query.OrderBy(input.Sorting).PageBy(input).ToListAsync();

            return new PagedResultDto<SiteTablesListDto>(
                totalCount,
                items);
        }
		/// <summary>
		/// 新增或修改国家
		/// </summary>
		/// <param name="input"></param>
		/// <returns></returns>
		public async Task CreateOrUpdateSiteTables(SiteTablesInput input)
		{
			if (!input.Id.HasValue)
			{
				await CreateSiteTablesAsync(input);
			}
			else
			{
				await UpdateSiteTablesAsync(input);
			}
		}
		/// <summary>
		/// Create 
		/// </summary>
		/// <param name="input">query parameter</param>
		/// <returns></returns>
		protected virtual async Task CreateSiteTablesAsync(SiteTablesInput input)
		{
			//判断Code是否重复
			if (_siteTablesRepository.GetAll().Any(p => p.Code == input.Code))
			{
				throw new UserFriendlyException(3000, "站点代码已经存在！");
			}
			SiteTable st = new SiteTable();
			st.Code = input.Code;
			st.SiteName = input.SiteName;
			st.ENSiteName = input.ENSiteName;
			st.CountryCode = input.CountryCode;
			st.IsEnable = input.IsEnable;
			st.Remarks = input.Remarks;
			st.CreatorUserId = AbpSession.UserId;
			st.CreationTime = DateTime.Now;
			st.TenantId = AbpSession.TenantId;
			await _siteTablesRepository.InsertAsync(st);
		}

		/// <summary>
		/// modify 
		/// </summary>
		/// <param name="input">query parameter</param>
		/// <returns></returns>
		protected virtual async Task UpdateSiteTablesAsync(SiteTablesInput input)
		{
			Debug.Assert(input.Id != null, "必须设置input.Id的值");

			var st = await _siteTablesRepository.GetAsync(input.Id.Value);
			//判断Code是否重复
			if (_siteTablesRepository.GetAll().Any(p => p.Code == input.Code && input.Code!=st.Code))
			{
				throw new UserFriendlyException(3000, "站点代码已经存在！");
			}
			st.Code = input.Code;
			st.SiteName = input.SiteName;
			st.ENSiteName = input.ENSiteName;
			st.CountryCode = input.CountryCode;
			st.IsEnable = input.IsEnable;
			st.Remarks = input.Remarks;
			st.LastModifierUserId = AbpSession.UserId;
			st.LastModificationTime = DateTime.Now;
		}
		/// <summary>
		/// 删除国家
		/// </summary>
		/// <param name="ids"></param>
		/// <returns></returns>
		public async Task BatchDeleteSiteTables(List<int> ids)
		{
			foreach (var id in ids)
			{
				if (id != null)
				{
					var siteTable = await _siteTablesRepository.GetAsync(id);
					siteTable.IsDeleted = true;
					siteTable.DeleterUserId = AbpSession.UserId;
					siteTable.DeletionTime = DateTime.Now;
				}
			}
		}
		/// <summary>
		/// 获取单个信息
		/// </summary>
		/// <param name="id"></param>
		/// <returns></returns>
		public async Task<SiteTablesInput> GetOneSiteTable(int? id)
		{
			SiteTablesInput st;
			if (id != null)
			{
				var info = await _siteTablesRepository.GetAsync(id.Value);
				st = info.MapTo<SiteTablesInput>();
			}
			else
			{
				st = new SiteTablesInput();

			}

			return st;
		}
	}
}
