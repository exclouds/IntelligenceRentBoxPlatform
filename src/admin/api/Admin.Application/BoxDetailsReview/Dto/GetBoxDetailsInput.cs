using Abp.Runtime.Validation;
using Magicodes.Admin.Dto;
using System;
using System.Collections.Generic;
using System.Text;

namespace Magicodes.Admin.BoxDetailsReview.Dto
{
    public class GetBoxDetailsInput : PagedAndSortedInputDto, IShouldNormalize
    {
        /// <summary>
        /// 单号
        /// </summary>
        public string BoxTenantNO { get; set; }
        /// <summary>
        /// 是否审核
        /// </summary>
        public bool? IsVerify { get; set; }
        /// <summary>
        /// 尺寸
        /// </summary>
        public string Size { get; set; }
        /// <summary>
        /// 箱型
        /// </summary>
        public string Box { get; set; }
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
