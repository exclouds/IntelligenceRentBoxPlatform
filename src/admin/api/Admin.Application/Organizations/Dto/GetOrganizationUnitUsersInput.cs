using System.ComponentModel.DataAnnotations;
using Abp.Runtime.Validation;
using Magicodes.Admin.Dto;

namespace Magicodes.Admin.Organizations.Dto
{
    public class GetOrganizationUnitUsersInput : PagedAndSortedInputDto, IShouldNormalize
    {
        [Range(1, long.MaxValue)]
        public long Id { get; set; }

        public string code { get; set; }

        public string FilterText { get; set; }

        public void Normalize()
        {
            if (string.IsNullOrEmpty(Sorting))
            {
                Sorting = "AddedTime ASC";
            }
            //if (string.IsNullOrEmpty(Sorting))
            //{
            //    Sorting = "user.Name, user.Surname";
            //}
            //else if (Sorting.Contains("userName"))
            //{
            //    Sorting = Sorting.Replace("userName", "user.userName");
            //}
            //else if (Sorting.Contains("addedTime"))
            //{
            //    Sorting = Sorting.Replace("addedTime", "uou.creationTime");
            //}
        }
    }
}