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
using Admin.Application.Custom.API.InformationDelivery.XDDto;
using Abp.AutoMapper;
using Admin.Application.Custom.API.PublicArea.Annex.Dto;
using Admin.Application.Custom.API.PublicArea.PubContactNO;
using Magicodes.Admin.Attachments;
using Microsoft.AspNetCore.Http;
using Admin.Application.Custom.API.InformationDelivery.ZKDto;

namespace Admin.Application.Custom.API.InformationDelivery
{
    public class XDInforDeliveryAppService : AppServiceBase
    {
        #region 注入依赖
        private readonly IRepository<BoxInfo, int> _BoxInfoRepository;
        private readonly IRepository<BoxDetails, int> _BoxDetailsRepository;
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

        public XDInforDeliveryAppService(IRepository<BoxInfo, int> BoxInfoRepository,
            IRepository<BoxDetails, int> BoxDetailsRepository
            , IRepository<MyOrganization, long> organizationUnitRepository
            , IRepository<User, long> userRepository,
            IRepository<AttachmentInfo, long> AttachmentInfoRepository,
            IDapperRepository<MyOrganization, long> sqlDapperRepository,
            IHttpContextAccessor httpContextAccessor,
           IContactNOAPPService IContactNOAPPService,
            IRepository<SiteTable, int> SiteTableRepository,
            IRepository<LinSite, int> LinSiteRepository,
            IRepository<Line, int> LineRepository
             )
        {
            _BoxInfoRepository = BoxInfoRepository;
            _BoxDetailsRepository = BoxDetailsRepository;
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

        #region  查询

        /// <summary>
        /// 查询
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public async Task<PagedResultDto<XDDelInfoListDto>> GetXDDelInfoList(XDDelInfoQueryDto input)
        {
            async Task<PagedResultDto<XDDelInfoListDto>> GetListFunc()
            {
                var query = CreateXDDelInfoQuery(input);
                //获取行数
                var resultCount =  query.Count();
                //排序，分页
                var results =  query
                    .OrderBy(input.Sorting)
                    .PageBy(input)
                    .ToList();

                var allids = results.Select(p => p.Id).ToList(); 
                if (allids.Count > 0)
                {
                    var alldetail = _BoxDetailsRepository.GetAll().Where(p => allids.Contains(p.BoxTenantInfo)).ToList();
                    results.ForEach(item => 
                    {
                        var boxdetai = alldetail.Where(p => p.BoxTenantInfo == item.Id).ToList();
                        item.xxcc = boxdetai.Count==0 ? "" : string.Join(",", boxdetai.Select(p => p.Size+ p.Box).Distinct().ToList());
                    });
                }
                return new PagedResultDto<XDDelInfoListDto>(resultCount, results.MapTo<List<XDDelInfoListDto>>());
            }
            return await GetListFunc();
        }

        private IQueryable<XDDelInfoListDto> CreateXDDelInfoQuery(XDDelInfoQueryDto input)
        {
            var query = from XDDelInfo in _BoxInfoRepository.GetAll().Where(p=>p.CreatorUserId==AbpSession.UserId)                
                             .WhereIf(!input.BillNO.IsNullOrEmpty(), p => p.BillNO.Contains(input.BillNO.Trim().ToUpper()))
                              .WhereIf(!input.StartStation.IsNullOrEmpty(), p => p.StartStation == input.StartStation)
                              .WhereIf(!input.EndStation.IsNullOrEmpty(), p => p.EndStation == input.EndStation)
                              .WhereIf(!input.ReturnStation.IsNullOrEmpty(), p => p.ReturnStation == input.ReturnStation)
                             .WhereIf(input.IsVerify.HasValue, p => p.IsVerify == input.IsVerify)
                             .WhereIf(input.IsEnable.HasValue, p => p.IsEnable == input.IsEnable)
                              .WhereIf(input.Finish.HasValue, p => p.Finish == input.Finish)
                              .WhereIf(input.IsInStock.HasValue, p => p.IsInStock == input.IsInStock)


                        join site in _SiteTableRepository.GetAll() on XDDelInfo.StartStation equals site.Code into startsite
                        from startline in startsite.DefaultIfEmpty()

                        join site in _SiteTableRepository.GetAll() on XDDelInfo.EndStation equals site.Code into endsite
                        from endline in endsite.DefaultIfEmpty()

                        join line in _LineRepository.GetAll() on XDDelInfo.Line equals line.Id into lines
                        from xdline in lines.DefaultIfEmpty()

                        //join boxdetail in _BoxDetailsRepository.GetAll().GroupBy(p=>p.BoxTenantInfo) on XDDelInfo.Id equals ( boxdetail.FirstOrDefault()==null?0: boxdetail.FirstOrDefault().BoxTenantInfo)


                        select new XDDelInfoListDto
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
                            IsEnable = XDDelInfo.IsEnable,
                            IsVerify = XDDelInfo.IsVerify,
                            VerifyRem = XDDelInfo.VerifyRem,
                            InquiryNum = XDDelInfo.InquiryNum,
                            Finish = XDDelInfo.Finish,
                            CreationTime = XDDelInfo.CreationTime,
                            Remarks = XDDelInfo.Remarks,
                           // xxcc= boxdetail==null? "":string.Join(",", boxdetail.Select(p => p.Box + p.Size).Distinct().ToList())
                        };


            return query;
        }
        #endregion

        #region 获取单个信息
        /// <summary>
        /// 获取基础
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task<XDDelInfoDto> GetXDDelInfoSingle(int id)
        {
            XDDelInfoDto info = new XDDelInfoDto()
            {
                BoxInfo=new BoxInfo (),
                BoxDetails=new List<BoxDetails> (),
                fileList=new List<FileInfoModel> ()
            };
          

            if (info == null)
            {
                throw new UserFriendlyException($"未查找到相关信息!");
            }

            var BoxInfo = _BoxInfoRepository.Get(id);
            info.BoxInfo = BoxInfo;

            var allfeelis = _BoxDetailsRepository.GetAll().Where(p => p.BoxTenantInfo == id).ToList();
            info.BoxDetails = allfeelis;

            var url = _httpContextAccessor.HttpContext.Request.Host;//取后台Url路径
            var allfile = _AttachmentInfoRepository.GetAll().Where(p => p.ContainerName == id.ToString())
                .Select(p => new FileInfoModel
                {
                    Id = p.Id,
                    Name = p.Name,
                    Url = "http://" + url.Value  +"/DBService/" + p.Url,
                    CreationTime= p.CreationTime
                }).ToList();
            info.fileList = allfile;

            return info;
        }
     
        #endregion

        #region 新增编辑
        /// <summary>
        /// 新增编辑
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public async Task<BoxInfo> XDDelInfoAddEdit(EditXDDelInfoDto input)
        {
            if (input.Id.HasValue)
            {
                var  model=await UpdateXDDelInfoAsync(input);
                return model;
            }
            else
            {
                var model = await CreateXDDelInfoAsync(input);
                return model;
            }
        }

        private async Task<BoxInfo> UpdateXDDelInfoAsync(EditXDDelInfoDto input)
        {
            var XDDelInfo = await _BoxInfoRepository.GetAsync(input.Id.Value);
            if (XDDelInfo == null)
            {
                throw new UserFriendlyException($"未查找到相关信息!");
            }
            XDDelInfo.StartStation = input.StartStation;
            XDDelInfo.EndStation = input.EndStation;
            XDDelInfo.ReturnStation = input.ReturnStation;
            XDDelInfo.IsInStock = input.IsInStock;                           
            XDDelInfo.PredictTime = input.PredictTime;                            
            XDDelInfo.EffectiveSTime = input.EffectiveSTime;
            XDDelInfo.EffectiveETime = input.EffectiveETime;
            XDDelInfo.SellingPrice = input.SellingPrice;
            XDDelInfo.Line = input.Line;
            XDDelInfo.IsEnable = input.IsEnable;
            //XDDelInfo.IsVerify = input.IsVerify;
           // XDDelInfo.VerifyRem = input.VerifyRem;
           // XDDelInfo.InquiryNum = input.InquiryNum;
            //XDDelInfo.Finish = input.Finish;
                            
            XDDelInfo.Remarks = input.Remarks;
            XDDelInfo.LastModifierUserId = AbpSession.UserId;
            XDDelInfo.LastModificationTime = DateTime.Now;

            foreach (var item in input.BoxDetails)
            {
                if (item.Id.HasValue)
                {
                    await UpdateDetailsInfoAsync(item);
                }
                else
                {
                    await CreateDetailsoAsync(input.Id.Value,item);
                }
            }
            return XDDelInfo;
        }

        private async Task CreateDetailsoAsync(int id,EditXDDetailsDto input)
        {
            var XDDetails = new BoxDetails()
            {
                Remarks = input.Remarks,
                CreatorUserId = AbpSession.UserId,
                CreationTime = DateTime.Now,
                TenantId = AbpSession.TenantId,
                BoxTenantInfo = id,
                Box = input.Box,
                Size = input.Size,
                Quantity = input.Quantity,
                BoxNO = input.BoxNO,
                BoxAge = input.BoxAge,
                IsVerify = false,
               


            };
            await _BoxDetailsRepository.InsertAndGetIdAsync(XDDetails);
        }

        private async Task UpdateDetailsInfoAsync(EditXDDetailsDto input)
        {
            var XDDetails = await _BoxDetailsRepository.GetAsync(input.Id.Value);
            if (XDDetails == null)
            {
                throw new UserFriendlyException($"未查找到相关信息!");
            }
            XDDetails.Remarks = input.Remarks;                              
            XDDetails.Box = input.Box;
            XDDetails.Size = input.Size;
            XDDetails.Quantity = input.Quantity;
            XDDetails.BoxNO = input.BoxNO;
            XDDetails.BoxAge = input.BoxAge;
           // XDDetails.IsVerify = input.IsVerify;

            XDDetails.Remarks = input.Remarks;
            XDDetails.LastModifierUserId = AbpSession.UserId;
            XDDetails.LastModificationTime = DateTime.Now;
        }

        private async Task<BoxInfo> CreateXDDelInfoAsync(EditXDDelInfoDto input)
        {
            string billno = await _IContactNOAPPService.GetBusNO("XD");
            var XDDelInfo = new BoxInfo()
            {
                Remarks = input.Remarks,
                CreatorUserId = AbpSession.UserId,
                CreationTime = DateTime.Now,
                TenantId = AbpSession.TenantId,
                BillNO = billno,
                StartStation = input.StartStation,
                EndStation = input.EndStation,
                ReturnStation = input.ReturnStation,
                IsInStock = input.IsInStock,
                PredictTime = input.PredictTime,
                EffectiveSTime = input.EffectiveSTime,
                EffectiveETime = input.EffectiveETime,
                SellingPrice = input.SellingPrice,
                Line = input.Line,
                IsEnable = input.IsEnable,
                IsVerify = false,
                
                InquiryNum = 0,
                Finish = false,
                

            };
            var id = await _BoxInfoRepository.InsertAndGetIdAsync(XDDelInfo);
            foreach (var item in input.BoxDetails)
            {
                await CreateDetailsoAsync(id, item);
            }
            return XDDelInfo;
            //if (!input.filename.IsNullOrEmpty())
            //{
            //    PublicResource filemodel = new PublicResource
            //    {
            //        CreationTime = DateTime.Now,
            //        CreatorUserId = AbpSession.UserId,
            //        TenantId = AbpSession.TenantId,
            //        IsDeleted = false,

            //        ConnectionId = id.ToString().ToUpper(),
            //        type = 11,
            //        ResoursePath = input.filepath,
            //        OriginalFileName = input.filename,
            //        Remarks = input.remarks
            //    };
            //    await _PublicResourceRepository.InsertAsync(filemodel);
            //}



        }
        #endregion

        /// <summary>
        /// 批量删除
        /// </summary>
        /// <param name="ids"></param>
        /// <returns></returns>
        public void BatchDelete(List<string> ids)
        {
            ids = ids.ConvertAll(p => p.ToUpper()).ToList();
            var alllist = _BoxInfoRepository.GetAll().Where(p => ids.Contains(p.Id.ToString().ToUpper())).ToList();
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

            //删除详情
            var alldetail = _BoxDetailsRepository.GetAll()
                .Where(p => ids.Contains(p.BoxTenantInfo.ToString())).ToList();
            foreach (var item in alldetail)
            {
                item.IsDeleted = true;
                item.DeleterUserId = AbpSession.UserId;
                item.DeletionTime = DateTime.Now;
            }

            //删除附件
            var allfile = _AttachmentInfoRepository.GetAll().Where(p => !string.IsNullOrEmpty(p.ContainerName))
                .Where(p => ids.Contains(p.ContainerName)).ToList();
            foreach (var item in allfile)
            {
                item.IsDeleted = true;
                item.DeleterUserId = AbpSession.UserId;
                item.DeletionTime = DateTime.Now;
            }
        }
        #region 批量启用或者停用
        public void BatchOP(BathOPDto dto)
        {
            var alllist = _BoxInfoRepository.GetAll().Where(p => dto.ids.Contains(p.Id)).ToList();
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
