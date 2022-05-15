using Admin.Application.Custom.API.PublicArea.Annex.Dto;
using Magicodes.Admin.Core.Custom.Business;
using System;
using System.Collections.Generic;
using System.Text;

namespace Admin.Application.Custom.API.InformationDelivery.XDDto
{
    public class XDDelInfoDto
    {
       public BoxInfo BoxInfo { get; set; }
        public List<BoxDetails> BoxDetails { get; set; }
        public List<FileInfoModel> fileList { get; set; }
    }
}
