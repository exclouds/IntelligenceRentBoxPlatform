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
        /// <summary>
        /// 用户性质（0：租客客户，1：箱东客户；2：平台管理员）
        /// </summary>
        public int UserNature { get; set; }
        /// <summary>
        /// 是否锁定
        /// </summary>
        public bool OnlyLockedUsers { get; set; }
        /// <summary>
        /// 是否审核
        /// </summary>
        public bool IsVerify { get; set; }
        public bool? IsAdmin { get; set; }


        public void Normalize()
        {
            if (string.IsNullOrEmpty(Sorting))
            {
                Sorting = "CreationTime desc";
            }
        }
    }
}