using System;
using System.Collections.Generic;
using System.Text;

namespace Admin.Application.Custom.API.OnlineSearch.Dto
{
    public class ZKSearchList
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
        /// 询价次数
        /// </summary>
        public int? InquiryNum { get; set; }

        /// <summary>
        /// 单据是否完成（界面打完成标记）
        /// </summary>
        public bool Finish { get; set; }
       
        public DateTime? CreationTime { get; set; }
    }
}
