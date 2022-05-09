using System;
using System.Collections.Generic;
using System.Text;

namespace Admin.Application.Custom.API.PublicArea.Import.Dto
{
    public class ReturnResult
    {

        public string name { get; set; }
        public FileResult response { get; set; }
        public string url { get; set; }
    }
    /// <summary>
    /// 文件上传到服务器后返回的数据
    /// </summary>
    public class FileResult
    {
        public string code { get; set; }
        public string subCode { get; set; }
        public string msg { get; set; }
        public string ok { get; set; }
        public string url { get; set; }
        public string thumbUrl { get; set; }
        public string size { get; set; }
    }
}
