using System;
using System.Collections.Generic;
using System.Text;

namespace Admin.Application.Custom.API.OnlineSearch.Dto
{
    public class XDSearchList
    {
        /// <summary>
        /// Id
        /// </summary>     
        public int? Id { get; set; }
        /// <summary>
        /// 单号（箱东首字母XD）
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
        /// 还箱地
        /// </summary>

        public string ReturnStation { get; set; }
        /// <summary>
        /// 是否库存
        /// </summary>
        public bool IsInStock { get; set; } = true;
        /// <summary>
        /// 预计到站时间
        /// </summary>
        public DateTime? PredictTime { get; set; }

        /// <summary>
        /// 有效时间起
        /// </summary>
        public DateTime? EffectiveSTime { get; set; }

        /// <summary>
        /// 有效时间止
        /// </summary>
        public DateTime? EffectiveETime { get; set; }
        /// <summary>
        /// 租金
        /// </summary>
        public decimal SellingPrice { get; set; }

        /// <summary>
        /// 所属路线（可根据站点自动关联，可选）
        /// </summary>
        public string Line { get; set; }

        /// <summary>
        /// 单据是否完成（界面打完成标记）
        /// </summary>
        public bool? Finish { get; set; }
        public DateTime? CreationTime { get; set; }

        /// <summary>
        /// 发布的箱型尺寸
        /// </summary>
        public string xxcc { get; set; }
    }
}
