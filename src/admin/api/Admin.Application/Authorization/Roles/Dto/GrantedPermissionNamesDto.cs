using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Magicodes.Admin.Authorization.Roles.Dto
{
    public class GrantedPermissionNamesDto
    {
        [Required]
        public int Id { get; set; }

        [Required]
        public List<string> GrantedPermissionNames { get; set; }
    }
}
