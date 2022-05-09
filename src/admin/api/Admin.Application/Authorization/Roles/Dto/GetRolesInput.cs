using System.Collections.Generic;
using Abp.Runtime.Validation;
using Magicodes.Admin.Dto;

namespace Magicodes.Admin.Authorization.Roles.Dto
{
    public class GetRolesInput : PagedAndSortedInputDto, IShouldNormalize
    {
        public List<string> PermissionNames { get; set; }

        public string FilterText { get; set; }

        public void Normalize()
        {
            if (string.IsNullOrEmpty(Sorting)) Sorting = "Id";
        }
    }
}
