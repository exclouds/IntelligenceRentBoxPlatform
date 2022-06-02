using System;
using System.Collections.Generic;
using System.Text;

namespace Magicodes.Admin.LineData.Dto
{
    public class LineInput
    {
        public int? Id { get; set; }
        /// <summary>
        /// 名称
        /// </summary>
        public string LineName { get; set; }
        /// <summary>
        /// 是否启用
        /// </summary>
        public bool IsEnable { get; set; } = true;
        /// <summary>
        /// 备注
        /// </summary>
        public string Remarks { get; set; }
    }
}
