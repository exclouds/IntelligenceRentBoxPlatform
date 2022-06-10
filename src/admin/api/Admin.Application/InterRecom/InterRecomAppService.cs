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
using Magicodes.Admin.Attachments;
using Microsoft.AspNetCore.Http;
using Magicodes.Admin.InterRecom.Dto;
using Microsoft.AspNetCore.Mvc;

namespace Magicodes.Admin.InterRecom
{
    public class InterRecomAppService : AdminAppServiceBase
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

        public InterRecomAppService(IRepository<BoxInfo, int> BoxInfoRepository,
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
        [HttpPost]
        public async Task<List<ShowPageList>> GetInterRecomList(InteRcommenDto dto)
        {
            string belong = "";
            if (dto.belong.IsNullOrEmpty())
            {
                if (!dto.billNo.IsNullOrEmpty())
                {
                    dto.belong = dto.billNo.Substring(0, 2);
                }
            }
            belong = dto.belong;

            string sql = "";

            if (belong == "ZK")
            {

                string sqlstr = @"
                      select distinct XDDelInfo.Id, 'XD' AS type, XDDelInfo.BillNO, 
                      (case when isnull(startline.SiteName,'')= '' then XDDelInfo.StartStation else startline.SiteName end) as StartStation,
                      (case when isnull(endline.SiteName,'') = '' then XDDelInfo.EndStation else endline.SiteName end) as EndStation,
                      XDDelInfo.ReturnStation,XDDelInfo.IsInStock,XDDelInfo.PredictTime,
                      convert(varchar(50), XDDelInfo.EffectiveSTime, 111) as EffectiveSTime,
                      convert(varchar(50), XDDelInfo.EffectiveETime, 111) as EffectiveETime,
                      XDDelInfo.SellingPrice Price, xdline.LineName Line, XDDelInfo.Finish,XDDelInfo.CreationTime,u.Name,XDDelInfo.Remarks
                      
                      from  BoxInfos XDDelInfo
                      left join SiteTables startline on XDDelInfo.StartStation = startline.Code
                      left join SiteTables endline on XDDelInfo.EndStation = endline.Code
                      left join Lines xdline on XDDelInfo.Line = xdline.Id
                      left join AbpUsers u on XDDelInfo.CreatorUserId=u.id
                      left join BoxDetailses det on XDDelInfo.BillNO=det.BoxTenantInfoNO
                      Where XDDelInfo.IsEnable = 1 AND XDDelInfo.IsVerify = 1 AND ISNULL(XDDelInfo.VerifyRem,'')= '' and XDDelInfo.IsDeleted = 0";
                if (!dto.billNo.IsNullOrEmpty())
                {
                    var box = _BoxInfoRepository.GetAll().Where(b => b.BillNO == dto.billNo).FirstOrDefault();
                    if (box != null)
                    {
                        var boxdet = _BoxDetailsRepository.GetAll().Where(b => b.BoxTenantInfoNO == box.BillNO);

                        var cc = boxdet == null ? "" : boxdet.Select(b => b.Size).ToArray().JoinAsString("','");
                        var xx = boxdet == null ? "" : boxdet.Select(b => b.Box).ToArray().JoinAsString("','");
                        //1
                        sql = sqlstr + @" and (det.Size in ('" + cc + "')" +
                                    " and det.Box in ('" + xx + "')" +
                                    " and XDDelInfo.line like '%" + box.Line + "%'" +
                                    " and XDDelInfo.StartStation like '%" + box.StartStation + "%'" +
                                    " and XDDelInfo.EndStation like '%" + box.EndStation + "%'" +
                                    " and XDDelInfo.EffectiveSTime like '%" + box.EffectiveSTime + "%'" +
                                    " and XDDelInfo.EffectiveETime like '%" + box.EffectiveETime + "%')";
                        //2
                        sql += " union all";
                        sql = sqlstr + @" and (det.Size in ('" + cc + "')" +
                                    " and det.Box in ('" + xx + "')" +
                                    " and XDDelInfo.line like '%" + box.Line + "%'" +
                                    " and (XDDelInfo.StartStation like '%" + box.StartStation + "%'" +
                                    " or XDDelInfo.EndStation like '%" + box.EndStation + "%')" +
                                    " and (XDDelInfo.EffectiveSTime like '%" + box.EffectiveSTime + "%'" +
                                    " or XDDelInfo.EffectiveETime like '%" + box.EffectiveETime + "%'))";
                        //3
                        sql += " union all";
                        sql = sqlstr + @" and (det.Size in ('" + cc + "')" +
                                    " or det.Box in ('" + xx + "')" +
                                    " or (XDDelInfo.line like '%" + box.Line + "%'" +
                                    " and XDDelInfo.StartStation like '%" + box.StartStation + "%'" +
                                    " and XDDelInfo.EndStation like '%" + box.EndStation + "%'" +
                                    " and XDDelInfo.EffectiveSTime like '%" + box.EffectiveSTime + "%'" +
                                    " and XDDelInfo.EffectiveETime like '%" + box.EffectiveETime + "%'))";
                        //4
                        sql += " union all";
                        sql = sqlstr + @" and (det.Size in ('" + cc + "')" +
                                    " or det.Box in ('" + xx + "')" +
                                    " or (XDDelInfo.line like '%" + box.Line + "%'" +
                                    " and (XDDelInfo.StartStation like '%" + box.StartStation + "%'" +
                                    " or XDDelInfo.EndStation like '%" + box.EndStation + "%')" +
                                    " and (XDDelInfo.EffectiveSTime like '%" + box.EffectiveSTime + "%'" +
                                    " or XDDelInfo.EffectiveETime like '%" + box.EffectiveETime + "%'))";
                        //5
                        sql += " union all";
                        sql = sqlstr + @" and (det.Size in ('" + cc + "')" +
                                    " and det.Box in ('" + xx + "')" +
                                    " and XDDelInfo.line like '%" + box.Line + "%'" +
                                    " or (XDDelInfo.StartStation like '%" + box.StartStation + "%'" +
                                    " or XDDelInfo.EndStation like '%" + box.EndStation + "%')" +
                                    " or (XDDelInfo.EffectiveSTime like '%" + box.EffectiveSTime + "%'" +
                                    " or XDDelInfo.EffectiveETime like '%" + box.EffectiveETime + "%'))";
                        //6
                        sql += " union all";
                        sql = sqlstr + @" and (det.Size in ('" + cc + "')" +
                                    " and det.Box in ('" + xx + "')" +
                                    " and XDDelInfo.line like '%" + box.Line + "%'" +
                                    " or XDDelInfo.StartStation like '%" + box.StartStation + "%'" +
                                    " or XDDelInfo.EndStation like '%" + box.EndStation + "%'" +
                                    " or XDDelInfo.EffectiveSTime like '%" + box.EffectiveSTime + "%'" +
                                    " or XDDelInfo.EffectiveETime like '%" + box.EffectiveETime + "%')";

                    }
                }
                else
                {
                    sql = sqlstr;
                }
            }

            //if (belong == "")
            //{
            //    sql += " union ";
            //}

            if (belong == "XD")
            {
                string sqlstr = @"select distinct ZKDelInfo.Id, 'ZK' AS type,ZKDelInfo.BillNO, 
                        (case when isnull(startline.SiteName,'')='' then  ZKDelInfo.StartStation else startline.SiteName end) as StartStation,
                        (case when isnull(endline.SiteName,'') ='' then ZKDelInfo.EndStation else endline.SiteName end) as EndStation,
                        '' AS ReturnStation,1 AS  IsInStock,NULL AS PredictTime,
                        convert(varchar(50),ZKDelInfo.EffectiveSTime,111)    as EffectiveSTime,
                        convert(varchar(50),ZKDelInfo.EffectiveETime,111)  as  EffectiveETime,
                        ZKDelInfo.HopePrice Price,xdline.LineName Line, ZKDelInfo.Finish,ZKDelInfo.CreationTime,u.Name,ZKDelInfo.Remarks
                      
                        from TenantInfos ZKDelInfo
                        left join SiteTables startline on ZKDelInfo.StartStation = startline.Code 
                        left join SiteTables endline on ZKDelInfo.EndStation = endline.Code 
                        left join Lines xdline on ZKDelInfo.Line = xdline.Id 
                        left join AbpUsers u on ZKDelInfo.CreatorUserId=u.id
                        left join BoxDetailses det on ZKDelInfo.BillNO=det.BoxTenantInfoNO
                        Where ZKDelInfo.IsEnable=1 AND ZKDelInfo.IsVerify=1 AND ISNULL(ZKDelInfo.VerifyRem,'')='' and  ZKDelInfo.IsDeleted=0  ";
                if (!dto.billNo.IsNullOrEmpty())
                {
                    var ten = _TenantInfoRepository.GetAll().Where(b => b.BillNO == dto.billNo).FirstOrDefault();
                    if (ten != null)
                    {
                        var boxdet = _BoxDetailsRepository.GetAll().Where(b => b.BoxTenantInfoNO == ten.BillNO);

                        var cc = boxdet == null ? "" : boxdet.Select(b => b.Size).ToArray().JoinAsString("','");
                        var xx = boxdet == null ? "" : boxdet.Select(b => b.Box).ToArray().JoinAsString("','");
                        //1
                        sql = sqlstr + @" and (det.Size in ('" + cc + "')" +
                                    " and det.Box in ('" + xx + "')" +
                                    " and ZKDelInfo.line like '%" + ten.Line + "%'" +
                                    " and ZKDelInfo.StartStation like '%" + ten.StartStation + "%'" +
                                    " and ZKDelInfo.EndStation like '%" + ten.EndStation + "%'" +
                                    " and ZKDelInfo.EffectiveSTime like '%" + ten.EffectiveSTime + "%'" +
                                    " and ZKDelInfo.EffectiveETime like '%" + ten.EffectiveETime + "%')";
                        //2
                        sql += " union all";
                        sql = sqlstr + @" and (det.Size in ('" + cc + "')" +
                                    " and det.Box in ('" + xx + "')" +
                                    " and ZKDelInfo.line like '%" + ten.Line + "%'" +
                                    " and (ZKDelInfo.StartStation like '%" + ten.StartStation + "%'" +
                                    " or ZKDelInfo.EndStation like '%" + ten.EndStation + "%')" +
                                    " and (ZKDelInfo.EffectiveSTime like '%" + ten.EffectiveSTime + "%'" +
                                    " or ZKDelInfo.EffectiveETime like '%" + ten.EffectiveETime + "%'))";
                        //3
                        sql += " union all";
                        sql = sqlstr + @" and (det.Size in ('" + cc + "')" +
                                    " or det.Box in ('" + xx + "')" +
                                    " or (ZKDelInfo.line like '%" + ten.Line + "%'" +
                                    " and ZKDelInfo.StartStation like '%" + ten.StartStation + "%'" +
                                    " and ZKDelInfo.EndStation like '%" + ten.EndStation + "%'" +
                                    " and ZKDelInfo.EffectiveSTime like '%" + ten.EffectiveSTime + "%'" +
                                    " and ZKDelInfo.EffectiveETime like '%" + ten.EffectiveETime + "%'))";
                        //4
                        sql += " union all";
                        sql = sqlstr + @" and (det.Size in ('" + cc + "')" +
                                    " or det.Box in ('" + xx + "')" +
                                    " or (ZKDelInfo.line like '%" + ten.Line + "%'" +
                                    " and (ZKDelInfo.StartStation like '%" + ten.StartStation + "%'" +
                                    " or ZKDelInfo.EndStation like '%" + ten.EndStation + "%')" +
                                    " and (ZKDelInfo.EffectiveSTime like '%" + ten.EffectiveSTime + "%'" +
                                    " or ZKDelInfo.EffectiveETime like '%" + ten.EffectiveETime + "%'))";
                        //5
                        sql += " union all";
                        sql = sqlstr + @" and (det.Size in ('" + cc + "')" +
                                    " and det.Box in ('" + xx + "')" +
                                    " and ZKDelInfo.line like '%" + ten.Line + "%'" +
                                    " or (ZKDelInfo.StartStation like '%" + ten.StartStation + "%'" +
                                    " or ZKDelInfo.EndStation like '%" + ten.EndStation + "%')" +
                                    " or (ZKDelInfo.EffectiveSTime like '%" + ten.EffectiveSTime + "%'" +
                                    " or ZKDelInfo.EffectiveETime like '%" + ten.EffectiveETime + "%'))";
                        //6
                        sql += " union all";
                        sql = sqlstr + @" and (det.Size in ('" + cc + "')" +
                                    " and det.Box in ('" + xx + "')" +
                                    " and ZKDelInfo.line like '%" + ten.Line + "%'" +
                                    " or ZKDelInfo.StartStation like '%" + ten.StartStation + "%'" +
                                    " or ZKDelInfo.EndStation like '%" + ten.EndStation + "%'" +
                                    " or ZKDelInfo.EffectiveSTime like '%" + ten.EffectiveSTime + "%'" +
                                    " or ZKDelInfo.EffectiveETime like '%" + ten.EffectiveETime + "%')";
                    }
                }
                else
                {
                    sql = sqlstr;
                }
            }
            sql = "select distinct * from (" + sql + ") t";
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
                results.ForEach(item =>
                {
                    var boxdetai = alldetail.Where(p => p.BoxTenantInfoNO == item.BillNO).ToList();

                    if (boxdetai.Count > 0)
                    {
                        item.xxcc = string.Join(",", boxdetai.GroupBy(p => new { p.Size, p.Box }).Select(p => p.FirstOrDefault().Size + p.FirstOrDefault().Box + "X" + p.Sum(P => P.Quantity).ToString()).Distinct().ToList());

                    }

                });
            }
            return results;
        }

        /// <summary>
        /// 搜索箱东订单匹配租客订单
        /// </summary>
        /// <param name="dto"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<ShowResultPageList> GetInterRecomAllList(InteRcommenDto dto)
        {
            string sql = @"
                       select distinct XDDelInfo.Id, 'XD' AS type, XDDelInfo.BillNO, 
                      (case when isnull(startline.SiteName,'')= '' then XDDelInfo.StartStation else startline.SiteName end) as StartStation,
                      (case when isnull(endline.SiteName,'') = '' then XDDelInfo.EndStation else endline.SiteName end) as EndStation,XDDelInfo.ReturnStation,
                      convert(varchar(50), XDDelInfo.EffectiveSTime, 111) as EffectiveSTime,
                      convert(varchar(50), XDDelInfo.EffectiveETime, 111) as EffectiveETime,
                      XDDelInfo.SellingPrice Price, xdline.LineName Line, XDDelInfo.Finish,XDDelInfo.CreationTime,u.Name,XDDelInfo.Remarks
                      
                      from  BoxInfos XDDelInfo
                      left join SiteTables startline on XDDelInfo.StartStation = startline.Code
                      left join SiteTables endline on XDDelInfo.EndStation = endline.Code
                      left join Lines xdline on XDDelInfo.Line = xdline.Id
                      left join AbpUsers u on XDDelInfo.CreatorUserId=u.id
                      left join BoxDetailses det on XDDelInfo.BillNO=det.BoxTenantInfoNO
                      Where XDDelInfo.IsEnable = 1 AND XDDelInfo.IsVerify = 1 AND ISNULL(XDDelInfo.VerifyRem,'')= '' and XDDelInfo.IsDeleted = 0 ";
            if (!dto.StartStation.IsNullOrEmpty())
            {
                sql += " and XDDelInfo.StartStation='"+dto.StartStation+"'";
            }
            if (!dto.EndStation.IsNullOrEmpty())
            {
                sql += " and XDDelInfo.EndStation='" + dto.EndStation + "'";
            }
            if (dto.Line.HasValue)
            {
                sql += " and Size='"+dto.Size+"'";
            }
            if (!dto.Box.IsNullOrEmpty())
            {
                sql += " and Box='" + dto.Box + "'";
            }
            if (dto.EffectiveSTime.HasValue)
            {
                sql += " and EffectiveSTime>='"+dto.EffectiveSTime+"'";
            }
            if (dto.EffectiveETime.HasValue)
            {
                sql += " and EffectiveETime<='" + dto.EffectiveETime + "'";
            }
            sql = string.Format(sql, dto.StartStation, dto.EndStation, dto.Line, dto.Size, dto.Box, dto.EffectiveSTime, dto.EffectiveETime);
            var query = _sqlDapperRepository.Query<ShowPageList>(sql).AsQueryable();

            var resultCount = query.Count();
            //排序，分页
            var xdResults = query
                .OrderBy(dto.Sorting)
                .PageBy(dto)
                .ToList();
            var allids = xdResults.Select(p => p.BillNO).ToList();

            if (allids.Count > 0)
            {
                //var alldetail = _BoxDetailsRepository.GetAll().Where(p => allids.Contains(p.BoxTenantInfoNO)).ToList();
                string billnos = string.Join("','", allids);
                string bosql = "select * from BoxDetailses where isdeleted=0 and BoxTenantInfoNO in('" + billnos + "')";
                var query1 = _sqlDapperRepository.Query<BoxDetails>(bosql).AsQueryable();
                var alldetail = query1.ToList();
                xdResults.ForEach(item =>
                {
                    var boxdetai = alldetail.Where(p => p.BoxTenantInfoNO == item.BillNO).ToList();
                    if (boxdetai.Count > 0)
                    {
                        item.xxcc = string.Join(",", boxdetai.GroupBy(p => new { p.Size, p.Box }).Select(p => p.FirstOrDefault().Size + p.FirstOrDefault().Box + "X" + p.Sum(P => P.Quantity).ToString()).Distinct().ToList());
                    }
                });
            }

            List<ShowPageList> zklist = new List<ShowPageList>();
            foreach (var item in xdResults)
            {
                var box = _BoxInfoRepository.Get(item.Id.Value);
                var boxde = _BoxDetailsRepository.GetAll().Where(b => b.BoxTenantInfoNO == box.BillNO).ToList();
                var cc = boxde.Select(b => b.Size).ToList().JoinAsString("','");
                var xx = boxde.Select(b => b.Box).ToList().JoinAsString("','");
                sql = @"select distinct ZKDelInfo.Id, 'ZK' AS type,ZKDelInfo.BillNO, 
                        (case when isnull(startline.SiteName,'')='' then  ZKDelInfo.StartStation else startline.SiteName end) as StartStation,
                        (case when isnull(endline.SiteName,'') ='' then ZKDelInfo.EndStation else endline.SiteName end) as EndStation,'' AS ReturnStation,
                        convert(varchar(50),ZKDelInfo.EffectiveSTime,111)    as EffectiveSTime,
                        convert(varchar(50),ZKDelInfo.EffectiveETime,111)  as  EffectiveETime,
                        ZKDelInfo.HopePrice Price,xdline.LineName Line, ZKDelInfo.Finish,ZKDelInfo.CreationTime,u.Name,ZKDelInfo.Remarks
                      
                        from TenantInfos ZKDelInfo
                        left join SiteTables startline on ZKDelInfo.StartStation = startline.Code 
                        left join SiteTables endline on ZKDelInfo.EndStation = endline.Code 
                        left join Lines xdline on ZKDelInfo.Line = xdline.Id 
                        left join AbpUsers u on ZKDelInfo.CreatorUserId=u.id
                        left join BoxDetailses det on ZKDelInfo.BillNO=det.BoxTenantInfoNO
                        Where ZKDelInfo.IsEnable=1 AND ZKDelInfo.IsVerify=1 AND ISNULL(ZKDelInfo.VerifyRem,'')='' and  ZKDelInfo.IsDeleted=0
                        and (
                        (ZKDelInfo.StartStation='{0}' and ZKDelInfo.EndStation='{1}' and Line='{2}' and Size in ('{3}') and Box in ('{4}') 
                        and EffectiveSTime>='{5}' and EffectiveETime<='{6}')
                        or ((ZKDelInfo.StartStation='{0}' or ZKDelInfo.EndStation='{1}') and Line='{2}' and Size in ('{3}') and Box in ('{4}') 
                        and (EffectiveSTime>='{5}' or EffectiveETime<='{6}'))
                        )";
                sql = string.Format(sql, box.StartStation, box.EndStation, box.Line, cc, xx, box.EffectiveSTime, box.EffectiveETime);
                var zkquery = _sqlDapperRepository.Query<ShowPageList>(sql).FirstOrDefault();
                if (zkquery == null)
                {
                    Random rd = new Random();
                    zklist.Add(new ShowPageList() { Id = rd.Next(1000000, 9999999) });
                }
                else
                {
                    zklist.Add(zkquery);
                }
            }

            var allids2 = zklist.Select(p => p.BillNO).ToList();

            if (allids2.Count > 0)
            {
                //var alldetail = _BoxDetailsRepository.GetAll().Where(p => allids.Contains(p.BoxTenantInfoNO)).ToList();
                string billnos = string.Join("','", allids2);
                string bosql = "select * from BoxDetailses where isdeleted=0 and BoxTenantInfoNO in('" + billnos + "')";
                var query1 = _sqlDapperRepository.Query<BoxDetails>(bosql).AsQueryable();
                var alldetail = query1.ToList();
                zklist.ForEach(item =>
                {
                    var boxdetai = alldetail.Where(p => p.BoxTenantInfoNO == item.BillNO).ToList();

                    if (boxdetai.Count > 0)
                    {
                        item.xxcc = string.Join(",", boxdetai.GroupBy(p => new { p.Size, p.Box }).Select(p => p.FirstOrDefault().Size + p.FirstOrDefault().Box + "X" + p.Sum(P => P.Quantity).ToString()).Distinct().ToList());

                    }

                });
            }
            ShowResultPageList srp = new ShowResultPageList();
            srp.xdlist = xdResults;
            srp.zklist = zklist;
            return srp;
        }
        #endregion
    }
}
