using Magicodes.Admin.Dto;
using System;
using System.Collections.Generic;
using System.Text;

namespace Magicodes.Admin.Organizations.Dto
{
  public  class NewFindOrganizationUnitUsersInput : PagedAndFilteredInputDto
    {
        /// <summary>
        /// 部门Id
        /// </summary>
        public long OrganizationUnitId { get; set; }
        /// <summary>
        /// 1：获取部门id下的人员，2：获取不在该部门下的人员
        /// </summary>
        public int Type { get; set; }
    }
}
