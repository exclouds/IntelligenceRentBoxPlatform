using System.Collections.Generic;
using Abp.Runtime.Validation;
using Abp.UI.Inputs;
using Magicodes.Admin.Dto;

namespace Magicodes.Admin.Authorization.Users.Dto
{
    public class GetUsersInput : PagedAndSortedInputDto, IShouldNormalize
    {
        /// <summary>
        /// 文字模糊搜索,登录名、真实姓名、邮箱地址
        /// </summary>
        public string Filter { get; set; }
        /// <summary>
        /// 权限列表
        /// </summary>
        public List<string> Permission { get; set; }
        /// <summary>
        /// 检索角色Id列表
        /// </summary>
        public List<int> Role { get; set; }
        /// <summary>
        /// 公司
        /// </summary>
        public string code { get; set; }
        /// <summary>
        /// 组织结构（部门）
        /// </summary>
        public List<string> Organization { get; set; }

        ///// <summary>
        ///// 是否锁定
        ///// </summary>
        //public bool OnlyLockedUsers { get; set; }
        

        public void Normalize()
        {
            if (string.IsNullOrEmpty(Sorting))
            {
                Sorting = "CreationTime desc";
            }
        }
    }
}