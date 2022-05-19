﻿using Abp.Application.Services.Dto;
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
using Admin.Application.Custom.API.Recommend.Dto;

namespace Admin.Application.Custom.API.Recommend
{
    [AbpAllowAnonymous]
    public class RecommendAppService : AppServiceBase
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
        private readonly IRepository<SiteTable, int> _SiteTableRepository;
        private readonly IRepository<LinSite, int> _LinSiteRepository;
        private readonly IRepository<Line, int> _LineRepository;

        // private TokenAuthController

        public RecommendAppService(IRepository<BoxInfo, int> BoxInfoRepository,
            IRepository<BoxDetails, int> BoxDetailsRepository,
            IRepository<TenantInfo, int> TenantInfoRepository
            , IRepository<MyOrganization, long> organizationUnitRepository
            , IRepository<User, long> userRepository,
            IRepository<AttachmentInfo, long> AttachmentInfoRepository,
            IDapperRepository<MyOrganization, long> sqlDapperRepository,
            IHttpContextAccessor httpContextAccessor,
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
            _SiteTableRepository = SiteTableRepository;
            _LinSiteRepository = LinSiteRepository;
            _LineRepository = LineRepository;

        }
        #endregion

        #region 查询
        public async Task<ShowResultPageList> GetIntelligRcommendList(InteRcommenDto  dto)
        {
            ShowResultPageList returnresult = new ShowResultPageList
            {
                xdlist=new List<ShowPageList> (),
                zklist=new List<ShowPageList>()
            }
            ;
            int UserNature = -1;
            List<ShowPageList> results = new List<ShowPageList>();
            if (AbpSession.UserId.HasValue)
            {
                var usrinfo = await _userRepository.GetAsync(AbpSession.UserId.Value);
                if (usrinfo == null)
                {
                    throw new UserFriendlyException($"未查找到相关登录用户信息，请重新登录!");
                }
                UserNature = usrinfo.UserNature;
            }

          

            if (UserNature != 1)
            {
                string sql = @"
                     select XDDelInfo.Id, 'XD' AS type, XDDelInfo.BillNO, 
                     (case when isnull(startline.SiteName,'')= '' then XDDelInfo.StartStation else startline.SiteName end) as StartStation,
                    (case when isnull(endline.SiteName,'') = '' then XDDelInfo.EndStation else endline.SiteName end) as EndStation,
                    XDDelInfo.ReturnStation,XDDelInfo.IsInStock,XDDelInfo.PredictTime,
                    convert(varchar(50), XDDelInfo.EffectiveSTime, 120) as EffectiveSTime,
                    convert(varchar(50), XDDelInfo.EffectiveETime, 120) as EffectiveETime,
                    XDDelInfo.SellingPrice Price, xdline.LineName Line, XDDelInfo.Finish,XDDelInfo.CreationTime


                     from  BoxInfos XDDelInfo
                    left join   SiteTables startline on XDDelInfo.StartStation = startline.Code
                    left join  SiteTables endline on XDDelInfo.EndStation = endline.Code
                    left join  Lines xdline on XDDelInfo.Line = xdline.Id


                    Where XDDelInfo.IsEnable = 1 AND XDDelInfo.IsVerify = 1 AND ISNULL(XDDelInfo.VerifyRem,'')= '' and XDDelInfo.IsDeleted = 0";

                var query = _sqlDapperRepository.Query<ShowPageList>(sql).AsQueryable();

                var resultCount = query.Count();
                //排序，分页
                results = query
                    .OrderBy(dto.Sorting)
                    .PageBy(dto)
                    .ToList();
                var allids = results.Select(p => p.Id).ToList();

                if (allids.Count > 0)
                {
                    var alldetail = _BoxDetailsRepository.GetAll().Where(p => allids.Contains(p.BoxTenantInfo)).ToList();
                    results.ForEach(item =>
                    {
                        var boxdetai = alldetail.Where(p => p.BoxTenantInfo == item.Id).ToList();
                        if (boxdetai.Count > 0)
                        {
                            item.xxcc = string.Join(",", boxdetai.GroupBy(p => new { p.Size, p.Box }).Select(p => p.FirstOrDefault().Size + p.FirstOrDefault().Box + "X" + p.Sum(P => P.Quantity).ToString()).Distinct().ToList());

                        }

                    });
                }
                returnresult.xdlist = results;
            }
            
            if ( UserNature != 0)
            {
                string sql= @"   select  ZKDelInfo.Id, 'ZK' AS type,ZKDelInfo.BillNO, 
                         (case when isnull(startline.SiteName,'')='' then  ZKDelInfo.StartStation else startline.SiteName end) as StartStation,
                        (case when isnull(endline.SiteName,'') ='' then ZKDelInfo.EndStation else endline.SiteName end) as EndStation,
                        '' AS ReturnStation,1 AS  IsInStock,NULL AS PredictTime,
                        convert(varchar(50),ZKDelInfo.EffectiveSTime,120)    as EffectiveSTime,
                        convert(varchar(50),ZKDelInfo.EffectiveETime,120)  as  EffectiveETime,
                        ZKDelInfo.HopePrice Price,xdline.LineName Line, ZKDelInfo.Finish,ZKDelInfo.CreationTime
                      

                        from  TenantInfos ZKDelInfo
                        left join   SiteTables startline on ZKDelInfo.StartStation = startline.Code 
                        left join  SiteTables endline on ZKDelInfo.EndStation = endline.Code 
                        left join  Lines xdline on ZKDelInfo.Line = xdline.Id 
                      
                        Where ZKDelInfo.IsEnable=1 AND ZKDelInfo.IsVerify=1 AND ISNULL(ZKDelInfo.VerifyRem,'')='' and  ZKDelInfo.IsDeleted=0  ";
                var query = _sqlDapperRepository.Query<ShowPageList>(sql).AsQueryable();

                var resultCount = query.Count();
                //排序，分页
                results = query
                    .OrderBy(dto.Sorting)
                    .PageBy(dto)
                    .ToList();
               
                returnresult.zklist = results;

            }



           
            return returnresult;
        }      

        private IQueryable<ShowPageList> GetXDInfoListQuery()
        {
            var query = from XDDelInfo in _BoxInfoRepository.GetAll()
                        .Where(p => p.IsEnable && p.IsVerify && string.IsNullOrEmpty(p.VerifyRem))

                        join site in _SiteTableRepository.GetAll() on XDDelInfo.StartStation equals site.Code into startsite
                        from startline in startsite.DefaultIfEmpty()

                        join site in _SiteTableRepository.GetAll() on XDDelInfo.EndStation equals site.Code into endsite
                        from endline in endsite.DefaultIfEmpty()

                        join line in _LineRepository.GetAll() on XDDelInfo.Line equals line.Id into lines
                        from xdline in lines.DefaultIfEmpty()


                        select new ShowPageList
                        {
                            Id = XDDelInfo.Id,
                            type = "XD",
                            BillNO = XDDelInfo.BillNO,
                            StartStation = string.IsNullOrEmpty(startline.SiteName) ? XDDelInfo.StartStation : startline.SiteName,
                            EndStation = string.IsNullOrEmpty(endline.SiteName) ? XDDelInfo.EndStation : endline.SiteName,
                            ReturnStation = XDDelInfo.ReturnStation,
                            IsInStock = XDDelInfo.IsInStock,
                            PredictTime = XDDelInfo.PredictTime,
                            EffectiveSTime = XDDelInfo.EffectiveSTime.ToString("yyyy-MM-dd"),
                            EffectiveETime = XDDelInfo.EffectiveETime.ToString("yyyy-MM-dd"),
                            Price = XDDelInfo.SellingPrice,
                            Line = xdline.LineName,                          
                            Finish = XDDelInfo.Finish,
                            CreationTime = XDDelInfo.CreationTime,
                        };


            return query;
        }

        private IQueryable<ShowPageList> GetZKInfoListQuery( )
        {
            var query = from ZKDelInfo in _TenantInfoRepository.GetAll().Where(p => p.IsEnable && p.IsVerify && string.IsNullOrEmpty(p.VerifyRem))
                            

                        join site in _SiteTableRepository.GetAll() on ZKDelInfo.StartStation equals site.Code into startsite
                        from startline in startsite.DefaultIfEmpty()

                        join site in _SiteTableRepository.GetAll() on ZKDelInfo.EndStation equals site.Code into endsite
                        from endline in endsite.DefaultIfEmpty()

                        join line in _LineRepository.GetAll() on ZKDelInfo.Line equals line.Id into lines
                        from zkline in lines.DefaultIfEmpty()


                        select new ShowPageList
                        {
                            Id = ZKDelInfo.Id,
                            type="ZK",
                            BillNO = ZKDelInfo.BillNO,
                            StartStation = string.IsNullOrEmpty(startline.SiteName) ? ZKDelInfo.StartStation : startline.SiteName,
                            EndStation = string.IsNullOrEmpty(endline.SiteName) ? ZKDelInfo.EndStation : endline.SiteName,
                            EffectiveSTime = ZKDelInfo.EffectiveSTime.ToString("yyyy-MM-dd"),
                            EffectiveETime = ZKDelInfo.EffectiveETime.ToString("yyyy-MM-dd"),
                            Price = ZKDelInfo.HopePrice,
                            Line = zkline.LineName,
                            Finish = ZKDelInfo.Finish,
                            CreationTime = ZKDelInfo.CreationTime,
                        };


            return query;
        }
        #endregion
    }
}