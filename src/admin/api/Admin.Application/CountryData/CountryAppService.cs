using Abp.Dapper.Repositories;
using Abp.Domain.Repositories;
using Magicodes.Admin.Core.Custom.Basis;
using System;
using System.Collections.Generic;
using Abp.Linq.Extensions;
using Abp.UI;
using Magicodes.Admin.SiteTab.Dto;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Threading.Tasks;
using Magicodes.Admin.CountryData.Dto;
using Abp.Application.Services.Dto;
using Abp.Extensions;
using Abp.AutoMapper;

namespace Magicodes.Admin.CountryData
{
	public class CountryAppService : AdminAppServiceBase
	{
		private readonly IRepository<Country, int> _countryRepository;
		private readonly IDapperRepository<Country, int> _countryDapperRepository;

		public CountryAppService(IRepository<Country, int> countryRepository, IDapperRepository<Country, int> countryDapperRepository)
		{
			_countryRepository = countryRepository;
			_countryDapperRepository = countryDapperRepository;
		}

		/// <summary>
		/// 查询国家信息列表
		/// </summary>
		/// <param name="input"></param>
		/// <returns></returns>
		[HttpPost]
		public async Task<PagedResultDto<CountryListDto>> GetAllCountryInfo(GetCountryInput input)
		{
			var query = from country in _countryRepository.GetAll()
						.WhereIf(!input.Code.IsNullOrEmpty(), s => s.Code == input.Code)
						.WhereIf(!input.Filter.IsNullOrWhiteSpace(),
						   s => s.Name.Contains(input.Filter))
						select new CountryListDto
						{
							Id = country.Id,
							Code = country.Code,
							Name = country.Name,
							Remarks = country.Remarks,
							IsEnable = country.IsEnable,
							CreatorUserId = country.CreatorUserId,
							CreationTime = country.CreationTime
						};
			var totalCount = await query.CountAsync();
			var items = await query.OrderBy(input.Sorting).PageBy(input).ToListAsync();

			return new PagedResultDto<CountryListDto>(
				totalCount,
				items);
		}
		/// <summary>
		/// 新增或修改国家
		/// </summary>
		/// <param name="input"></param>
		/// <returns></returns>
		public async Task CreateOrUpdateCountry(CountryInput input)
		{
			if (!input.Id.HasValue)
			{
				await CreateCountryAsync(input);
			}
			else
			{
				await UpdateCountryAsync(input);
			}
		}
		/// <summary>
		/// Create 
		/// </summary>
		/// <param name="input">query parameter</param>
		/// <returns></returns>
		protected virtual async Task CreateCountryAsync(CountryInput input)
		{
			//判断Code是否重复
			if (_countryRepository.GetAll().Any(p => p.Code == input.Code))
			{
				throw new UserFriendlyException(3000, "国家代码已经存在！");
			}
			Country country = new Country();
			country.Code = input.Code;
			country.Name = input.Name;
			country.IsEnable = input.IsEnable;
			country.Remarks = input.Remarks;
			country.CreatorUserId = AbpSession.UserId;
			country.CreationTime = DateTime.Now;
			country.TenantId = AbpSession.TenantId;
			await _countryRepository.InsertAsync(country);
		}

		/// <summary>
		/// modify 
		/// </summary>
		/// <param name="input">query parameter</param>
		/// <returns></returns>
		protected virtual async Task UpdateCountryAsync(CountryInput input)
		{
			Debug.Assert(input.Id != null, "必须设置input.Id的值");

			var country = await _countryRepository.GetAsync(input.Id.Value);
			//判断Code是否重复
			if (_countryRepository.GetAll().Any(p => p.Code == input.Code && input.Code!= country.Code))
			{
				throw new UserFriendlyException(3000, "国家代码已经存在！");
			}
			country.Code = input.Code;
			country.Name = input.Name;
			country.IsEnable = input.IsEnable;
			country.Remarks = input.Remarks;
			country.LastModifierUserId = AbpSession.UserId;
			country.LastModificationTime = DateTime.Now;
		}
		/// <summary>
		/// 删除国家
		/// </summary>
		/// <param name="ids"></param>
		/// <returns></returns>
		public async Task BatchDeleteCountry(List<int> ids)
		{
			foreach (var id in ids)
			{
				if (id != null)
				{
					var country = await _countryRepository.GetAsync(id);
					country.IsDeleted = true;
					country.DeleterUserId = AbpSession.UserId;
					country.DeletionTime = DateTime.Now;
				}
			}
		}
		/// <summary>
		/// 获取单个信息
		/// </summary>
		/// <param name="id"></param>
		/// <returns></returns>
		public async Task<CountryInput> GetOneCountry(int? id)
		{
			CountryInput country;
			if (id != null)
			{
				var info = await _countryRepository.GetAsync(id.Value);
				country = info.MapTo<CountryInput>();
			}
			else
			{
				country = new CountryInput();

			}
			return country;
		}
	}
}
