using System;
using System.Collections.Generic;
using System.Text;

namespace Magicodes.Admin.IMChat.Dto
{
    public class IMChatMsgInputDto
    {
        public int Id { get; set; }
        /// <summary>
        /// 会话Id (客户端用户ID)
        /// </summary>
        public int ClientChatId { get; set; }
        /// <summary>
        /// 消息所有者身份：sys 系统消息；client 会话消息
        /// </summary>
        public string Role { get; set; }
        /// <summary>
        /// 消息类型：文本；文件
        /// </summary>
        public string ContentType { get; set; }
        /// <summary>
        /// 消息内容
        /// </summary>
        public string Content { get; set; }
        /// <summary>
        /// 会话框内容
        /// </summary>
        public bool IsNewMsg { get; set; }
        public DateTime CreationTime { get; set; }
        public int CreatorUserId { get; set; }
        public DateTime LastModificationTime { get; set; }
        public int LastModifierUserId { get; set; }
        public bool IsDeleted { get; set; }
        public int DeleterUserId { get; set; }
        public int DeletionTime { get; set; }
    }
}
