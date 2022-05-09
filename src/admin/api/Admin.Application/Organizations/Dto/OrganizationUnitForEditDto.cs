using System;
using System.Collections.Generic;
using System.Text;

namespace Magicodes.Admin.Organizations.Dto
{
   public class OrganizationUnitForEditDto
   {
       /// <summary>
       /// id
       /// </summary>
       public string Id { get; set; }
       /// <summary>
       /// 部门名称 
       /// </summary>
       public string UnitName { get; set; }
       /// <summary>
       /// 上级部门Id
       /// </summary>
       public string ParentId { get; set; }
       /// <summary>
       /// 上级部门名称 
       /// </summary>
       public string ParentName { get; set; }
    }
}
