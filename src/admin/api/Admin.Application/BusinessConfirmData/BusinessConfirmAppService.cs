using Abp.Application.Services.Dto;
using Abp.Authorization;
using Abp.Authorization.Users;
using Abp.Dapper.Repositories;
using Abp.Domain.Repositories;
using Abp.Extensions;
using Abp.Linq.Extensions;
using Magicodes.Admin.Authorization.Users;
using Magicodes.Admin.BoxDetailsReview.Dto;
using Magicodes.Admin.BusinessConfirmData.Dto;
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

namespace Magicodes.Admin.BusinessConfirmData
{
    public class BusinessConfirmAppService : AdminAppServiceBase
    {
        private readonly IRepository<BusinessConfirm, int> _businessConfirmRepository;
        private readonly IRepository<BoxInfo, int> _boxInfoRepository;
        private readonly IRepository<TenantInfo, int> _tenantInfoRepository;
        private readonly IRepository<User, long> _userRepository;//用户信息
        private readonly IRepository<MyOrganization, long> _organizationUnitRepository;
        public BusinessConfirmAppService(IRepository<BusinessConfirm, int> businessConfirmRepository, IRepository<BoxInfo, int> boxInfoRepository, IRepository<TenantInfo, int> tenantInfoRepository, IRepository<User, long> userRepository, IRepository<MyOrganization, long> organizationUnitRepository)
        {
            _businessConfirmRepository = businessConfirmRepository;
            _boxInfoRepository = boxInfoRepository;
            _tenantInfoRepository = tenantInfoRepository;
            _userRepository = userRepository;
            _organizationUnitRepository = organizationUnitRepository;
        }

        /// <summary>
        /// 查询成交列表
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<PagedResultDto<BusinessConfirmListDto>> GetAllBusinessConfirm(GetBusinessConfirmInput input)
        {
            var query = from bus in _businessConfirmRepository.GetAll()
                        .WhereIf(input.CreationTimeS.HasValue, b => b.CreationTime >= input.CreationTimeS)
                        .WhereIf(input.CreationTimeE.HasValue, b => b.CreationTime <= input.CreationTimeE)
                        select new BusinessConfirmListDto
                        {
                            Id = bus.Id,
                            BoxInfoBillNO = bus.BoxInfoBillNO,
                            BoxId = bus.BoxId,
                            TenantInfoBillNO = bus.TenantInfoBillNO,
                            TenantId = bus.TenantInfoId,
                            TenantMargin = bus.TenantMargin,
                            SellingPrice = bus.SellingPrice,
                            PurchasePrice = bus.PurchasePrice,
                            Remarks = bus.Remarks,
                            CreatorUserId = bus.CreatorUserId,
                            CreationTime = bus.CreationTime
                        };
            var totalCount = await query.CountAsync();
            var items = await query.OrderBy(input.Sorting).PageBy(input).ToListAsync();

            return new PagedResultDto<BusinessConfirmListDto>(
                totalCount,
                items);
        }
        /// <summary>
        /// 获取箱东和租客订单信息
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<BoxInfoTenantInfoDto> GetBusinessConfirmInfo(GetBusinessConfirmInput input)
        {
            BoxInfoTenantInfoDto boxInfoTenantInfoDto = new BoxInfoTenantInfoDto();
            if (input.Id.HasValue)
            {
                var bus = await _businessConfirmRepository.GetAsync(input.Id.Value);
                input.BoxInfoBillNO = bus.BoxInfoBillNO;
                input.TenantInfoBillNO = bus.TenantInfoBillNO;
            }
            if (!input.BoxInfoBillNO.IsNullOrEmpty())
            {
                var box = await _boxInfoRepository.GetAll().Where(b => (b.BillNO.Contains(input.BoxInfoBillNO) && !b.Finish)
                || (b.CreatorUserId.ToString().Contains(input.BoxInfoBillNO) && !b.Finish)).OrderByDescending(b => b.CreationTime).FirstOrDefaultAsync();
                if (box != null)
                {
                    boxInfoTenantInfoDto.BoxInfo = box;
                    var usr = (from u in _userRepository.GetAll().Where(u => u.Id == box.CreatorUserId)
                               join org in _organizationUnitRepository.GetAll() on u.OrganizationCode equals org.Code into o2
                               from o in o2.DefaultIfEmpty()
                               select new BoxInfoTenantInfoDto
                               {
                                   CreateName = u.Name,
                                   Company = o.DisplayName,
                                   PhoneNumber = u.PhoneNumber,
                                   TelNumber = u.TelNumber
                               }).FirstOrDefault();
                    boxInfoTenantInfoDto.CreateName = usr.CreateName;
                    boxInfoTenantInfoDto.Company = usr.Company;
                    boxInfoTenantInfoDto.PhoneNumber = usr.PhoneNumber;
                    boxInfoTenantInfoDto.TelNumber = usr.TelNumber;
                }
            }
            if (!input.TenantInfoBillNO.IsNullOrEmpty())
            {
                var tenant = await _tenantInfoRepository.GetAll().Where(t => (t.BillNO.Contains(input.TenantInfoBillNO) && !t.Finish)
                || (t.CreatorUserId.ToString().Contains(input.TenantInfoBillNO) && !t.Finish)).OrderByDescending(t => t.CreationTime).FirstOrDefaultAsync();
                if (tenant != null)
                {
                    boxInfoTenantInfoDto.TenantInfo = tenant;
                    var usr = (from u in _userRepository.GetAll().Where(u => u.Id == tenant.CreatorUserId)
                               join org in _organizationUnitRepository.GetAll() on u.OrganizationCode equals org.Code into o2
                               from o in o2.DefaultIfEmpty()
                               select new BoxInfoTenantInfoDto
                               {
                                   CreateName = u.Name,
                                   Company = o.DisplayName,
                                   PhoneNumber = u.PhoneNumber,
                                   TelNumber = u.TelNumber
                               }).FirstOrDefault();
                    boxInfoTenantInfoDto.CreateName = usr.CreateName;
                    boxInfoTenantInfoDto.Company = usr.Company;
                    boxInfoTenantInfoDto.PhoneNumber = usr.PhoneNumber;
                    boxInfoTenantInfoDto.TelNumber = usr.TelNumber;
                }
            }
            if (boxInfoTenantInfoDto.BoxInfo == null && boxInfoTenantInfoDto.TenantInfo == null)
            {
                boxInfoTenantInfoDto.Msg = "提单不存在或已完成";
            }
            return boxInfoTenantInfoDto;
        }
        /// <summary>
        /// 成交
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<BusinessConfirmDto> UpdateBusinessConfirmInfo(UpdateBusinessConfirmInput input)
        {
            BusinessConfirm bus = new BusinessConfirm();
            bus.BoxInfoBillNO = input.BoxInfoBillNO;
            bus.BoxId = input.BoxId;
            bus.TenantInfoBillNO = input.TenantInfoBillNO;
            bus.TenantId = input.TenantId;
            bus.TenantMargin = input.TenantMargin;
            bus.SellingPrice = input.SellingPrice;
            bus.PurchasePrice = input.PurchasePrice;
            bus.TenantId = AbpSession.TenantId;
            bus.CreatorUserId = AbpSession.UserId;
            bus.CreationTime = DateTime.Now;
            await _businessConfirmRepository.InsertAsync(bus);

            var box = await _boxInfoRepository.GetAll().Where(b => b.BillNO == input.BoxInfoBillNO).FirstOrDefaultAsync();
            box.Finish = true;
            await _boxInfoRepository.UpdateAsync(box);

            var tenant = await _tenantInfoRepository.GetAll().Where(t => t.BillNO == input.TenantInfoBillNO).FirstOrDefaultAsync();
            tenant.Finish = true;
            await _tenantInfoRepository.UpdateAsync(tenant);

            return await CreateBusinessConfirmDto(bus);
        }
        private async Task<BusinessConfirmDto> CreateBusinessConfirmDto(BusinessConfirm businessConfirm)
        {
            var dto = ObjectMapper.Map<BusinessConfirmDto>(businessConfirm);
            return dto;
        }
    }
}
