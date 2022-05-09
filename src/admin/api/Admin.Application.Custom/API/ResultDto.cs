using System;
using System.Collections.Generic;
using System.Text;

namespace Admin.Application.Custom.API
{
    public class ResultDto
    {
        /// <summary>
        /// 是否成功
        /// </summary>
        public bool success { get; set; }
        /// <summary>
        /// 代码
        /// </summary>
        public string code { get; set; }
        /// <summary>
        /// 信息
        /// </summary>
        public string msg { get; set; }
    }
}
