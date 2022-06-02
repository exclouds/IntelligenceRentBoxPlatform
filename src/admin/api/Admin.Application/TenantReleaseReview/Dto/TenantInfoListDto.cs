using System;
using System.Collections.Generic;
using System.Text;

namespace Magicodes.Admin.TenantReleaseReview.Dto
{
    public class TenantInfoListDto : EntityBase<int>
    {
        /// <summary>
        /// 单号（租客首字母ZK）
        /// </summary>
        public string BillNO { get; set; }

        /// <summary>
        /// 起运站
        /// </summary>
        public string StartStation { get; set; }

        /// <summary>
        /// 目的站
        /// </summary>
        public string EndStation { get; set; }

        /// <summary>
        /// 所属路线（可根据站点自动关联，可选）
        /// </summary>
        public int? Line { get; set; }

        /// <summary>
        /// 路线名称
        /// </summary>
        public string LineName { get; set; }

        /// <summary>
        /// 有效时间起
        /// </summary>
        public DateTime EffectiveSTime { get; set; }

        /// <summary>
        /// 有效时间止
        /// </summary>
        public DateTime EffectiveETime { get; set; }
        /// <summary>
        /// 期望成交价
        /// </summary>
        public decimal HopePrice { get; set; }

        /// <summary>
        /// 是否审核
        /// </summary>
        public bool IsVerify { get; set; }

        /// <summary>
        /// 审核评语
        /// </summary>
        public string VerifyRem { get; set; }

        /// <summary>
        /// 询价次数
        /// </summary>
        public int? InquiryNum { get; set; }

        /// <summary>
        /// 单据是否完成（界面打完成标记）
        /// </summary>
        public bool Finish { get; set; }
        /// <summary>
        /// 尺寸
        /// </summary>
        public string Size { get; set; }
        /// <summary>
        /// 箱型
        /// </summary>
        public string Box { get; set; }
        /// <summary>
        /// 制单人
        /// </summary>
        public string CreateName { get; set; }
        /// <summary>
        /// 公司
        /// </summary>
        public string Company { get; set; }
        /// <summary>
        /// 手机
        /// </summary>
        public string PhoneNumber { get; set; }
        /// <summary>
        /// 电话
        /// </summary>
        public string TelNumber { get; set; }
    }
}
