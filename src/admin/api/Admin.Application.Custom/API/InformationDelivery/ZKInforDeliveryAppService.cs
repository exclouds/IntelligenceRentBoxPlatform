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
using Admin.Application.Custom.API.InformationDelivery.ZKDto;
using System.Linq.Dynamic.Core;
using Abp.Linq.Extensions;
using Microsoft.EntityFrameworkCore;
using Magicodes.Admin.Core.Custom.Business;
using Abp.AutoMapper;
using Admin.Application.Custom.API.PublicArea.Annex.Dto;
using Magicodes.Admin.Attachments;
using Microsoft.AspNetCore.Http;
using Admin.Application.Custom.API.PublicArea.PubContactNO;

namespace Admin.Application.Custom.API.InformationDelivery
{
    public class ZKInforDeliveryAppService : AppServiceBase
    {
        #region 注入依赖
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

        public ZKInforDeliveryAppService(IRepository<TenantInfo, int> TenantInfoRepository
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
            _TenantInfoRepository = TenantInfoRepository;
            _organizationUnitRepository = organizationUnitRepository;
            _userRepository = userRepository;
            _AttachmentInfoRepository = AttachmentInfoRepository;
            _sqlDapperRepository = sqlDapperRepository;
            _httpContextAccessor=httpContextAccessor;
            _IContactNOAPPService = IContactNOAPPService;
            _SiteTableRepository = SiteTableRepository;
            _LinSiteRepository = LinSiteRepository;
            _LineRepository = LineRepository;

        }
        #endregion

        #region  查询

        /// <summary>
        /// 查询
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public async Task<PagedResultDto<ZKDelInfoListDto>> GetZKDelInfoList(ZKDelInfoQueryDto input)
        {
            async Task<PagedResultDto<ZKDelInfoListDto>> GetListFunc()
            {
                var query = CreateZKDelInfoQuery(input);
                //获取行数
                var resultCount = await query.CountAsync();
                //排序，分页
                var results =  query
                    .OrderBy(input.Sorting)
                    .PageBy(input)
                    .ToList();
                return new PagedResultDto<ZKDelInfoListDto>(resultCount, results.MapTo<List<ZKDelInfoListDto>>());
            }
            return await GetListFunc();
        }

        private IQueryable<ZKDelInfoListDto> CreateZKDelInfoQuery(ZKDelInfoQueryDto input)
        {
            var query = from ZKDelInfo in _TenantInfoRepository.GetAll().Where(p => p.CreatorUserId == AbpSession.UserId)
                             .WhereIf(!input.BillNO.IsNullOrEmpty(), p => p.BillNO.Contains(input.BillNO.Trim().ToUpper()))
                              .WhereIf(!input.StartStation.IsNullOrEmpty(), p => p.StartStation == input.StartStation)
                              .WhereIf(!input.EndStation.IsNullOrEmpty(), p => p.EndStation == input.EndStation)
                             .WhereIf(input.IsVerify.HasValue, p => p.IsVerify == input.IsVerify)
                             .WhereIf(input.IsEnable.HasValue, p => p.IsEnable == input.IsEnable)
                              .WhereIf(input.Finish.HasValue, p => p.Finish == input.Finish)
                        join site in _SiteTableRepository.GetAll() on ZKDelInfo.StartStation equals site.Code into startsite
                          from startline in startsite.DefaultIfEmpty()

                        join site in _SiteTableRepository.GetAll() on ZKDelInfo.EndStation equals site.Code into endsite
                        from endline in endsite.DefaultIfEmpty()

                        join line in _LineRepository.GetAll() on ZKDelInfo.Line equals line.Id into lines
                        from zkline in lines.DefaultIfEmpty()


                        select new ZKDelInfoListDto
                        {
                            Id = ZKDelInfo.Id,
                            BillNO = ZKDelInfo.BillNO,
                            StartStation =string.IsNullOrEmpty(startline.SiteName)? ZKDelInfo.StartStation: startline.SiteName,
                            EndStation = string.IsNullOrEmpty(endline.SiteName) ? ZKDelInfo.EndStation : endline.SiteName ,
                          
                            EffectiveSTime = ZKDelInfo.EffectiveSTime,
                            EffectiveETime = ZKDelInfo.EffectiveETime,
                            HopePrice = ZKDelInfo.HopePrice,
                            Line = zkline.LineName,
                            IsEnable = ZKDelInfo.IsEnable,
                            IsVerify = ZKDelInfo.IsVerify,
                            VerifyRem = ZKDelInfo.VerifyRem,
                            InquiryNum = ZKDelInfo.InquiryNum,
                            Finish = ZKDelInfo.Finish,
                            CreationTime = ZKDelInfo.CreationTime,
                            Remarks = ZKDelInfo.Remarks,
                        };


            return query;
        }
        #endregion


        /// 获取基础费目编辑
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task<ZKDelInfoDto> GetZKDelInfoSingle(int id)
        {
            var query = from ZKDelInfo in  _TenantInfoRepository.GetAll().Where (p=>p.Id==id)
                        select new ZKDelInfoDto
                        {
                            Id = ZKDelInfo.Id,
                            BillNO = ZKDelInfo.BillNO.Trim().ToUpper(),
                            StartStation = ZKDelInfo.StartStation,
                            EndStation = ZKDelInfo.EndStation,
                            EffectiveSTime = ZKDelInfo.EffectiveSTime,
                            EffectiveETime = ZKDelInfo.EffectiveETime,
                            HopePrice = ZKDelInfo.HopePrice,
                            Line = ZKDelInfo.Line,
                            IsEnable = ZKDelInfo.IsEnable,
                            IsVerify = ZKDelInfo.IsVerify,
                            VerifyRem = ZKDelInfo.VerifyRem,
                            InquiryNum = ZKDelInfo.InquiryNum,
                            Finish = ZKDelInfo.Finish,                          
                            Remarks = ZKDelInfo.Remarks,
                            fileList=new List<FileInfoModel>()
                        };
            // var info = entity.MapTo<ZKDelInfoListDto>();


             var info = query.FirstOrDefault();

            if (info == null)
            {
                throw new UserFriendlyException($"未查找到相关信息!");
            }
            var url = _httpContextAccessor.HttpContext.Request.Host;//取后台Url路径
            var allfile = _AttachmentInfoRepository.GetAll().Where(p => p.ContainerName == info.Id.ToString())
                .Select(p => new FileInfoModel
                {
                    Id=p.Id,
                    Name=p.Name,
                    Url= "http://" + url.Value + "/" + p.Url
                }).ToList();
            info.fileList = allfile;

            return info;
        }

        #region 新增编辑
        /// <summary>
        /// 新增编辑
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public async Task ZKDelInfoAddEdit(ZKDelInfoListDto input)
        {
            if (input.Id.HasValue)
            {
                await UpdateZKDelInfoAsync(input);
            }
            else
            {               
                await CreateZKDelInfoAsync(input);
            }
        }

        private async Task UpdateZKDelInfoAsync(ZKDelInfoListDto dto)
        {
            var ZKDelInfo = await _TenantInfoRepository.GetAsync(dto.Id.Value);
            if (ZKDelInfo == null)
            {
                throw new UserFriendlyException($"未查找到相关信息!");
            }
           
            //ZKDelInfo.BillNO = dto.BillNO.Trim().ToUpper();
            ZKDelInfo.StartStation = dto.StartStation;
            ZKDelInfo.EndStation = dto.EndStation;
                          
            ZKDelInfo.EffectiveSTime = dto.EffectiveSTime;
            ZKDelInfo.EffectiveETime = dto.EffectiveETime;
            ZKDelInfo.HopePrice = dto.HopePrice;
            if (!string.IsNullOrEmpty(dto.Line))
            {
                ZKDelInfo.Line = int.Parse(dto.Line);

            }
            ZKDelInfo.IsEnable = dto.IsEnable.Value;
           
            ZKDelInfo.Remarks = dto.Remarks;

            ZKDelInfo.LastModifierUserId = AbpSession.UserId;
            ZKDelInfo.LastModificationTime = DateTime.Now;

           // return ZKDelInfo;


        }

        private async Task CreateZKDelInfoAsync(ZKDelInfoListDto dto)
        {
            string billno =await _IContactNOAPPService.GetBusNO("ZK");
            var ZKDelInfo = new TenantInfo()
            {
                Remarks = dto.Remarks,
                CreatorUserId = AbpSession.UserId,
                CreationTime = DateTime.Now,
                TenantId = AbpSession.TenantId,
                BillNO = billno,
                StartStation = dto.StartStation,
                EndStation = dto.EndStation,

                EffectiveSTime = dto.EffectiveSTime,
                EffectiveETime = dto.EffectiveETime,
                HopePrice = dto.HopePrice,
              
                IsEnable = dto.IsEnable.Value,
                IsVerify =false,
             
                InquiryNum = 0,
                Finish = false,

            };
            if (!string.IsNullOrEmpty(dto.Line))
            {
                ZKDelInfo.Line = int.Parse(dto.Line);

            }
            var id = await _TenantInfoRepository.InsertAndGetIdAsync(ZKDelInfo);
           // ZKDelInfo.Id = id;

            //return ZKDelInfo;


        }
        #endregion

        #region 批量删除
        /// <summary>
        /// 批量删除
        /// </summary>
        /// <param name="ids"></param>
        /// <returns></returns>
        public void BatchDelete(List<int> ids)
        {
            var alllist = _TenantInfoRepository.GetAll().Where(p => ids.Contains(p.Id)).ToList();
            if (alllist.Any(p => p.IsVerify))
            {
                throw new UserFriendlyException($"存在已审核的单据，不允许删除，请重新确认!");
            }
            if (alllist.Any(p => p.Finish))
            {
                throw new UserFriendlyException($"存在已完成的单据，不允许删除，请重新确认!");
            }
            foreach (var item in alllist)
            {
                item.IsDeleted = true;
                item.DeleterUserId = AbpSession.UserId;
                item.DeletionTime = DateTime.Now;
            }
            var idstr = ids.Select(p => p.ToString()).ToList();
            //删除附件
            var allfile = _AttachmentInfoRepository.GetAll().Where(p => !string.IsNullOrEmpty(p.ContainerName))
                .Where(p => idstr.Contains(p.ContainerName)).ToList();
            foreach (var item in allfile)
            {
                item.IsDeleted = true;
                item.DeleterUserId = AbpSession.UserId;
                item.DeletionTime = DateTime.Now;
            }
        }
        #endregion

        #region 批量启用或者停用
        public void BatchOP(BathOPDto dto)
        {
            var alllist = _TenantInfoRepository.GetAll().Where(p => dto.ids.Contains(p.Id)).ToList();
            if (alllist.Any(p => p.Finish))
            {
                throw new UserFriendlyException($"存在已完成的单据，不允许进行操作，请重新确认!");
            }
            //if (alllist.Any(p => p.IsVerify && string.IsNullOrEmpty(p.VerifyRem)))
            //{
            //    throw new UserFriendlyException($"存在已完成的单据，不允许进行操作，请重新确认!");
            //}
            foreach (var item in alllist)
            {
                item.IsEnable = dto.IsEnable;
              
            }      
        }
        #endregion
    }
}
