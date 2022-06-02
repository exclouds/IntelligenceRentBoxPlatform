using System;
using System.Collections.Generic;
using System.Text;

namespace Magicodes.Admin.InterRecom.Dto
    
{
    public class ShowResultPageList
    {
        public List<ShowPageList> xdlist;
        public List<ShowPageList> zklist;
    }
    public class ShowPageList
    {
        /// <summary>
        /// Id
        /// </summary>     
        public int? Id { get; set; }
        /// <summary>
        /// 发布方
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// 类型，：主要区分箱东信息还是租客信息
        /// </summary>
        public string type { get; set; }
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
        public string EffectiveSTime { get; set; }
        /// <summary>
        /// 有效时间止
        /// </summary>
        public string EffectiveETime { get; set; }
        /// <summary>
        /// 租金/期望成交价
        /// </summary>
        public decimal Price { get; set; }
        /// <summary>
        /// 所属路线（可根据站点自动关联，可选）
        /// </summary>
        public string Line { get; set; }
        /// <summary>
        /// 单据是否完成（界面打完成标记）
        /// </summary>
        public bool? Finish { get; set; }
        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTime? CreationTime { get; set; }
        /// <summary>
        /// 发布的箱型尺寸
        /// </summary>
        public string xxcc { get; set; }
        /// <summary>
        /// 备注
        /// </summary>
        public string Remarks { get; set; }
    }
}
