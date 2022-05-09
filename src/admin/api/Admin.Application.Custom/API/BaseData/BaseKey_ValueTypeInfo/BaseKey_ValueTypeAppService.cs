using Abp.Authorization;
using Abp.Dapper.Repositories;
using Abp.Domain.Repositories;
using Abp.Extensions;
using Abp.Runtime.Session;
using Abp.UI;
using Magicodes.Admin;
using Magicodes.Admin.Core.Custom.Authorization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Abp.AutoMapper;
using Magicodes.Admin.Core.Custom.DataDictionary;
using Microsoft.AspNetCore.Mvc;
using Admin.Application.Custom.API.BaseData.BaseKey_ValueTypeInfo.Dto;

namespace Admin.Application.Custom.API.BaseData.BaseKey_ValueTypeInfo
{
    /// <summary>
    /// 键值对类型对照表
    /// </summary>

    //[AbpAuthorize(AppCustomPermissions.Customs_DictionaryType)]
    //[AbpAllowAnonymous]
    public partial class BaseKey_ValueTypeAppService : AppServiceBase, IBaseKey_ValueTypeAppService
    {
        //依赖注入键值对类型
        private readonly IRepository<BaseKey_ValueType, int> _baseKey_ValueTypeRepository;
        private readonly IDapperRepository<BaseKey_ValueType, int> _baseKey_ValueTypeDapperRepository;

        public BaseKey_ValueTypeAppService(
            IRepository<BaseKey_ValueType, int> baseKey_ValueTypeRepository,
            IDapperRepository<BaseKey_ValueType, int> baseKey_ValueTypeDapperRepository
            ) : base()
        {
            _baseKey_ValueTypeRepository = baseKey_ValueTypeRepository;
            _baseKey_ValueTypeDapperRepository = baseKey_ValueTypeDapperRepository;

        }

        /// <summary>
        /// 获取键值对类型对照表
        /// </summary>
        /// <param name="code">父级id（没有的话取根目录）</param>
        /// <returns></returns>
        //[AbpAuthorize(AppCustomPermissions.Customs_DictionaryType)]
        public List<GetAllTypeListDto> GetAllTypeList(string code)
        {
            if (!AbpSession.TenantId.HasValue)
            {
                throw new UserFriendlyException("登陆超时，请退出后重新登陆");
            }
            string sql = "SELECT * FROM  BaseKey_ValueType WHERE IsDeleted=0 and tenantid = " + AbpSession.TenantId.ToString();
            if (!code.IsNullOrEmpty())
            {
                sql += " and parentcode= '" + code.Trim() + "'";
            }
            else
            {
                sql += " and parentcode is null or parentcode=''";
            }
            var result = _baseKey_ValueTypeDapperRepository.Query<GetAllTypeListDto>(sql).AsQueryable().ToList();

            foreach (var item in result)
            {
                item.Children = GetAllTypeList(item.TypeCode);
            }
            return result;
        }

        /// <summary>
        /// 新增或者编辑
        /// </summary>
        /// <param name="input">修改项</param>
        /// <returns></returns>
        [AbpAuthorize(AppCustomPermissions.Customs_DictionaryType_Create, AppCustomPermissions.Customs_DictionaryType_Edit)]
        public async Task CreateOrUpdateBaseKey_ValueType(CreateUpdateKeyValueTypeDto input)
        {
            if (string.IsNullOrEmpty(input.KeyValueType.Id.ToString()))
            {
                await CreateBaseKey_ValueType(input);
            }
            else
            {
                await UpdateBaseKey_ValueType(input);
            }
        }

        /// <summary>
        /// 新增
        /// </summary>
        /// <param name="input">新增参数</param>
        /// <returns></returns>
        protected virtual async Task CreateBaseKey_ValueType(CreateUpdateKeyValueTypeDto input)
        {
            if (_baseKey_ValueTypeRepository.GetAllIncluding().Any(p => p.TypeCode == input.KeyValueType.TypeCode))
            {
                throw new UserFriendlyException("已存在相同类型代码，请核对后重试。");
            }

            if (_baseKey_ValueTypeRepository.GetAllIncluding().Any(p => p.TypeName == input.KeyValueType.TypeName))
            {
                throw new UserFriendlyException("已存在相同类型名称，请核对后重试。");
            }
            var baseKey_ValueType = new BaseKey_ValueType()
            {
                TypeCode = input.KeyValueType.TypeCode == null ? null : input.KeyValueType.TypeCode.ToUpper().Trim(),
                TypeName = input.KeyValueType.TypeName == null ? null : input.KeyValueType.TypeName.ToUpper(),
                ParentCode = input.KeyValueType.ParentCode == null ? null : input.KeyValueType.ParentCode.ToUpper(),
                Remarks = input.KeyValueType.Remarks == null ? null : input.KeyValueType.Remarks,
                CreatorUserId = AbpSession.UserId,
                CreationTime = DateTime.Now,
                TenantId = AbpSession.TenantId
            };
            await _baseKey_ValueTypeRepository.InsertAsync(baseKey_ValueType);

        }
        /// <summary>
        /// 修改
        /// </summary>
        /// <param name="input">修改参数</param>
        /// <returns></returns>
        protected virtual async Task UpdateBaseKey_ValueType(CreateUpdateKeyValueTypeDto input)
        {
            var baseKey_ValueType = await _baseKey_ValueTypeRepository.GetAsync(Convert.ToInt32(input.KeyValueType.Id));
            if (baseKey_ValueType.SystemSetting == true)
            {
                throw new UserFriendlyException("此数据为系统设置数据，不能修改!");
            }
            //类型代码不能重复
            if (input.KeyValueType.TypeCode != baseKey_ValueType.TypeCode
                || input.KeyValueType.TypeName != baseKey_ValueType.TypeName
                || input.KeyValueType.ParentCode != baseKey_ValueType.ParentCode)
            {
                if (_baseKey_ValueTypeRepository.GetAllIncluding().Any(p => p.TypeCode == input.KeyValueType.TypeCode
                                                                   && p.Id != Convert.ToInt32(input.KeyValueType.Id)))
                {
                    throw new UserFriendlyException("已存在相同类型代码，请核对后重试。");
                }

                if (_baseKey_ValueTypeRepository.GetAllIncluding().Any(p => p.TypeName == input.KeyValueType.TypeName
                                                                    && p.Id != Convert.ToInt32(input.KeyValueType.Id)))
                {
                    throw new UserFriendlyException("已存在相同类型名称，请核对后重试。");
                }
                baseKey_ValueType.LastModificationTime = DateTime.Now;
                baseKey_ValueType.LastModifierUserId = AbpSession.GetUserId();
                baseKey_ValueType.TypeCode = input.KeyValueType.TypeCode == null ? null : input.KeyValueType.TypeCode.ToUpper().Trim();
                baseKey_ValueType.TypeName = input.KeyValueType.TypeName == null ? null : input.KeyValueType.TypeName.ToUpper();
                baseKey_ValueType.ParentCode = input.KeyValueType.ParentCode == null ? null : input.KeyValueType.ParentCode.ToUpper();
                baseKey_ValueType.Remarks = input.KeyValueType.Remarks == null ? null : input.KeyValueType.Remarks;
            }
            else
            {
                throw new UserFriendlyException("没有做任何更改。");
            }

        }

        /// <summary>
        /// 删除
        /// </summary>
        /// <param name="input">id号</param>
        /// <returns></returns>
        [HttpPost]
        [AbpAuthorize(AppCustomPermissions.Customs_DictionaryType_Delete)]
        public async Task DeleteBaseKey_ValueType(string input)
        {
            var basekeyvaluetype = await _baseKey_ValueTypeRepository.GetAsync(Convert.ToInt32(input));
            basekeyvaluetype.IsDeleted = true;
            basekeyvaluetype.DeleterUserId = AbpSession.GetUserId();
            basekeyvaluetype.DeletionTime = DateTime.Now;
        }

        /// <summary>
        /// 查询单个明细
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task<SingleDto> GetSingle(string id)
        {
            var result = from child in _baseKey_ValueTypeRepository.GetAllIncluding().Where(x => x.Id == Convert.ToInt32(id))
                         join parent in _baseKey_ValueTypeRepository.GetAllIncluding()
                             on child.ParentCode equals parent.TypeCode
                             into temp
                         from a in temp.DefaultIfEmpty()

                         select new SingleDto
                         {
                             Id = child.Id.ToString(),
                             TypeCode = child.TypeCode,
                             TypeName = child.TypeName,
                             ParentCode = child.ParentCode,
                             ParentName = a.TypeName,

                         };
            return result.FirstOrDefault().MapTo<SingleDto>();
        }
    }
}
