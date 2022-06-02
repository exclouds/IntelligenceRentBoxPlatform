using System;
using System.Collections.Generic;
using System.Text;

namespace Magicodes.Admin.BoxDetailsReview.Dto
{
    public class BoxDetailsListDto : EntityBase<int>
    {
        /// <summary>
        /// 箱东信息
        /// </summary>
        public String BoxTenantInfo { get; set; }
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
        public double BoxAge { get; set; }
        /// <summary>
        /// 是否审核
        /// </summary>
        public bool IsVerify { get; set; }
    }
}
