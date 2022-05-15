using Abp.Runtime.Validation;
using Magicodes.Admin.Dto;
using System;
using System.Collections.Generic;
using System.Text;

namespace Admin.Application.Custom.API.CustRegist.Dto
{
    public class UserSearchDto : PagedAndSortedInputDto, IShouldNormalize
    {
        /// <summary>
        /// 文字模糊搜索,登录名、真实姓名、邮箱地址
        /// </summary>
        public string Filter { get; set; }
       // public int UserNature { get; set; }
        public void Normalize()
        {
            if (string.IsNullOrEmpty(Sorting))
            {
                Sorting = "CreationTime DESC";
            }
        }
    }
}
