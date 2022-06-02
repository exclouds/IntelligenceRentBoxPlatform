﻿using Abp.Domain.Repositories;
using Admin.Application.Custom.API.PublicArea.Combox.Dto;
using Magicodes.Admin;
using Magicodes.Admin.Core.Custom.DataDictionary;
using System.Threading.Tasks;
using Abp.Application.Services.Dto;
using Abp.Collections.Extensions;
using Abp.Extensions;
using Abp.UI;
using System.Linq;
using System.Linq.Dynamic.Core;
using Abp.Authorization;
using Magicodes.Admin.Core.Custom.Basis;
using System.Collections.Generic;
using Magicodes.Admin.Authorization.Users;
using Magicodes.Admin.Organizations;
using Abp.Linq.Extensions;

namespace Admin.Application.Custom.API.PublicArea.Combox
{

    /// <summary>
    /// 公共下拉服务
    /// </summary>
    [AbpAllowAnonymous]
    public class PublicComboxAppSevrice : AppServiceBase
    {
        #region 依赖注入
        
        //字典表
        private readonly IRepository<BaseKey_Value, long> _BaseKey_ValueRepository;
        private readonly IRepository<Country, int> _CountryRepository;
        private readonly IRepository<SiteTable, int> _SiteTableRepository;
        private readonly IRepository<LinSite, int> _LinSiteRepository;
        private readonly IRepository<Line, int> _LineRepository;
        private readonly IRepository<User, long> _userRepository;
        private readonly IRepository<MyOrganization, long> _organizationUnitRepository;

        /// <summary>
        /// 构造函数
        /// </summary>
        public PublicComboxAppSevrice(
            IRepository<BaseKey_Value, long> BaseKey_ValueRepository,
            IRepository<Country, int> CountryRepository,
            IRepository<SiteTable, int> SiteTableRepository,
            IRepository<LinSite, int> LinSiteRepository,
            IRepository<Line, int> LineRepository,
            IRepository<User, long> userRepository,
            IRepository<MyOrganization, long> organizationUnitRepository
            ) : base()
        {
            _BaseKey_ValueRepository = BaseKey_ValueRepository;
            _CountryRepository = CountryRepository;
            _SiteTableRepository = SiteTableRepository;
            _LinSiteRepository = LinSiteRepository;
            _LineRepository = LineRepository;
            _userRepository = userRepository;
            _organizationUnitRepository = organizationUnitRepository;
        }
        #endregion

        /// <summary>
        /// 根据字典类型获取字典项下拉
        /// </summary>
        /// <returns></returns>
        public async Task<PublicComboBoxDto> GetBaseKeyValueComboBox(BaseKeyValueComboBoxQueryDto input)
        {
            var output = new PublicComboBoxDto();
            if (input.BaseKeyValueTypeCode.IsNullOrEmpty())
            {
                return output;
            }
            output.Comboxs = _BaseKey_ValueRepository.GetAll().Where(p => p.BaseKey_ValueTypeCode == input.BaseKeyValueTypeCode)
                .Select(p => new ComboxStringDto(p.Code.ToString(), p.Name))
                .ToList();
            return output;
        }

        #region 获取国家下拉国家
        /// <summary>
        /// 获取国家下拉国家
        /// </summary>
        /// <param name="IsEnable">是否启用</param>
        /// <returns></returns>
        public List<ComboxDto> GetCountryList( bool? IsEnable)
        {
            var output = _CountryRepository.GetAll().WhereIf(IsEnable.HasValue,p => p.IsEnable == IsEnable)
                .Select(p => new ComboxDto
                {
                    Value = p.Code,
                    DisplayText = p.Name
                }).ToList();

            return output;
        }
        #endregion

        #region 获取站点下拉
        /// <summary>
        /// 获取站点下拉
        /// </summary>
        /// <param name="CountryCode">国家代码</param>
        /// <param name="IsEnable">是否启用</param>
        /// <returns></returns>
        public List<ComboxDto> GetSiteList(string CountryCode,bool? IsEnable)
        {
            var output = _SiteTableRepository.GetAll()
                .WhereIf(IsEnable.HasValue, p => p.IsEnable == IsEnable)
                .WhereIf(!string.IsNullOrEmpty(CountryCode), p => CountryCode == p.CountryCode)
                .Select(p => new ComboxDto {
                   Value= p.Code,
                   DisplayText= p.SiteName
                }).ToList();

            return output;
        }

        #endregion

        #region 获取路线下拉
        /// <summary>
        /// 获取路线下拉
        /// </summary>
        /// <param name="Code">站点代码</param>
        /// <param name="IsEnable">是否启用</param>
        /// <returns></returns>
        public List<ComboxDto> GetLineList(string Code, bool? IsEnable)
        {
            var query = from a in _LineRepository.GetAll()
                        .WhereIf(IsEnable.HasValue, p => p.IsEnable == IsEnable)

                        join b in _LinSiteRepository.GetAll() 
                         .WhereIf(!string.IsNullOrEmpty(Code), p => Code == p.Code)
                        on a.Id.ToString() equals b.LineId

                        select new ComboxDto
                        {
                            Value = a.Id.ToString(),
                            DisplayText = a.LineName
                        };


           
            var output = query.ToList();

            return output;
        }

        #endregion

        #region 客户端公司查询
        /// <summary>
        /// 客户端公司查询
        /// </summary>
        /// <param name="IsAdmin">是否管理账户</param>
        /// <returns></returns>
        public List<ComboxDto> GetCustCompanyList(bool? IsAdmin)
        {
            var query = from a in _userRepository.GetAll()
                        .Where(u => new List<int>() { 0, 1 }.Contains(u.UserNature))
                        .WhereIf(IsAdmin.HasValue, u => u.IsAdmin)
                        join b in _organizationUnitRepository.GetAll() on a.OrganizationCode equals b.Code into o
                        from org in o.DefaultIfEmpty()
                        select new ComboxDto
                        {
                            Value = org.Code,
                            DisplayText = org.DisplayName
                        };

            var output = query.ToList();

            return output;
        }
        #endregion
    }
}