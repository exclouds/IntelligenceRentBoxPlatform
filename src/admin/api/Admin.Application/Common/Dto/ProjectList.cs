using System;
using System.Collections.Generic;
using System.Text;

namespace Magicodes.Admin.Common.Dto
{
    public class ProjectList
    {
        public int Id { get; set; }
        /// <summary>
        /// 行号
        /// </summary>
        public int LinNO { get; set; }
        /// <summary>
        /// 项目名称
        /// </summary>
        public string ProjectName { get; set; }
        /// <summary>
        /// 对应科目代码
        /// </summary>
        public string ProjectCode { get; set; }

    }
}
