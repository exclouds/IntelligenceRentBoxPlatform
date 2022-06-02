﻿using Abp.Runtime.Validation;
using Magicodes.Admin.Dto;
using System;
using System.Collections.Generic;
using System.Text;

namespace Magicodes.Admin.CountryData.Dto
{
    public class GetCountryInput : PagedAndSortedInputDto, IShouldNormalize
    {
        /// <summary>
        /// 国家代码
        /// </summary>
        public string Code { get; set; }
        /// <summary>
        /// 筛选
        /// </summary>
        public string Filter { get; set; }

        public void Normalize()
        {
            if (string.IsNullOrEmpty(Sorting))
            {
                Sorting = "CreationTime ASC";
            }
        }
    }
}
