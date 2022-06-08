using Admin.Application.Custom.API.InformationDelivery.XDDto;
using System;
using System.Collections.Generic;
using System.Text;

namespace Admin.Application.Custom.API.InformationDelivery.ZKDto
{
    public class EditZKDelInfoDto
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
        public List<string> EndStation { get; set; }

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
        /// 是否启用
        /// </summary>
        public bool? IsEnable { get; set; }
        public string Remarks { get; set; }
        public List<EditXDDetailsDto> BoxDetails { get; set; }
    }
}
