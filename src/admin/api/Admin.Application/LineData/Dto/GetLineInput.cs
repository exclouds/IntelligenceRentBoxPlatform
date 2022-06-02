using Abp.Runtime.Validation;
using Magicodes.Admin.Dto;
using System;
using System.Collections.Generic;
using System.Text;

namespace Magicodes.Admin.LineData.Dto
{
    public class GetLineInput : PagedAndSortedInputDto, IShouldNormalize
    {
        /// <summary>
        /// 筛选
        /// </summary>
        public string Filter { get; set; }

        public void Normalize()
        {
            if (string.IsNullOrEmpty(Sorting))
            {
                Sorting = "CreationTime ASC";
            }
        }
    }
}
