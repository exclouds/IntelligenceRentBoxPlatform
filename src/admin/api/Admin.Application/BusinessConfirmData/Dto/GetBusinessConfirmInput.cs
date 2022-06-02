using Abp.Runtime.Validation;
using Magicodes.Admin.Dto;
using System;
using System.Collections.Generic;
using System.Text;

namespace Magicodes.Admin.BusinessConfirmData.Dto
{
    public class GetBusinessConfirmInput : PagedAndSortedInputDto, IShouldNormalize
    {
        public int? Id { get; set; }
        #region 箱东和租客单号不可同时为空
        /// <summary>
        /// 箱东单号（当箱东单号为空时可用用户Id替代）
        /// </summary>
        public string BoxInfoBillNO { get; set; }
        /// <summary>
        /// 箱东用户Id
        /// </summary>
        public int? BoxId { get; set; }
        /// <summary>
        /// 租客单号(租客Id)
        /// </summary>
        public string TenantInfoBillNO { get; set; }
        /// <summary>
        /// 租客Id
        /// </summary>
        public int? TenantId { get; set; }
        #endregion
        public DateTime? CreationTimeS { get; set; }
        public DateTime? CreationTimeE { get; set; }
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
