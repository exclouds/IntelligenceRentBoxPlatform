using System;
using System.Collections.Generic;
using System.Text;

namespace Admin.Application.Custom.API.InformationDelivery.XDDto
{
    public class EditXDDelInfoDto
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
        public List<string> EndStation { get; set; }

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
        public DateTime EffectiveSTime { get; set; }

        /// <summary>
        /// 有效时间止
        /// </summary>
        public DateTime EffectiveETime { get; set; }
        /// <summary>
        /// 租金
        /// </summary>
        public decimal SellingPrice { get; set; }

        /// <summary>
        /// 所属路线（可根据站点自动关联，可选）
        /// </summary>
        public int? Line { get; set; }

        /// <summary>
        /// 是否启用
        /// </summary>
        public bool IsEnable { get; set; } = true;
        public string Remarks { get; set; }
        public List<EditXDDetailsDto> BoxDetails { get; set; }
    }

    public class EditXDDetailsDto
    {
        /// <summary>
        /// Id
        /// </summary>     
        public int? Id { get; set; }
        /// <summary>
        /// 箱东租客信息
        /// </summary>
        public string BoxTenantInfo { get; set; }


        /// <summary>
        /// 箱型
        /// </summary>
        public string Box { get; set; }
        /// <summary>
        /// 尺寸
        /// </summary>
        public string Size { get; set; }

        /// <summary>
        /// 箱量
        /// </summary>
        public int Quantity { get; set; }

        /// <summary>
        /// 箱号
        /// </summary>
        public string BoxNO { get; set; }
        /// <summary>
        /// 箱龄
        /// </summary>
        public string BoxAge { get; set; }

      
        public string Remarks { get; set; }
        /// <summary>
        /// 最大载重
        /// </summary>
        public double? MaxWeight { get; set; }
        /// <summary>
        /// 冻柜型号
        /// </summary>
        public string FreezerModel { get; set; }
        /// <summary>
        /// 箱标
        /// </summary>
        public string BoxLabel { get; set; }

        /// <summary>
        /// 生产年限
        /// </summary>
        public DateTime? BoxTime { get; set; }
    }
}
