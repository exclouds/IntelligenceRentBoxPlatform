using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Abp.Application.Services.Dto;

namespace Magicodes.Admin.Editions.Dto
{
    public class CreateOrUpdateEditionDto
    {
        [Required]
        public EditionEditDto Edition { get; set; }

        //TODO：为了兼容新版UI暂时将必填先移除
        //[Required]
        public List<NameValueDto> FeatureValues { get; set; }
    }
}