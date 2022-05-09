using Abp.Application.Services.Dto;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Magicodes.Admin.Organizations.Dto
{
  public  class NewOrganizationUnitDto
    {
        public string nodeUUid { get; set; }
        public long nodeId { get; set; }

        public string nodeName { get; set; }

        public string Message { get; set; }

        public bool disabled { get; set; } = false;
        public bool? ismulti { get; set; } = false;

        public List<NewOrganizationUnitDto> children { get; set; }
    }


}
