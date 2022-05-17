using System;
using System.Collections.Generic;
using System.Text;

namespace Admin.Application.Custom.API.PublicArea.Annex.Dto
{
    public class FileInfoModel
    {
        public long Id { get; set; }
        /// <summary>
        /// 文件名
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// 文件路径
        /// </summary>
        public string Url { get; set; }
        public DateTime? CreationTime { get; set; }
    }
}
