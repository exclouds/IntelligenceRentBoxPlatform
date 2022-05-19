using Abp.Application.Services.Dto;
using Abp.Authorization;
using Abp.Collections.Extensions;
using Abp.Dapper.Repositories;
using Abp.Domain.Repositories;
using Abp.Extensions;
using Abp.UI;
using Magicodes.Admin;
using Magicodes.Admin.Authorization.Users;
using Magicodes.Admin.Core.Custom.Basis;
using Magicodes.Admin.Organizations;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Linq.Dynamic.Core;
using Abp.Linq.Extensions;
using Microsoft.EntityFrameworkCore;
using Magicodes.Admin.Core.Custom.Business;
using Abp.AutoMapper;
using Admin.Application.Custom.API.PublicArea.Annex.Dto;
using Magicodes.Admin.Attachments;
using Microsoft.AspNetCore.Http;
using Admin.Application.Custom.API.PublicArea.PubContactNO;
using Admin.Application.Custom.API.OnlineSearch.Dto;

namespace Admin.Application.Custom.API.OnlineSearch
{
    public class OnlineSearchAppService : AppServiceBase
    {
        #region 注入依赖
        private readonly IRepository<BoxInfo, int> _BoxInfoRepository;
        private readonly IRepository<BoxDetails, int> _BoxDetailsRepository;
        private readonly IRepository<TenantInfo, int> _TenantInfoRepository;
        private readonly IRepository<MyOrganization, long> _organizationUnitRepository;
        private readonly IRepository<User, long> _userRepository;

        private readonly IDapperRepository<MyOrganization, long> _sqlDapperRepository;
        private readonly IRepository<AttachmentInfo, long> _AttachmentInfoRepository;
        private readonly IHttpContextAccessor _httpContextAccessor;
        public IContactNOAPPService _IContactNOAPPService;
        private readonly IRepository<SiteTable, int> _SiteTableRepository;
        private readonly IRepository<LinSite, int> _LinSiteRepository;
        private readonly IRepository<Line, int> _LineRepository;

        // private TokenAuthController

        public OnlineSearchAppService(IRepository<BoxInfo, int> BoxInfoRepository,
            IRepository<BoxDetails, int> BoxDetailsRepository,
            IRepository<TenantInfo, int> TenantInfoRepository
            , IRepository<MyOrganization, long> organizationUnitRepository
            , IRepository<User, long> userRepository,
            IRepository<AttachmentInfo, long> AttachmentInfoRepository,
            IDapperRepository<MyOrganization, long> sqlDapperRepository,
            IHttpContextAccessor httpContextAccessor,
            IContactNOAPPService IContactNOAPPService,
            IRepository<SiteTable, int> SiteTableRepository,
            IRepository<LinSite, int> LinSiteRepository,
            IRepository<Line, int> LineRepository)
        {
            _BoxInfoRepository = BoxInfoRepository;
            _BoxDetailsRepository = BoxDetailsRepository;
            _TenantInfoRepository = TenantInfoRepository;
            _organizationUnitRepository = organizationUnitRepository;
            _userRepository = userRepository;
            _AttachmentInfoRepository = AttachmentInfoRepository;
            _sqlDapperRepository = sqlDapperRepository;
            _httpContextAccessor = httpContextAccessor;
            _IContactNOAPPService = IContactNOAPPService;
            _SiteTableRepository = SiteTableRepository;
            _LinSiteRepository = LinSiteRepository;
            _LineRepository = LineRepository;

        }
        #endregion

        #region  箱东查询

        /// <summary>
        /// 查询
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public async Task<PagedResultDto<XDSearchList>> GetXDDelInfoList(XDSearchDto input)
        {
            async Task<PagedResultDto<XDSearchList>> GetListFunc()
            {
                var query = CreateXDDelInfoQuery(input);
                //获取行数
                var resultCount = query.Count();
                //排序，分页
                var results = query
                    .OrderBy(input.Sorting)
                    .PageBy(input)
                    .ToList();

                var allids = results.Select(p => p.BillNO).ToList();
                if (allids.Count > 0)
                {
                    var alldetail = _BoxDetailsRepository.GetAll().Where(p => allids.Contains(p.BoxTenantInfoNO)).ToList();
                    results.ForEach(item =>
                    {
                        var boxdetai = alldetail.Where(p => p.BoxTenantInfoNO == item.BillNO).ToList();
                        item.xxcc = boxdetai.Count == 0 ? "" : string.Join(",", boxdetai.Select(p => p.Size + p.Box).Distinct().ToList());
                    });
                }
                return new PagedResultDto<XDSearchList>(resultCount, results.MapTo<List<XDSearchList>>());
            }
            return await GetListFunc();
        }

        private IQueryable<XDSearchList> CreateXDDelInfoQuery(XDSearchDto input)
        {
            var query = from XDDelInfo in _BoxInfoRepository.GetAll().Where(p => p.IsEnable && p.IsVerify && string.IsNullOrEmpty(p.VerifyRem))
                             .WhereIf(!input.BillNO.IsNullOrEmpty(), p => p.BillNO.Contains(input.BillNO.Trim().ToUpper()))
                              .WhereIf(!input.StartStation.IsNullOrEmpty(), p => p.StartStation == input.StartStation)
                              .WhereIf(!input.EndStation.IsNullOrEmpty(), p => p.EndStation == input.EndStation)
                              .WhereIf(!input.ReturnStation.IsNullOrEmpty(), p => p.ReturnStation == input.ReturnStation)                            
                              .WhereIf(input.IsInStock.HasValue, p => p.IsInStock == input.IsInStock)
                              .WhereIf(input.startprice.HasValue, p => p.SellingPrice>= input.startprice)
                              .WhereIf(input.endprice.HasValue, p => p.SellingPrice <= input.endprice)


                        join site in _SiteTableRepository.GetAll() on XDDelInfo.StartStation equals site.Code into startsite
                        from startline in startsite.DefaultIfEmpty()

                        join site in _SiteTableRepository.GetAll() on XDDelInfo.EndStation equals site.Code into endsite
                        from endline in endsite.DefaultIfEmpty()

                        join line in _LineRepository.GetAll() on XDDelInfo.Line equals line.Id into lines
                        from xdline in lines.DefaultIfEmpty()


                        select new XDSearchList
                        {
                            Id = XDDelInfo.Id,
                            BillNO = XDDelInfo.BillNO,
                            StartStation = string.IsNullOrEmpty(startline.SiteName) ? XDDelInfo.StartStation : startline.SiteName,
                            EndStation = string.IsNullOrEmpty(endline.SiteName) ? XDDelInfo.EndStation : endline.SiteName,
                            ReturnStation = XDDelInfo.ReturnStation,
                            IsInStock = XDDelInfo.IsInStock,
                            PredictTime = XDDelInfo.PredictTime,
                            EffectiveSTime = XDDelInfo.EffectiveSTime,
                            EffectiveETime = XDDelInfo.EffectiveETime,
                            SellingPrice = XDDelInfo.SellingPrice,
                            Line = xdline.LineName,                           
                            Finish = XDDelInfo.Finish,
                            CreationTime = XDDelInfo.CreationTime,
                        };


            return query;
        }
        #endregion

        
        #region 获取集装箱信息
        /// <summary>
        /// 获取基础
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task<List<XDSeachDtailDto>> GetXDBoxDetail(string billno)
        {
           
            var allfeelis = _BoxDetailsRepository.GetAll().Where(p => p.BoxTenantInfoNO == billno)
                .Select(p=>new XDSeachDtailDto
                {
                    Id=p.Id,
                    BoxNO=p.BoxNO,
                    Box=p.Box,
                    Size=p.Size,
                    BoxAge=p.BoxAge,
                    Quantity= p.Quantity,
                }).ToList();
            ;

           
            return allfeelis;
        }
        #endregion
    

        #region  租客查询

        /// <summary>
        /// 查询
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public async Task<PagedResultDto<ZKSearchList>> GetZKDelInfoList(ZKSearchDto input)
        {
            async Task<PagedResultDto<ZKSearchList>> GetListFunc()
            {
                var query = CreateZKDelInfoQuery(input);
                //获取行数
                var resultCount = await query.CountAsync();
                //排序，分页
                var results = query
                    .OrderBy(input.Sorting)
                    .PageBy(input)
                    .ToList();
                var allids = results.Select(p => p.BillNO).ToList();
                if (allids.Count > 0)
                {
                    var alldetail = _BoxDetailsRepository.GetAll().Where(p => allids.Contains(p.BoxTenantInfoNO)).ToList();
                    results.ForEach(item =>
                    {
                        var boxdetai = alldetail.Where(p => p.BoxTenantInfoNO == item.BillNO).ToList();
                        item.xxcc = boxdetai.Count == 0 ? "" : string.Join(",", boxdetai.Select(p => p.Size + p.Box).Distinct().ToList());
                    });
                }
                return new PagedResultDto<ZKSearchList>(resultCount, results.MapTo<List<ZKSearchList>>());
            }
            return await GetListFunc();
        }

        private IQueryable<ZKSearchList> CreateZKDelInfoQuery(ZKSearchDto input)
        {
            var query = from ZKDelInfo in _TenantInfoRepository.GetAll().Where(p => p.IsEnable && p.IsVerify &&string.IsNullOrEmpty(p.VerifyRem))
                             .WhereIf(!input.BillNO.IsNullOrEmpty(), p => p.BillNO.Contains(input.BillNO.Trim().ToUpper()))
                              .WhereIf(!input.StartStation.IsNullOrEmpty(), p => p.StartStation == input.StartStation)
                              .WhereIf(!input.EndStation.IsNullOrEmpty(), p => p.EndStation == input.EndStation)
                              .WhereIf(input.startprice.HasValue, p => p.HopePrice >= input.startprice)
                              .WhereIf(input.endprice.HasValue, p => p.HopePrice <= input.endprice)

                        join site in _SiteTableRepository.GetAll() on ZKDelInfo.StartStation equals site.Code into startsite
                        from startline in startsite.DefaultIfEmpty()

                        join site in _SiteTableRepository.GetAll() on ZKDelInfo.EndStation equals site.Code into endsite
                        from endline in endsite.DefaultIfEmpty()

                        join line in _LineRepository.GetAll() on ZKDelInfo.Line equals line.Id into lines
                        from zkline in lines.DefaultIfEmpty()


                        select new ZKSearchList
                        {
                            Id = ZKDelInfo.Id,
                            BillNO = ZKDelInfo.BillNO,
                            StartStation = string.IsNullOrEmpty(startline.SiteName) ? ZKDelInfo.StartStation : startline.SiteName,
                            EndStation = string.IsNullOrEmpty(endline.SiteName) ? ZKDelInfo.EndStation : endline.SiteName,

                            EffectiveSTime = ZKDelInfo.EffectiveSTime,
                            EffectiveETime = ZKDelInfo.EffectiveETime,
                            HopePrice = ZKDelInfo.HopePrice,
                            Line = zkline.LineName,                          
                            InquiryNum = ZKDelInfo.InquiryNum,
                            Finish = ZKDelInfo.Finish,
                            CreationTime = ZKDelInfo.CreationTime,
                        };


            return query;
        }
        #endregion
    }
}
