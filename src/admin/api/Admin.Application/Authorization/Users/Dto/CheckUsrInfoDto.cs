using System;
using System.Collections.Generic;
using System.Text;

namespace Magicodes.Admin.Authorization.Users.Dto
{
    public class CheckUsrInfoDto
    {
        /// <summary>
        /// id
        /// </summary>
        public string Id { get; set; }
        /// <summary>
        /// 是否审核
        /// </summary>
        public bool IsVerify { get; set; }
        /// <summary>
        /// 审核意见
        /// </summary>
        public string VerifyRem { get; set; }
        /// <summary>
        /// 是否通过
        /// </summary>
        public bool IsActive { get; set; }       

    }
}
