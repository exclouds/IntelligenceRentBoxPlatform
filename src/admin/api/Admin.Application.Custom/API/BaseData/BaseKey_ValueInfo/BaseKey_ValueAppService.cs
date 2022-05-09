using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Threading.Tasks;
using Abp.UI;
using Abp.Linq.Extensions;
using Abp.Domain.Repositories;
using Abp.Application.Services.Dto;
using Microsoft.EntityFrameworkCore;
using Abp.Authorization;
using Abp.AutoMapper;
using Abp.Runtime.Session;
using Abp.Domain.Uow;
using Magicodes.Admin;
using Magicodes.Admin.Core.Custom.Authorization;
using Abp.Extensions;
using System;
using Magicodes.Admin.Core.Custom.DataDictionary;
using Microsoft.AspNetCore.Mvc;
using Admin.Application.Custom.API.BaseData.BaseKey_ValueInfo.Dto;

namespace Admin.Application.Custom.API.BaseData.BaseKey_ValueInfo
{
    /// <summary>
    /// 数据字典
    /// </summary>
    //[AbpAuthorize(AppCustomPermissions.Customs_Dictionary)]
    [AbpAllowAnonymous()]
    public partial class BaseKey_ValueAppService : AppServiceBase, IBaseKey_ValueAppService
    {
        //字典
        private readonly IRepository<BaseKey_Value, long> _baseKey_ValueRepository;
        //工作单元
        readonly IUnitOfWorkManager _unitOfWorkManager;

        public BaseKey_ValueAppService(
           IRepository<BaseKey_Value, long> baseKey_ValueRepository,
           IUnitOfWorkManager unitOfWorkManager
           ) : base()
        {
            _baseKey_ValueRepository = baseKey_ValueRepository;
            _unitOfWorkManager = unitOfWorkManager;
        }
        /// <summary>
        /// 根据字典类型获取字典值，缓存值
        /// </summary>
        /// <param name="TypeCode"></param>
        /// <returns></returns>
        //public async Task<List<BaseKey_ValueDto>> GetValueByKyeType(string TypeCode)
        //{
        //	var results = _cacheManager.GetCache("common").Get("kv", () => _baseKey_ValueRepository.GetAllIncluding().ToList()) as List<BaseKey_Value>;

        //	return results.Where(x => x.BaseKey_ValueTypeCode == TypeCode).MapTo<List<BaseKey_ValueDto>>();
        //}
        /// <summary>
        /// 根据查询条件查询列表
        /// </summary>
        /// <param name="input">查询条件，支持代码查询，名称查询，所属分类查询</param>
        /// <returns></returns>
        [AbpAllowAnonymous]
        public async Task<PagedResultDto<BaseKey_ValueDto>> GetBaseKey_Value(GetBaseKey_ValuesInputDto input)
        {
            async Task<PagedResultDto<BaseKey_ValueDto>> GetListFunc()
            {
                var query = GetQuery(input);
                var resultCount = await query.CountAsync();
                var results = await query
                    .PageBy(input)
                    .ToListAsync();
                return new PagedResultDto<BaseKey_ValueDto>(resultCount, results.MapTo<List<BaseKey_ValueDto>>());
            }
            return await GetListFunc();
        }
        /// <summary>
        /// 根据查询条件组织查询(查询列表，带分页)
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        IQueryable<BaseKey_Value> GetQuery(GetBaseKey_ValuesInputDto input)
        {
            var query = _baseKey_ValueRepository.GetAllIncluding();

            query = query.WhereIf(!input.Id.IsNullOrEmpty(), x => x.Id == Convert.ToInt32(input.Id))//按Id查询
                .WhereIf(!input.Code.IsNullOrEmpty(), x => x.Code == input.Code)//按代码查
                .WhereIf(!input.Name.IsNullOrEmpty(), x => x.Name.Contains(input.Name))//按名称查
                .WhereIf(!input.TypeCode.IsNullOrWhiteSpace(), x => x.BaseKey_ValueTypeCode == input.TypeCode)//按所属分类查
                .OrderBy(x => x.CreationTime);
            return query;

        }

        /// <summary>
        /// 编辑或新增
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        [AbpAuthorize(AppCustomPermissions.Customs_DictionaryType_Create, AppCustomPermissions.Customs_DictionaryType_Edit)]
        public async Task CreateOrUpdateBaseKey_Value(CreateOrUpdateBaseKey_ValueDto input)
        {
            if (input.BaseKey_Value.Id.IsNullOrEmpty())
            {
                await CreateBaseKey_Value(input);
            }
            else
            {
                await UpdateBaseKey_Value(input);
            }
        }
        /// <summary>
        /// 新增
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        protected virtual async Task CreateBaseKey_Value(CreateOrUpdateBaseKey_ValueDto input)
        {
            if (_baseKey_ValueRepository.GetAll().Any(p => p.BaseKey_ValueTypeCode == input.BaseKey_Value.BaseKey_ValueTypeCode && p.Code == input.BaseKey_Value.Code))
            {
                throw new UserFriendlyException("代码已存在");
            }
            if (_baseKey_ValueRepository.GetAll().Any(p => p.BaseKey_ValueTypeCode == input.BaseKey_Value.BaseKey_ValueTypeCode && p.Name == input.BaseKey_Value.Name))
            {
                throw new UserFriendlyException("名称已存在");
            }

            var baseKey_Value = new BaseKey_Value()
            {
                BaseKey_ValueTypeCode = input.BaseKey_Value.BaseKey_ValueTypeCode,
                Code = input.BaseKey_Value.Code == null ? null : input.BaseKey_Value.Code.ToUpper().Trim(),
                Name = input.BaseKey_Value.Name == null ? null : input.BaseKey_Value.Name.ToUpper(),
                Remarks = input.BaseKey_Value.Remarks == null ? null : input.BaseKey_Value.Remarks.ToUpper(),
                CreatorUserId = AbpSession.UserId,
                CreationTime = DateTime.Now,
                TenantId = AbpSession.TenantId
            };
            await _baseKey_ValueRepository.InsertAsync(baseKey_Value);

        }
        /// <summary>
        /// 编辑
        /// </summary>
        /// <param name="input">编辑项</param>
        /// <returns></returns>
        protected virtual async Task UpdateBaseKey_Value(CreateOrUpdateBaseKey_ValueDto input)
        {
            var baseKey_Value = await _baseKey_ValueRepository.GetAsync(Convert.ToInt32(input.BaseKey_Value.Id));

            if (input.BaseKey_Value.Code != baseKey_Value.Code)
            {
                if (_baseKey_ValueRepository.GetAllIncluding().Any(p => p.Code == input.BaseKey_Value.Code && p.BaseKey_ValueTypeCode == input.BaseKey_Value.BaseKey_ValueTypeCode))
                {
                    throw new UserFriendlyException("代码已存在");
                }
            }
            if (input.BaseKey_Value.Name != baseKey_Value.Name)
            {
                if (_baseKey_ValueRepository.GetAllIncluding().Any(p => p.Name == input.BaseKey_Value.Name && p.BaseKey_ValueTypeCode == input.BaseKey_Value.BaseKey_ValueTypeCode))
                {
                    throw new UserFriendlyException("名称已存在");
                }
            }
            baseKey_Value.LastModificationTime = DateTime.Now;
            baseKey_Value.LastModifierUserId = AbpSession.GetUserId();
            baseKey_Value.BaseKey_ValueTypeCode = input.BaseKey_Value == null ? null : input.BaseKey_Value.BaseKey_ValueTypeCode.ToUpper().Trim();
            baseKey_Value.Code = input.BaseKey_Value.Code == null ? null : input.BaseKey_Value.Code.ToUpper().Trim();
            baseKey_Value.Name = input.BaseKey_Value.Name == null ? null : input.BaseKey_Value.Name.ToUpper();
            baseKey_Value.Remarks = input.BaseKey_Value.Remarks == null ? null : input.BaseKey_Value.Remarks.ToUpper();
        }
        /// <summary>
        /// 删除指定项
        /// </summary>
        /// <param name="Id">删除项Id</param>
        /// <returns></returns>
        [HttpPost]
        [AbpAuthorize(AppCustomPermissions.Customs_DictionaryType_Delete)]
        public async Task DeleteBaseKey_Value(string Id)
        {
            var baseKey_Value = await _baseKey_ValueRepository.GetAsync(Convert.ToInt32(Id));
            baseKey_Value.IsDeleted = true;
            baseKey_Value.DeleterUserId = AbpSession.GetUserId();
            baseKey_Value.DeletionTime = DateTime.Now;
        }


        //public void RefreshCache()
        //{
        //	_cacheManager.GetCache("common").SetAsync("kv", _baseKey_ValueRepository.GetAllIncluding().ToList());
        //}

    }
}
