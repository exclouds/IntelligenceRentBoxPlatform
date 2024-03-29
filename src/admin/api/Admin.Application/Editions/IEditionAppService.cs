﻿using System.Collections.Generic;
using System.Threading.Tasks;
using Abp.Application.Services;
using Abp.Application.Services.Dto;
using Magicodes.Admin.Editions.Dto;

namespace Magicodes.Admin.Editions
{
    public interface IEditionAppService : IApplicationService
    {
        Task<ListResultDto<EditionListDto>> GetEditions();

        Task<GetEditionEditOutput> GetEditionForEdit(NullableIdDto input);

        Task CreateOrUpdateEdition(CreateOrUpdateEditionDto input);

        Task DeleteEdition(EntityDto input);

        Task<List<SubscribableEditionComboboxItemDto>> GetEditionComboboxItems(int? selectedEditionId = null,
            bool addAllItem = false, bool onlyFree = false, bool addNotAssignedItem = true);

        /// <summary>
        /// 批量删除
        /// </summary>
        /// <param name="input">要删除的集合</param>
        /// <returns></returns>
        Task BatchDelete(List<EntityDto> input);
    }
}