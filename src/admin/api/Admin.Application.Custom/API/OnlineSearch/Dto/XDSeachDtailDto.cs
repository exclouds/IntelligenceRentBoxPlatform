using Admin.Application.Custom.API.PublicArea.Annex.Dto;
using System;
using System.Collections.Generic;
using System.Text;

namespace Admin.Application.Custom.API.OnlineSearch.Dto
{
    public class XDSeachDtailDto
    {
        /// <summary>
        /// Id
        /// </summary>      
        public int Id { get; set; }

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
        public string  BoxAge { get; set; }
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
