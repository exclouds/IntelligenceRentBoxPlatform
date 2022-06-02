using System;
using System.Collections.Generic;
using System.Text;

namespace Magicodes.Admin.BusinessConfirmData.Dto
{
    public class BusinessConfirmDto : EntityBase<int>
    {
        #region 箱东和租客单号不可同时为空
        /// <summary>
        /// 箱东单号（当箱东单号为空时可用用户Id替代）
        /// </summary>
        public string BoxInfoBillNO { get; set; }
        /// <summary>
        /// 箱东用户Id
        /// </summary>
        public int BoxId { get; set; }
        /// <summary>
        /// 租客单号(租客Id)
        /// </summary>
        public string TenantInfoBillNO { get; set; }
        /// <summary>
        /// 租客Id
        /// </summary>
        public string TenantId { get; set; }
        #endregion

        /// <summary>
        /// 租客保证金
        /// </summary>
        public decimal TenantMargin { get; set; }
        /// <summary>
        /// 实际卖价
        /// </summary>
        public decimal SellingPrice { get; set; }
        /// <summary>
        /// 实际买价
        /// </summary>
        public decimal PurchasePrice { get; set; }
    }
}
