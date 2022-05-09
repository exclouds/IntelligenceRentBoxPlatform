using System;
using System.Collections.Generic;
using System.Text;

namespace Magicodes.Admin.Authorization.Users.Dto
{
    public class UpdatePasswordInput
    {
        /// <summary>
        /// 旧密码
        /// </summary>
        public string oldPassword { get; set; }
        /// <summary>
        /// 新密码
        /// </summary>
        public string newPassword { get; set; }
    }
}
