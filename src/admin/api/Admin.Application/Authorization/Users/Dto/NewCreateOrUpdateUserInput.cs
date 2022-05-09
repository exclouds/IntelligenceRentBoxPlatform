using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Magicodes.Admin.Authorization.Users.Dto
{
   public class NewCreateOrUpdateUserInput
    {
        [Required]
        public NewUserEditDto User { get; set; }

        /// <summary>
        /// 部门
        /// </summary>
        public List<UnitList> Dpts { get; set; }

        /// <summary>
        /// 角色
        /// </summary>
        public List<string> Roles { get; set; }

        public NewCreateOrUpdateUserInput()
        {
            Roles = new List<string>();
           Dpts = new List<UnitList>();
        }
    }

   public class UnitList
   {
       public string Id { get; set; }
       public string DisplayName { get; set; }
   }
}
