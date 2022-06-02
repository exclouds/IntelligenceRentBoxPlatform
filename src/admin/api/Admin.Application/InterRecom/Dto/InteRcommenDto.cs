using Abp.Runtime.Validation;
using Magicodes.Admin.Dto;
using System;
using System.Collections.Generic;
using System.Text;

namespace Magicodes.Admin.InterRecom.Dto
{
    public class InteRcommenDto : PagedAndSortedInputDto, IShouldNormalize
    {
        public string billNo { get; set; }
        public string belong { get; set; }

        public void Normalize()
        {
            if (string.IsNullOrEmpty(Sorting))
            {
                Sorting = "CreationTime DESC";
            }
        }
    }
}
