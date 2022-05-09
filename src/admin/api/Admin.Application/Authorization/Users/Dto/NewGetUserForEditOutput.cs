using System;
using System.Collections.Generic;
using System.Text;

namespace Magicodes.Admin.Authorization.Users.Dto
{
    public class NewGetUserForEditOutput
    {
        public Guid? ProfilePictureId { get; set; }

        public NewUserEditDto User { get; set; }
        /// <summary>
        /// 公司
        /// </summary>
        public List<UnitList> Dpts { get; set; }

        /// <summary>
        /// 角色
        /// </summary>
        public List<string> Roles { get; set; }       


        public NewGetUserForEditOutput()
        {
            Roles = new List<string>();
        }

    }
}
