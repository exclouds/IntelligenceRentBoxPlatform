using System;
using System.Collections.Generic;
using System.Text;

namespace Magicodes.Admin.IMChat.Dto
{
    public class IMServerEnInputDto
    {
        public int Id { get; set; }
        public int ServerChatId { get; set; }
        public string ServerChatName { get; set; }
        /// <summary>
        /// 在线状态 ：on 在线；off离线
        /// </summary>
        public string State { get; set; }
        /// <summary>
        /// 访问时间
        /// </summary>
        public string AccessTime { get; set; }
        public DateTime CreationTime { get; set; }
        public int CreatorUserId { get; set; }
        public DateTime LastModificationTime { get; set; }
        public int LastModifierUserId { get; set; }
        public bool IsDeleted { get; set; }
        public int DeleterUserId { get; set; }
        public int DeletionTime { get; set; }
    }
}
