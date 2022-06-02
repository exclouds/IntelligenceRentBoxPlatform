using Abp.Application.Services.Dto;
using Abp.Authorization;
using Abp.Authorization.Users;
using Abp.Dapper.Repositories;
using Abp.Domain.Repositories;
using Abp.Extensions;
using Abp.Linq.Extensions;
using Magicodes.Admin.BoxDetailsReview.Dto;
using Magicodes.Admin.Core.Custom.Business;
using Magicodes.Admin.TenantReleaseReview.Dto;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Threading.Tasks;

namespace Magicodes.Admin.BoxDetailsReview
{
    public class BoxDetailsReviewAppService : AdminAppServiceBase
    {
        private readonly IRepository<BoxDetails, int> _boxDetailsRepository;
        private readonly IDapperRepository<BoxDetails, int> _boxDetailsDapperRepository;

        public BoxDetailsReviewAppService(IRepository<BoxDetails, int> boxDetailsRepository, IDapperRepository<BoxDetails, int> boxDetailsDapperRepository)
        {
            _boxDetailsRepository = boxDetailsRepository;
            _boxDetailsDapperRepository = boxDetailsDapperRepository;
        }

        /// <summary>
        /// 查询租客发布信息列表
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<PagedResultDto<BoxDetailsListDto>> GetAllBoxDetailsInfo(GetBoxDetailsInput input)
        {
            var query = from boxDetails in _boxDetailsRepository.GetAll()
                        .WhereIf(!input.BoxTenantNO.IsNullOrEmpty(), b => b.BoxTenantInfoNO == input.BoxTenantNO)
                        .WhereIf(input.IsVerify.HasValue, b => b.IsVerify == input.IsVerify)
                        .WhereIf(!input.Size.IsNullOrEmpty(), b=> b.Size.Contains(input.Size))
                        .WhereIf(!input.Box.IsNullOrEmpty(), b => b.Box.Contains(input.Box))
                        .WhereIf(!input.Filter.IsNullOrWhiteSpace(),
                           u => u.BoxNO.Contains(input.Filter))
                        select new BoxDetailsListDto
                        {
                            Id = boxDetails.Id,
                            BoxTenantInfo = boxDetails.BoxTenantInfoNO,
                            Box = boxDetails.Box,
                            Size = boxDetails.Size,
                            Quantity = boxDetails.Quantity,
                            BoxNO = boxDetails.BoxNO,
                            BoxAge = boxDetails.BoxAge,
                            IsVerify = boxDetails.IsVerify,
                            CreatorUserId = boxDetails.CreatorUserId,
                            CreationTime = boxDetails.CreationTime
                        };
            var totalCount = await query.CountAsync();
            var items = await query.OrderBy(input.Sorting).PageBy(input).ToListAsync();

            return new PagedResultDto<BoxDetailsListDto>(
                totalCount,
                items);
        }
    }
}
