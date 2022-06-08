using System;
using System.Collections.Generic;
using System.Text;

namespace Admin.Application.Custom.API.InformationDelivery.ZKDto
{
    public class ZKDelInfoListDto
    {
        /// <summary>
        /// Id
        /// </summary>     
        public int? Id { get; set; }
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
        public string EndStationCode { get; set; }
        
        /// <summary>
        /// 所属路线（可根据站点自动关联，可选）
        /// </summary>
        public string Line { get; set; }

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
        /// 是否启用
        /// </summary>
        public bool? IsEnable { get; set; } 
        public DateTime? CreationTime { get; set; }
        public string Remarks { get; set; }
        /// <summary>
        /// 箱型尺寸
        /// </summary>
        public string xxcc { get; set; }
        
    }
}
