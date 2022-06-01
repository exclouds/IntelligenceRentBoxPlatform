using Admin.Application.Custom.API.OnlineSearch.Dto;
using Admin.Application.Custom.API.PublicArea.Annex.Dto;
using System;
using System.Collections.Generic;
using System.Text;

namespace Admin.Application.Custom.API.Recommend.Dto
{
    public class ShowPageInfo
    {
        /// <summary>
        /// Id
        /// </summary>     
        public int? Id { get; set; }
        /// <summary>
        /// 单号（租客首字母ZK）
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
        
        public string ReturnStation { get; set; }
        /// <summary>
        /// 所属路线（可根据站点自动关联，可选）
        /// </summary>
        public string Line { get; set; }

        /// <summary>
        /// 有效时间起
        /// </summary>
        public string EffectiveSTime { get; set; }

        /// <summary>
        /// 有效时间止
        /// </summary>
        public string EffectiveETime { get; set; }

        /// <summary>
        /// 单据是否完成（界面打完成标记）
        /// </summary>
        public string Finish { get; set; }
        /// <summary>
        /// 是否库存
        /// </summary>
        public string IsInStock { get; set; } 
        /// <summary>
        /// 预计到站时间
        /// </summary>
        public string PredictTime { get; set; }

        public List<FileInfoModel> fileList { get; set; }
        public List<XDSeachDtailDto> BoxDetails { get; set; }
    }
}
