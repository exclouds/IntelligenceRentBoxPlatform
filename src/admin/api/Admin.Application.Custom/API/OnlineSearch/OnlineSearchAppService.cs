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
using Admin.Application.Custom.API.BaseDto;

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

                //var allids = results.Select(p => p.BillNO).ToList();
                if (results.Count > 0)
                {
                    //var alldetail = _BoxDetailsRepository.GetAll().Where(p => allids.Contains(p.BoxTenantInfoNO)).ToList();
                    //var alllinesite = _LinSiteRepository.GetAll().GroupBy(p => p.LineId).Select(p => new LineSiteDto { LineId = p.FirstOrDefault().LineId, site = string.Join(",", p.Select(x => x.Code).ToList()) }).ToList();
                    var allsite = _SiteTableRepository.GetAll().ToList();
                    results.ForEach(item =>
                    {
                        if (string.IsNullOrEmpty(item.EndStation))
                        {
                            item.EndStation = "全部路线站点";
                        }
                        else
                        {
                            var sitelis = allsite.Where(p => ("," + item.EndStation + ",").Contains("," + p.Code + ",")).Select(p => p.SiteName).ToList();
                            item.EndStation = string.Join(",", sitelis);
                        }
                        //var boxdetai = alldetail.Where(p => p.BoxTenantInfoNO == item.BillNO).ToList();
                        //item.xxcc = boxdetai.Count == 0 ? "" : string.Join(",", boxdetai.Select(p => p.Size + p.Box).Distinct().ToList());
                    });
                }
                return new PagedResultDto<XDSearchList>(resultCount, results.MapTo<List<XDSearchList>>());
            }
            return await GetListFunc();
        }

        private IQueryable<XDSearchList> CreateXDDelInfoQuery(XDSearchDto input)
        {
            /*
            var query = from XDDelInfo in _BoxInfoRepository.GetAll().Where(p => p.IsEnable && p.IsVerify && string.IsNullOrEmpty(p.VerifyRem))
                             .WhereIf(!input.BillNO.IsNullOrEmpty(), p => p.BillNO.Contains(input.BillNO.Trim().ToUpper()))
                              .WhereIf(!input.StartStation.IsNullOrEmpty(), p => p.StartStation == input.StartStation)
                              .WhereIf(!input.EndStation.IsNullOrEmpty(), p => p.EndStation == input.EndStation)
                              .WhereIf(!input.ReturnStation.IsNullOrEmpty(), p => p.ReturnStation == input.ReturnStation)                            
                             // .WhereIf(input.IsInStock.HasValue, p => p.IsInStock == input.IsInStock)
                              .WhereIf(input.EffectiveTime.HasValue, p => p.EffectiveSTime <= input.EffectiveTime.Value && p.EffectiveETime>=input.EffectiveTime.Value)


                        join site in _SiteTableRepository.GetAll() on XDDelInfo.StartStation equals site.Code into startsite
                        from startline in startsite.DefaultIfEmpty()

                        join site in _SiteTableRepository.GetAll() on XDDelInfo.EndStation equals site.Code into endsite
                        from endline in endsite.DefaultIfEmpty()

                        join site in _SiteTableRepository.GetAll() on XDDelInfo.ReturnStation equals site.Code into resite
                        from reline in resite.DefaultIfEmpty()

                        join line in _LineRepository.GetAll() on XDDelInfo.Line equals line.Id into lines
                        from xdline in lines.DefaultIfEmpty()


                        select new XDSearchList
                        {
                            Id = XDDelInfo.Id,
                            BillNO = XDDelInfo.BillNO,
                            StartStation = string.IsNullOrEmpty(startline.SiteName) ? XDDelInfo.StartStation : startline.SiteName,
                            EndStation = string.IsNullOrEmpty(endline.SiteName) ? XDDelInfo.EndStation : endline.SiteName,
                            ReturnStation = string.IsNullOrEmpty(reline.SiteName) ? XDDelInfo.ReturnStation : reline.SiteName,
                        
                            //IsInStock = XDDelInfo.IsInStock,
                            //PredictTime = XDDelInfo.PredictTime,
                            EffectiveSTime = XDDelInfo.EffectiveSTime,
                            EffectiveETime = XDDelInfo.EffectiveETime,
                            Line = xdline.LineName,                           
                            Finish = XDDelInfo.Finish,
                            CreationTime = XDDelInfo.CreationTime,
                        };

            */
            string sql = @"--先获取符合条件的主表信息
                            select a.id,a.BillNO,b.box,b.size ,isnull(sum(b.Quantity),0) as qty
                            INTO #XDBILLINFO
                            from BoxInfos a
                            left join BoxDetailses b on a.BillNO=b.BoxTenantInfoNO and b.isdeleted=0
                            where a.IsEnable=1 AND a.IsVerify=1 AND ISNULL(a.VerifyRem,'')='' and a.isdeleted=0";
            if (!string.IsNullOrEmpty(input.BillNO))
            {
                sql += " and a.BillNO like '%"+ input.BillNO.Trim()+ "%' ";
            }
            if (!string.IsNullOrEmpty(input.StartStation))
            {
                sql += " and a.StartStation = '" + input.StartStation + "' ";
            }
            if (!string.IsNullOrEmpty(input.Line))
            {
                sql += " and a.Line = '" + input.Line + "' ";
            }
            if (!string.IsNullOrEmpty(input.EndStation))
            {
                // sql += " and charindex(','+'" + input.EndStation + "'+',',','+a.EndStation+',') >0 ";
                sql += " and   ( case when isnull(a.EndStation,'')='' then (select count(*) from LinSites  where LinSites.isdeleted=0 and LinSites.Lineid = a.Line  and LinSites.code='" + input.EndStation + "') else charindex(',' + '" + input.EndStation + "' + ',', ',' + a.EndStation + ',') end )  > 0  ";
            }
            if (!string.IsNullOrEmpty(input.ReturnStation))
            {
                sql += " and a.ReturnStation = '" + input.ReturnStation + "' ";
            }
            //用箱范围
            if (input.EffectiveSTime.HasValue && input.EffectiveETime.HasValue)
            {
                sql += " and NOT( a.EffectiveSTime > '" + input.EffectiveETime.Value.ToString("yyyy-MM-dd") + "' or  a.EffectiveETime<'" + input.EffectiveSTime.Value.ToString("yyyy-MM-dd") + "')";
            }
            else if (input.EffectiveSTime.HasValue)
            {
                sql += " and  a.EffectiveSTime > ='" + input.EffectiveSTime.Value.ToString("yyyy-MM-dd") + "'  ";
            }
            else if (input.EffectiveETime.HasValue)
            {
                sql += " and   a.EffectiveETime<'" + input.EffectiveETime.Value.AddDays(1).ToString("yyyy-MM-dd") + "'";
            }
            //else if (input.EffectiveSTime.HasValue)
            //{
            //    sql += " and  a.EffectiveSTime > '" + input.EffectiveSTime.Value.ToString("yyyy-MM-dd") + "'   and    a.EffectiveETime>='" + input.EffectiveSTime.Value.ToString("yyyy-MM-dd") + "'"; ;
            //}
            //else if(input.EffectiveETime.HasValue)
            //{
            //    sql += " and  a.EffectiveSTime >= '" + input.EffectiveETime.Value.ToString("yyyy-MM-dd") + "'   and    a.EffectiveETime>='" + input.EffectiveETime.Value.ToString("yyyy-MM-dd") + "'";
            //}
            if (input.Finish.HasValue)
            {
                if (input.Finish.Value)
                    sql += " and a.Finish = 1 ";
                else
                    sql += " and (a.Finish = 0 or a.Finish is null) ";
            }

            sql += @"       
                         group by a.id,a.BillNO,b.box,b.size

                            --去重
                            select id,BillNO  INTO #XDBILLINFO1
                            from #XDBILLINFO ";

            if (!string.IsNullOrEmpty(input.XXCC))
            {
                string[] data = input.XXCC.Split('|');
                sql += " where box='"+ data[1] + "' and size='" + data[0] + "' ";
                if (data[2] != "" && data[2] != "0")
                {
                    sql += " and qty>=" + data[2];
                }
            }
            sql += @"
                            group by id,BillNO

                            select  a.Id,a.BillNO,a.Line as LineID,
                            (case when isnull(startline.SiteName,'')='' then startline.SiteName else startline.SiteName end )StartStation,
                            --(case when isnull(endline.SiteName,'')='' then endline.SiteName else endline.SiteName end )EndStation,
                             a.EndStation,
                            (case when isnull(reline.SiteName,'')='' then reline.SiteName else reline.SiteName end )ReturnStation,
                            a.EffectiveSTime,a.EffectiveETime, xdline.LineName as Line,a.Finish,a.CreationTime,
                            ( SELECT RTRIM(typename) + ';'
                                               FROM
                                               (
                                                   SELECT Size + BOX + '×' + CONVERT(NVARCHAR(50), sum(b.qty)) AS typename
                                                   FROM #XDBILLINFO b
                                                   WHERE  a.BillNO=b.BillNO
                              
                                                   GROUP BY box,
                                                            Size
                                               ) a
                                               FOR XML PATH('') )as xxcc
                            from  #XDBILLINFO1 b
                            join BoxInfos a on a.id=b.id
                            left join SiteTables startline on a.StartStation = startline.Code 
                           -- left join SiteTables endline on a.EndStation = endline.Code 
                            left join SiteTables reline on a.ReturnStation = reline.Code 
                            left join Lines xdline on a.Line = xdline.Id 

                            DROP TABLE #XDBILLINFO,#XDBILLINFO1";
            var query = _sqlDapperRepository.Query<XDSearchList>(sql).AsQueryable();


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
            var dtnow = DateTime.Now.Year;
            var allfeelis = _BoxDetailsRepository.GetAll().Where(p => p.BoxTenantInfoNO == billno)
                .Select(p=>new XDSeachDtailDto
                {
                    Id=p.Id,
                    BoxNO=p.BoxNO,
                    Box=p.Box,
                    Size=p.Size,
                    //BoxAge=p.BoxAge,
                    Quantity= p.Quantity,
                    BoxAge = p.BoxTime.HasValue ? (dtnow - p.BoxTime.Value.Year).ToString() : "",
                    MaxWeight = p.MaxWeight,
                    BoxLabel = p.BoxLabel,
                    BoxTime = p.BoxTime,
                    FreezerModel = p.FreezerModel,
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
                var resultCount = query.Count();
                //排序，分页
                var results = query
                    .OrderBy(input.Sorting)
                    .PageBy(input)
                    .ToList();
                //var allids = results.Select(p => p.BillNO).ToList();
                if (results.Count > 0)
                {
                    //var alldetail = _BoxDetailsRepository.GetAll().Where(p => allids.Contains(p.BoxTenantInfoNO)).ToList();
                    //先获取所有航线站点信息
                   // var alllinesite = _LinSiteRepository.GetAll().GroupBy(p => p.LineId).Select(p => new LineSiteDto { LineId = p.FirstOrDefault().LineId, site = string.Join(",", p.Select(x => x.Code).ToList()) }).ToList();
                    var allsite = _SiteTableRepository.GetAll().ToList();
                    results.ForEach(item =>
                    {
                        if (string.IsNullOrEmpty(item.EndStation))
                        {
                            item.EndStation = "全部路线站点";
                        }
                        else
                        {
                            var sitelis = allsite.Where(p => ("," + item.EndStation + ",").Contains("," + p.Code + ",")).Select(p => p.SiteName).ToList();
                            item.EndStation = string.Join(",", sitelis);
                        }
                        //var boxdetai = alldetail.Where(p => p.BoxTenantInfoNO == item.BillNO).ToList();
                        //item.xxcc = boxdetai.Count == 0 ? "" : string.Join(",", boxdetai.Select(p => p.Size + p.Box).Distinct().ToList());
                    });
                }
                return new PagedResultDto<ZKSearchList>(resultCount, results.MapTo<List<ZKSearchList>>());
            }
            return await GetListFunc();
        }

        private IQueryable<ZKSearchList> CreateZKDelInfoQuery(ZKSearchDto input)
        {
            /*
            var query = from ZKDelInfo in _TenantInfoRepository.GetAll().Where(p => p.IsEnable && p.IsVerify &&string.IsNullOrEmpty(p.VerifyRem))
                             .WhereIf(!input.BillNO.IsNullOrEmpty(), p => p.BillNO.Contains(input.BillNO.Trim().ToUpper()))
                              .WhereIf(!input.StartStation.IsNullOrEmpty(), p => p.StartStation == input.StartStation)
                              .WhereIf(!input.EndStation.IsNullOrEmpty(), p => p.EndStation == input.EndStation)
                               .WhereIf(input.EffectiveTime.HasValue, p => p.EffectiveSTime <= input.EffectiveTime.Value && p.EffectiveETime >= input.EffectiveTime.Value)
                               .WhereIf(input.Finish.HasValue, p => p.Finish == input.Finish)
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
                            Line = zkline.LineName,                          
                            InquiryNum = ZKDelInfo.InquiryNum,
                            Finish = ZKDelInfo.Finish,
                            CreationTime = ZKDelInfo.CreationTime,
                        };
                        */

            string sql = @"--先获取符合条件的主表信息
                            select a.id,a.BillNO,b.box,b.size ,isnull(sum(b.Quantity),0) as qty
                            INTO #ZKDelInfo
                            from TenantInfos a
                            left join BoxDetailses b on a.BillNO=b.BoxTenantInfoNO and b.isdeleted=0
                            where a.IsEnable=1 AND a.IsVerify=1 AND ISNULL(a.VerifyRem,'')='' and a.isdeleted=0";
            if (!string.IsNullOrEmpty(input.BillNO))
            {
                sql += " and a.BillNO like '%" + input.BillNO.Trim() + "%' ";
            }
            if (!string.IsNullOrEmpty(input.StartStation))
            {
                sql += " and a.StartStation = '" + input.StartStation + "' ";
            }
            if (!string.IsNullOrEmpty(input.Line))
            {
                sql += " and a.Line = '" + input.Line + "' ";
            }
            if (!string.IsNullOrEmpty(input.EndStation))
            {
                //sql += " and a.EndStation = '" + input.EndStation + "' ";
                sql += " and   ( case when isnull(a.EndStation,'')='' then (select count(*) from LinSites  where LinSites.isdeleted=0 and LinSites.Lineid = a.Line  and LinSites.code='" + input.EndStation + "') else charindex(',' + '" + input.EndStation + "' + ',', ',' + a.EndStation + ',') end )  > 0  ";
            }
            if (input.Finish.HasValue)
            {
                if(input.Finish.Value)
                  sql += " and a.Finish = 1 ";
                else
                  sql += " and (a.Finish = 0 or a.Finish is null) ";
            }
            //用箱范围
            if (input.EffectiveSTime.HasValue && input.EffectiveETime.HasValue)
            {
                sql += " and NOT( a.EffectiveSTime > '" + input.EffectiveETime.Value.ToString("yyyy-MM-dd") + "' or  a.EffectiveETime<'" + input.EffectiveSTime.Value.ToString("yyyy-MM-dd") + "')";
            }

            else if (input.EffectiveSTime.HasValue)
            {
                sql += " and  a.EffectiveSTime > ='" + input.EffectiveSTime.Value.ToString("yyyy-MM-dd") + "'  ";
            }
            else if (input.EffectiveETime.HasValue)
            {
                sql += " and   a.EffectiveETime<'" + input.EffectiveETime.Value.AddDays(1).ToString("yyyy-MM-dd") + "'";
            }
            //.WhereIf(!input.BillNO.IsNullOrEmpty(), p => p.BillNO.Contains(input.BillNO.Trim().ToUpper()))
            //                 .WhereIf(!input.StartStation.IsNullOrEmpty(), p => p.StartStation == input.StartStation)
            //                 .WhereIf(!input.EndStation.IsNullOrEmpty(), p => p.EndStation == input.EndStation)
            //                 .WhereIf(!input.ReturnStation.IsNullOrEmpty(), p => p.ReturnStation == input.ReturnStation)
            //                 // .WhereIf(input.IsInStock.HasValue, p => p.IsInStock == input.IsInStock)
            //                 .WhereIf(input.EffectiveTime.HasValue, p => p.EffectiveSTime <= input.EffectiveTime.Value && p.EffectiveETime >= input.EffectiveTime.Value)


            sql += @"       
                         group by a.id,a.BillNO,b.box,b.size

                            --去重
                            select id,BillNO  INTO #ZKDelInfo1
                            from #ZKDelInfo ";

            if (!string.IsNullOrEmpty(input.XXCC))
            {
                string[] data = input.XXCC.Split('|');
                sql += " where box='" + data[1] + "' and size='" + data[0] + "' ";
                if (data[2] != "" && data[2] != "0")
                {
                    sql += " and qty>=" + data[2];
                }
            }
            sql += @"
                            group by id,BillNO

                            select  a.Id,a.BillNO,a.Line as LineID,
                            (case when isnull(startline.SiteName,'')='' then startline.SiteName else startline.SiteName end )StartStation,
                            --(case when isnull(endline.SiteName,'')='' then endline.SiteName else endline.SiteName end )EndStation,                           
                             a.EndStation,a.EffectiveSTime,a.EffectiveETime, xdline.LineName as Line,a.Finish,a.CreationTime,a.InquiryNum,
                            ( SELECT RTRIM(typename) + ';'
                                               FROM
                                               (
                                                   SELECT Size + BOX + '×' + CONVERT(NVARCHAR(50), sum(b.qty)) AS typename
                                                   FROM #ZKDelInfo b
                                                   WHERE  a.BillNO=b.BillNO
                              
                                                   GROUP BY box,
                                                            Size
                                               ) a
                                               FOR XML PATH('') )as xxcc
                            from  #ZKDelInfo1 b
                            join TenantInfos a on a.id=b.id
                            left join SiteTables startline on a.StartStation = startline.Code 
                           -- left join SiteTables endline on a.EndStation = endline.Code                            
                            left join Lines xdline on a.Line = xdline.Id 

                            DROP TABLE #ZKDelInfo,#ZKDelInfo1";
            var query = _sqlDapperRepository.Query<ZKSearchList>(sql).AsQueryable();
            return query;
        }
        #endregion
    }
}
