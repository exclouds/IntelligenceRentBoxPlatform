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
using Admin.Application.Custom.API.OnlineSearch.Dto;

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
        public async Task<List<ShowPageList>> GetIntelligRcommendList(InteRcommenDto  dto)
        {         
            int UserNature = -1;
            if (AbpSession.UserId.HasValue)
            {
                var usrinfo = await _userRepository.GetAsync(AbpSession.UserId.Value);
                if (usrinfo == null)
                {
                    throw new UserFriendlyException($"未查找到相关登录用户信息，请重新登录!");
                }
                UserNature = usrinfo.UserNature;
            }
            string sql = "";



            if (UserNature != 1)
            {
                 sql = @"
                     select XDDelInfo.Id, 'XD' AS type, XDDelInfo.BillNO, XDDelInfo.Line as LineID,
                     (case when isnull(startline.SiteName,'')= '' then XDDelInfo.StartStation else startline.SiteName end) as StartStation,
                    --(case when isnull(endline.SiteName,'') = '' then XDDelInfo.EndStation else endline.SiteName end) as EndStation,
                    XDDelInfo.EndStation,XDDelInfo.ReturnStation,
                    convert(varchar(50), XDDelInfo.EffectiveSTime, 111) as EffectiveSTime,
                    convert(varchar(50), XDDelInfo.EffectiveETime, 111) as EffectiveETime,
                    XDDelInfo.SellingPrice Price, xdline.LineName Line, XDDelInfo.Finish,XDDelInfo.CreationTime,u.Name

                     from  BoxInfos XDDelInfo
                    left join   SiteTables startline on XDDelInfo.StartStation = startline.Code
                    --left join  SiteTables endline on XDDelInfo.EndStation = endline.Code
                    left join  Lines xdline on XDDelInfo.Line = xdline.Id
                    left join  AbpUsers u on XDDelInfo.CreatorUserId=u.id

                    Where XDDelInfo.IsEnable = 1 AND XDDelInfo.IsVerify = 1 AND ISNULL(XDDelInfo.VerifyRem,'')= '' and XDDelInfo.IsDeleted = 0";

              
            }
            if (UserNature ==-1)
            {
                sql += " union ";
            }

            if ( UserNature != 0)
            {
                 sql += @"   select  ZKDelInfo.Id, 'ZK' AS type,ZKDelInfo.BillNO, ZKDelInfo.Line as LineID,
                         (case when isnull(startline.SiteName,'')='' then  ZKDelInfo.StartStation else startline.SiteName end) as StartStation,
                        --(case when isnull(endline.SiteName,'') ='' then ZKDelInfo.EndStation else endline.SiteName end) as EndStation,
                        ZKDelInfo.EndStation,'' AS ReturnStation,
                        convert(varchar(50),ZKDelInfo.EffectiveSTime,111)    as EffectiveSTime,
                        convert(varchar(50),ZKDelInfo.EffectiveETime,111)  as  EffectiveETime,
                        ZKDelInfo.HopePrice Price,xdline.LineName Line, ZKDelInfo.Finish,ZKDelInfo.CreationTime,u.Name
                      

                        from  TenantInfos ZKDelInfo
                        left join   SiteTables startline on ZKDelInfo.StartStation = startline.Code 
                      --  left join  SiteTables endline on ZKDelInfo.EndStation = endline.Code 
                        left join  Lines xdline on ZKDelInfo.Line = xdline.Id 
                        left join  AbpUsers u on ZKDelInfo.CreatorUserId=u.id
                        Where ZKDelInfo.IsEnable=1 AND ZKDelInfo.IsVerify=1 AND ISNULL(ZKDelInfo.VerifyRem,'')='' and  ZKDelInfo.IsDeleted=0  ";             
            }

            var query = _sqlDapperRepository.Query<ShowPageList>(sql).AsQueryable();

            var resultCount = query.Count();
            //排序，分页
            var results = query
                .OrderBy(dto.Sorting)
                .PageBy(dto)
                .ToList();
            var allids = results.Select(p => p.BillNO).ToList();

            if (allids.Count > 0)
            {
                //var alldetail = _BoxDetailsRepository.GetAll().Where(p => allids.Contains(p.BoxTenantInfoNO)).ToList();
                string billnos = string.Join("','", allids);
                string bosql = "select * from BoxDetailses where isdeleted=0 and BoxTenantInfoNO in('" + billnos + "')";
                var query1 = _sqlDapperRepository.Query<BoxDetails>(bosql).AsQueryable();
                var alldetail = query1.ToList();

                string sitesql = "select * from SiteTables where isdeleted=0 ";
                var query2 = _sqlDapperRepository.Query<SiteTable>(sitesql).AsQueryable();
                var allsites = query2.ToList();

                results.ForEach(item =>
                {
                    var boxdetai = alldetail.Where(p => p.BoxTenantInfoNO == item.BillNO).ToList();
                   
                    if (boxdetai.Count > 0)
                    {
                        item.xxcc = string.Join(",", boxdetai.GroupBy(p => new { p.Size, p.Box }).Select(p => p.FirstOrDefault().Size + p.FirstOrDefault().Box + "X" + p.Sum(P => P.Quantity).ToString()).Distinct().ToList());

                    }

                    if (string.IsNullOrEmpty(item.EndStation))
                    {
                        item.EndStation = "全部路线站点";
                    }
                    else
                    {
                        var sitelis = allsites.Where(p => ("," + item.EndStation + ",").Contains("," + p.Code + ",")).Select(p => p.SiteName).ToList();
                        item.EndStation = string.Join(",", sitelis);
                    }

                });
            }
            return results;
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
                            //IsInStock = XDDelInfo.IsInStock,
                            //PredictTime = XDDelInfo.PredictTime,
                            EffectiveSTime = XDDelInfo.EffectiveSTime.ToString("yyyy-MM-dd"),
                            EffectiveETime = XDDelInfo.EffectiveETime.ToString("yyyy-MM-dd"),
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
                            Line = zkline.LineName,
                            Finish = ZKDelInfo.Finish,
                            CreationTime = ZKDelInfo.CreationTime,
                        };


            return query;
        }
        #endregion

        #region 获取单个发布信息详细信息
        public async Task<ShowPageInfo> GetSingleInfo(int id, string type)
        {
            ShowPageInfo info = new ShowPageInfo();
            if (type == "XD")
            {
                var query = from DelInfo in _BoxInfoRepository.GetAll().Where(p => p.Id == id)
                            //防止单据状态变更
                            .Where(p => p.IsEnable && p.IsVerify && string.IsNullOrEmpty(p.VerifyRem))

                            join site in _SiteTableRepository.GetAll() on DelInfo.StartStation equals site.Code into startsite
                            from startline in startsite.DefaultIfEmpty()

                            join site in _SiteTableRepository.GetAll() on DelInfo.ReturnStation equals site.Code into endsite
                            from endline in endsite.DefaultIfEmpty()

                            join line in _LineRepository.GetAll() on DelInfo.Line equals line.Id into lines
                            from xdline in lines.DefaultIfEmpty()
                            select new ShowPageInfo
                            {
                                Id = DelInfo.Id,
                                BillNO = DelInfo.BillNO.Trim().ToUpper(),
                                StartStation = string.IsNullOrEmpty(startline.SiteName) ? DelInfo.StartStation : startline.SiteName,
                                //EndStation = string.IsNullOrEmpty(endline.SiteName) ? DelInfo.EndStation : endline.SiteName,
                                ReturnStation = string.IsNullOrEmpty(endline.SiteName) ? DelInfo.ReturnStation : endline.SiteName,
                                EndStation = DelInfo.EndStation,
                                EffectiveSTime = DelInfo.EffectiveSTime.ToString("yyyy-MM-dd"),
                                EffectiveETime = DelInfo.EffectiveETime.ToString("yyyy-MM-dd"),
                                Line = xdline.LineName,
                                //PredictTime = DelInfo.PredictTime.HasValue? DelInfo.PredictTime.Value.ToString("yyyy-MM-dd"):"",
                                //IsInStock =DelInfo.IsInStock == true ? "是" : "否",
                                Finish = DelInfo.Finish==true?"是":"否",
                                BoxDetails = new List<XDSeachDtailDto>(),
                                fileList = new List<FileInfoModel>()
                            };
                info = query.FirstOrDefault();
            }
            else if (type == "ZK")
            {
                var query = from DelInfo in _TenantInfoRepository.GetAll().Where(p => p.Id == id)
                            join site in _SiteTableRepository.GetAll() on DelInfo.StartStation equals site.Code into startsite
                            from startline in startsite.DefaultIfEmpty()

                            join site in _SiteTableRepository.GetAll() on DelInfo.EndStation equals site.Code into endsite
                            from endline in endsite.DefaultIfEmpty()

                            join line in _LineRepository.GetAll() on DelInfo.Line equals line.Id into lines
                            from xdline in lines.DefaultIfEmpty()
                            select new ShowPageInfo
                            {
                                Id = DelInfo.Id,
                                BillNO = DelInfo.BillNO.Trim().ToUpper(),
                                StartStation = string.IsNullOrEmpty(startline.SiteName) ? DelInfo.StartStation : startline.SiteName,
                                // EndStation = string.IsNullOrEmpty(endline.SiteName) ? DelInfo.EndStation : endline.SiteName,
                                EndStation = DelInfo.EndStation,
                                EffectiveSTime = DelInfo.EffectiveSTime.ToString("yyyy-MM-dd"),
                                EffectiveETime = DelInfo.EffectiveETime.ToString("yyyy-MM-dd"),
                                Line = xdline.LineName,
                               
                                Finish = DelInfo.Finish == true ? "是" : "否",
                                BoxDetails = new List<XDSeachDtailDto>(),
                                fileList = new List<FileInfoModel>()
                            };
                info = query.FirstOrDefault();
            }
          
            if (info == null)
            {
                throw new UserFriendlyException($"未查找到相关信息!");
            }
            if (string.IsNullOrEmpty(info.EndStation))
            {
                info.EndStation = "全部路线站点";
            }
            else
            {
                var sitelis = _SiteTableRepository.GetAll().Where(p => ("," + info.EndStation + ",").Contains("," + p.Code + ",")).Select(p => p.SiteName).ToList();
                info.EndStation = string.Join(",", sitelis);
            }
            var dtnow = DateTime.Now.Year;
            var allfeelis = _BoxDetailsRepository.GetAll().Where(p => p.BoxTenantInfoNO.ToUpper() == info.BillNO.ToUpper())
                          .Select(p => new XDSeachDtailDto
                          {
                              Id = p.Id,
                              BoxNO = p.BoxNO,
                              Box = p.Box,
                              Size = p.Size,
                              //BoxAge = p.BoxAge,
                              Quantity = p.Quantity,
                              BoxAge = p.BoxTime.HasValue ? (dtnow - p.BoxTime.Value.Year).ToString() : "",
                              MaxWeight = p.MaxWeight,
                              BoxLabel = p.BoxLabel,
                              BoxTime = p.BoxTime,
                              FreezerModel = p.FreezerModel,
                          }).ToList();
            info.BoxDetails = allfeelis;

            var url = _httpContextAccessor.HttpContext.Request.Host;//取后台Url路径
            var allfile = _AttachmentInfoRepository.GetAll().Where(p => p.ContainerName == info.Id.ToString())
                .Select(p => new FileInfoModel
                {
                    Id = p.Id,
                    Name = p.Name,
                    Url = "http://" + url.Value + "/DBService/" + p.Url,
                    CreationTime = p.CreationTime
                }).ToList();
            info.fileList = allfile;

            return info;
        }
        #endregion
    }
}
