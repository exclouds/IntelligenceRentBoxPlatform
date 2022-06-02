using Abp.Application.Services.Dto;
using Abp.Authorization;
using Abp.Authorization.Users;
using Abp.Dapper.Repositories;
using Abp.Domain.Repositories;
using Abp.Extensions;
using Abp.Linq.Extensions;
using Magicodes.Admin.Authorization.Users;
using Magicodes.Admin.Core.Custom.Basis;
using Magicodes.Admin.Core.Custom.Business;
using Magicodes.Admin.Organizations;
using Magicodes.Admin.TenantReleaseReview.Dto;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Threading.Tasks;

namespace Magicodes.Admin.TenantReleaseReview
{
    public class TenantReleaseReviewAppService : AdminAppServiceBase
    {
        private readonly IRepository<TenantInfo, int> _tenantInfoRepository;
        private readonly IDapperRepository<TenantInfo, int> _tenantInfoDapperRepository;
        private readonly IRepository<BoxDetails, int> _boxDetailsRepository;
        private readonly IRepository<SiteTable, int> _siteTablesRepository;
        private readonly IRepository<Line, int> _lineRepository;
        private readonly IRepository<User, long> _userRepository;
        private readonly IRepository<MyOrganization, long> _organizationUnitRepository;
        public TenantReleaseReviewAppService(IRepository<TenantInfo, int> tenantInfoRepository, IDapperRepository<TenantInfo, int> tenantInfoDapperRepository, 
            IRepository<BoxDetails, int> boxDetailsRepository, IRepository<SiteTable, int> siteTablesRepository, IRepository<Line, int> lineRepository,
            IRepository<User, long> userRepository, IRepository<MyOrganization, long> organizationUnitRepository)
        {
            _tenantInfoRepository = tenantInfoRepository;
            _tenantInfoDapperRepository = tenantInfoDapperRepository;
            _boxDetailsRepository = boxDetailsRepository;
            _siteTablesRepository = siteTablesRepository;
            _lineRepository = lineRepository;
            _userRepository = userRepository;
            _organizationUnitRepository = organizationUnitRepository;
        }

        /// <summary>
        /// 查询租客发布信息列表
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<PagedResultDto<TenantInfoListDto>> GetAllTenantReleaseInfo(GetTenantInfoInput input)
        {
            var query = from ten in _tenantInfoRepository.GetAll()
                        .WhereIf(input.IsVerify != null, t => t.IsVerify == input.IsVerify)
                        .WhereIf(input.Finish.HasValue, t => t.Finish == input.Finish)
                        .WhereIf(!input.StartStation.IsNullOrEmpty(), t => t.StartStation == input.StartStation)
                        .WhereIf(!input.EndStation.IsNullOrEmpty(), t => t.EndStation == input.EndStation)
                        .WhereIf(input.EffectiveSTime.HasValue, t => t.EffectiveSTime >= input.EffectiveSTime)
                        .WhereIf(input.EffectiveETime.HasValue, t => t.EffectiveETime <= input.EffectiveETime)
                        .WhereIf(!input.Filter.IsNullOrWhiteSpace(), t => t.BillNO.Contains(input.Filter))
                        join site1 in _siteTablesRepository.GetAll() on ten.StartStation equals site1.Code into a
                        from qyz in a.DefaultIfEmpty()
                        join site2 in _siteTablesRepository.GetAll() on ten.EndStation equals site2.Code into b
                        from mdz in b.DefaultIfEmpty()
                        join line1 in _lineRepository.GetAll() on ten.Line equals line1.Id into c
                        from line in c.DefaultIfEmpty()
                        join user1 in _userRepository.GetAll() on ten.CreatorUserId equals user1.Id into e
                        from usr in e.DefaultIfEmpty()
                        join org1 in _organizationUnitRepository.GetAll() on usr.OrganizationCode equals org1.Code into f
                        from org in f.DefaultIfEmpty()
                        join boxDet in _boxDetailsRepository.GetAll() on ten.BillNO equals boxDet.BoxTenantInfoNO into d
                        from boxDetails in d.DefaultIfEmpty()
                        select new TenantInfoListDto
                        {
                            Id = ten.Id,
                            BillNO = ten.BillNO,
                            StartStation = qyz.SiteName,
                            EndStation = mdz.SiteName,
                            Line = ten.Line,
                            LineName = line.LineName,
                            EffectiveSTime = ten.EffectiveSTime,
                            EffectiveETime = ten.EffectiveETime,
                            HopePrice = ten.HopePrice,
                            IsVerify = ten.IsVerify,
                            VerifyRem = ten.VerifyRem,
                            InquiryNum = ten.InquiryNum,
                            Finish = ten.Finish,
                            Remarks = ten.Remarks,
                            CreatorUserId = ten.CreatorUserId,
                            CreationTime = ten.CreationTime,
                            Size = boxDetails.Size,
                            Box = boxDetails.Box,
                            CreateName = usr.Name,
                            Company = org.DisplayName,
                            PhoneNumber = usr.PhoneNumber,
                            TelNumber = usr.TelNumber
                        };
            if (!input.Size.IsNullOrEmpty())
            {
                query = query.Where(t => t.Size.Contains(input.Size));
            }
            if (!input.Box.IsNullOrEmpty())
            {
                query = query.Where(t => t.Box.Contains(input.Box));
            }
            query = from q in query
                    select new TenantInfoListDto
                    {
                        Id = q.Id,
                        BillNO = q.BillNO,
                        StartStation = q.StartStation,
                        EndStation = q.StartStation,
                        Line = q.Line,
                        LineName = q.LineName,
                        EffectiveSTime = q.EffectiveSTime,
                        EffectiveETime = q.EffectiveETime,
                        HopePrice = q.HopePrice,
                        IsVerify = q.IsVerify,
                        VerifyRem = q.VerifyRem,
                        InquiryNum = q.InquiryNum,
                        Finish = q.Finish,
                        Remarks = q.Remarks,
                        CreatorUserId = q.CreatorUserId,
                        CreationTime = q.CreationTime,
                        CreateName = q.CreateName,
                        Company = q.Company,
                        PhoneNumber = q.PhoneNumber,
                        TelNumber = q.TelNumber
                    };
            query = query.Distinct();
            var totalCount = await query.CountAsync();
            var items = await query.OrderBy(input.Sorting).PageBy(input).ToListAsync();

            return new PagedResultDto<TenantInfoListDto>(
                totalCount,
                items);
        }
        /// <summary>
        /// 审核
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<TenantInfoDto> UpdateTenantReleaseInfo(UpdateTenantInfoInput input)
        {
            var ten = await _tenantInfoRepository.GetAsync(input.Id);

            ten.IsVerify = input.IsVerify;
            ten.VerifyRem = input.VerifyRem;
            ten.IsEnable = input.IsEnable;
            await _tenantInfoRepository.UpdateAsync(ten);

            var details = await _boxDetailsRepository.GetAll().Where(t => t.BoxTenantInfoNO == ten.BillNO).ToListAsync();
            foreach (var item in details)
            {
                item.IsVerify = true;
                _boxDetailsRepository.Update(item);
            }

            return await CreateTenantReleaseInfoDto(ten);
        }
        private async Task<TenantInfoDto> CreateTenantReleaseInfoDto(TenantInfo tenantInfo)
        {
            var dto = ObjectMapper.Map<TenantInfoDto>(tenantInfo);
            return dto;
        }
        /// <summary>
        /// 获取箱东发布详细信息
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<TenantInfoDto> GetTenantReleaseInfo(UpdateTenantInfoInput input)
        {
            TenantInfoDto dto = new TenantInfoDto();
            var ten = await _tenantInfoRepository.GetAsync(input.Id);
            if (ten != null)
            {
                dto.tenant = ten;
                var usr = (from u in _userRepository.GetAll().Where(u => u.Id == ten.CreatorUserId)
                           join org in _organizationUnitRepository.GetAll() on u.OrganizationCode equals org.Code into o2
                           from o in o2.DefaultIfEmpty()
                           select new TenantInfoListDto
                           {
                               CreateName = u.Name,
                               Company = o.DisplayName,
                               PhoneNumber = u.PhoneNumber,
                               TelNumber = u.TelNumber
                           }).FirstOrDefault();
                dto.CreateName = usr.CreateName;
                dto.Company = usr.Company;
                dto.PhoneNumber = usr.PhoneNumber;
                dto.TelNumber = usr.TelNumber;
            }
            return dto;
        }
    }
}
