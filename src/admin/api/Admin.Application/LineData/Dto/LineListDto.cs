using System;
using System.Collections.Generic;
using System.Text;

namespace Magicodes.Admin.LineData.Dto
{
    public class LineListDto : EntityBase<int>
    {
        /// <summary>
        /// 名称
        /// </summary>
        public string LineName { get; set; }

        /// <summary>
        /// 是否启用
        /// </summary>
        public bool IsEnable { get; set; } = true;
    }
}
