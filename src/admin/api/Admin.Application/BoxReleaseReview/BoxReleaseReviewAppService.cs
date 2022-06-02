using Abp.Application.Services.Dto;
using Abp.Authorization;
using Abp.Authorization.Users;
using Abp.Dapper.Repositories;
using Abp.Domain.Repositories;
using Abp.Extensions;
using Abp.Linq.Extensions;
using Abp.UI;
using AutoMapper;
using Magicodes.Admin.Authorization;
using Magicodes.Admin.Authorization.Users;
using Magicodes.Admin.BoxReleaseReview.Dto;
using Magicodes.Admin.Core.Custom.Basis;
using Magicodes.Admin.Core.Custom.Business;
using Magicodes.Admin.EntityFrameworkCore;
using Magicodes.Admin.Organizations;
using Magicodes.Admin.Organizations.Dto;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Threading.Tasks;

namespace Magicodes.Admin.BoxReleaseReview
{
    public class BoxReleaseReviewAppService : AdminAppServiceBase
    {
        private readonly IRepository<BoxInfo, int> _boxInfoRepository;
        private readonly IDapperRepository<BoxInfo, int> _boxInfoDapperRepository;
        private readonly IRepository<BoxDetails, int> _boxDetailsRepository;
        private readonly IRepository<SiteTable, int> _siteTablesRepository;
        private readonly IRepository<Line, int> _lineRepository;
        private readonly IRepository<User, long> _userRepository;
        private readonly IRepository<MyOrganization, long> _organizationUnitRepository;

        public BoxReleaseReviewAppService(IRepository<BoxInfo, int> boxInfoRepository, IDapperRepository<BoxInfo, int> boxInfoDapperRepository, 
            IRepository<BoxDetails, int> boxDetailsRepository,IRepository<SiteTable, int> siteTablesRepository,IRepository<Line, int> lineRepository,
            IRepository<User, long> userRepository, IRepository<MyOrganization, long> organizationUnitRepository)
        {
            _boxInfoRepository = boxInfoRepository;
            _boxInfoDapperRepository = boxInfoDapperRepository;
            _boxDetailsRepository = boxDetailsRepository;
            _siteTablesRepository = siteTablesRepository;
            _lineRepository = lineRepository;
            _userRepository = userRepository;
            _organizationUnitRepository = organizationUnitRepository;
        }
        /// <summary>
        /// 查询箱东发布信息列表
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<PagedResultDto<BoxInfoListDto>> GetAllBoxReleaseInfo(GetBoxInfoInput input)
        {
            var query = from box in _boxInfoRepository.GetAll()
                        .WhereIf(input.IsVerify.HasValue, b => b.IsVerify == input.IsVerify)
                        .WhereIf(input.Finish.HasValue, b => b.Finish == input.Finish)
                        .WhereIf(!input.StartStation.IsNullOrEmpty(), b => b.StartStation == input.StartStation)
                        .WhereIf(!input.EndStation.IsNullOrEmpty(), b => b.EndStation == input.EndStation)
                        .WhereIf(input.EffectiveSTime.HasValue, b => b.EffectiveSTime >= input.EffectiveSTime)
                        .WhereIf(input.EffectiveETime.HasValue, b => b.EffectiveETime <= input.EffectiveETime)
                        .WhereIf(!input.Filter.IsNullOrWhiteSpace(), b => b.BillNO.Contains(input.Filter))
                        join site1 in _siteTablesRepository.GetAll() on box.StartStation equals site1.Code into a
                        from qyz in a.DefaultIfEmpty()
                        join site2 in _siteTablesRepository.GetAll() on box.EndStation equals site2.Code into b
                        from mdz in b.DefaultIfEmpty()
                        join site3 in _siteTablesRepository.GetAll() on box.ReturnStation equals site3.Code into c
                        from hxd in c.DefaultIfEmpty()
                        join line1 in _lineRepository.GetAll() on box.Line equals line1.Id into d
                        from line in d.DefaultIfEmpty()
                        join user1 in _userRepository.GetAll() on box.CreatorUserId equals user1.Id into e
                        from usr in e.DefaultIfEmpty()
                        join org1 in _organizationUnitRepository.GetAll() on usr.OrganizationCode equals org1.Code into f
                        from org in f.DefaultIfEmpty()
                        join boxDet in _boxDetailsRepository.GetAll()
                        .WhereIf(!input.Size.IsNullOrEmpty(), b => b.Size.Contains(input.Size))
                        .WhereIf(!input.Box.IsNullOrEmpty(), b => b.Box.Contains(input.Box))
                        on box.BillNO equals boxDet.BoxTenantInfoNO
                        select new BoxInfoListDto
                        {
                            Id = box.Id,
                            BillNO = box.BillNO,
                            StartStation = qyz.SiteName,
                            EndStation = mdz.SiteName,
                            ReturnStation = hxd.SiteName,
                            //IsInStock = box.IsInStock,
                            //PredictTime = box.PredictTime,
                            EffectiveSTime = box.EffectiveSTime,
                            EffectiveETime = box.EffectiveETime,
                            SellingPrice = box.SellingPrice,
                            Line = box.Line,
                            LineName = line.LineName,
                            IsEnable = box.IsEnable,
                            IsVerify = box.IsVerify,
                            VerifyRem = box.VerifyRem,
                            InquiryNum = box.InquiryNum,
                            Finish = box.Finish,
                            Remarks = box.Remarks,
                            CreatorUserId = box.CreatorUserId,
                            CreationTime = box.CreationTime,
                            CreateName = usr.Name,
                            Company = org.DisplayName,
                            PhoneNumber = usr.PhoneNumber,
                            TelNumber = usr.TelNumber
                        };
            query = query.Distinct();
            var totalCount = await query.CountAsync();
            var items = await query.OrderBy(input.Sorting).PageBy(input).ToListAsync();

            return new PagedResultDto<BoxInfoListDto>(
                totalCount,
                items);
        }
        /// <summary>
        /// 审核
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        [HttpPost]
        //[AbpAuthorize(AppPermissions.?)]
        public async Task<BoxInfoDto> UpdateBoxReleaseInfo(UpdateBoxInfoInput input)
        {
            var box = await _boxInfoRepository.GetAsync(input.Id);

            box.IsVerify = input.IsVerify;
            box.VerifyRem = input.VerifyRem;
            box.IsEnable = input.IsEnable;
            await _boxInfoRepository.UpdateAsync(box);

            var details = await _boxDetailsRepository.GetAll().Where(b => b.BoxTenantInfoNO == box.BillNO).ToListAsync();
            foreach (var item in details)
            {
                item.IsVerify = true;
                _boxDetailsRepository.Update(item);
            }

            return await CreateBoxReleaseInfoDto(box);
        }
        private async Task<BoxInfoDto> CreateBoxReleaseInfoDto(BoxInfo boxInfo)
        {
            var dto = ObjectMapper.Map<BoxInfoDto>(boxInfo);
            return dto;
        }
        /// <summary>
        /// 获取箱东发布详细信息
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<BoxInfoDto> GetBoxReleaseInfo(UpdateBoxInfoInput input)
        {
            BoxInfoDto dto = new BoxInfoDto();
            var box = await _boxInfoRepository.GetAsync(input.Id);
            if (box != null)
            {
                dto.boxInfo = box;
                var usr = (from u in _userRepository.GetAll().Where(u => u.Id == box.CreatorUserId)
                           join org in _organizationUnitRepository.GetAll() on u.OrganizationCode equals org.Code into o2
                           from o in o2.DefaultIfEmpty()
                           select new BoxInfoListDto
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
