using Abp.Dapper.Repositories;
using Abp.Domain.Repositories;
using Magicodes.Admin.Core.Custom.Basis;
using System;
using System.Collections.Generic;
using Abp.Linq.Extensions;
using Abp.UI;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Threading.Tasks;
using Abp.Application.Services.Dto;
using Abp.Extensions;
using Abp.AutoMapper;
using Magicodes.Admin.LinSiteData.Dto;
using Abp.Collections.Extensions;

namespace Magicodes.Admin.LinSiteData
{
	/// <summary>
	/// 航线站点
	/// </summary>
	public class LinSiteAppService : AdminAppServiceBase
	{
		private readonly IRepository<LinSite, int> _linSiteRepository;
		private readonly IDapperRepository<LinSite, int> _linSiteDapperRepository;
		private readonly IRepository<Line, int> _lineRepository;
		private readonly IRepository<SiteTable, int> _siteTablesRepository;

		public LinSiteAppService(IRepository<LinSite, int> linSiteRepository, IDapperRepository<LinSite, int> linSiteDapperRepository, IRepository<Line, int> lineRepository, IRepository<SiteTable, int> siteTablesRepository)
		{
			_linSiteRepository = linSiteRepository;
			_linSiteDapperRepository = linSiteDapperRepository;
			_lineRepository = lineRepository;
			_siteTablesRepository = siteTablesRepository;
		}

		/// <summary>
		/// 查询航线站点信息列表
		/// </summary>
		/// <param name="input"></param>
		/// <returns></returns>
		[HttpPost]
		public async Task<PagedResultDto<LinSiteListDto>> GetAllLinSiteInfo(GetLinSiteInput input)
		{
			var query = from linSite in _linSiteRepository.GetAll()
						.WhereIf(!input.Code.IsNullOrEmpty(), s => s.Code == input.Code)
                        join line in _lineRepository.GetAll() on linSite.LineId equals line.Id.ToString() into a
						from l in a.DefaultIfEmpty()
						join site in _siteTablesRepository.GetAll() on linSite.Code equals site.Code into b
						from s in b.DefaultIfEmpty()
						select new LinSiteListDto
						{
							Id = linSite.Id,
							Code = s.SiteName,
							LineId = l.LineName,
							Remarks = linSite.Remarks,
							CreatorUserId = linSite.CreatorUserId,
							CreationTime = linSite.CreationTime
						};
			var totalCount = await query.CountAsync();
			var items = await query.OrderBy(input.Sorting).PageBy(input).ToListAsync();

			return new PagedResultDto<LinSiteListDto>(
				totalCount,
				items);
		}
		/// <summary>
		/// 新增或修改航线站点
		/// </summary>
		/// <param name="input"></param>
		/// <returns></returns>
		public async Task CreateOrUpdateLinSite(LinSiteInput input)
		{
			if (!input.Id.HasValue)
			{
				await CreateLinSiteAsync(input);
			}
			else
			{
				await UpdateLinSiteAsync(input);
			}
		}
		/// <summary>
		/// Create 
		/// </summary>
		/// <param name="input">query parameter</param>
		/// <returns></returns>
		protected virtual async Task CreateLinSiteAsync(LinSiteInput input)
		{
			//判断Code是否重复
			if (_linSiteRepository.GetAll().Any(p => p.Code == input.Code && p.LineId == input.LineId))
			{
				throw new UserFriendlyException(3000, "航线站点关联已经存在！");
			}
            
				LinSite linSite = new LinSite();
				linSite.Code = input.Code;
				linSite.LineId = input.LineId;
				linSite.Remarks = input.Remarks;
				linSite.CreatorUserId = AbpSession.UserId;
				linSite.CreationTime = DateTime.Now;
				linSite.TenantId = AbpSession.TenantId;
				await _linSiteRepository.InsertAsync(linSite);
		}

		/// <summary>
		/// modify 
		/// </summary>
		/// <param name="input">query parameter</param>
		/// <returns></returns>
		protected virtual async Task UpdateLinSiteAsync(LinSiteInput input)
		{
			Debug.Assert(input.Id != null, "必须设置input.Id的值");

			var linSite = await _linSiteRepository.GetAsync(input.Id.Value);
			if (input.Code != linSite.Code && input.LineId != linSite.LineId)
			{
				//判断是否重复
				if (_linSiteRepository.GetAll().Any(p => p.Code == input.Code && p.LineId == input.LineId))
				{
					throw new UserFriendlyException(3000, "航线站点关联已经存在！");
				}
			}
			linSite.Code = input.Code;
			linSite.LineId = input.LineId;
			linSite.Remarks = input.Remarks;
			linSite.LastModifierUserId = AbpSession.UserId;
			linSite.LastModificationTime = DateTime.Now;
		}
		/// <summary>
		/// 删除航线站点
		/// </summary>
		/// <param name="ids"></param>
		/// <returns></returns>
		public async Task BatchDeleteLinSite(List<int> ids)
		{
			foreach (var id in ids)
			{
				if (id != null)
				{
					var linSite = await _linSiteRepository.GetAsync(id);
					linSite.IsDeleted = true;
					linSite.DeleterUserId = AbpSession.UserId;
					linSite.DeletionTime = DateTime.Now;
				}
			}
		}
		/// <summary>
		/// 获取单个信息
		/// </summary>
		/// <param name="id"></param>
		/// <returns></returns>
		public async Task<LinSiteInput> GetOneLinSite(int? id)
		{
			LinSiteInput linSite;
			if (id != null)
			{
				var info = await _linSiteRepository.GetAsync(id.Value);
				linSite = info.MapTo<LinSiteInput>();
			}
			else
			{
				linSite = new LinSiteInput();

			}
			return linSite;
		}
	}
}
